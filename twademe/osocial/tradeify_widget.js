// JavaScript Document


/* ------------------------------
     rendering of widget 
   ------------------------------*/

var offersJson=-1; 
var selected_tags=new Array(); 

function compile_render_functions() {
    // most complicated is rendering to offers div
    var offers = jQuery('#results_by_date .template').mapDirective({
        'div.offer': 'offer <- messages',
        'a.username[href]': 'offer.user.more_info_url',
        'a.username': 'offer.user.screen_name',
		'.avatar img[src]': 'offer.user.profile_pic_url',
        '.msg .text': 'offer.offer_text',
        '.msg a.more_info_link[href]': '#{offer.more_info_url}',
        '.when': 'offer.date'
	});
    //// '.msg a.thumb[href]': '#{offer.thumbnail_url}',
    //.msg img[src]': 'offer.thumbnail_url',

    var tagsList = jQuery('span.tags', offers).mapDirective({
        '.tag': 'tag <- offer.tags',
        'a[href]': 'tag.tag',
        'a': 'tag.tag'
    });

    jQuery('span.tags', offers).html(tagsList); //place sub-template tagsList into offers template
    $p.compile(offers, 'offers_render_fn'); //compile to a function
	//gadgets.window.adjustHeight(5000);
    /// -- end offers function
}
          
function update_offers(json_url) {
    var json_url=build_search_query("http://tradeify.org/offers_json.aspx"); //?jsoncallback=?
    jQuery.getJSON(json_url, function(context) {
        //alert("context:" + context);
        offersJson=context;
        jQuery('#results_by_date').html($p.render('offers_render_fn', context));
    });
        
}


/* ------------------------------
     search
   ------------------------------*/
   
function search(){
    alert("start searching");
    var tag=document.forms["search"].searchdata.value;
    alert("searching for "+tag);	
    var newMessages=new Array();
    var arrayCount=0;
    alert("itterating through messages");
    for(var i=0;i<offersJson.messages.length;i++){
        var msg=offersJson.messages[i];
        //alert("message "+msg.offer_text);
        for(var j=0;j<msg.tags.length;j++){
            if(msg.tags[j].tag==tag){				
                alert("found tag "+msg.tags[j].tag);
                newMessages[arrayCount]=msg;
                arrayCount++;
            }
        }
    }
    offersJson=newMessages;
    alert("found "+newMessages.length);
    jQuery('#results_by_date').html($p.render('offers_render_fn', newMessages));
} 	
    
   
function queryServer(){
    var query=document.forms["search"].searchdata.value;
    var queryIdx=jQuery.inArray(query,selected_tags);
    if(queryIdx!=-1){	
        return;
    }
    selected_tags.push(query);
    update_offers();
}

function build_search_query(baseUrl) {
    var query="";
    for(tag in selected_tags) {
        if(query!="")
            query = query + "&";
        query = query+"tag="+selected_tags[tag];
    }
    return baseUrl + "?" + query+"&jsoncallback=?";
}
 
    
/* ------------------------------
     sending data from form
   ------------------------------*/
   
function onGetData(data) {
    var message=document.forms["myform"].message.value;
    alert("in my form "+message);
    var viewer = data.get('viewer').getData();
    var viewerJson= gadgets.json.stringify(viewer);
    var name = viewer.getDisplayName();
    var thumbnail= viewer.getField(opensocial.Person.Field.THUMBNAIL_URL,null);
    alert("display name: "+ name);  

    //REMOVE LATER
    var dataPacket = {};
    dataPacket.Message = message;
    dataPacket.User = viewer;
    var dataPacketJSON = gadgets.json.stringify(dataPacket);
    
    var mes = {
        RawMessage:message,
        UserName:name,
        Thumbnail:thumbnail,
        DataPacket:dataPacketJSON
    };
    makeRequest("http://tradeify.org/accept_post.aspx",mes);
} 

function sendData(){
    var req = opensocial.newDataRequest();
    req.add(req.newFetchPersonRequest(opensocial.IdSpec.PersonId.VIEWER), 'viewer');
    req.send(onGetData);
}

function makeRequest(url, postdata) {
    var params = {};
    postdata = gadgets.io.encodeValues(postdata);
    params[gadgets.io.RequestParameters.METHOD] = gadgets.io.MethodType.POST;
    params[gadgets.io.RequestParameters.POST_DATA]= postdata;
    var response=-1;
    gadgets.io.makeRequest(url,response , params);
}


/* ------------------------------
     form related function
   ------------------------------*/
		
jQuery('#create-user').click(function() {
    jQuery('#dialog').dialog('open');
}).hover(
    function(){ 
        jQuery(this).addClass("ui-state-hover"); 
    },
    function(){ 
        jQuery(this).removeClass("ui-state-hover"); 
    }
).mousedown(function(){
    jQuery(this).addClass("ui-state-active"); 
})
.mouseup(function(){
        jQuery(this).removeClass("ui-state-active");
});

