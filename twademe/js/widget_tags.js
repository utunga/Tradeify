

function Tag() {
    this.tag = "";
    this.type = "tag";
    this.active = false;
    this.fixed = false;
}

function Tags() {

    if (!(this instanceof arguments.callee))
        return new Tags(); //ensure context even if someone forgets to create via 'new'

    var tags = [];

    var find_tag = function(text) {
        var found_tag = null;
        $.each(tags, function() {
            if (text == this.tag) {
                found_tag = this;
            }
        });
        return found_tag;
    };


    var has_tag = function(text) {
        var found = false;
        $.each(tags, function() {
            if (text == this.tag) {
                found = true;
            }
        });
        return found;
    };

    var toggle_filter = function(text) {
        if (!!find_tag(text)) {
            remove_tag(text);
        }
        else {
            add_tag(text, (arguments.length > 1) ? arguments[1] : "tag", (arguments.length > 2) ? arguments[2] : false);
        }
    };
    
    var add_tag = function(text) {
        if (!!find_tag(text)) {
            console.log("refuse to add tag " + text + " as it already exists");
            return;
        }
        var type = (arguments.length > 1) ? arguments[1] : "tag";
        var active = (arguments.length > 2) ? arguments[2] : false;

        var tag = new Tag();
        tag.type = type;
        tag.tag = text;
        tag.active = active;
        
        tags.push(tag);
        return tag;
    }
    
    // a fixed tag can't be removed by 'normal' add/remove operations
    // you have to call explicit 'remove_fixed_tag' operation
    var add_fixed_tag = function(text) {
        var existing = find_tag(text);
        if (!!existing) {
            existing.fixed = true;
            return existing;
        }
        else {
            var new_tag = add_tag.apply(this, arguments);
            new_tag.fixed = true;
            return new_tag;
        }
    }

    var remove_tag = function(text) {
        text = text.trim();
        var found_tag_to_remove = false;
        var tmp = [];
        //removes all instances of tags with this text (in the case there is more than one)
        $.each(tags, function() {
            if (text != this.tag) {
                tmp.push(this);
            }
            else {
                if (this.fixed) {
                    console.log("won't remove fixed tag " + text);
                    tmp.push(this);
                }
                else {
                    found_tag_to_remove = true;
                }
            }
        });
        tags = tmp;
        return found_tag_to_remove;
    }
    
    var get_active_tags = function() {
        var active_tags = new Array();
        $.each(tags, function() {
            if (this.active) active_tags.push(this.tag.toString());
        });
        return active_tags;
    }
    
    var remove_fixed_tag = function(text) {
        var existing = find_tag(text)
        if (!!existing) {
            existing.fixed = false;
        }
        remove_tag.apply(arguments);
    }

    var toggle_active = function(text) {
        var existing = find_tag(text)
        if (!!existing) {
            existing.active = !existing.active;
        }
    }

    var get_fixed_tags = function() {
        var fixed_tags = new Array();
        $.each(tags, function() {
            if (this.fixed == true) fixed_tags.push(this.tag);
        });
        return fixed_tags;
    }
    
    var decorate_url = function(baseUrl) {
        var query = $(tags).map(function() {
            return this.type + "=" + escape(this.tag);
        }).get().join("&");
        return baseUrl + "?" + query + "&jsoncallback=?";
    }
    
    var decorate_active_url = function(baseUrl) {
        var query = "";
        $.each(tags, function() {
            if (this.active) query += "&" + this.type + "=" + escape(this.tag);
        });

        return baseUrl + query + "&jsoncallback=?";
    }

    var get_active_tags_text = function() {
        var tagString = "";
        $.each(tags, function() {
            if (this.active) {
                tagString = tagString + " #" + this.tag;
            }
        });
        return tagString;
    }
    
     var get_all_tags_text = function() {
        var tagString = "";
        $.each(tags, function() {
            tagString = tagString + " #" + this.tag;
        });
        return tagString;
    }

    // make all methods public
    this.find_tag = find_tag;
    this.has_tag = has_tag;
    this.toggle_filter = toggle_filter;
    this.add_tag = add_tag;
    this.add_fixed_tag = add_fixed_tag;
    this.remove_tag = remove_tag;
    this.get_active_tags = get_active_tags;
    this.remove_fixed_tag = remove_fixed_tag;
    this.toggle_active = toggle_active;
    this.decorate_url = decorate_url;
    this.decorate_active_url = decorate_active_url;
    this.get_active_tags_text = get_active_tags_text;
    this.get_all_tags_text = get_all_tags_text;
    this.get_fixed_tags = get_fixed_tags;
    this.get_tags_array = function() {
        return tags;
    }

}

//////////////////////////////////////////////////////////////////////

