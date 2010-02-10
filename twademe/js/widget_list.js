﻿function ListWidget(offers_selector, current_tags_selector) {

    var offers;
    var tags;
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
                        'a': 'tag.tag'/*,
                        '+a@class': 'tag.type'*/
                    }
                },
                '.when': 'offer.timestamp'
                
            }
        }
    }; 
   
    var init = function() {
       
        offers_uri = container.offers_uri;

        //compile to a function as soon as possible (ie in 'constructor')
        offers_render_fn = $(offers_selector + ' .template').compile(offers_directives);

        current_tags = new Tags(current_tags_selector);
        current_tags.tag_click(function() {
            remove_filter($(this).text());
            return false;
        });
    }

    var on_offers_updated = function(functionRef) {
        _offers_updated.push(functionRef);
    }

    var update_offers = function() {
        var json_url = build_search_query(offers_uri);
        $.getJSON(json_url, function(data) {
            $.each(data.Messages, function() {
                var overall = this.timestamp.split("T");
                var dates = overall[0].split("-");
                var times = overall[1].split(":");
                var time = times[0] + ":" + times[1];
                var date = dates[2] + "-" + dates[1] + "-" + dates[0];
                this.timestamp = time + " " + date;
                //this.timestamp = Date.parse(this.timestamp);
            }
            );
            $(offers_selector + ' .template').render(data, offers_render_fn);
            $(offers_selector + ' .tags a').click(function() {
                //add a filter when tags under a message are clicked
                add_filter($(this).text(), $(this).css());
            });
            offers = data.Messages;
            tags = data.Tags;
            if (!!_offers_updated) {
                $.each(_offers_updated, function() {
                    this(offers);
                });
            }
            $(offers_selector + " .template").quickPager({ pageSize: 4 }, "#pager");
        });
    };

    var add_filter = function(tag_text, tag_type) {
        current_tags.add_tag(tag_text, tag_type);
        update_offers();
    };
    
    var toggle_filter = function(tag_text, tag_type) {
        current_tags.toggle_filter(tag_text, tag_type);
        update_offers();
    };
    
    var remove_filter = function(tag_text) {
        current_tags.remove_tag(tag_text);
        update_offers();
    };

    var add_fixed_filter = function(tag_text, tag_type) {
        current_tags.add_fixed_tag(tag_text, tag_type, false);
        update_offers();
    };

    var build_search_query = function(baseUrl) {
        var query = "";
        var current_tags_array = (arguments.length > 1) ? arguments[1] : current_tags.tags;
        for (tag in current_tags_array) {
                if (query != "")
                    query = query + "&";
                query = query + "tag=" + current_tags_array[tag].tag;
        }
        return baseUrl + "?" + query + "&jsoncallback=?";
    };


    init();
    
    /* 'public' methods */
    this.toggle_filter = toggle_filter;
    this.current_tags = current_tags;
    this.update_offers = update_offers;
    this.on_offers_updated = on_offers_updated;
    this.add_filter = add_filter;
    this.remove_filter = remove_filter;
    this.add_fixed_filter = add_fixed_filter;
    this.build_search_query = build_search_query;
    this.get_offers = function() {
        return offers;
    }
    this.get_tags = function() {
        return tags;
    }
}

