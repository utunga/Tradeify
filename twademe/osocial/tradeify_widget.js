
/* ------------------------------
couple useful bits and pieces  
------------------------------*/

String.prototype.trim = function() {
    return this.replace(/^\s+|\s+$/g, "");
}
String.prototype.ltrim = function() {
    return this.replace(/^\s+/, "");
}
String.prototype.rtrim = function() {
    return this.replace(/\s+$/, "");
}

////define console.log so that we can log to firebug console, but not get errors if people don't have firebug installed
//if (!console) {
//    var console = {}
//    console.log = function(text) {
//        return; //ie do nothing
//    }
//}

var keyup_threshold = 200;
var details_keyup_threshold = 200;
var message_change_threshold = 800;
var address_keyup_stack = 0;
var message_keyup_stack = 0;
var details_keyup_stack = 0;
var message_change_stack = 0;
var tags_change_stack = 0;

function address_keyup() {
	address_keyup_stack++;
	setTimeout(function() {
		address_keyup_stack--;
		if (address_keyup_stack == 0) {
			geo_code_address()
		}
	}, keyup_threshold);
}

function details_keyup() {
	details_keyup_stack++;
	setTimeout(function() {
		details_keyup_stack--;
		if (details_keyup_stack == 0) {
			parse_details_for_tags()
		}
	}, details_keyup_threshold);
}

function message_keyup() {
    message_keyup_stack++;
	setTimeout(function() {
	    message_keyup_stack--;
	    if (message_keyup_stack == 0) {
	        parse_offer();
	    }
	}, keyup_threshold);
}

function message_change() {
    message_change_stack++;
	setTimeout(function() {
	    message_change_stack--;
	    if (message_change_stack == 0) {
	        parse_offer();
	    }
	}, message_change_threshold);
}


/* ------------------------------
     search
   ------------------------------*/

function queryLocationTag() {
    var query = $("#search_location_tag").val();
    list_widget.add_filter(query, "tag");
}

function set_form_text() {
    var val = $("#offer").val();
    if (val == "(details)") $("#offer").val("");
}

var toggle_wanted_on = true;
var toggle_offered_on = true;
function toggle_wanted() {
    toggle_wanted_on = !toggle_wanted_on;
    if (!toggle_wanted_on && !toggle_offered_on) {
        // neither toggle is on, not legal
        // set this one back to 'on' and return
        toggle_wanted_on = true;
        return;
    }
    $("#wanted_filter").toggleClass("ui-state-active");
    update_type_filters();
}

function toggle_offered() {
    toggle_offered_on = !toggle_offered_on;
    if (!toggle_wanted_on && !toggle_offered_on) {
        // neither toggle is on, not legal
        // set this one back to 'on' and return
        toggle_offered_on = true;
        return;
    }
    $("#offered_filter").toggleClass("ui-state-active");
    update_type_filters();
}

function update_type_filters() {
    if (toggle_offered_on && toggle_wanted_on) {
        list_widget.remove_filter("offer", "msg_type");
        list_widget.remove_filter("wanted", "msg_type");
    }
    else if (toggle_offered_on && !toggle_wanted_on) {
        list_widget.add_filter("offer", "msg_type");
        list_widget.remove_filter("wanted", "msg_type");
    }
    else if (!toggle_offered_on && toggle_wanted_on) {
        list_widget.add_filter("wanted", "msg_type");
        list_widget.remove_filter("offer", "msg_type");
    }
    //bit counterintuitve but we want no messages shown and adding both filters works
    else {
        list_widget.add_filter("wanted","msg_type");
        list_widget.add_filter("offer","msg_type");
    }
}


/* ------------------------------
     form related function
   ------------------------------*/
		

function MessagePart(type, field) {
    this.prefix = type;
    this.field = field;
}

var offerPrefix = "OFFER: ";
var wantedPrefix = "WANTED: ";
var locationPrefix = " in l:";
var locationSuffix = ":";
var forPrefix = " for ";
var untilPrefix = " until ";
var group = "ooooby";

function get_until() {
	var until =$("#until").val().trim();
    return (until.length == 0) ?  "" : untilPrefix + until;
}

function get_currency() {
    var currency = post_your_own_currency_tags.get_active_tags_text();
    return (post_your_own_currency_tags.length == 0) ? "" : forPrefix + currency;
}

function get_location() {
    var location = $("#location").val().trim();
    return (location.length == 0) ?  "" : locationPrefix + location + locationSuffix;
}

