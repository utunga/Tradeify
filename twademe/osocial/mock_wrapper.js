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
    this.parse_uri ="/parse.aspx";
    //assumes test is running in local web
    this.accept_post_url = "/accept_post.aspx";
   
    this.adjustHeight = function(height) {    
        //does nothing in test
    }
    
    this.post_message = function(message) {
       
        callback = (arguments.length>1) ? arguments[1] : function() {}
        
        var name = "utunga";
        var thumbnail= "http://api.ning.com/files/e8j55wYy8dOF4G2Tc2xlF2SnUpF9HbyUgbsCA43rqCZ5eNG2TFQu5zb7JfXErTROYhPch5PaDGwsxuXMwwZSXSQtPap9zCDd/267662688.bin?crop=1%3A1";
        var profileUrl ="http://utunga.ning.com/profile/MilesThompson";

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