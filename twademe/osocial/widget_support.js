
function TradeifyWidget(offers_selector, current_tags_selector) {
    var offers;
    var offers_render_fn;
    var current_tags; 
    var offers_uri;

    var init = function() {

        offers_uri = container.offers_uri;

        var offers_directives = {
            'div.offer': {
                'offer <- messages': {
                    'a.username@href': 'offer.user.more_info_url',
                    'a.username': 'offer.user.screen_name',
                    '.avatar img@src': 'offer.user.profile_pic_url',
                    '.msg .text': 'offer.offer_text',
                    'span.tags': {
                        'tag <- offer.tags': {
                            'a': 'tag.tag',
                            '+a@class': 'tag.type'
                        }
                    },
                    '.when': 'offer.date'
                }
            }
        };

        //compile to a function as soon as possible (ie in 'constructor')
        offers_render_fn = $(offers_selector + ' .template').compile(offers_directives);

        current_tags = new Tags(current_tags_selector);
        current_tags.tag_click(function() {
            remove_filter($(this).text());
            return false;
        });
        update_offers();
        
    }

    var update_offers = function() {
        var json_url = build_search_query(offers_uri);
        $.getJSON(json_url, function(data) {
            this.offers = data.messages;
            $(offers_selector + ' .template').render(data, offers_render_fn);

            $(offers_selector + ' .tags a').click(function() {
                //add a filter when tags under a message are clicked
                add_filter($(this).text(), $(this).css());
            });
            $(".template").quickPager({ pageSize: 2 });
        });
    };

    var add_filter = function(tag_text, tag_type) {
        current_tags.add_tag(tag_text, tag_type);
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
        for (tag in current_tags.tags) {
            if (query != "")
                query = query + "&";
            query = query + "tag=" + current_tags.tags[tag].text;
        }
        return baseUrl + "?" + query + "&jsoncallback=?";
    };

    init();

    /* 'public' methods */
    this.update_offers = update_offers;
    this.add_filter = add_filter;
    this.add_fixed_filter = add_fixed_filter;

}