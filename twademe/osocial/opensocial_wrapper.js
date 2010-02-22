function OSocialContainer() {
    this.cross_uri = "http://tradeify.org/osocial/Cross.png";
    this.tick_uri = "http://tradeify.org/osocial/Tick.png";
    this.offers_uri = "http://tradeify.org/tradeify_json.aspx";
    this.tags_uri = "http://tradeify.org/tags_json.aspx";
    this.parse_uri = "http://tradeify.org/parse.aspx?jsoncallback=?";
    this.accept_post_url = "http://tradeify.org/accept_post.aspx";
    this.tags_ahead_uri = "http://tradeify.org/tags_ahead.aspx?jsoncallback=?";
    this.remove_message_uri = "http://tradeify.org/remove_message.aspx?jsoncallback=?";
    this.active_prompt = false;

    var user;
    this.adjustHeight = function(height) {
        gadgets.window.adjustHeight(height);
    }
    this.remove_id = function(id, callback) {
        $.getJSON(this.remove_message_uri + "&id=" + "ooooby/" + id, function(data) {
            setTimeout(callback, 2000);
        });
    }
    this.get_user = function(callback) {
        if (!!user) callback(user);
        var req = opensocial.newDataRequest();
        req.add(req.newFetchPersonRequest(opensocial.IdSpec.PersonId.VIEWER), 'viewer');
        req.send(function(response) {
            var user = response
            //var viewerJson = gadgets.json.stringify(viewer);
            if(!!callback)callback(user);
        });
    }
    this.autocomplete_suggested_tags=function(selector){
    $(selector).autocomplete(this.tags_ahead_uri + "&type=tag", {
            dataType: "json",
            formatItem: function(row) { this.active_prompt = true; return row[0]; },
            extraParams: {/*type:"tag", jsoncallback:"?"*/
        }
            //formatResult: function(row) { alert("format match"); active_prompt = false; return row[0]; }
        });
        $(selector).result(function(){active_prompt=false});
    }

    this.autocomplete_tag_search = function(selector) {
    $(selector).autocomplete(this.tags_ahead_uri, { dataType: "json"}/*, { extraParams: { jsoncallback: "?"} }*/);
    }
    /* ------------------------------
         sending data from form
       ------------------------------*/
    this.get_user_location = function() {
    /*
        var req = opensocial.newDataRequest();
        req.add(req.newFetchPersonRequest(opensocial.IdSpec.PersonId.VIEWER), 'viewer');
        var location;
        req.send(function(response) {
            var viewer = response.get('viewer').getData();
            //var viewerJson = gadgets.json.stringify(viewer);
            var viewerlocations = viewer.getField(opensocial.Person.Field.ADDRESSES, null);
            var viewerlocation = viewerlocations[0];
            if (!!viewerlocation) {
                location = viewerlocation.getField(opensocial.Address.Field.UNSTRUCTURED_ADDRESS, null);
            }
        });
        */
        var location;
        return location;
    };
    this.get_user_name = function(callback) {
            this.get_user(function(response){
            var viewer = response.get('viewer').getData();
            //var viewerJson = gadgets.json.stringify(viewer);
            var name = viewer.getDisplayName();
            callback(name);
            });           
        });
    };
    this.post_message = function(message) {

        callback = (arguments.length > 1) ? arguments[1] : function() { }

        var message = message;
        var url = this.accept_post_url;
        /*var req = opensocial.newDataRequest();
        req.add(req.newFetchPersonRequest(opensocial.IdSpec.PersonId.VIEWER), 'viewer');
        req.send(function(response) {*/
        this.getUser(function(reponse){
            var viewer = response.get('viewer').getData();
            //var viewerJson = gadgets.json.stringify(viewer);
            var name = viewer.getDisplayName();
            var thumbnail = viewer.getField(opensocial.Person.Field.THUMBNAIL_URL, null);
            var profileUrl = viewer.getField(opensocial.Person.Field.PROFILE_URL, null);

            //not sure if we need/want this approach or not
            var dataPacket = {};
            dataPacket.Message = message;
            dataPacket.User = viewer;
            var dataPacketJSON = gadgets.json.stringify(dataPacket);

            var message_data = {
                message: message,
                username: name,
                thumbnail: thumbnail,
                namespace: "ooooby", //FIXME
                profileurl: profileUrl,
                DataPacket: dataPacketJSON
            };

            var params = {};
            var postdata = gadgets.io.encodeValues(message_data);
            params[gadgets.io.RequestParameters.METHOD] = gadgets.io.MethodType.POST;
            params[gadgets.io.RequestParameters.POST_DATA] = postdata;
            gadgets.io.makeRequest(url, callback, params);

            var params = {};
            params[opensocial.Activity.Field.TITLE] = message;
            var activity = opensocial.newActivity(params);
            /*"If the activity is of high importance, it will be created even if this requires asking the user for permission. 
            This may cause the container to open a user flow which may navigate away from your gagdet. 
            If the activity is of low importance, it will not be created if the user has not given permission for the current app to create activities.
            With this priority, the requestCreateActivity call will never open a user flow"*/
            opensocial.requestCreateActivity(activity, opensocial.CreateActivityPriority.LOW);
        });
    }
//    this.parse_message = function(message,callback) {
//        var message_data = {
//            message: message,
//        };
//        var params = {};
//        var postdata = gadgets.io.encodeValues(message_data);
//        params[gadgets.io.RequestParameters.METHOD] = gadgets.io.MethodType.POST;
//        params[gadgets.io.RequestParameters.POST_DATA] = postdata;
//        gadgets.io.makeRequest(this.parse_uri+"?jsoncallback=?",callback, params);
//    }

}

var container = new OSocialContainer();