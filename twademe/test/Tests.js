//http://ejohn.org/blog/fireunit/
function test_tag_click() {
    var suggested_tags_selector = $('#suggested_tags_widget .tag:first');
    var suggested_tags_val = suggested_tags_selector.text();
    //this is assuming that we call the replace method in our code
    suggested_tags_val = suggested_tags_val.replace("\n", "").trim();
    suggested_tags_selector.trigger('click');
    var details = $('#offer').val();
    //look for the tag in details
    var details_result = details.search("#" + suggested_tags_val);
    fireunit.ok(details_result > -1, "Testing if tag click adds item to details");
    var currency_tags_selector = $('#post_your_own_currency_tags .tag:first');
    currency_tags_val = currency_tags_selector.text();
    currency_tags_val = currency_tags_val.replace("\n", "").trim();
    currency_tags_selector.trigger('click');
    //look for both the currency tag in details now this assumes that we do not immediately update 'message_to_send' after
    //a suggested tag is clicked
    var message_to_send=$("#message_to_send").val();
    var message_result = message_to_send.search("#" + currency_tags_val);
    fireunit.ok(message_result > -1, "Testing if currency tag click adds to message_to_send");
    message_result = message_to_send.search("#" + suggested_tags_val);
    fireunit.ok(message_result > -1, "Testing if suggested tag is added to message_to_send");
    
};
setTimeout(test_tag_click, 10000);