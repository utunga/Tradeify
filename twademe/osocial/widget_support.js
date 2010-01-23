
function TradeifyWidget(offers_selector, current_tags_selector) {
    var offers;
    var offers_render_fn;
    var current_tags; 
    var offers_uri;
    var _offers_updated;
    var map;

 
    var offers_directives = {
        'div.offer': {
            'offer <- messages': {
                'a.username@href': 'offer.user.more_info_url',
                'a.avatar@href': 'offer.user.more_info_url',
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
  
   
    var init = function() {
       
        offers_uri = container.offers_uri;

        //compile to a function as soon as possible (ie in 'constructor')
        offers_render_fn = $(offers_selector + ' .template').compile(offers_directives);

        current_tags = new Tags(current_tags_selector);
        current_tags.tag_click(function() {
            remove_filter($(this).text());
            return false;
        });
        //update_offers();
    }

    var on_offers_updated = function(functionRef) {
        _offers_updated = functionRef;
    }

    var update_offers = function() {
        var json_url = build_search_query(offers_uri);
        $.getJSON(json_url, function(data) {
            $(offers_selector + ' .template').render(data, offers_render_fn);
            $(offers_selector + ' .tags a').click(function() {
                //add a filter when tags under a message are clicked
                add_filter($(this).text(), $(this).css());
            });
            offers = data.messages;
            if (!!_offers_updated) {
                _offers_updated(offers);
            }
            $(offers_selector + " .template").quickPager({ pageSize: 4 }, "#pager");
        });
        //$("#results").tabs();
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
    this.update_offers = update_offers;
    this.on_offers_updated = on_offers_updated;
    this.add_filter = add_filter;
    this.add_fixed_filter = add_fixed_filter;
    this.build_search_query = build_search_query;
    this.get_offers = function() {
        return offers;
    }
}


function MapWidget(map_selector, map_popup_selector, list_widget) {

    var map;
    var offers_uri;
    
    var map_directives = {
        'div.map_offer': {
            'map_offer <- messages': {
                'a.map_username@href': 'map_offer.user.more_info_url',
                'a.map_username': 'map_offer.user.screen_name',
                '.map_avatar img@src': 'map_offer.user.profile_pic_url',
                '.map_msg .map_text': 'map_offer.offer_text',
                'span.map_tags': {
                    'map_tag <- map_offer.tags': {
                        'a': 'map_tag.tag',
                        '+a@class': 'map_tag.type'
                    }
                },
                '.map_when': 'map_offer.date'
            }
        }
    };

    var init = function() {

        var map_popup = $(map_popup_selector + ' .template').compile(map_directives);
        $(map_popup_selector + ' .template').hide();
        offers_uri = container.offers_uri;
    }
 
    var create_popup = function (map, marker) {
        var tags_backup = current_tags.tags.concat(marker.tags);

        var json_url = list_widget.build_search_query(offers_uri, tags_backup);
        $.getJSON(json_url, function(raw_data) {
            $(map_popup_selector + ' .template').render(raw_data, map_popup);

             var infowindow = new google.maps.InfoWindow(
            {
                content: $(map_popup_selector).html()
            });
            $(map_popup_selector + ' .template').hide();
            infowindow.open(map, marker);
        });
    }

    var update_map = function() {

        if (map == undefined) {
            var myLatlng = new google.maps.LatLng(-25.363882, 131.044922);
            var myOptions = {
                zoom: 2,
                center: myLatlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            }
            map = new google.maps.Map(document.getElementById(map_selector), myOptions);
            list_widget.on_offers_updated(update_map);
        }
        else {
            // map.clearMarkers();
        }

        var offers = list_widget.get_offers();
        var latlng = new Array();
        $.each(offers, function() {
            var post = new google.maps.LatLng(this.offer_latitude, this.offer_longitude);
            latlng.push(post);
            var title = this.offer_address; //this.offer_text + " " + this.user.more_info_url;
            var tags = new Array();
            $.each(this.tags, function() {
                if (this.type == "loc") tags.push(this);
            });
            var marker = new google.maps.Marker({
                clickable: true,
                title: title,
                position: post,
                map: map,
                tags: tags
            });

            google.maps.event.addListener(marker, "click", function() {
                create_popup(map, marker);
            });
        });

        var latlngbounds = new google.maps.LatLngBounds();
        for (var i = 0; i < latlng.length; i++) {
            // latlngbounds.extend(latlng[i]);
            latlngbounds.extend(latlng[i]);
        }
        map.fitBounds(latlngbounds);
        if (map.getZoom() > 15) { map.setZoom(15) };
    }

    init();
    
    this.update_map = update_map;

};


/*
google.maps.Map.prototype.markers = new Array();

google.maps.Map.prototype.getMarkers = function() {
return this.markers
};

google.maps.Map.prototype.clearMarkers = function() {
for (var i = 0; i < this.markers.length; i++) {
this.markers[i].setMap(null);
}
this.markers = new Array();
};

google.maps.Marker.prototype._setMap = google.maps.Marker.prototype.setMap;

google.maps.Marker.prototype.setMap = function(map) {
if (map) {
map.markers[map.markers.length] = this;
}
this._setMap(map);
};
*/