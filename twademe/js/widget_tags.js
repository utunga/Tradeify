﻿

function Tag() {
    this.tag = "";
    this.type = "tag";
    this.active = false;
    this.fixed = false;
    this.on_remove;
};

function Tags() {

    if (!(this instanceof arguments.callee))
        return new Tags(); //ensure context even if someone forgets to create via 'new'

    var tags = [];
    
    var load_from_simple_array = function(tags_array, tag_type) {
        tags = [];
        $.each(tags_array, function() {
            add_tag(this.toString(), tag_type, false);
        });
    }

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
        var on_remove_tag = (arguments.length > 3) ? arguments[3] : null;
        var tag = new Tag();
        tag.on_remove = on_remove_tag;
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
                    if ($.isFunction(this.on_remove)) this.on_remove();
                }
            }
        });
        tags = tmp;
        return found_tag_to_remove;
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
    var is_active = function(text) {
        var existing = find_tag(text)
        if (!!existing) {
            return existing.active;
        }
    }
    
    var get_all_tags = function() {
        var all_tags = [];
        $.each(tags, function() {
            all_tags.push(this.tag.toString());
        });
        return all_tags;
    }
    var get_all_tags_objects = function() {
        var all_tags = [];
        $.each(tags, function() {
            all_tags.push(this);
        });
        return all_tags;
    }
  
    var get_active_tags = function() {
        var active_tags = [];
        $.each(tags, function() {
            if (this.active) active_tags.push(this.tag.toString());
        });
        return active_tags;
    }
  
    var get_fixed_tags = function() {
        var fixed_tags = new Array();
        $.each(tags, function() {
            if (this.fixed == true) fixed_tags.push(this.tag);
        });
        return fixed_tags;
    }

    var decorate_url = function(baseUrl) {
        var arg_tags = (arguments.length > 1) ? arguments[1] : tags;
        var query = $(arg_tags).map(function() {
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
    this.is_active = is_active;
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
    this.get_all_tags = get_all_tags;
    this.get_all_tags_objects = get_all_tags_objects;
    this.get_fixed_tags = get_fixed_tags;
    this.load_from_simple_array = load_from_simple_array;
    this.get_tags_array = function() {
        return tags;
    }

};

//////////////////////////////////////////////////////////////////////

function TagsWidget(selector) {

    var tags;
    var _tag_click_ref;
    var _initial_tags;
    var _active_tags;
    var _tag_type;
   
    var init = function() {
        tags = new Tags();
    }

    var init_from = function(initial_tags, active_tags, tag_type) {
        tags = new Tags();
        if (!_initial_tags) _initial_tags = initial_tags;
        _tag_type = tag_type;
        $.each(initial_tags, function() {
            var active = ($.inArray(this.toString(), active_tags) > -1);
            tags.add_tag(this.toString(), tag_type, active);
        });
        update_view();
    }

    var update_view = function() {
        if ($(selector)) {
            $(selector).html(_get_html());
            //fix the onclick (again) as html has been reset
            $(selector + " .fg-buttonset a.tag").click(_tag_click);
            //$("#current_tags .tag-unfixed").addClass("ui-tag-close");
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
    var add_close_button_ref = function(tag_active, tag_fixed) {
        return tag_active;
    }
    var _get_html = function() {
        var max_tag_count = 15;
        var tagString = "<div class=\"fg-buttonset fg-buttonset-multi\">";
        var tags_array = tags.get_tags_array();
        for (var i = 0; i < tags_array.length && i < max_tag_count; i++) {
            var tag = tags_array[i];
            var ui_state_class = (tag.active) ? "ui-state-active" : "";
            var ui_tag_close = (add_close_button_ref(tag.active,tag.fixed)) ? "ui-tag-close" : "";
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
                "<span class=\"ui-icon " + ui_icon_class + "\"></span>" + tag.tag + "<span class=\"" + ui_tag_close + "\"></span></a>";
        }

        tagString = tagString + "</div>";
        return tagString;
    }

    init();

    // public methods
    this.reset = function() {
        init_from(_initial_tags, [], _tag_type);
        update_view();
    }
    
    this.on_tag_click = on_tag_click;
    this.update_view = update_view;
    this.init_from = init_from;
   
    // note the external caller has to remember to call update_view() after
    // calling these methods if they want the rendered html to change
    
    //ARGH! OK here was the bug the next line is *NOT* the same as below
    //this.get_active_tags = return tags.get_active_tags;
        
    this.get_active_tags = function() {
        return tags.get_active_tags();
    };
    this.set_add_close_button_ref = function(ref) { add_close_button_ref=ref }
    this.is_active = function(text) { return tags.is_active(text); }
    this.get_all_tags = function() { return tags.get_all_tags(); }
    this.find_tag = function(text) { return tags.find_tag(text); };
    this.has_tag = function(text) { return tags.has_tag(text); };
    this.toggle_filter = function(text) { return tags.toggle_filter(text) };
    this.add_tag = function(text, type, active, on_remove_tag) { return tags.add_tag(text, type, active, on_remove_tag); }; //return tags.add_tag.apply(tags, arguments); }; //uhm is this right? FIXME
    this.add_fixed_tag = function(text, type, active) { return tags.add_fixed_tag(text,type, active); }; //return tags.add_fixed_tag.apply(tags, arguments); }; //uhm is this right for variable args //FIXME
    this.remove_tag = function(text) { return tags.remove_tag(text); };
    this.remove_fixed_tag = function(text) { return tags.remove_fixed_tag(text); };
    this.toggle_active = function(text) { return tags.toggle_active(text); };
    this.get_fixed_tags = function() { return tags.get_fixed_tags(); };
    this.decorate_url = function(baseUrl) { return (arguments.length > 1) ? tags.decorate_url(baseUrl, arguments[1]) : tags.decorate_url(baseUrl); };
    this.decorate_active_url = function(baseUrl) { return tags.decorate_active_url(baseUrl); };
    this.get_active_tags_text = function() { return tags.get_active_tags_text(); };
    this.get_all_tags_text = function() { return tags.get_all_tags_text(); };
    this.get_all_tags_objects = function() { return tags.get_all_tags_objects(); };
};

////////////////////////////////////////////////////////////////////////

function SuggestedTagsWidget(selector, general_tagset) {

    if (!(this instanceof arguments.callee))
        return new SuggestedTagsWidget(selector, general_tagset); //ensure context even if someone forgets to create via 'new'

    var tags_widget;
    var tag_type = "tag";
    var _tags_updated_fn = new Array();
    
    var init = function() {
        tags_widget = new TagsWidget(selector);
        tags_widget.on_tag_click(tags_widget_click);
        set_suggested_from_array(general_tagset);
    }
    
    var set_suggested_from_array = function(suggested_tags) {

        var tag_string_before = tags_widget.get_all_tags_text();
       
        tags_widget.init_from(suggested_tags, tags_widget.get_active_tags(), tag_type);
        
        tags_widget.update_view();
        //tags_widget.on_tag_click(tags_widget_click);
        var tag_string_after = tags_widget.get_all_tags_text();
        if (tag_string_before!=tag_string_after) {
            $(selector).effect("highlight", {}, 3000);
        }
        
        if (!!_tags_updated_fn) {
            $.each(_tags_updated_fn, function() {
                this(tags_widget);
            });
        }
     }
    
     var on_tags_updated = function(functionRef) {
        _tags_updated_fn.push(functionRef);
    }

    var update_suggested_tags = function(source) {
    
        if (tags_widget.get_active_tags().length == 0) {
            // if no tags currently, just set to the main categories
            set_suggested_from_array(general_tagset);
        }
        else {
        
            // otherwise, get suggested tags based on currently active tags
            var json_url = tags_widget.decorate_active_url(container.tags_uri + "?type=tag");
            $(selector).parent().block(block_message);
            
            setTimeout(function() {
                $(selector).parent().unblock();
            }, 2000);
            
            $.getJSON(json_url, function(data) {
                var suggested_tags = [];

                // make sure all the currently active tags are included
                // (this would be ensured by the data, except for the faking out by fake tags)
                $.each(tags_widget.get_active_tags(), function() {
                    suggested_tags.push(this.toString());
                });

                $.each(data, function() {
                    if (this.type == tag_type) {
                        suggested_tags.push(this.tag);
                    }
                });


                set_suggested_from_array(suggested_tags);
                $(selector).parent().unblock();
            });
        }
    };

    var tags_widget_click = function(tag_text, tag_type) {
        //ensure that active tags are in the details box
        if (tags_widget.is_active(tag_text) == false) remove_tag_from_details(tag_text);
        ensure_details_includes_active_tags();       
        update_suggested_tags();

    }

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

    this.add_custom_tag = function(tag_text) {
        if (tags_widget.has_tag(tag_text)) {
            tags_widget.toggle_active(tag_text);
        }
        else {
            tags_widget.add_tag(tag_text, tag_type, true);
        }
    };
    
    this.update_view = function() {
        tags_widget.update_view();
    }

    this.has_tag = function(text) { return tags_widget.has_tag(text); };

    this.remove_tag = function(text) { return tags_widget.remove_tag(text); };

    this.add_tag = function(text, type, active) { return tags_widget.add_tag(text, type, active); }; //return tags.add_tag.apply(tags, arguments); }; //uhm is this right? FIXME

    this.set_active = function(text) {
        var setActive = (arguments.length > 1) ? arguments[1] : true;
        var tag = tags_widget.find_tag(text);
        if (!setActive) tags_widget.toggle_active(text);
        else tag.active = true;
    }
    this.get_active_tags = function() {
    return tags_widget.get_active_tags();
    };    
    this.on_tags_updated = on_tags_updated;
    
};

