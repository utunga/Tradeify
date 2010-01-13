//-------------------------------------------------
//		Quick Pager jquery plugin
//		Created by dan and emanuel @geckonm.com
//		www.geckonewmedia.com
// 
//		v1.1
//		18/09/09 * bug fix by John V - http://blog.geekyjohn.com/
//-------------------------------------------------
var pageCount=-2;
var selector;
var options;
function initialPageClick(currentPage) {
    $(".pagerPage").hide();
    $(".simplePagerPage" + currentPage).show();
}
function pageClick(currentPage) {
    initialPageClick(currentPage);
    $("#pager").pager({ pagenumber: currentPage, pagecount: pageCount, buttonClickCallback: pageClick });
}
(function($) {

    $.fn.quickPager = function(options) {

        var defaults = {
            pageSize: 10,
            currentPage: 1,
            holder: null,
            pagerLocation: "after"
        };

        options = $.extend(defaults, options);

        //pageCount = (((this)[0]).firstChild.length / options.pageSize).toFixed(0) + 1;
        return this.each(function() {


            selector = $(this);
            var pageCounter = 1;

            //selector.wrap("<div class='simplePagerContainer'></div>");

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
            if (pageCount == -2) pageCount = pageCounter;
            initialPageClick(options.currentPage);


            if (pageCounter <= 1) {
                return;
            }
            //selector.after("<div id=\"pager\" ></div>");
            $("#pager").pager({ pagenumber: 1, pagecount: pageCounter, buttonClickCallback: pageClick });
        });


        /*
        //Build pager navigation
        var pageNav = "<ul class='simplePagerNav'>";	
        for (i=1;i<=pageCounter;i++){
        if (i==options.currentPage) {
        pageNav += "<li class='currentPage simplePageNav"+i+"'><a rel='"+i+"' href='#'>"+i+"</a></li>";	
        }
        else {
        pageNav += "<li class='simplePageNav"+i+"'><a rel='"+i+"' href='#'>"+i+"</a></li>";
        }
        }
        pageNav += "</ul>";
			
			if(!options.holder) {
        switch(options.pagerLocation)
        {
        case "before":
        selector.before(pageNav);
        break;
        case "both":
        selector.before(pageNav);
        selector.after(pageNav);
        break;
        default:
        selector.after(pageNav);
        }
        }
        else {
        $(options.holder).append(pageNav);
        }
			
			//pager navigation behaviour
        selector.parent().find(".simplePagerNav a").click(function() {
					
				//grab the REL attribute 
        var clickedLink = $(this).attr("rel");
        options.currentPage = clickedLink;
				
				if(options.holder) {
        $(this).parent("li").parent("ul").parent(options.holder).find("li.currentPage").removeClass("currentPage");
        $(this).parent("li").parent("ul").parent(options.holder).find("a[rel='"+clickedLink+"']").parent("li").addClass("currentPage");
        }
        else {
        //remove current current (!) page
        $(this).parent("li").parent("ul").parent(".simplePagerContainer").find("li.currentPage").removeClass("currentPage");
        //Add current page highlighting
        $(this).parent("li").parent("ul").parent(".simplePagerContainer").find("a[rel='"+clickedLink+"']").parent("li").addClass("currentPage");
        }
				
				//hide and show relevant links
        selector.children().hide();			
        selector.find(".simplePagerPage"+clickedLink).show();
				
				return false;
        });
        */

    }


})(jQuery);