function TagsWidget(selector) {

    var tags;
    var _tag_click_ref;
   
    var init = function() {
        tags = new Tags();
        _tag_click_ref = function() {} //no-op
    }
    
    var init_from = function(initial_tags, active_tags, tag_type) 
    {
        tags = new Tags();
        $.each(initial_tags, function() {
            var isInActiveTags = !!active_tags.find_tag(this);
            tags.add_tag(this, tag_type, isInActiveTags);
        });
        update_view();
    }

    var update_view = function() {
        if ($(selector)) {
            $(selector).html(_get_html());
            //fix the onclick (again) as html has been reset
            $(selector + " .fg-buttonset a.tag").click(_tag_click);
        }
    }

    var on_tag_click = function(functionRef) {
        _tag_click_ref = functionRef;
    }

    var _tag_click = function() {
        var tag_text = $(this).text().replace("\n", "");
        tags.toggle_active(tag_text);
        
        var existing = tags.find_tag(tag_text);
        if (!!existing && _tag_click_ref != undefined) {
            _tag_click_ref(existing.tag, existing.type);
        }
        update_view();
        return false;
    }

    var _get_html = function() {
        var max_tag_count = 15;
        var tagString = "<div class=\"fg-buttonset fg-buttonset-multi\">";
        var tags_array = tags.get_tags_array();
        for (var i = 0; i < tags_array.length && i < max_tag_count; i++) {
            var tag = tags_array[i];
            var ui_state_class = (tag.active) ? "ui-state-active" : "";
            var ui_icon_class;
            switch (tag.type) {
                case ("group"):
                    ui_icon_class = "ui-icon-person";
                    break;
                case ("loc"):
                    //FIXME the icon here is not great - a map would be beter
                    // eg http://famfamfam.com/lab/icons/mini/icons/page_url.gif
                    ui_icon_class = "ui-icon-image";
                    break;
                case ("currency"):
                    //FIXME created the icon 'ui-icon-currency' but has bad jaggies so use this for now
                    ui_icon_class = "ui-icon-transfer-e-w";
                    break;
                default:
                    ui_icon_class = "ui-icon-tag";
            }

            tagString = tagString + "\n" +
            "<a href=\"#\" class=\"tag fg-button fg-button-icon-left " + ui_state_class + " ui-corner-tag\">\n" +
                "<span class=\"ui-icon " + ui_icon_class + "\"></span>" + tag.tag + "</a>";
        }

        tagString = tagString + "</div>";
        return tagString;
    }
     

    init();


    // public methods
    this.reset = init;
    this.on_tag_click = on_tag_click;
    this.update_view = update_view;
    this.init_from = init_from;
   
    // note the external caller has to remember to call update_view() after
    // calling these methods if they want the rendered html to change
    this.find_tag = tags.find_tag;
    this.has_tag = tags.has_tag;
    this.toggle_filter = tags.toggle_filter;
    this.add_tag = tags.add_tag;
    this.add_fixed_tag = tags.add_fixed_tag;
    this.remove_tag = tags.remove_tag;
    this.get_active_tags = tags.get_active_tags;
    this.remove_fixed_tag = tags.remove_fixed_tag;
    this.toggle_active = tags.toggle_active;
    this.get_fixed_tags = tags.get_fixed_tags;
    this.decorate_url = tags.decorate_url;
    this.decorate_active_url = tags.decorate_active_url;
    this.get_active_tags_text = tags.get_active_tags_text;
    this.get_all_tags_text = tags.get_all_tags_text;
};

////////////////////////////////////////////////////////////////////////

function SuggestedTagsWidget(selector, general_tagset) {
    var tags_widget;
    var tag_type = "tag";
    
    var init = function() {
        tags_widget = new TagsWidget(selector);
        tags_widget.on_tag_click(tags_widget_click);
        update_suggested_tags();
    }
    
    var tags_widget_click = function(tag_text, tag_type) {
        //update_and_dont_parse();
        update_suggested_tags();
    }

    
    var update_suggested_tags = function() {
    
        //store currently selected tags (if any) 
        var active_tags = tags_widget.get_active_tags();
        
        // (re)create the widget
        tags_widget = new TagsWidget(selector);
                
        // currently selected tags are the 'active' tags on the internal widget
        var tag_string_before = tags_widget.get_all_tags_text();
        var suggested_tags = [];        
        if (tags_widget.get_active_tags().length == 0) {
            suggested_tags = general_tagset;
        }
        else {
        
            // get suggested tags based on currently active tags
            var json_url = tags_widget.decorate_active_url(container.tags_uri + "?type=tag");
            $.getJSON(json_url, function(data) {
                $.each(data, function() {
                    if (this.type == tag_type) {
                        suggested_tags.push(this.tag);
                    }
                });
            });
        }

        // add suggested tags to the list
        $.each(suggested_tags, function() {
            var str = this.toString();
            var isSelected = ($.inArray(str, active_tags) >= 0);
            tags_widget.add_tag(str, tag_type, isSelected);
        });
        
        var tag_string_after = tags_widget.get_all_tags_text();
        if (tag_string_before!=tag_string_after) {
            $(selector).effect("highlight", {}, 3000);
        }
        
        tags_widget.update_view();
        tags_widget.on_tag_click(tags_widget_click);
    };

    init();

    this.reset = function() {
        tags_widget.reset();
    };
    
    this.get_active_tags_text = function() {
        return tags_widget.get_active_tags_text();
    };
    
    this.get_active_tags = function() {
        return tags_widget.get_active_tags();
    };
    
    this.length = function() {
        return tags_widget.get_tags_array().length;
    }
};

