function ListWidget(offers_selector, current_tags_selector, username) {

    var offers;
    var tags;
    var username_filter;
    var offers_render_fn;
    var current_tags;
    var offers_uri;
    var _offers_updated=new Array();
    var map;

    var offers_directives = {
        'div.offer': {
            'offer <- Messages': {
                'a.username@href': 'offer.created_by.profile_url',
                'a.avatar@href': 'offer.created_by.profile_url',
                'a.username': 'offer.created_by.provider_user_name',
                '.avatar img@src': 'offer.created_by.profile_pic_url',
                '.msg .text': 'offer.offer_text',
                
                'span.tags': {
                    'tag <- offer.tags': {
                        'a': 'tag.tag',
                        '+a@class': 'tag.type'
                    }
                },
                '.when': 'offer.timestamp',
                '.id': 'offer.message_pointer.message_id'  
            }
        }
    };

    var init = function() {
        username_filter = username;
        offers_uri = container.offers_uri;
        //compile to a function as soon as possible (ie in 'constructor')
        offers_render_fn = $(offers_selector + ' .template').compile(offers_directives);
        if (!username_filter) {
            current_tags = new TagsWidget(current_tags_selector);
            current_tags.set_add_close_button_ref(function(tag_active, tag_fixed) {
                return !tag_fixed;
            });
            current_tags.on_tag_click(function(tag_text, tag_type) {
                remove_filter(tag_text);
                return false;
            });
        }
    }

    var on_offers_updated = function(functionRef) {
        //ensure no duplicate functions are added
        if (!!$.inArray(functionRef, _offers_updated)) {
            _offers_updated.push(functionRef);
        }
    }
    
    var update_offers_change_threshold = 50; //50 milliseconds
    var update_offers_stack =0;
    
    var queue_update_offers = function() {
	    update_offers_stack++;
	    setTimeout(function() {
		    update_offers_stack--;
		    if (update_offers_stack == 0) {
			    update_offers()
		    }
	    }, update_offers_change_threshold);
    }
    

    var formatTimestamp=function(timestamp){
        var overall = timestamp.split("T");
        var dates = overall[0].split("-");
        var times = overall[1].split(":");
        var time = times[0] + ":" + times[1];
        var date = dates[2] + "-" + dates[1] + "-" + dates[0];
        return time + " " + date;
    }

    var update_offers = function(onUpdateCallback) {

        var json_url;
        //list widget
        $(".left_col").block({ message: "" });
        $(".right_col .fg-buttonset").block({ message: "" });
        if (!username_filter && !!current_tags_selector)
            json_url = current_tags.decorate_url(offers_uri); //standard
        else if (!!username_filter & !current_tags_selector)
            json_url = offers_uri + "?jsoncallback=?" + "&username=" + username_filter + "&namespace=ooooby"; //my list widget
        //else
          //  json_url = current_tags.decorate_url(offers_uri) + "&username=" + username_filter + "&namespace=ooooby"; //admin list widget

        $.getJSON(json_url, function(data) {
            $.each(data.Messages, function() {
                this.timestamp = formatTimestamp(this.timestamp);
            });
            $(offers_selector + ' .template').render(data, offers_render_fn);
            if (username_filter == null) {
                $(offers_selector + ' .tags a').click(function() {
                    //add a filter when tags under a message are clicked
                    add_filter($(this).text(), $(this).attr("class"));
                });
            }
            offers = data.Messages;
            tags = data.Tags;
            if (!!_offers_updated) {
                $.each(_offers_updated, function() {
                    this(offers);
                });
            }

            $(offers_selector + " .template").quickPager({ pageSize: 4 }, offers_selector + " .pager", offers_selector + " .template");
            if (!!username_filter) {
                $(".remove").click(function() {
                    $(this).parent().parent().effect("highlight", {});
                    var answer = confirm("This message will be removed permanently, are you sure you want to remove this message?");
                    if (answer)
                        container.remove_id($(this).parent().children(".id").text(), function() {
                            update_offers();
                        });
                    return false;
                });
            }
            $(".left_col").unblock();
            $(".right_col .fg-buttonset").unblock();
            if ($.isFunction(onUpdateCallback)) onUpdateCallback();
        });
    };

    var add_filter = function(tag_text, tag_type,onRemoveFilter) {
        current_tags.add_tag(tag_text, tag_type,false,onRemoveFilter);
        current_tags.update_view();
        queue_update_offers();
    };
    
    var filter_by_user = function(username) {
        username_filter = username;
        queue_update_offers();
    };
    
    var toggle_filter = function(tag_text, tag_type) {
        current_tags.toggle_filter(tag_text, tag_type);
        current_tags.update_view();
        queue_update_offers();

        $("#current_tags .tag-unfixed").addClass("ui-tag-close");
        update_offers();
    };
    
    var remove_filter = function(tag_text) {
        current_tags.remove_tag(tag_text);
        current_tags.update_view();
        queue_update_offers();
    };

    var add_fixed_filter = function(tag_text, tag_type) {
        current_tags.add_fixed_tag(tag_text, tag_type, false);
        current_tags.update_view();
        queue_update_offers();
    };

    init();

    /* 'public' methods */
    this.filter_by_user = filter_by_user;
    this.toggle_filter = toggle_filter;
    this.update_offers = update_offers;
    this.on_offers_updated = on_offers_updated;
    this.add_filter = add_filter;
    this.remove_filter = remove_filter;
    this.add_fixed_filter = add_fixed_filter;
    this.get_offers = function() {
        return offers;
    }
    this.get_tags = function() {
        return tags;
    }
    this.current_tags = function() {
        return current_tags;
    }
}

