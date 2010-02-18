function MyTrades(offers_selector) {

    var offers;
    var tags;
    var offers_render_fn;
    var current_tags;
    var offers_uri;
    var _offers_updated=new Array();
    var map;


    var offers_directives = {
        'div.my_offer': {
            'my_offer <- Messages': {
                'a.my_username@href': 'my_offer.created_by.profile_url',
                'a.my_avatar@href': 'my_offer.created_by.profile_url',
                'a.my_username': 'my_offer.created_by.provider_user_name',
                '.my_avatar img@src': 'my_offer.created_by.profile_pic_url',
                '.my_msg .my_text': 'my_offer.offer_text',
                'span.my_tags': {
                    'my_tag <- my_offer.tags': {
                        'a': 'my_tag.tag',
                        '+a@class': 'my_tag.type'
                    }
                },
                '.my_when': 'offer.timestamp'
            }
        }
    };

    var init = function() {

        offers_uri = container.offers_uri;
        //compile to a function as soon as possible (ie in 'constructor')
        offers_render_fn = $(offers_selector + ' .template').compile(offers_directives);
        
        //current_tags = new TagsWidget(current_tags_selector);
    }
    
    var update_offers = function(username) {
        var json_url = offers_uri;
        if (!!username) json_url += "&username=" + arguments[0] + "&namespace=ooooby";
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
            /*
            $(offers_selector + ' .tags a').click(function() {
                //add a filter when tags under a message are clicked
                add_filter($(this).text(), $(this).css());
            });
            */
            offers = data.Messages;
            tags = data.Tags;
            if (!!_offers_updated) {
                $.each(_offers_updated, function() {
                    this(offers);
                });
            }
            $(offers_selector + " .template").quickPager({ pageSize: 4 }, "#my_pager");
        });
    };

    this.update = function() {
        container.filter_by_user_name(update_offers);
    }
    init();

}

