/*
  Javascript wrapper class that provides a mock version of relevant opensocial bits and pieces
  for local testing purposes - include this *after* or instead of other wrappers for testing purposes
*/

//override this url
    
function mock_container() {
    this.cross_uri = "Cross.png";
    this.tick_uri = "Tick.png";
    this.offers_uri = "/tradeify_json.aspx";
    this.tags_uri = "/tags_json.aspx";
    this.parse_uri = "/parse.aspx";
    this.tags_ahead_uri = "/tags_ahead.aspx";
    //assumes test is running in local web
    this.accept_post_url = "/accept_post.aspx";
    this.remove_message_uri = "/remove_message.aspx";
    this.active_prompt = false;
    this.busy_uri = "../images/busy.gif";
   
    this.adjustHeight = function(height) {    
        //does nothing in test
}
this.remove_id = function(id, name_space, callback) {
    $.getJSON(this.remove_message_uri + "?id=" + name_space + "/" + id, function(data) {
        callback();
    });
}
this.get_user = function(callback) {
    callback();
}
this.get_user_name = function(callback) {
    callback("just_a_test");
}
this.autocomplete_suggested_tags = function(selector) {
    // var active_prompt = false;
    var active_prompt = this.active_prompt;
    $(selector).autocomplete(this.tags_ahead_uri, {
        dataType: "json",
        formatItem: function(row) {
            active_prompt = true;
            return row[0];
        },
        extraParams: { type: "tag" }
        //formatResult: function(row) { alert("format match"); active_prompt = false; return row[0]; }
    });
    $(selector).result(function() {
        active_prompt = false
    });
}

    this.autocomplete_tag_search = function(selector) {
        var sel = $(selector);
        sel.autocomplete(container.tags_ahead_uri, {dataType:"json"});
    }
    
    
    this.get_user_location = function() { }
    this.post_message = function(message) {
       
        callback = (arguments.length>1) ? arguments[1] : function() {}
        
        var name = "just_a_test"
        var thumbnail= "http://s3.amazonaws.com/twitter_production/profile_images/255244783/n581121541_991717_2060_normal.jpg";
        var profileUrl ="http://twitter.com/just_a_test";

        var message_data = {
            message:message,
            username:name,
            thumbnail:thumbnail,
            namespace:"ooooby", //FIXME
            profileurl:profileUrl
        };
        
        
        $.post(this.accept_post_url, message_data, callback);
    }
 
}

var container = new mock_container();