function MessagePart(type, field) {
    this.prefix = type;
    this.field = field;
}

var offerPrefix = " I am offering ";
var locationPrefix = " in L:";
var locationSuffix = ": ";
var forPrefix = " for ";
var untilPrefix = " until ";
var linkPrefix = "here is a link to an image: ";
function getUntil() {
    if (jQuery("#until").val().length == 0) {
        return "";
    }
    else {
        return untilPrefix + jQuery("#until").val();
    }
}

function updateOffer() {
    var concatMessage =
             offerPrefix +
             jQuery("#offer").val() +
             locationPrefix + jQuery("#location").val() + locationSuffix +
             forPrefix + jQuery("#for").val() +
             getUntil();
    jQuery("#message").val(concatMessage);
}
var selected_tags = [];
var tags = [];

function onClick() {
    var tagField = jQuery("#tags").val();
    var pushed_tag = jQuery(this).html();
    if (jQuery.inArray(pushed_tag, selected_tags) <= -1) {
        if (tagField != "")
            jQuery("#tags").val(tagField + " " + pushed_tag);
        else jQuery("#tags").val(pushed_tag);
    }
    else {
        //var txt = new RegExp("(,\s*" + pushed_tag + "\s*)|(^\s*" + pushed_tag + "\s*,*)");
        var replacementText = tagField.replace(pushed_tag, "");
        jQuery("#tags").val(replacementText);
    }
    updateTags();
}

function getTagString() {
    var tagString = "";
    jQuery.each(selected_tags, function() {
        tagString = tagString + " #" + this;
    });
    return tagString;
}

function checkCSS() {
    jQuery.each(jQuery(".select_tag"), function() {
        if (jQuery.inArray(jQuery(this).html(), selected_tags) > -1) {
            jQuery(this).addClass("on");
        }
        else if (jQuery(this).hasClass("on")) jQuery(this).removeClass("on");
    });
}

var threshold = 200;
var keyChangeStack = 0;
function timeoutKeyChange() {
    keyChangeStack++;
    setTimeout(function() {
        keyChangeStack--;
        if (keyChangeStack == 0) {
            updateTags();
        }
    }, threshold);
}

function updateTags() {

    //get rid of multiple spaces...
    var txt = new RegExp("\\s\\s+");
    jQuery("#tags").val(jQuery("#tags").val().replace(txt, " "));
    //just in case a single \t or \n is present
    txt = new RegExp("\\s+");
    jQuery("#tags").val(jQuery("#tags").val().replace(txt, " "));

    selected_tags = jQuery("#tags").val().split(" ");

    var selectedTagsHTML = jQuery(selected_tags).map(function() {
        return "#" + this;
    }).get().join(", ");


    jQuery("#selected_tags").html(selectedTagsHTML);

    var json_url = build_search_query_tags("http://tradeify.org/tags_json.aspx");
    jQuery.getJSON(json_url, function(context) {
        tags = context.tags_json.overall;
        var tagString = "";
        jQuery.each(tags, function() {
            var on = "";
            var endon = "";
            if ((jQuery.inArray(this.tag, selected_tags) > -1)) {
                on = "<li class=\"on\">";
                endon = "</li>";
            }
            else {
                on = "<li>";
                endon = "</li>";
            }
            tagString = (tags === "") ? this.tag : tagString + "\n" + on + "<a href=\"#\" class=\"select_tag\">" + this.tag + "</a>" + endon;
        });

        jQuery('#suggested_tags').html(tagString);
        jQuery("#suggested_tags .select_tag").click(onClick);
    });
}

function build_search_query_tags(baseUrl) {
    var query = "";
    jQuery.each(tags, function() {
        if (jQuery.inArray(this.tag, selected_tags) > -1)
            query = query + this.type + "=" + escape(this.tag) + "&";
    });
    query = query.substring(0, query.length - 1);
    return baseUrl + "?" + query +"&jsoncallback=?";
}

 
//tooltip courtesy Alen Grakalic (http://cssglobe.com)
// visit http://cssglobe.com/post/1695/easiest-tooltip-and-image-preview-using-jquery
this.tooltip = function(){	
	/* CONFIG */		
		xOffset = 10;
		yOffset = 20;		
		// these 2 variable determine popup's distance from the cursor
	/* END CONFIG */		
	$("a.tooltip").hover(
		function(e){											  
			this.t = this.title;
			this.title = "";									  
			$("body").append("<p id='tooltip'>"+ this.t +"</p>");
			$("#tooltip")
				.css("top",(e.pageY - xOffset) + "px")
				.css("left",(e.pageX + yOffset) + "px")
				.fadeIn("fast");		
		},
		function(){
			this.title = this.t;		
			$("#tooltip").remove();
		}
	);	
	$("a.tooltip").mousemove(function(e){
		$("#tooltip")
			.css("top",(e.pageY - xOffset) + "px")
			.css("left",(e.pageX + yOffset) + "px");
	});			
};


// on load   
    //openForm();
    //sendData(); 


