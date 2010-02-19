//http://ejohn.org/blog/fireunit/
function test_add_your_own_tag() {
    $('#typed_tag').val("wxy");
    //$('#typed_tag').trigger('keyup');
    setTimeout(function() {
        var own_tag = $("#suggested_tags_widget .tag:contains('wxy')");
        fireunit.ok(own_tag.length == 1, "Testing if tag has been added");
        var isSelected = own_tag.attr("class").search("ui-state-active")
        fireunit.ok(isSelected > -1, "Testing if tag is selected");
    }, 7000);
}
function test_tag_click() {
    var suggested_tags_selector = $('#suggested_tags_widget .tag:first');
    suggested_tags_selector.trigger('click');
    var suggested_tags_val = suggested_tags_selector.text();
    //this is assuming that we call the replace method in our code
    suggested_tags_val = suggested_tags_val.replace("\n", "").trim();
    setTimeout(function() {
     var isSelected = $("#suggested_tags_widget .tag:contains('"+suggested_tags_val+"')").attr("class").search("ui-state-active");
        fireunit.ok(isSelected > -1, "Testing if tag is selected");
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
        var message_to_send = $("#message_to_send").val();
        var message_result = message_to_send.search("#" + currency_tags_val);
        fireunit.ok(message_result > -1, "Testing if currency tag click adds to message_to_send");
        message_result = message_to_send.search("#" + suggested_tags_val);
        fireunit.ok(message_result > -1, "Testing if suggested tag is added to message_to_send");
    }, 4000);

};
/*
function test_tag_click() {
    var suggested_tags_selector = $('#suggested_tags_widget .tag:first');
    var suggested_tags_val = suggested_tags_selector.text();
    //this is assuming that we call the replace method in our code
    suggested_tags_val = suggested_tags_val.replace("\n", "").trim();
    suggested_tags_selector.trigger('click');
    var isSelected = $(suggested_tags_selector).attr("class").search("ui-state-active");
    fireunit.ok(isSelected > -1, "Testing if tag is selected");
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
*/
setTimeout(test_add_your_own_tag, 10000);
setTimeout(test_tag_click, 18000);
