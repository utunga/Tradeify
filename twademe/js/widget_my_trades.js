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
            '.when': 'offer.timestamp',
            '.id': 'offer.message_pointer.message_id'    
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
        var json_url = offers_uri + "?jsoncallback=?";
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
            $(offers_selector + " .template").quickPager({ pageSize: 10 }, "#my_pager", offers_selector + " .template");
            $(".remove").click(function() {
            var answer = confirm("This message will be removed permanently, are you sure you want to remove this message?");
                if(answer)
                    container.remove_id($(this).parent().children(".id").text(), this.update);
                return false;
            });
        });

    };

    this.update = function() {
        container.filter_by_user_name(update_offers);
    }
    init();

}

