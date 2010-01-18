
function TradeifyWidget(offers_selector, current_tags_selector) {
    var offers;
    var offers_render_fn;
    var current_tags; 
    var offers_uri;


    var map;
    function initialize() {

        updateMap();
        //google.maps.event.addListener(map, "click", clicked);
        /*
         marker2viidz465dm6r = new GMarker(point2viidz465dm6r,{icon:baseIcon, title:"nick myhre"});
390 GEvent.addListener(marker2viidz465dm6r, "click", function() {
391 marker2viidz465dm6r.openInfoWindowHtml("<div class='user'><a href='http://ooooby.ning.com/profile/2viidz465dm6r' target='_top'><img src='http://api.ning.com/files/SJx7Iz4Taic-OTZQzUhOX-FAuVIAXziERFXalCQwepEAud*5Bn4pXWljYZ1p9n*DKnJYP-TvAacNsqtvkd39nCmINYjRK-gq/ATT000022.jpg?crop=1%3A1&width=75' class='upic'/></a><b><a href='http://ooooby.ning.com/profile/2viidz465dm6r' target='_top'>nick myhre</a></b><br /><small style='color:#000000 !important;'>over five foot ten</small><br /><br /><small style='float:right;'><a href='http://ooooby.ning.com/profile/2viidz465dm6r' target='_top'>View My Profile Page &gt;&gt;</a></small></div>");
392 }); */

    }
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
    function updateMap() {
        var myLatlng = new google.maps.LatLng(-25.363882, 131.044922);
        var myOptions = {
            zoom: 2,
            center: myLatlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        }
        map = new google.maps.Map(document.getElementById("offer_map"), myOptions);
        //map.clearMarkers();
        $.each(offers, function() {
            var post = new google.maps.LatLng(this.offer_latitude, this.offer_longitude);
            var title = this.offer_text + " " + this.user.more_info_url;
            var marker = new google.maps.Marker({
                clickable: true,
                title: title,
                position: post,
                map: map
            });


            google.maps.event.addListener(marker, "click", function() {
                createPopup(map,marker);
            });

        });
    }
    function createPopup(map, marker) {
        var map_popup = $("#map_popup" + ' .map_template').compile(offers_directives);
        $("#map_popup" + ' map_template').render(offers, offers_render_fn)
        $("#map_offer_template").quickPager({ pageSize: 2 });
        var infowindow = new google.maps.InfoWindow(
            { content: $("#map_popup").html()
            });
            infowindow.open(map, marker);    
    }
    var init = function() {
    $("#results").tabs({
        //event: 'mouseover'
        fx: { height: 'toggle', opacity: 'toggle' },
        show: function(event, ui) {
            if (ui.panel.id == "results-2") {
                $(ui.panel).css("height", "100%");
                initialize();
                // map.checkResize();

            }
        }
    }); 
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

    var update_offers = function() {
        var json_url = build_search_query(offers_uri);
        $.getJSON(json_url, function(data) {
            offers = data.messages;
            $(offers_selector + ' .template').render(data, offers_render_fn);

            $(offers_selector + ' .tags a').click(function() {
                //add a filter when tags under a message are clicked
                add_filter($(this).text(), $(this).css());
            });
            $("#offer_template").quickPager({ pageSize: 4 });
            updateMap();
            //$("#results").tabs();
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
    this.get_offers = function() {
        return offers;
    }
}
