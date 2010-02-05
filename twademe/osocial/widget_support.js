
function TradeifyWidget(offers_selector, current_tags_selector) {
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
        //update_offers();
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
    this.toggle_filter = toggle_filter;
    this.current_tags = current_tags;
    /* 'public' methods */
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


function MapWidget(map_selector, map_popup_selector, list_widget) {

    var map;
    var map_popup_render_fn;
    var offers_uri;
  
    var map_directives = {
        'div.map_offer': {
            'map_offer <- Messages': {
            'a.map_username@href': 'map_offer.created_by.profile_url',
            'a.map_avatar@href': 'map_offer.created_by.profile_url',
            'a.map_username': 'map_offer.created_by.provider_user_name',
            '.map_avatar img@src': 'map_offer.created_by.profile_pic_url',
                '.map_msg .map_text': 'map_offer.offer_text',
                'span.map_tags': {
                    'map_tag <- map_offer.tags': {
                        'a': 'map_tag.tag',
                        '+a@class': 'map_tag.type'
                    }
                },
                '.map_when': 'offer.timestamp'
            }
        }
    };

    var init = function() {
        map_popup_render_fn = $(map_popup_selector + ' .template').compile(map_directives);
        $(map_popup_selector + ' .template').hide();
        offers_uri = container.offers_uri;
    }
    
    function count_location_tags(tags) {
        var count=0;
        $.each(tags, function() {
            if (this.type == "loc") count++;
        });
        return count;
    }
    
    var create_popup = function(map, marker) {
        var tags_backup = list_widget.current_tags.tags.concat(marker.tags);

        var json_url = list_widget.build_search_query(offers_uri, tags_backup);
        $.getJSON(json_url, function(raw_data) {
            var messages = new Array();
            //make sure that only the messages with the exact same number of tags are included
            $.each(raw_data.Messages, function() {
                if (count_location_tags(this.tags) == marker.tags.length) messages.push(this);
            });
            raw_data.Messages = messages;
            $(map_popup_selector + ' .template').render(raw_data, map_popup_render_fn);
            var infowindow = new google.maps.InfoWindow(
            {
                content: $(map_popup_selector).html()
            });
            $('.map_tag').click(function() {
                //add a filter when tags under a message are clicked
                add_filter($(this).text(), $(this).css());
            });
            $(map_popup_selector + ' .template').hide();
            infowindow.open(map, marker);
        });
    }

    var update_map = function() {

        if (typeof google == 'undefined') return; //do nothing

        if (map == undefined) {
            //var myLatlng = new google.maps.LatLng(-25.363882, 131.044922);
            var myOptions = {
                zoom: 2,
                //center: myLatlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            }
            map = new google.maps.Map(document.getElementById(map_selector), myOptions);
            list_widget.on_offers_updated(update_map);
        }

        var offers = list_widget.get_offers();
        var latlng = new Array();
        $.each(offers, function() {
            var post = new google.maps.LatLng(this.location.geo_lat, this.location.geo_long);
            latlng.push(post);
            var title = this.address; //this.offer_text + " " + this.user.more_info_url;
            var tags = new Array();
            $.each(this.tags, function() {
                if (this.type == "loc") tags.push(this);
            });
            var marker;
            if (this.message_type == "offer") {
                marker = new google.maps.Marker({
                    clickable: true,
                    title: title,
                    position: post,
                    map: map,
                    tags: tags
                });
            }
            else {
                var image_url = "http://www.google.com/intl/en_us/mapfiles/ms/micons/blue-dot.png";
                marker = new google.maps.Marker({
                    clickable: true,
                    title: title,
                    position: post,
                    map: map,
                    tags: tags,
                    icon: image_url
                });
            }
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

function TagsWidget(selector, initial_tags, active_tags, tag_type) {
    var tags;
    var after_click = (arguments.length > 4) ? arguments[4] : function() {};
    
    var init = function() {
        tags = new Tags(selector);

        $.each(initial_tags, function() {
            var isInActiveTags = !!active_tags.find_tag(this);
            tags.add_tag(this, tag_type, isInActiveTags);
        });

        tags.tag_click(function() {
            var tag_text = $(this).text().replace("\n", "");
            tags.toggle_active(tag_text,tag_type);
            after_click(tag_text, tag_type);
            return false;
        });
    }
    
    init();

    this.get_active_tags_text = function() {
        return tags.get_active_tags_text();
    };
    this.get_active_tags = function() {
        return tags.get_active_tags();
    };
    this.update_view = function() {
        tags.update_view();
    };
    this.reset = function() {
        init();
    };
    this.Tags = tags;
};

function SuggestedTagsWidget(selector, initial_tags, active_tags, tag_type) {
    var tags;
    var after_click = (arguments.length > 4) ? arguments[4] : function() { };

    var init = function() {
        tags = new Tags(selector);

        $.each(initial_tags, function() {
            var str = this.toString();
            var isInActiveTags = ($.inArray(str, active_tags) >= 0);
            tags.add_tag(str, tag_type, isInActiveTags);
        });

        tags.tag_click(function() {
            var tag_text = $(this).text().replace("\n", "");
            tags.toggle_active(tag_text, tag_type);
            after_click(tag_text, tag_type);
            return false;
        });
        
    }

    init();

    this.get_active_tags_text = function() {
        return tags.get_active_tags_text();
    };
    this.get_active_tags = function() {
        return tags.get_active_tags();
    };
    this.update_view = function() {
        tags.update_view();
    };
    this.Tags = tags;
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