function get_offer() {

    var offer = $("#offer").val().trim();
    return (offer == $("#offer")[0].title) ? "" : " " + offer;
}

function get_tags() {
    if (suggested_tags_widget.get_active_tags().length == 0) 
        return "";
    else 
       return suggested_tags_widget.get_active_tags_text();
}

function get_prefix() {
   
    var prefix = $("input[@name='message_type']:checked").val();
    return(prefix == "OFFER") ? offerPrefix : wantedPrefix;

}
//function get_tags() {
//	if (selected_tags.length == 0) return "";
//	return " #" + selected_tags.join(" #");
//}

//function get_imagelink() {
//	var picture =$("#picture").val().trim();
//    return (picture.length == 0) ?  "" : " " + picture;
//}

function update_offer() {
    update_and_dont_parse();
    //make sure the updated message to send is parsed again
    message_change();
}

function update_and_dont_parse() {
    var concatMessage =
            get_prefix() +
             get_offer() +
			 get_location() + 
             get_currency() +
             get_until() +             
            //           get_tags() +  
            //			 get_imagelink() +
			 " #"+group;
    $("#message_to_send").val(concatMessage);
}

/* tag selection code */
function parse_offer() {
    var message_data = {
        message: $("#message_to_send").val()
    };
    $.getJSON(container.parse_uri,message_data, display_results_of_parse_offer);
}

function details_keyup() {
	details_keyup_stack++;
	setTimeout(function() {
		details_keyup_stack--;
		if (details_keyup_stack == 0) {
			parse_details_for_tags()
		}
	}, details_keyup_threshold);
}


     
function reset_parse_offer(){
    $(".send_message").attr("disabled", "disabled");
    $(".status").css({ "background-image": "url('" + container.cross_uri + "')" });
}

function display_results_of_parse_offer(response) {
    var reasons = response.validationFailReasons;
    if(reasons.length==0){
    $(".send_message").removeAttr("disabled");
    }
    else $(".send_message").attr("disabled","disabled");
    //"NeedsCurrencyTag", "NeedsLocation", "NeedsGroupTag"
    if ($.inArray("TooLong", reasons) > -1) {
        if($("#too_long").length==0)
            $('<div id="too_long" style="color: red;">Message is too long</div>').insertAfter("#message_to_send");
    }
    else $("#too_long").remove();
    switchStatus("NeedsCurrencyTag","currency_detail",reasons);
    switchLocationStatus("NeedsLocation", "location_detail", reasons);
    switchStatus("NeedsGroupTag","group_detail",reasons);  
}
function switchStatus(value,selector,array){
    if($.inArray(value,array)>-1)
        $("."+selector).css({"background-image": "url('"+container.cross_uri+"')"});
    else $("." + selector).css({ "background-image": "url('" + container.tick_uri + "')" });
}
function switchLocationStatus(value, selector, array) {

    if ($.inArray(value, array) > -1 || $("#message_to_send").val().search($("#location")[0].title) > -1)
        $("." + selector).css({ "background-image": "url('" + container.cross_uri + "')" });
    else $("." + selector).css({ "background-image": "url('" + container.tick_uri + "')" });
    /* form support for styling */
}	   
$(function() {
    /* Bind  functions for handling css jquery-ui class to jQuery events */
    $(".send_message").attr("disabled","disabled");

    $(".ui-state-default:not(.ui-state-disabled)").live("mouseover", function() {
        $(this).addClass("ui-state-hover");
    });
    $(".ui-state-default:not(.ui-state-disabled)").live("mouseout", function() {
        $(this).removeClass("ui-state-hover").removeClass("ui-state-focus");
    });
    $(".ui-state-default:not(.ui-state-disabled)").live("mousedown", function() {
        $(this).addClass("ui-state-focus");
    });
    $(".ui-state-default:not(.ui-state-disabled)").live("mouseup", function() {
        $(this).removeClass("ui-state-focus");
    });
    $(".ui-state-default:not(.ui-state-disabled)").live("focus", function() {
        $(this).addClass("ui-state-hover");
    });
    $(".ui-state-default:not(.ui-state-disabled)").live("blur", function() {
        $(this).removeClass("ui-state-hover");
        $(this).removeClass("ui-state-focus");
    });
    $(".ui-state-default:not(.ui-state-disabled)").live("keydown", function() {
        $(this).addClass("ui-state-focus");
    });
    $(".ui-state-default:not(.ui-state-disabled)").live("keyup", function() {
        $(this).removeClass("ui-state-focus");
    });
});




