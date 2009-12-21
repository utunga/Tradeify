<%@ Page Title="" Language="C#" MasterPageFile="~/masters/site.Master" AutoEventWireup="true"
    CodeBehind="post_offer.aspx.cs" Inherits="twademe.post_offer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.js"></script>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" type="text/javascript"></script>
    <script src="http://tradeify.org/joav/jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>
    <script src="http://maps.google.com/jsapi?key=ABQIAAAABEpdHyPr3QztCREcH5edthTaUEcZrrSdLPsGRmAPjnLD6mzdjRRmQoKpGS1a_BMeq5GbhgxcOOVJBg" type="text/javascript"></script>

    <script language="Javascript" type="text/javascript">
        //<![CDATA[
        google.load("maps", "2");

        var geocoder;
        var map;

        function address_geocoded(point) {
            if (!point) {
                $(".location .infobox").html(".. unable to understand that address..");
                map.clearOverlays();
            }
            else {
                $(".location .infobox").html("<nobr>" + point + "</nobr>");
                map.setCenter(point, 13);
                var marker = new GMarker(point);
                map.addOverlay(marker);
                var address = $(".location #location").val();
                $(".location a").html("<nobr>" + address + "</nobr>");
            }
        }

        function geo_code_search_location() {
            $("#map_canvas").show();
            var address = $(".location #location").val();
            if (geocoder) {
                geocoder.getLatLng(address, address_geocoded)
            }
        }

        // This function called after google api has been loaded
        function google_initialize() {
            $("#map_canvas").show();
            map = new GMap2(document.getElementById("map_canvas"));
            //$("#map_canvas").hide();
            //$(".location #location").hide();
            if (google.loader.ClientLocation) {
                currentLocation = google.loader.ClientLocation;
                var currentAddr = currentLocation.address.city + ", " + currentLocation.address.region + ", " + currentLocation.address.country_code
                $(".location #location").val(currentAddr);
                $(".location .infobox").html("(" + currentLocation.latitude + "," + currentLocation.longitude + ")");
                map.setCenter(new GLatLng(currentLocation.latitude, currentLocation.longitude), 12);
            }
            $(".location  #location").keyup(function(event) {
                if ((event.keyCode != 27) || // escape key
                   (event.keyCode != 13))  // enter key
                {
                    var timeoutID;
                    window.clearTimeout(timeoutID);
                    timeoutID = window.setTimeout(geo_code_search_location, 200);
                }
            });
            geocoder = new GClientGeocoder();
        }

        google.setOnLoadCallback(google_initialize);
        //]]>
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:PlaceHolder ID="AuthRequiredWarning" runat="server" Visible="true">
        <h4>
            You need to sign in to Twitter first</h4>
        <h5>
            Authorize this website using the 'sign in with twitter' button at top right then
            you can get started</h5>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="PostMessage" runat="server">
        <h4>
            What are you offering?</h4>
        <div style="float: left;">
            <div class="info">
                <textarea accesskey="u" id="Status" runat="server" class="part_of_message" rows="2"
                    cols="40">#offr </textarea>
            </div>
        </div>
        <div>
            <span class="numeric" id="chars_left_notice"><strong class="char-counter" id="status-field-char-counter"
                style="color: rgb(204, 204, 204);">140</strong> </span>
            <%--<h5>The message is not yet valid but keep typing</h5>--%>
            <input type="submit" value="TWEET IT!" />
        </div>
        <div class="clear">
        </div>
        <hr />
        <asp:PlaceHolder ID="PostedMessageStatus" runat="server" Visible="false">
            <h4>
                Successfully Posted</h4>
        </asp:PlaceHolder>
        <h5>
            Or use the form below to gaurantee your message is valid</h5>
        <input type="hidden" id="twitterUserName" name="twitterUserName" />
        <ul>
            <li>
                <label class="desc" id="title2" for="message">
                </label>
                <div>
                    <textarea id="message" name="message" class="field textarea small" rows="10" cols="50"
                        tabindex="1"></textarea>
                </div>
            </li>
            <li>
                <label class="desc" for="offer">
                    I am offering <span id="req_3" class="req">*</span>
                </label>
                <div>
                    <textarea onkeyup="updateOffer()" id="offer" name="offer" type="text" rows="5" cols="50"></textarea>
                </div>
            </li>
            <li class="location">
                <label class="desc" for="location">
                    In <span id="req_6" class="req">* L:</span>
                </label>
                <div>
                    <input id="location" onkeyup="updateOffer()" name="location" type="text" class="field text medium"
                        maxlength="255" tabindex="3" />
                    <div id="map_canvas">
                    </div>
                    <div class="infobox">
                    </div>
                    <div class="example">
                        e.g. "in L:Wellington, NZ" (click to change)</div>
                </div>
            </li>
            <li>
                <label class="desc" for="for">
                    For <span id="req_209" class="req">*</span>
                </label>
                <div>
                    <select id="for" name="for" class="field select medium" tabindex="4">
                        <option value="#free">#free </option>
                        <option value="#barter">#barter </option>
                    </select>
                </div>
            </li>
            <li id="li_date">
                <label class="desc" for="until">
                    Until
                </label>
                <div>
                    <input id="until" name="until" type="text"></input>
                </div>
            </li>
        </ul> 
        
        <hr style="width: 500px; height: 1px; color: Black; display: block; text-align: left;" />
        
		<ul>
            <li>
                <label class="desc" id="title212" for="Field212">
                    Link to picture
                </label>
                <img src="" />
                <input id="Field212" type="text" class="field text medium" value="" maxlength="255"
                    tabindex="8" />
            </li>
            
            <li>
                <label class="desc" id="Label1" for="Field212">
                    Tags:
                </label>
                <input id="tags" class="field text medium" value="" onkeyup="timeoutKeyChange()"
                    maxlength="255" tabindex="8" />
            </li>
            <li>Current tags:
                <ul id="selected_tags">
                    <li><a href="#" class="tag"></a></li>
                </ul>
            </li>
            <li>Suggested Tags:
                <div id="suggested_tags">
                    <span class="template">
                        <ul class="select_tag_container">
                            <li><a href="#" class="tag"></a></li>
                        </ul>
                    </span>
                </div>
            </li>
            <li>
                <input type="submit" value="test" />
            </li>
        </ul>

        <script type="text/javascript">
            $(function() {
                $("#until").datepicker();
            });


            function MessagePart(type, field) {
                this.prefix = type;
                this.field = field;
            }

            var offerPrefix = " I am offering ";
            var locationPrefix = " in L:";
            var locationSuffix = ": ";
            var forPrefix = " for ";
            var untilPrefix = " until ";
            var linkPrefix = "here is a link to an image: ";
            function getUntil() {
                if ($("#until").val().length == 0) {
                    return "";
                }
                else {
                    return untilPrefix + $("#until").val();
                }
            }

            function updateOffer() {
                var concatMessage =
                         offerPrefix +
                         $("#offer").val() +
                         locationPrefix + $("#location").val() + locationSuffix +
                         forPrefix + $("#for").val() +
                         getUntil();
                $("#message").val(concatMessage);
            }
            var selected_tags = [];
            var tags = [];

            function onClick() {
                var tagField = $("#tags").val();
                var pushed_tag = $(this).html();
                if ($.inArray(pushed_tag, selected_tags) <= -1) {
                    if (tagField != "")
                        $("#tags").val(tagField + " " + pushed_tag);
                    else $("#tags").val(pushed_tag);
                }
                else {
                    //var txt = new RegExp("(,\s*" + pushed_tag + "\s*)|(^\s*" + pushed_tag + "\s*,*)");
                    var replacementText = tagField.replace(pushed_tag, "");
                    $("#tags").val(replacementText);
                }
                updateTags();
                return false;
            }

            function getTagString() {
                var tagString = ""
                $.each(selected_tags, function() {
                    tagString = tagString + " #" + this;
                });
                return tagString;
            }

            function checkCSS() {
                $.each($(".select_tag"), function() {
                    if ($.inArray($(this).html(), selected_tags) > -1) {
                        $(this).addClass("on");
                    }
                    else if ($(this).hasClass("on")) $(this).removeClass("on");
                });
            }

            var threshold = 200;
            var keyChangeStack = 0;
            function timeoutKeyChange() {
                keyChangeStack++;
                setTimeout(function() {
                    keyChangeStack--;
                    if (keyChangeStack == 0) {
                        updateTags();
                    }
                }, threshold);
            }

            function updateTags() {

                //get rid of multiple spaces...
                var txt = new RegExp("\\s\\s+");
                $("#tags").val($("#tags").val().replace(txt, " "));
                //just in case a single \t or \n is present
                txt = new RegExp("\\s+");
                $("#tags").val($("#tags").val().replace(txt, " "));

                selected_tags = $("#tags").val().split(" ");

                var selectedTagsHTML = $(selected_tags).map(function() {
                    return "#" + this;
                }).get().join(", ");


                $("#selected_tags").html(selectedTagsHTML);

                var json_url = build_search_query_tags("/tags_json.aspx");
                $.getJSON(json_url, function(context) {
                    tags = context.tags_json.overall;
                    var tagString = "";
                    $.each(tags, function() {
                        var on = "";
                        var endon = "";
                        if (($.inArray(this.tag, selected_tags) > -1)) {
                            on = "<li class=\"on\">";
                            endon = "</li>";
                        }
                        else {
                            on = "<li>";
                            endon = "</li>";
                        }
                        tagString = (tags === "") ? this.tag : tagString + "\n" + on + "<a href=\"#\" class=\"select_tag\">" + this.tag + "</a>" + endon;
                    });

                    //alert(tagString);
                    //if (!(typeof suggested_tags_render_fn == 'function')) {
                    //if not yet compiled compile it
                    /*    compile_render_fn();
                
                var render = $p.render('suggested_tags_render_fn', tags);
                    */
                    $('#suggested_tags').html(tagString);
                    $("#suggested_tags .select_tag").click(onClick);
                });


                //checkCSS();
                //alert(getTagString());
            }

            //        function compile_render_fn() {

            //            var selected_tags = $('#suggested_tags .template').mapDirective({
            //                '.select_tag_container': 'tag <- overall',
            //                '.select_tag_container .tag': 'tag.tag'
            //            });
            //            $p.compile(selected_tags, 'suggested_tags_render_fn'); //compile to a function
            //        }

            function build_search_query_tags(baseUrl) {
                if (tags.length == 0) return baseUrl;
                var query = "";
                $.each(tags, function() {
                    if ($.inArray(this.tag, selected_tags) > -1)
                        query = query + this.type + "=" + escape(this.tag) + "&";
                });
                query = query.substring(0, query.length - 1);
                return baseUrl + "?" + query;
            }
            function build_search_query(baseUrl) {
                var query = $(selected_tags).map(function() {
                    return this.type + "=" + escape(this.tag);
                }).get().join("&");
                return baseUrl + "?" + query;
            }
            
        </script>

    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="RightContent" runat="server">
</asp:Content>
