//-------------------------------------------------
//		Quick Pager jquery plugin
//		Created by dan and emanuel @geckonm.com
//		www.geckonewmedia.com
// 
//		v1.1
//		18/09/09 * bug fix by John V - http://blog.geekyjohn.com/
//-------------------------------------------------

(function($) {

    $.fn.quickPager = function(options, pager, selector) {

        var defaults = {
            pageSize: 10,
            currentPage: 1,
            holder: null,
            pagerLocation: "after"
        };
        var pageCount;
        var options = $.extend(defaults, options);
        return this.each(function() {


            var selector = $(this);
            var pageCounter = 1;
            selector.children().each(function(i) {

                if (i < pageCounter * options.pageSize && i >= (pageCounter - 1) * options.pageSize) {
                    $(this).addClass("pagerPage");
                    $(this).addClass("simplePagerPage" + pageCounter);
                }
                else {
                    $(this).addClass("pagerPage");
                    $(this).addClass("simplePagerPage" + (pageCounter + 1));
                    pageCounter++;
                }

            });
            if (pageCounter < 1) {
                return;
            }
            var pageClick = function(currentPage) {
                $(selector).children(".pagerPage").hide();
                $(selector).children(".simplePagerPage" + currentPage).show();
                $(pager).pager({ pagenumber: currentPage, pagecount: pageCounter, buttonClickCallback: pageClick });
            }
            var page = (options.currentPage <= pageCounter) ? options.currentPage : pageCounter;
            pageClick(page);
        });

    }


})(jQuery);

