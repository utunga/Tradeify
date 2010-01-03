// JScript source code

function Tag() {
    this.text = "";
    this.type = "tag";
    this.active = false;
}

function Tags() {

    this.tags = [];
    this.target_selector;

    this.add_tag_type_active = function(type, text, active) 
    {
        var tag = new Tag();
        tag.type = type;
        tag.text = text;
        tag.active = active;
        this.tags.push(tag);
        this.update_view();
    }
    
    this.add_tag_type = function(type, text) 
    {
        this.add_tag_type_active(type, text, true);
        this.update_view();   
    }
    
    this.add_tag = function(text) {
        this.add_tag_type("tag", text);   
    }
    
    this.remove_tag = function(text) {
        var found_tag_to_remove = false;
        var tmp = []; 
        $.each(this.tags, function() {
            if (text != this.text) {
                tmp.push(this);
            }
            else {
                found_tag_to_remove = true;
            }
        });
        this.tags = tmp;
        this.update_view();
        return found_tag_to_remove;
    }
    
    this.find_tag = function(text) {
        var found_tag = null;
         $.each(this.tags, function() {
            if (text == this.text) {
                found_tag = this;
            }
        });
        return found_tag;
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
        	
	        var ui_state_class = (this.active) ?  "ui-state-active" : "ui-state-default";
	        var ui_icon_class;
	        switch(this.type) {
	            case("group"):
	                ui_icon_class = "ui-icon-person";
	                break;
	            case("type"):
	                ui_icon_class = "ui-icon-currency";
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