/* ------------------------------
   Google API stuff
   ------------------------------*/

var geocoder;
var map;
var address_marker;
var post_your_own_form_initialized = false;

// this function called on tab show()
// we wait till now to initalized the map 
// inside the tab - so as to avoid a bug 
function post_your_own_form_init() {
    
    if (post_your_own_form_initialized) return; //already initalized
    if (typeof google !== 'undefined') {
        google_initialize();
    }
}

var region="NZ"
function google_initialize() {
	geocoder = new google.maps.Geocoder({'region':region});
	var myOptions = {
	  zoom: 13,
	  navigationControl: true,
      scaleControl: true,
	  mapTypeControl:false,
	  mapTypeId: google.maps.MapTypeId.TERRAIN
	  
	};
	map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
		
//	if (google.loader.ClientLocation) {
//			currentLocation = google.loader.ClientLocation;
//			var currentAddr = currentLocation.address.city + ", " + currentLocation.address.region + ", " + currentLocation.address.country_code
//			$(".location #location").val(currentAddr);
//			geo_code_address();
//	}
    var user_location=container.get_user_location();
    if (!!user_location) {
        $(".location #location").val(currentAddr);
        geo_code_address();
    }
    
	$("#post_your_own_form .location  #location").keyup(address_keyup);
	$("#post_your_own_form textarea#offer").keyup(details_keyup);
	
	post_your_own_form_initialized = true;
}

function geo_code_address() {
	var address = $(".location #location").val();
	//map.clearOverlays();
	if (geocoder) {
	    geocoder.geocode({ 'address': address, 'region': region }, function(results, status) {
			 if (status == google.maps.GeocoderStatus.OK) {
				 map.setCenter(results[0].geometry.location);
				 if (address_marker!=null) {
					 address_marker.setMap(null); //remove any existing marker
				 }
				 address_marker = new google.maps.Marker({
					 map: map,
					 position: results[0].geometry.location
				 });
			 } else {
				if (address_marker!=null) {
					 address_marker.setMap(null); //remove any existing marker
				}
			 }
	 	})
	}
}

//////////////////////////////////

function get_tags_from_text() {
    var tags_from_text = new Tags();
    var details = $("#post_your_own_form textarea#offer").val();
	var hashTagsRegex = /#([A-Za-z0-9_\-]+)/g; ///[\s^]#([A-Za-z0-9_\-]+)/g;  ///#(\S)+/g; 
    var matches = details.match(hashTagsRegex);
    if (matches == null) return tags_from_text;
    $.each(matches, function() {
        var tag_text = this.toString().substring(1);
        tags_from_text.add_tag(tag_text);
    });
    return tags_from_text;
}

var lastTagsFromText;
function parse_details_for_tags() {
	
    var tagsFromText = get_tags_from_text();
    if (tagsFromText.get_all_tags_text() == "") return;
    
    if (lastTagsFromText==null || 
        lastTagsFromText.get_all_tags_text() != tagsFromText.get_all_tags_text()) {
        
        // remove any tags that we added based on typing last time, 
        // that are no longer in the list
        if (lastTagsFromText !=null ) {
            $.each(lastTagsFromText.get_all_tags(), function() {
                var tagtext = this.toString();
                if (!tagsFromText.has_tag(tagtext) &&
                    suggested_tags_widget.has_tag(tagtext)) {
                    //remove tag from type ahead
                    suggested_tags_widget.remove_tag(tagtext);
                }
            });
        }
        
        $.each(tagsFromText.get_all_tags(), function() {
            var tagtext = this.toString();
            if (!suggested_tags_widget.has_tag(this.toString())) {
                suggested_tags_widget.add_tag(tagtext, "tag", true);           
            }
            else {
                suggested_tags_widget.set_active(tagtext);
            }
        });
        
        suggested_tags_widget.update_view();
        lastTagsFromText = tagsFromText;
    }
}

function ensure_details_includes_active_tags() {
    var tagsFromText = get_tags_from_text();
    var not_included = [];
    $.each(suggested_tags_widget.get_active_tags(), function() {
        var tag_text= this.toString();
        if (!tagsFromText.has_tag(tag_text)) {
            not_included.push(tag_text);
        }
    });
    var append_text = "";
    $.each(not_included, function()  {
        append_text = append_text + " #" + this.toString();
    });
    
    var details = $("#post_your_own_form textarea#offer");
    details.val( details.val() + append_text);
}

