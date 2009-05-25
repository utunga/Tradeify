<%@ Page Title="" Language="C#" MasterPageFile="~/masters/site.Master" AutoEventWireup="true"
    CodeBehind="index.aspx.cs" Inherits="twademe.index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript" src="js/jquery-1.3.2.js"></script>
<script type="text/javascript" src="js/purePacked.js"></script>
<script type="text/javascript">


    function build_search_query(baseUrl) {

        //tag, location, type, group
        var tags = $(".sorted_tags .tag input:checked").map(function() {
            return "tag=" + escape($(this).val());
        }).get().join("&");

        var types = $(".sorted_tags .type input:checked").map(function() {
            return "type=" + escape($(this).val());
        }).get().join("&");

        var groups = $(".sorted_tags .group input:checked").map(function() {
            return "group=" + escape($(this).val());
        }).get().join("&");

        var locations = $(".sorted_tags .loc input:checked").map(function() {
            return "loc=" + escape($(this).val());
        }).get().join("&");

        var query = tags + "&" + types + "&" + groups + "&" + locations;
        return baseUrl +"?" + query;
    }
    
    function compile_render_functions() {

        // most complicated is rendering to offers div
        var offers = $('#results_by_date  .template').mapDirective({
            'div.offer': 'offer <- messages',
            '.date': 'offer.date',
            '.user a[href]': 'offer.user.more_info_url',
            '.user img[src]': 'offer.user.profile_pic_url',
            '.user h4': 'offer.user.screen_name',
            '.msg .text': 'offer.offer_text',
            '.msg a.more_info_link[href]': '#{offer.more_info_url}',
            '.ratings .screen_name': 'offer.user.screen_name',       
            '.ratings .pos_count': 'offer.user.ratings_pos_count',
            '.ratings .neg_count': 'offer.user.ratings_neg_count',
            '.ratings .inc_count': 'offer.user.ratings_inc_count'
        });
    
        var tagsList = $('div.tags', offers).mapDirective({
            '.tag': 'tag <- offer.tags',
            'a[href]': 'tag.tag',
            'a+': 'tag.tag'
        });

        $('div.tags', offers).html(tagsList); //place sub-template tagsList into offers template
        $p.compile(offers, 'offers_render_fn'); //compile to a function
        
        /// -- end offers function
        
        ///  -- selected tags (bit at top) render function
        var selected_tags = $('#selected_tags .template').mapDirective({
            'span.select_tag_container': 'tag <- overall',
            '.select_tag_container .select_tag': 'tag.tag'
        });
        $p.compile(selected_tags, 'selected_tags_render_fn'); //compile to a function

        /// -- selected tags function
        
        
        /// -- available tags (down the side) render function
        var avail_tags = $('#avail_tags  .template').mapDirective({
            'ol.tags_sort': 'tag <- overall',
            '.tags_sort span': 'tag.tag',
            'input[value]': 'tag.tag',
            'em[style]': function(arg) {
                return "width:" + arg.item.pct + "px";
             },
             'input[onclick]': "'update();'",
             'a[onclick]': "'update();'",

        });
        $p.compile(avail_tags, 'avail_tags_render_fn'); //compile to a function
        /// -- end available tags (down the side) render function

    };
              
               
    function update_offers() {
        var json_url = build_search_query("/offers_json.aspx");
        $.getJSON(json_url, function(context) {
            //alert("context:" + context);
            $('#results_by_date').html($p.render('offers_render_fn', context));
        });
    }

    function update_selected_tags() {
        var json_url = build_search_query("/tagcounts_json.aspx");
        $.getJSON(json_url, function(context) {
            //alert("context:" + context);
             $('#selected_tags').html($p.render('selected_tags_render_fn', context));
        });
    }

    function update_avail_tags() {
        var json_url = build_search_query("/tags_json.aspx");
        $.getJSON(json_url, function(context) {
            //alert("context:" + context);
            $('#avail_tags').html($p.render('avail_tags_render_fn', context));
        });
    }

    function update() {
        update_offers();
        update_selected_tags();
        update_avail_tags();
    }

    $(document).ready(function() {
        compile_render_functions();
        $(".tags_sort input").click(update);
        update();
        //$("#content-offers").click(doOffersRender);
    });

	</script>  

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="box">
        <h2 id="selected_tags">
            <span class="template">
            <span class="select_tag_container">» <span class="select_tag">offers</span></span>
            </span>
        </h2>
    </div>
    <div id="content-offers" class="box">
       
        <div id="results_by_date">
         <span class="template">
            <!--- -RESULTS- ------>
            <div class="explanation">
	            <p>This screen shows offers that match your criteria.
	            To find out more, follow the link beside each one. All interactions need to be with
	            the person directly - if they don't provide enough information, we can't help you either.
	            Please provide enough info!</p>
            </div>
            
           
	        <div class="offer">
                <div class="offer_left">
                    <div class="date"></div><!-- /date -->
                    <div class="user">
                        <a href="#">
                        	<img alt="" src="#" />
                            <h4></h4>
                        </a>                                                                                                        
                    </div>
                    <div class="feedback">
                        <a href="#" class="report">report</a>
                        <a href="#" class="hide">hide</a>
                    </div>
                </div><!-- /offer-left -->
                <div class="offer_right">                                                                                                                                                                           
                    <div class="msg">                                                                                                                                               
                        <div class="text">offer text<!-- minus tags at end? -->
                        </div>
                        <div class="tags">
                            <span class="tag"><a class="" href="#">#</a>, </span>
                        </div><!-- /tags -->
                        <!-- float right, align baseline, if possible have preview -->
                        <a class="more_info_link" href="">more info »</a>
                    </div><!-- /msg -->
                 </div><!-- /offer_right -->
                <div class="ratings">
                    <b><span class="screen_name"></span> Ratings:</b>
                    <span class="rating"><span class="pos_count"></span> Positive</span>
                    <span class="rating"><span class="neg_count"></span> Negative</span>
                    <span class="rating"><span class="inc_count"></span> Incomplete</span>
					<a href="#">(how this works)</a>
                </div> <!-- ratings-->
            </div><!--/offer -->
               
            </span> <!-- template -->   
        </div><!--- /RESULTS- ------>
      
        
        <!-- end wrap -->
        <div id="bottom-nav">
            <p class="paging">
                <a rel="nofollow" href="/offers/ooooby/waiheke/garden?order=popular" class="disabled">
                &laquo; Previous</a> <a rel="nofollow" class="active" href="#">
                1</a> <a rel="nofollow" href="#">
                2</a> <a rel="nofollow" href="#">
                3</a> <a rel="nofollow" href="#">
                4</a> <a rel="nofollow" href="#">
                5</a> <a rel="nofollow" href="#">
                6</a> <a rel="nofollow" href="#">
                7</a> <a rel="nofollow" href="#">
                8</a> <a rel="nofollow" href="#">
                9</a> <a rel="nofollow" href="#">
                10</a> <a rel="nofollow" href="#">
                Next &raquo;</a>
            </p>
            <p class="backTop">
                <a href="/offers/ooooby/waiheke/garden#" onclick="javascript:document.scrollTo(0);return false;">
                    Back to top</a></p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="RightContent" runat="server">             
     <div class="sorted_tags box">
        <h3 class="section"><span>Filter by Tag</span></h3>
        <ol class="tags_sort tag">
            <li>
                <input type="checkbox"  value="garden" />
                <em style="width: 70px;"></em><a href="/offers/oooby/waiheke/"><span >garden</span></a></li>
            <li>
                <input type="checkbox" value="juice" />
                <em style="width: 30px;"></em><a href="/offers/oooby/waiheke/garden/mulch"><span >juice</span></a></li>
            <li>
                <input type="checkbox" value="fruit" />
                <em style="width: 20px;"></em><a href="/offers/supplies"><span >fruit</span></a></li>
            <li>
                <input type="checkbox" value="veges" />
                <em style="width: 18px;"></em><a href="/offers/veges"><span >veges</span></a></li>
            <li>
                <input type="checkbox" value="household" />
                <em style="width: 10px;"></em><a href="/offers/lemons"><span >household</span></a></li>
            
        </ol>
    </div>
    <div class="sorted_tags box">
        <h3><span>Filter by Location</span></h3>
        <ol class="tags_sort loc">
            <li>
                <input type="checkbox"  value="nz" />
                <em style="width: 80px;"></em><a href="/offers/oooby/nz/"><span >nz</span></a></li>
            <li>
                <input type="checkbox"  value="auckland" />
                <em style="width: 70px;"></em><a href="/offers/oooby/auckland"><span >auckland</span></a></li>
             <li>
                <input type="checkbox"  value="wellington" />
                <em style="width: 60px;"></em><a href="/offers/oooby/auckland"><span >wellington</span></a></li>                    
            <li>
                <input type="checkbox"  value="waiheke" />
                <em style="width: 30px;"></em><a href="/offers/waiheke"><span >waiheke</span></a></li>
            <li>
                <input type="checkbox" value="paekakariki" />
                <em style="width: 18px;"></em><a href="/offers/north_beach"><span >paekakariki</span></a></li>            
        </ol>
        <div id="Div3" style="padding: 0;">
            <h2>
                <span style="position: relative; margin-left: 27px;">»
                    <input type="text" name="" id="Text1" value="Enter a new location" />
                    <div id="Div4" class="tag-results">
                    </div>
                </span>
            </h2>
        </div>
    </div>   
	<div id="column_1" class="public">
    <div class="sorted_tags box">
        <h3><span>Filter by Type</span>
            <span class="any"><input type="checkbox"  value="any_type" />Any</span>
		</h3>
        <ol class="tags_sort type">
             <li>
                <input type="checkbox"  value="cash_only" />
                <em style="width: 70px;"></em><a href="/offers/oooby/waiheke/"><span >cash only</span></a></li>
            <li>
                <input type="checkbox"  value="free" />
                <em style="width: 50px;"></em><a href="/offers/supplies"><span >free</span></a></li>
            <li>
                <input type="checkbox"  value="barter" />
                <em style="width: 30px;"></em><a href="/offers/veges"><span >barter</span></a></li>
            <li>
                <input type="checkbox"  value="NZD" />
                <em style="width: 10px;"></em><a href="/offers/oooby/waiheke/garden/mulch"><span >NZD</span></a></li>
            <li>
                <input type="checkbox"  value="cash" />
                <em style="width: 10px;"></em><a href="/offers/oooby/waiheke/"><span >cash</span></a></li>

        </ol>
    </div>

    <div class="sorted_tags box">
        <h3><span>Filter by Group</span>
				  <span class="any"><input type="checkbox"  value="any_group" />Any</span>
			</h3>
        <ol class="tags_sort group">
            <li>
                <input type="checkbox" value="ooooby" />
                <em style="width: 70px;"></em><a href="/offers/oooby/waiheke/"><span >ooooby</span></a></li>
            <li>
                <input type="checkbox"  value="freecycle" />
                <em style="width: 30px;"></em><a href="/offers/oooby/waiheke/garden/mulch"><span >freecycle</span></a></li>           
        </ol>
        <a href="#">register your group</a>
    </div>
	</div>
    <div style="clear:both;" />
    <div class="module box" id="last-comments">
        <h3 class="section"><span>Comments</span></h3>
        <div style="float: left; width: 100%;  padding-top: 5px;
            margin-bottom: 10px;">
            <div style="float:left">
                <a rel="nofollow" >
                <img alt="profile_pic" style="margin-right: 5px; padding-right: 10px;" src="http://s3.amazonaws.com/twitter_production/profile_images/82440779/miles_normal.jpg" />
                <br />utunga</a>
             </div>
             <div >
                <strong style="float:right">Re: Shelly<br /><a href="#">trade xyz</a></strong>
                <p style="margin-bottom: 15px; padding-right: 20px;">Great trade, would trade again!</p>
             </div>
        </div>
    </div>
     <div id="avail_tags" class="sorted_tags box">
        <span class="template">
            <h3 class="section"><span>Filter by Tag</span></h3>
            <ol class="tags_sort tag">
                <li>
                    <input type="checkbox" value="garden" />
                    <em style="width:70px;"></em><a href="#"><span >garden</span></a></li>
            </ol>
        </span>
    </div>
</asp:Content>
