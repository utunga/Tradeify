
/* ------------------------------
     couple useful bits and pieces  
   ------------------------------*/
   
String.prototype.trim = function() {
	return this.replace(/^\s+|\s+$/g,"");
}
String.prototype.ltrim = function() {
	return this.replace(/^\s+/,"");
}
String.prototype.rtrim = function() {
	return this.replace(/\s+$/,"");
}

//define console.log so that we can log to firebug console, but not get errors if people don't have firebug installed
if (!console) {
        var console = {}
    console.log = function(text) {
        return; //ie do nothing
    }
}


function Tag() {
    this.text = "";
    this.type = "tag";
    this.active = false;
    this.fixed = false;
}

function Tags(target_selector) {

    if (!(this instanceof arguments.callee)) 
        return new Tags(target_selector); //ensure context even if someone forgets to create via 'new'
        
    this.tags = [];
    this.target_selector = target_selector;

    this.find_tag = function(text) {
        var found_tag = null;
         $.each(this.tags, function() {
            if (text == this.text) {
                found_tag = this;
            }
        });
        return found_tag;
    }
    
    this.add_tag = function(text) 
    {
        if (!!this.find_tag(text)) {
            console.log("refuse to add tag " + text + " as it already exists");
            return;
        }
        
        var type = (arguments.length>1) ? arguments[1] : "tag";
        var active = (arguments.length>2) ? arguments[2] : false;
      
        var tag = new Tag();
        tag.type = type;
        tag.text = text;
        tag.active = active;
        this.tags.push(tag);
        this.update_view();
        return tag;
    }
    
    // a fixed tag can't be removed by 'normal' add/remove operations
    // you have to call explicit 'remove_fixed_tag' operation
    this.add_fixed_tag = function(text) 
    {
        var existing = this.find_tag(text);
        if (!!existing) {
            existing.fixed = true;
            return existing;
        }
        else {
            var new_tag = this.add_tag.apply(this,arguments);
            new_tag.fixed = true;
            return new_tag;
        }
    }
     
    this.remove_tag = function(text) {
        text = text.trim();
        var found_tag_to_remove = false;
        var tmp = []; 
        //removes all instances of tags with this text (in the case there is more than one)
        $.each(this.tags, function() {
            if (text != this.text) {
                tmp.push(this);
            }
            else {
                if (this.fixed) 
                {
                    console.log("won't remove fixed tag " + text);
                     tmp.push(this);
                }
                else 
                {
                    found_tag_to_remove = true;
                }
            }
        });
        this.tags = tmp;
        this.update_view();
        return found_tag_to_remove;
    }
     
    this.remove_fixed_tag = function(text) {
        var existing = this.find_tag(text)
        if (!!existing) {
            existing.fixed = false;
        }
        this.remove_tag.apply(arguments);
    }
    this.get_fixed_tags = function() {
        var fixed_tags = new Array();
        $.each(this.tags, function() {
            if (this.fixed == true) fixed_tags.push(this);
        });
        return fixed_tags;
    }
    this._tag_click_ref = function () {
        return false;
    }  
      
    this.tag_click = function(functionRef) {
        this._tag_click_ref = functionRef;
        //set the onclick to the externally provided click function
        $(".fg-buttonset a.tag").click(this._tag_click_ref);
    }
    
    this.update_view = function() {
        if ($(this.target_selector)) {
            $(this.target_selector).html(this.get_html());
        }
        
        //hover and click view changes for tag links
        $(".fg-button:not(.ui-state-disabled)")
        .mousedown(function(){
		        if( $(this).is('.ui-state-active.fg-button-toggleable, .fg-buttonset-multi .ui-state-active') )
			        { $(this).removeClass("ui-state-active"); }
		        else { $(this).addClass("ui-state-active"); }	
        })
        .mouseup(function(){
	        if(! $(this).is('.fg-button-toggleable, .fg-buttonset-multi .fg-button') ){
		        $(this).removeClass("ui-state-active");
	        }
        });
        
        //re-attach click event
         $(".fg-buttonset a.tag").click(this._tag_click_ref);
        
    }
    
    this.decorate_url = function(baseUrl) {
        var query = $(this.tags).map(function() {
            return this.type + "=" + escape(this.text);
        }).get().join("&");
        return baseUrl + "?" + query + "&jsoncallback=?";
    }

    this.get_html = function() {

        var tagString = "<div class=\"fg-buttonset fg-buttonset-multi\">";
        $.each(this.tags, function() {
        	
	        var ui_state_class = (this.active) ?  "ui-state-active" : "";
	        var ui_icon_class;
	        switch(this.type) {
	            case("group"):
	                ui_icon_class = "ui-icon-person";
	                break;
	              case("loc"):
                    //FIXME the icon here is not great - a map would be beter
                    // eg http://famfamfam.com/lab/icons/mini/icons/page_url.gif
	                ui_icon_class = "ui-icon-image";
	                break;
	            case("type"):
	                //FIXME created the icon 'ui-icon-currency' but has bad jaggies so use this for now
	                ui_icon_class = "ui-icon-transfer-e-w";
	                break;
	            default:
	                ui_icon_class = "ui-icon-tag";
	        }
        	
	        tagString = tagString + "\n" +
            "<a href=\"#\" class=\"tag fg-button fg-button-icon-left " + ui_state_class + " ui-corner-tag\">\n"+
                "<span class=\"ui-icon " + ui_icon_class + "\"></span>" + this.text + "</a>";
        });

        tagString = tagString + "</div>";
        return tagString;
    }

    this.get_active_tags_text = function() {
    	var tagString="";
	    $.each(this.tags, function() {
	        if(this.active) {
		        tagString = tagString + " #" + this.text;
		    }
	    });
	    return tagString;
    }
}