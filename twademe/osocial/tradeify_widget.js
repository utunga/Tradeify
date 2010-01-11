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
     rendering of widget 
   ------------------------------*/



/* ------------------------------
     search
   ------------------------------*/
   
function queryServer(){
    var query=$("#searchdata").val();
    /*var queryIdx=$.inArray(query,search_tags);
    if(queryIdx!=-1){	
        return;
    }*/
    add_filter("tag",query);
    //update_offers();
}


/* ------------------------------
     form related function
   ------------------------------*/
		

function MessagePart(type, field) {
    this.prefix = type;
    this.field = field;
}

var offerPrefix = "I am offering ";
var locationPrefix = " in L:";
var locationSuffix = ":";
var forPrefix = " for ";
var untilPrefix = " until ";


function get_until() {
	var until =$("#until").val().trim();
    return (until.length == 0) ?  "" : untilPrefix + until;
}

function get_currency() {
    var currency = currency_tags.get_active_tags_text();
    return (currency.length == 0) ?  "" : forPrefix + currency;
}

function get_location() {
    var location =$("#location").val().trim();
    return (location.length == 0) ?  "" : locationPrefix + location + locationSuffix;
}

function get_offer() {
	var offer =$("#offer").val().trim();
    return (offer.length == 0) ?  offerPrefix + ".." : offerPrefix + offer;
}

function get_tags() {
	if (selected_tags.length == 0) return "";
	return " #" + selected_tags.join(" #");
}

function get_imagelink() {
	var picture =$("#picture").val().trim();
    return (picture.length == 0) ?  "" : " " + picture;
}

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
			 get_tags() +
			 get_imagelink() +
			 " #ooooby";
    $("#message_to_send").val(concatMessage);
}
/* tag selection code */
function parse_offer(){
        var message_data = {
        message: $("#message_to_send").val()
        };
        //encodeURIComponent()
        //$.post(container.parse_uri,message_data, display_results_of_parse_offer, "json");
        $.getJson(container.parse_uri,message_data, display_results_of_parse_offer);
     }

function display_results_of_parse_offer(response){
    var reasons = response.validationFailReasons;
    if(reasons.length==0){
    $(".send_message").removeAttr("disabled");
    }
    else $(".send_message").attr("disabled","disabled");
}

var selected_tags = [];
var tags = [];
var threshold = 200;
var keyChangeStack=0;

function timeoutKeyChange() {
 keyup(update_tags,threshold);
	/*keyChangeStack++;
	setTimeout(function() {
		keyChangeStack--;
		if (keyChangeStack == 0) {
			update_tags();
		}
	}, threshold);*/
}

function ontag_click() {
	var tagField = $("#tags").val();
	var pushed_tag = $(this).html();
	if ($.inArray(pushed_tag, selected_tags) <= -1) {
		if (tagField != "")
			$("#tags").val(tagField + " " + pushed_tag);
		else $("#tags").val(pushed_tag);
	}
	else {
		//var txt = new RegExp("(,\s*" + pushed_tag + "\s*)|(^\s*" + pushed_tag + "\s*,*)");
		var replacementText = tagField.replace(pushed_tag, "");
		$("#tags").val(replacementText);
	}
	update_tags();
	update_and_dont_parse(); 
	return false; //disable actual click
}

function getTagString() {
	var tagString=""
	$.each(selected_tags, function() {
		tagString = tagString + " #" + this;
	});
	return tagString;
}

function checkCSS() {
	$.each($(".select_tag"), function() {
	if ($.inArray($(this).html(), selected_tags) > -1) {
			$(this).addClass("on");
		}
	else if ($(this).hasClass("on"))$(this).removeClass("on");
	});
}


function update_tags() {

	//get rid of multiple spaces...
	var txt = new RegExp("\\s\\s+");
	$("#tags").val($("#tags").val().replace(txt, " "));
	//just in case a single \t or \n is present
	txt = new RegExp("\\s+");
	//var txt = new RegExp("\\s\\s*"); 
	var tagString=$("#tags").val().replace(txt, " ").trim();
	$("#tags").val(tagString);
	//if(selected_tags.length>0) 
	if(tagString!=""&&tagString!=" ")
	selected_tags = tagString.split(" ");
	else selected_tags=[];
	var selectedTagsHTML = $(selected_tags).map(function() {
		return "#" + this;
	}).get().join(", ");

	$("#selected_tags").html(selectedTagsHTML);
	
	var json_url = build_tags_query(this.container.tags_uri);
	$.getJSON(json_url, function(context) {
		tags = context.tags_json.overall;
		var tagString = "";
		$.each(tags, function() {
			var on = "";
			var endon = "";
			if (($.inArray(this.tag, selected_tags) > -1)) {
				on = "<li class=\"on\">";
				endon = "</li>";
			}
			else {
				on = "<li>";
				endon = "</li>";
			}
			tagString = (tags === "") ? this.tag : tagString + "\n" + on + "<a href=\"#\" class=\"select_tag\">" + this.tag + "</a>" + endon;
		});  
		
		//alert(tagString);
		//if (!(typeof suggested_tags_render_fn == 'function')) {
		//if not yet compiled compile it
		/*    compile_render_fn();
		
		var render = $p.render('suggested_tags_render_fn', tags);
		*/
		$('#suggested_tags').html(tagString);
		$("#suggested_tags .select_tag").click(ontag_click);
	});
}


function build_tags_query(baseUrl) {
	if (tags.length == 0) return baseUrl + "?jsoncallback=?";
	var query = "";
	$.each(tags, function() {
		if($.inArray(this.tag,selected_tags)>-1)
			query = query + this.type + "=" + escape(this.tag) + "&";
	});
	query = query.substring(0, query.length - 1);
	return baseUrl + "?" + query + "&jsoncallback=?";
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
var address_keyup_threshold = 200;
var address_keyup_stack=0;

function google_initialize() {
	geocoder = new google.maps.Geocoder();
	var myOptions = {
	  zoom: 13,
	  navigationControl: true,
      scaleControl: true,
	  mapTypeControl:false,
	  mapTypeId: google.maps.MapTypeId.TERRAIN,
	  
	}
	map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
	
	//funny but in quick 'hallway testing' with Louise the fact that twas already filled
	// in caused great confusion, so just leave it blank for them to fill in
	
	//if (google.loader.ClientLocation) {
	//		currentLocation = google.loader.ClientLocation;
	//		var currentAddr = currentLocation.address.city + ", " + currentLocation.address.region + ", " + currentLocation.address.country_code
	//		$(".location #location").val(currentAddr);
	//		geo_code_address();
	//	}

	$(".location  #location").keyup(function(){keyup(geo_code_address,address_keyup_threshold);});
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
	}, address_keyup_threshold);
}
function message_keyup(){
    keyup(parse_offer,threshold);
}
function keyup(fun,threshold) {
	address_keyup_stack++;
	setTimeout(function() {
		address_keyup_stack--;
		if (address_keyup_stack == 0) {
			fun();
		}
	}, threshold);
}
//function address_geocoded(point) {
//	map.clearOverlays();
//	if (!point) {
//		//$("#map_canvas").hide();
//		//$(".location .infobox").html(".. unable to understand that address..");
//		map.clearOverlays();
//	}
//	else {
//		//$("#map_canvas").show();
//		map.setCenter(point, 13);
//		var marker = new GMarker(point);
//		map.addOverlay(marker);
//	}
//}

google.load("maps", "3",  {callback: google_initialize, other_params:"sensor=false"});




// on load   
//openForm();
//sendData(); 


