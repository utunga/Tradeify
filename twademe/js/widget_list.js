function ListWidget(offers_selector, current_tags_selector) {

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
        current_tags = new TagsWidget(current_tags_selector);
        current_tags.on_tag_click(function(tag_text, tag_type) {
            remove_filter(tag_text);
            return false;
        });
    }

    var on_offers_updated = function(functionRef) {
        _offers_updated.push(functionRef);
    }

    var update_offers = function() {
        var json_url = current_tags.decorate_url(offers_uri);
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
        current_tags.update_view();
        update_offers();
    };
    
    var toggle_filter = function(tag_text, tag_type) {
        current_tags.toggle_filter(tag_text, tag_type);
        current_tags.update_view();
        update_offers();
    };
    
    var remove_filter = function(tag_text) {
        current_tags.remove_tag(tag_text);
        current_tags.update_view();
        update_offers();
    };

    var add_fixed_filter = function(tag_text, tag_type) {
        current_tags.add_fixed_tag(tag_text, tag_type, false);
        current_tags.update_view();
        update_offers();
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
    this.get_offers = function() {
        return offers;
    }
    this.get_tags = function() {
        return tags;
    }
}

