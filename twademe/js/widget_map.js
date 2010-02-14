﻿function MapWidget(map_selector, map_popup_selector, list_widget) {

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
        var tags_backup = list_widget.current_tags().get_active_tags().concat(marker.tags);

        var json_url = list_widget.current_tags().decorate_url(offers_uri, tags_backup);
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
            var title = this.address; //this.offer_text + " " + this.user.profile_url;
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
