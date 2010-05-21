function ListWidget(options, offers_selector, current_tags_selector) {

    var defaults = {
        offers_uri: 10,
        username: null,
        username_namespace: "ooooby",
        page_size: 10
    }; 
    
    var options = $.extend(defaults, options);

    var offers;
    var tags;
    var offers_render_fn;
    var current_tags;
    var _offers_updated=new Array();
    var _offers_updating=new Array();
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
                '.id': 'offer.message_pointer.message_id',
                '.namespace':'offer.message_pointer.provider_name_space' 
            }
        }
    };

    var init = function() {
        //offers_uri = container.offers_uri;
        //compile to a function as soon as possible (ie in 'constructor')
        offers_render_fn = $(offers_selector + ' .template').compile(offers_directives);
        if (!options.username) {
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

    var on_offers_updating = function(functionRef) {
        //ensure no duplicates are added
        if (!$.inArray(functionRef, _offers_updating)) {
            _offers_updating.push(functionRef);
        }
    }

    var on_offers_updated = function(functionRef) {
        //ensure no duplicate functions are added
        if (!$.inArray(functionRef, _offers_updated)) {
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
    
    var current_page = 1;
    var update_offers = function(onUpdateCallback) {

        var json_url;
        
        //notify that we're updating (so that 'waiting' ajax spinner stuff can be displayed for eg)
        if (!!_offers_updating) {
            $.each(_offers_updating, function() {
                    this();
            });
        }
        
        if (!options.username && !!current_tags_selector)
            json_url = current_tags.decorate_url(options.offers_uri); //standard
        else if (!!username_filter & !current_tags_selector)
            json_url = options.offers_uri + "?jsoncallback=?" + "&username=" + options.username + "&namespace=" + options.username_namespace; //my list widget

        $.getJSON(json_url, function(data) {
            $.each(data.Messages, function() {
                this.timestamp = formatTimestamp(this.timestamp);
            });
            $(offers_selector + ' .template').render(data, offers_render_fn);
            if (options.username == null) {
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

            $(offers_selector + " .template").quickPager({ pageSize: options.page_size, currentPage: current_page }, offers_selector + " .pager", offers_selector + " .template");
            current_page = 1;
            if (!!options.username) {
                $(".remove").click(function() {
                    $(this).parent().parent().effect("highlight", {});
                    var answer = confirm("This message will be removed permanently, are you sure you want to remove this message?");
                    if (answer) {
                        container.remove_id($(this).parent().children(".id").text(), $(this).parent().children(".namespace").text(), function() {
                            current_page = $(offers_selector + " .pgCurrent").text();
                            update_offers();
                            //$(offers_selector + " .template").quickPager({ pageSize: 10,currentPage:pageNum}, offers_selector + " .pager", offers_selector + " .template");
                        });
                    }
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
        options.username = username;
        queue_update_offers();
    };
    
    var toggle_filter = function(tag_text, tag_type) {
        current_tags.toggle_filter(tag_text, tag_type);
        current_tags.update_view();
        queue_update_offers();
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
    this.on_offers_updating = on_offers_updating;
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
