<%@ Page Title="" Language="C#" MasterPageFile="~/masters/site.Master" AutoEventWireup="true"
    CodeBehind="index.aspx.cs" Inherits="twademe.index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript" src="js/jquery-1.3.2.js"></script>
<script type="text/javascript" src="js/purePacked.js"></script>
<script type="text/javascript">

//    function submitQuery() {
//        $(".tags_sort li input").each(function() {
//            alert($(this).value());
//        });
//    }

//    $(document).ready(function() {
//        $(".tags_sort li input").click(submitQuery);
//        $(".tags_sort li a").click(submitQuery);
//    });


    function compile_offers_render_fn() {

        var offers = $('#results_by_date').mapDirective({
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
    };

    function get_offers() {
        var json_url = build_search_query();
        $.getJSON(json_url, function(context) {
            //alert("context:" + context);
            $('#results_by_date').html($p.render('offers_render_fn', context));
        });
    }

    function build_search_query() {

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
        return "/offers_json.aspx?" + query;
    }
    
    $(document).ready(function() {
        compile_offers_render_fn();
        get_offers();
        $(".tags_sort input").click(get_offers);    
        //$("#content-offers").click(doOffersRender);
    });

	</script>  

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div  class="box">
        <h2>
            <span class="select_tag">ooooby</span> <span>» </span><span class="select_tag">waiheke</span>
            <span>» </span><span class="select_tag">garden</span>
        </h2>
    </div>
    <div id="content-offers" class="box">
       
        <div id="results_by_date">
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
		
    <%--<div class="offer">
	            
                <div class="offer_left">
                    <div class="date">12 Jun 09</div><!-- /date -->
                    <div class="user">
                        <a href="user_profile">
                            <img src="http://s3.amazonaws.com/twitter_production/profile_images/82440779/miles_bigger.jpg" />
                            <h4>Utunga</h4>
                        </a>
                    </div>
                    <div class="feedback">
                        <a href="#" class="report">report</a>
                        <a href="#" class="hide">hide</a>
                    </div>
                </div><!-- /offer-left -->
                <div class="offer_right">
                    <div class="msg">
                        <div class="text">
                            I am offering #garden #mulch #free to anyone that wants it L:30 Waitohu road
                        </div>
                        <div class="tags">
                            <a href="#">#free</a>, 
                            <a href="#">#ooooby</a>, 
                            <a href="#">#garden</a>, 
                            <a href="#">#mulch</a>
                        </div><!-- /tags -->
                        <!-- float right, align baseline, if possible have preview -->
                        <a href="http://bit.ly/more_info_link" class="more_info_link">more info »</a>
                    </div><!-- /msg -->
                 </div><!-- /offer_right -->
                <div class="ratings">
                    <b>Utunga's Ratings:</b>
                    <span class="rating">12 Positive</span>
                    <span class="rating">1 Negative</span>
                    <span class="rating">14 Incomplete</span>
					<a href="#">(how this works)</a>
                </div> <!-- ratings-->
            </div><!--/offer -->

            <div class="offer">
                <div class="offer_left">
                    <div class="date">12 Jun 09</div><!-- /date -->
                    <div class="user">
                        <a href="user_profile">
                            <img src="http://s3.amazonaws.com/twitter_production/profile_images/140759410/avatar_bigger.jpg" />
                            <h4>Shelly</h4>
                        </a>
                    </div>
                    <div class="feedback">
                        <a href="#" class="report">report</a>
                        <a href="#" class="hide">hide</a>
                    </div>
                </div><!-- /offer-left -->
                <div class="offer_right">
                    <div class="msg">
                        <div class="text">
			            Come get some hot plum sauce before its all gone L:Breakhouse Bay
                        </div>
                        <div class="tags">
                            <a href="#">#cash_only</a>, 
                            <a href="#">#food</a>, 
                            <a href="#">#ooooby</a>, 
                            <a href="#">#garden</a>
                        </div><!-- /tags -->
                        <!-- float right, align baseline, if possible have preview -->
                        <a href="http://bit.ly/more_info_link" class="more_info_link">more info »</a>
                    </div><!-- /msg -->
                 </div><!-- /offer_right -->
                <div class="ratings">
                    <b>Shelly's Ratings:</b>
                    <span class="rating">1 Positive <a href="#">*</a></span>
                    <span class="rating">2 Incomplete <a href="#">*</a></span>
					<a href="#">(how this works)</a>
                </div> <!-- ratings-->
            </div><!--/offer -->


            <div class="offer">
                <div class="offer_left">
                    <div class="date">10 Jun 09</div><!-- /date -->
                    <div class="user">
                        <a href="user_profile">
                            <img src="http://s3.amazonaws.com/twitter_production/profile_images/140759410/avatar_bigger.jpg" />
                            <h4>Shelly</h4>
                        </a>
                    </div>
                    <div class="feedback">
                        <a href="#" class="report">report</a>
                        <a href="#" class="hide">hide</a>
                    </div>
                </div><!-- /offer-left -->
                <div class="offer_right">
                    <div class="msg">
                        <div class="text">
			            Why does no-one want my #Kumera ?! L:Breakhouse Bay
                        </div>
                        <div class="tags">
                            <a href="#">#cash_only</a>, 
                            <a href="#">#food</a>, 
                            <a href="#">#ooooby</a>, 
                            <a href="#">#garden</a>
                            <a href="#">#kumera</a>
                        </div><!-- /tags -->
                        <!-- float right, align baseline, if possible have preview -->
                        <a href="http://bit.ly/more_info_link" class="more_info_link">more info »</a>
                    </div><!-- /msg -->
                 </div><!-- /offer_right -->
                <div class="ratings">
                    <b>Shelly's Ratings:</b>
                    <span class="rating">1 Positive <a href="#">*</a></span>
                    <span class="rating">2 Incomplete <a href="#">*</a></span>
					<a href="#">(how this works)</a>
                </div> <!-- ratings-->
            </div>--%><!--/offer -->

			<%--offer.date
            offer.offer_text
            offer.more_info_url
            offer.tags
            offer.tags[i].tag
            offer.user.screen_name
            offer.user.profile_pic_url
            offer.user.more_info_url
            offer.user.ratings.positive_count
            offer.user.ratings.negative_count
            offer.user.ratings.incomplete_count
            
            <!-- will need later -->
			offer.location.geo_lat
            offer.location.geo_long
	        offer.location.text	
            
            <div class="offer">
                <div class="offer_left">
                    <div class="date"><%= offer.date %></div><!-- /date -->
                    <div class="user">
                        <a href="<%= offer.user.more_info_url %>">
                        	<%= offer.user.profile_pic_url %>
                            <h4><%= offer.user.screen_name %></h4>
                        </a>
                    </div>
                    <div class="feedback">
                        <a href="#" class="report">report</a>
                        <a href="#" class="hide">hide</a>
                    </div>
                </div><!-- /offer-left -->
                <div class="offer_right">
                    <div class="msg">
                        <div class="text">
							<%= offer.offer_text %> <!-- minus tags at end? -->
                        </div>
                        <div class="tags">
                            <% for(i=0; i<offer.tags.count; i++ ) { %> <!-- minus tags at end? -->
                            <a class="tag" href="#">#<%= offer.tags[i].tag %></a>
                            	<% if (i<offer.tags.count-1) 
                                	{%>,
                                 <% } %>
                            <% } %>
                        </div><!-- /tags -->
                        <!-- float right, align baseline, if possible have preview -->
                        <a href="<%= offer.more_info_url %>" class="more_info_link">more info »</a>
                    </div><!-- /msg -->
                 </div><!-- /offer_right -->
                <div class="ratings">
                    <b><%= offer.user.screen_name %> Ratings:</b>
                    <span class="rating"><%= offer.user.ratings.positive_count %> Positive</span>
                    <span class="rating"><%= offer.user.ratings.negative_count %> Negative</span>
                    <span class="rating"><%= offer.user.ratings.incomplete_count %> Incomplete</span>
					<a href="#">(how this works)</a>
                </div> <!-- ratings-->
            </div><!--/offer -->
--%>


            <!--- /RESULTS- ------>
        </div>
      


        
        <!-- end wrap -->
        <div id="bottom-nav">
            <p class="paging">
                <a rel="nofollow" href="/offers/ooooby/waiheke/garden?order=popular" class="disabled">
                &laquo; Previous</a> <a rel="nofollow" class="active" href="/offers/ooooby/waiheke/garden?order=popular">
                1</a> <a rel="nofollow" href="/offers/ooooby/waiheke/garden?page=2&amp;order=popular">
                2</a> <a rel="nofollow" href="/offers/ooooby/waiheke/garden?page=3&amp;order=popular">
                3</a> <a rel="nofollow" href="/offers/ooooby/waiheke/garden?page=4&amp;order=popular">
                4</a> <a rel="nofollow" href="/offers/ooooby/waiheke/garden?page=5&amp;order=popular">
                5</a> <a rel="nofollow" href="/offers/ooooby/waiheke/garden?page=6&amp;order=popular">
                6</a> <a rel="nofollow" href="/offers/ooooby/waiheke/garden?page=7&amp;order=popular">
                7</a> <a rel="nofollow" href="/offers/ooooby/waiheke/garden?page=8&amp;order=popular">
                8</a> <a rel="nofollow" href="/offers/ooooby/waiheke/garden?page=9&amp;order=popular">
                9</a> <a rel="nofollow" href="/offers/ooooby/waiheke/garden?page=10&amp;order=popular">
                10</a> <a rel="nofollow" href="/offers/ooooby/waiheke/garden?page=2&amp;order=popular">
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
                    <input type="checkbox" checked="checked" value="garden" />
                    <em style="width: 70px;"></em><a href="/offers/oooby/waiheke/"><span >garden</span></a></li>
                <li>
                    <input type="checkbox" value="mulch" />
                    <em style="width: 30px;"></em><a href="/offers/oooby/waiheke/garden/mulch"><span >mulch</span></a></li>
                <li>
                    <input type="checkbox" value="supplies" />
                    <em style="width: 20px;"></em><a href="/offers/supplies"><span >supplies</span></a></li>
                <li>
                    <input type="checkbox" value="veges" />
                    <em style="width: 18px;"></em><a href="/offers/veges"><span >veges</span></a></li>
                <li>
                    <input type="checkbox" value="lemons" />
                    <em style="width: 10px;"></em><a href="/offers/lemons"><span >lemons</span></a></li>
                
            </ol>
        </div>
        <div class="sorted_tags box">
            <h3><span>Filter by Location</span></h3>
            <ol class="tags_sort location">
                <li>
                    <input type="checkbox" checked="checked" value="nz" />
                    <em style="width: 80px;"></em><a href="/offers/oooby/nz/"><span >nz</span></a></li>
                <li>
                    <input type="checkbox" checked="checked" value="auckland" />
                    <em style="width: 80px;"></em><a href="/offers/oooby/auckland"><span >auckland</span></a></li>
                <li>
                    <input type="checkbox" checked="checked" value="waiheke" />
                    <em style="width: 80px;"></em><a href="/offers/waiheke"><span >waiheke</span></a></li>
                <li>
                    <input type="checkbox" value="north_beach" />
                    <em style="width: 18px;"></em><a href="/offers/north_beach"><span >north beach</span></a></li>
                <li>
                    <input type="checkbox" value="south_bay" />
                    <em style="width: 10px;"></em><a href="/offers/south_bay"><span >south bay</span></a></li>
                
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
	</div>    
	<div id="column_1" class="public">
    <div class="sorted_tags box">
        <h3><span>Filter by Type</span>
            <span class="any"><input type="checkbox" checked="checked" value="any_type" />Any</span>
		</h3>
        <ol class="tags_sort type">
            <li>
                <input type="checkbox" checked="checked" value="cash_only" />
                <em style="width: 70px;"></em><a href="/offers/oooby/waiheke/"><span >cash only</span></a></li>
            <li>
                <input type="checkbox" checked="checked" value="NZD" />
                <em style="width: 60px;"></em><a href="/offers/oooby/waiheke/garden/mulch"><span >NZD</span></a></li>
            <li>
                <input type="checkbox" checked="checked" value="free" />
                <em style="width: 20px;"></em><a href="/offers/supplies"><span >free</span></a></li>
            <li>
                <input type="checkbox" checked="checked" value="barter" />
                <em style="width: 18px;"></em><a href="/offers/veges"><span >barter</span></a></li>
        </ol>
    </div>

    <div class="sorted_tags box">
        <h3><span>Filter by Group</span>
				  <span class="any"><input type="checkbox" checked="checked" value="any_group" />Any</span>
			</h3>
        <ol class="tags_sort group">
            <li>
                <input type="checkbox" checked="checked" value="garden" />
                <em style="width: 70px;"></em><a href="/offers/oooby/waiheke/"><span >ooooby</span></a></li>
            <li>
                <input type="checkbox" checked="checked" value="mulch" />
                <em style="width: 30px;"></em><a href="/offers/oooby/waiheke/garden/mulch"><span >freecycle-waiheke</span></a></li>
            <li>
                <input type="checkbox" checked="checked" value="supplies" />
                <em style="width: 20px;"></em><a href="/offers/supplies"><span >freecycle-auckland</span></a></li>
            <li>
                <input type="checkbox" checked="checked" value="veges" />
                <em style="width: 18px;"></em><a href="/offers/veges"><span >AUC</span></a></li>            
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
</asp:Content>
