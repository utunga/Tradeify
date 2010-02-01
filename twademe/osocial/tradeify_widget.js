/* ------------------------------
     couple useful bits and pieces  
   ------------------------------*/
//   
//String.prototype.trim = function() {
//	return this.replace(/^\s+|\s+$/g,"");
//}
//String.prototype.ltrim = function() {
//	return this.replace(/^\s+/,"");
//}
//String.prototype.rtrim = function() {
//	return this.replace(/\s+$/,"");
//}

////define console.log so that we can log to firebug console, but not get errors if people don't have firebug installed
//if (!console) {
//    var console = {}
//    console.log = function(text) {
//        return; //ie do nothing
//    }
//}



/* ------------------------------
     search
   ------------------------------*/
   
//function queryGeneralTag(){
//    var query = $("#search_general_tag").val();
//    /*var queryIdx=$.inArray(query,search_tags);
//    if(queryIdx!=-1){	
//        return;
//    }*/
//    list_widget.add_filter(query, "tag");
//    //update_offers();
//}

function queryLocationTag() {
    var query = $("#search_location_tag").val();
    /*var queryIdx=$.inArray(query,search_tags);
    if(queryIdx!=-1){	
    return;
    }*/
    list_widget.add_filter(query, "tag");
    //update_offers();
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
    var location =$("#location").val().trim();
    return (location.length == 0) ?  "" : locationPrefix + location + locationSuffix;
}

function get_offer() {
    var offer = $("#offer").val().trim();
    var prefix = $("input[@name='message_type']:checked").val();
    return (offer.length == 0) ?  prefix + ".." : prefix + offer;
}

function get_category_tags() {
    var categories = post_your_own_general_tags.get_active_tags_text();
    return (post_your_own_general_tags.length == 0) ? "" : categories;
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
    parse_offer();
}
function update_and_dont_parse() {
    var concatMessage =
             get_offer() +
			 get_location() + 
             get_currency() +
             get_until() +
             get_category_tags() +
    //			 get_tags() +  //SUGGESTED TAGS (NOT USED FOR NOW)
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

function display_results_of_parse_offer(response) {
    var reasons = response.validationFailReasons;
    if(reasons.length==0){
    $(".send_message").removeAttr("disabled");
    }
    else $(".send_message").attr("disabled","disabled");
    //"NeedsCurrencyTag", "NeedsLocation", "NeedsGroupTag"
    switchStatus("NeedsCurrencyTag","currency_detail",reasons);
    switchStatus("NeedsLocation","location_detail",reasons);
    switchStatus("NeedsGroupTag","group_detail",reasons);  
}
function switchStatus(value,selector,array){
    if($.inArray(value,array)>-1)
        $("."+selector).css({"background-image": "url('"+container.cross_uri+"')"});
    else $("." + selector).css({ "background-image": "url('" + container.tick_uri + "')" });
}


       /* form support for styling */
	   
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
//called from widget 
//google.load("maps", "2");
//google.setOnLoadCallback(google_initialize);


var geocoder;
var map;
var adress_marker;
var keyup_threshold = 200;
var address_keyup_stack = 0;
var message_keyup_stack = 0;

function google_initialize() {
	geocoder = new google.maps.Geocoder();
	var myOptions = {
	  zoom: 13,
	  navigationControl: true,
      scaleControl: true,
	  mapTypeControl:false,
	  mapTypeId: google.maps.MapTypeId.TERRAIN
	  
	};
	map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
	
	//funny but in quick 'hallway testing' with Louise the fact that twas already filled
	// in caused great confusion, so just leave it blank for them to fill in
	
	if (google.loader.ClientLocation) {
			currentLocation = google.loader.ClientLocation;
			var currentAddr = currentLocation.address.city + ", " + currentLocation.address.region + ", " + currentLocation.address.country_code
			$(".location #location").val(currentAddr);
			geo_code_address();
	}

	$(".location  #location").keyup(function() { address_keyup(geo_code_address, keyup_threshold); });
}

function geo_code_address() {
	var address = $(".location #location").val();
	//map.clearOverlays();
	if (geocoder) {
		 geocoder.geocode( { 'address': address}, function(results, status) {
			 if (status == google.maps.GeocoderStatus.OK) {
				 map.setCenter(results[0].geometry.location);
				 if (adress_marker!=null) {
					 adress_marker.setMap(null); //remove any existing marker
				 }
				 adress_marker = new google.maps.Marker({
					 map: map,
					 position: results[0].geometry.location
				 });
			 } else {
				if (adress_marker!=null) {
					 adress_marker.setMap(null); //remove any existing marker
				}
			 }
	 	})
	}
}

function address_keyup() {
	address_keyup_stack++;
	setTimeout(function() {
		address_keyup_stack--;
		if (address_keyup_stack == 0) {
			geo_code_address()
		}
	}, keyup_threshold);
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

if (typeof google !== 'undefined') {
    google.load("maps", "3",  {callback: google_initialize, other_params:"sensor=false"});
}







/* ------------------------------
 Support for suggested tags 
 SUGGESTED TAGS (NOT USED FOR NOW)
------------------------------*/


/* dont need suggested tags for now */

