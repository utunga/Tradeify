<%@ Page Title="" Language="C#" MasterPageFile="~/masters/site.Master" AutoEventWireup="true"
    CodeBehind="index.aspx.cs" Inherits="twademe.index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript" src="js/jquery-1.3.2.js"></script>
<script type="text/javascript" src="js/purePacked.js"></script>
<script type="text/javascript">

    var selected_tags = [];
    var excluded_tags = [];

    function select_tag(tag_type, tag_txt) 
    {
        var tag = {};
        tag.tag = tag_txt;
        tag.type = tag_type;
        selected_tags.push(tag);
    }

    function unselect_tag( tag_type, tag_txt) 
    {
        var tag = {};
        tag.type = tag_type;
        tag.tag = tag_txt;
      
        var match_tag = tag.type + "_" + tag.tag;
        var found_tag_to_unselect = false;

        // iterate over current 'selected tags' and only keep them if
        // they are *not* equal to the tag we want to 'unselect'
        var tmp = []; 
        $.each(selected_tags, function() {
            var selected_match_tag = this.type + "_" + this.tag;
            if (match_tag != selected_match_tag) {
                tmp.push(this);
            }
            else {
                found_tag_to_unselect = true;
            }
        });
        selected_tags = tmp;
        
        // it is possible to 'unselect' a tag that was not already selected
        // this may sound whack, but it can happen if, a given tag is present on
        // all results, thus it's checkbox is 'checked' even though it wasn't explicitly checked.
        // so what do we do then? well it sounds a bit whack, but we want to add it to the 'exclude' 
        // list in this case
        if (!found_tag_to_unselect) {
            excluded_tags.push(tag);
        }
    }

    function toggle_select(input) 
    {
        if ($(input).attr("checked")) {
            select_tag(input.name, $(input).val());
        }
        else {
            unselect_tag(input.name, $(input).val());
        }
        update();
    }
    
    function is_selected(tag_type, tag_txt) {
        var match_tag = tag_type + "_" + tag_txt;
        var found = false;
        $.each(selected_tags, function() {
            var selected_match_tag = this.type + "_" + this.tag;
            if (match_tag == selected_match_tag) {
                found = true;
                return; // break out of this function
            }
        });
        return found;
    }
    
    function build_search_query(baseUrl) {

        //     turns out its not enough to have all the state present in the UI, 
        //     so we maintain state via a 'selected_tags' list.. (see above)
//        //tag, location, type, group
//        var tags = $(".sorted_tags.tag input:checked").map(function() {
//            return "tag=" + escape($(this).val());
//        }).get().join("&");
//        var types = $(".sorted_tags.type input:checked").map(function() {
//            return "type=" + escape($(this).val());
//        }).get().join("&");
//        var groups = $(".sorted_tags.group input:checked").map(function() {
//            return "group=" + escape($(this).val());
//        }).get().join("&");
//        var locations = $(".sorted_tags.loc input:checked").map(function() {
//            return "loc=" + escape($(this).val());
//        }).get().join("&");
//         var query = tags + "&" + types + "&" + groups + "&" + locations;

        var query = $(selected_tags).map(function() {
            return this.type + "=" + escape(this.tag);
        }).get().join("&");
        return baseUrl + "?" + query;
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
        var avail_tags = $('.sorted_tags .template').mapDirective({
            'ol.tags_sort': 'tag <- tags',
            '.tags_sort span': 'tag.tag',
            'input[value]': 'tag.tag',
            'input[name]': 'tag.type',
            'em[style]': function(arg) {
                return "width:" + arg.item.pct + "px";
            },
            'input[onclick]': "'toggle_select(this);'",
            'input[checked]': function(arg) {
                if (is_selected(arg.item.type, arg.item.tag)) { return "checked" }
                else return "";
            }
            //'a[onclick]': "'toggle_select(" + arg.item.tag + "," +  arg.item.tag + ");'"
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
            $('#selected_tags').html($p.render('selected_tags_render_fn', context));
            var tmp = [];
            $.each(context.overall, function() {
                tmp.push(this);
            });
            selected_tags = tmp;
        });
    }

    function update_avail_tags() {
        var json_url = build_search_query("/tags_json.aspx");
        var tag_types = ["tag", "loc", "group", "type"];
        $.getJSON(json_url, function(context) {
            // from the list of tags overall, we need to filter out tags of a given type
            $.each(tag_types, function() {
                var element_selector = '.sorted_tags.' + this + " .template"; // eg ".sorted_tags.loc .template"
                var matches = filter_tags(context.overall, this);
                $(element_selector).html($p.render('avail_tags_render_fn', { 'tags': matches }));
            });
        });
    }

    ///Used by above function to filter out tags of a particular type
    function filter_tags(source, desired_type) {
        var matches = [];
        $.each(source, function() {
            if (this.type == desired_type) {
                matches.push(this);
            }
        });
        return matches;
    }

    function update() {
        update_selected_tags();
        update_offers();
        update_avail_tags();
        $(".tags_sort input").click(update);
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
     <div class="sorted_tags box tag">
        <h3 class="section"><span>Filter by Tag</span></h3>
        <span class="template">
        <ol class="tags_sort">
                <li><input type="checkbox" value="" />
                    <em style="width:70px;"></em><a href="#"><span></span></a>
                </li>
        </ol>
        </span>
    </div>
    <div class="sorted_tags box loc">
        <h3><span>Filter by Location</span></h3>
        <span class="template">
        <ol class="tags_sort">
                <li><input type="checkbox" value="" />
                    <em style="width:70px;"></em><a href="#"><span></span></a>
                </li>
        </ol>
        </span>
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
    <div class="sorted_tags box type">
        <h3><span>Filter by Type</span>
            <span class="any"><input type="checkbox"  value="any_type" />Any</span>
		</h3>
		<span class="template">
        <ol class="tags_sort">
                <li><input type="checkbox" value="" />
                    <em style="width:70px;"></em><a href="#"><span></span></a>
                </li>
        </ol>
        </span>
    </div>

    <div class="sorted_tags box group">
        <h3><span>Filter by Group</span>
		    <span class="any"><input type="checkbox"  value="any_group" />Any</span>
	    </h3>
	    <span class="template">
        <ol class="tags_sort">
                <li><input type="checkbox" value="" />
                    <em style="width:70px;"></em><a href="#"><span></span></a>
                </li>
        </ol>
        </span>
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
