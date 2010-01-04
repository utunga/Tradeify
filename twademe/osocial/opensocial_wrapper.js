
/*
  Javascript wrapper class that wraps around all the open social/ning specific stuff (so that
  it can be swapped out for other classes more easily / swapped out for local testing etc 
*/
    
function OSocialContainer() {

    this.offers_uri = "http://tradeify.org/offers_json.aspx";
    this.tags_uri = "http://tradeify.org/tags_json.aspx";

    this.accept_post_url = "http://tradeify.org/accept_post.aspx";
    
    this.adjustHeight = function(height) {    
        gadgets.window.adjustHeight(height);
    }
        
    /* ------------------------------
         sending data from form
       ------------------------------*/

    this.post_message = function(message) {
    
        callback = (arguments.length>1) ? arguments[1] : function() {}
        
        var message = message;
        var req = opensocial.newDataRequest();
        req.add(req.newFetchPersonRequest(opensocial.IdSpec.PersonId.VIEWER), 'viewer');
        req.send(function(response) {
           
            var viewer = response.get('viewer').getData();
            var viewerJson= gadgets.json.stringify(viewer);
            var name = viewer.getDisplayName();
            var thumbnail= viewer.getField(opensocial.Person.Field.THUMBNAIL_URL,null);
            var profileUrl = viewer.getField(opensocial.Person.Field.PROFILE_URL,null);

            //not sure if we need/want this approach or not
            var dataPacket = {};
            dataPacket.Message = message;
            dataPacket.User = viewer;
            var dataPacketJSON = gadgets.json.stringify(dataPacket);
            
            var message_data = {
                message:message,
                username:name,
                thumbnail:thumbnail,
                namespace:"ooooby", //FIXME
                profileurl:profileUrl,
                DataPacket:dataPacketJSON
            };
            this.postToURL(this.accept_post_url,callback,message_data);
        });
    }

    this.postToURL = function(url, callback, postdata) {
        var params = {};
        postdata = gadgets.io.encodeValues(postdata);
        params[gadgets.io.RequestParameters.METHOD] = gadgets.io.MethodType.POST;
        params[gadgets.io.RequestParameters.POST_DATA]= postdata;
        gadgets.io.makeRequest(url,callback,params);
    }
}

var container = new OSocialContainer();