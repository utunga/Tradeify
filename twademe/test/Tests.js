//http://ejohn.org/blog/fireunit/
function test_tag_click() {
    var selector = $('#suggested_tags_widget .tag:first');
    var val = selector.text();
    val = val.replace("\n", "").trim();
    selector.trigger('click');
    var details = $('#offer').val();
    var details_result=details.search("#"+val);
    fireunit.ok( details_result>-1, "Testing if tag click adds item to details");
};
setTimeout(test_tag_click, 10000);