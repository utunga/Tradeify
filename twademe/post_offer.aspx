<%@ Page Title="" Language="C#" MasterPageFile="~/masters/site.Master" AutoEventWireup="true"
    CodeBehind="post_offer.aspx.cs" Inherits="twademe.post_offer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.js"></script>

    <%-- <!-- twademe.org key --><script src="http://maps.google.com/jsapi?key=ABQIAAAABEpdHyPr3QztCREcH5edthQypDhEeaS-lwZnXa8YYYptMMZM5xTv8mOncJRz0T03-h1yE09ZY6daEw" type="text/javascript"></script> --%>
    <!-- twooooby.org key -->

    <script src="http://maps.google.com/jsapi?key=ABQIAAAABEpdHyPr3QztCREcH5edthTaUEcZrrSdLPsGRmAPjnLD6mzdjRRmQoKpGS1a_BMeq5GbhgxcOOVJBg"
        type="text/javascript"></script>

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
    
    <style>
    .location input 
    {
    	width:400px;
    }
    </style>

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
        <h5>Or use the form below to gaurantee your message is valid</h5>
        <input type="hidden" id="twitterUserName" name="twitterUserName" />
        <div class="info">
            <h2>
                Offer Form</h2>
            <div>
                Fill out these fields to create an offer message.</div>
        </div>
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
                    <textarea onkeyup="updateOffer()" id="offer" name="offer" type="text" rows="5" cols="50" ></textarea>
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
                        /div>
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
                <input id="until" name="until" type="text"><!-- date picker goes here -->
                </div> </li>
        </ul>
        <hr style="width:500px; height:1px; color:Black; display:block; text-align:left;"/>
        <ul>
            <li>
                <label class="desc" id="title212" for="Field212">
                    Link to picture
                </label>
                <img src="" />
                <input id="Field212" type="text" class="field text medium" value=""
                    maxlength="255" tabindex="8" />
            </li>
            <li>
                 <label class="desc" id="Label1" for="Field212">
                    Tags:
                </label>
               
                <input id="tags" class="field text medium" value="" onchange="updateTags()"
                    maxlength="255" tabindex="8" />                    
            </li>
            <li>
            Recommended tags:  
            <div class="recommended_tags" name="recommended_tags" id="recommended_tags">None</div>
            </li>
            <li id="li_MoreInfo" class="     ">
                <label class="desc" id="lab_MoreInfo" for="More info:">
                    More info:
                </label>
                <input id="More info:" name="More info:" type="text" class="field text medium" value=""
                    maxlength="255" tabindex="8" />
            </li>
            <li class="buttons ">
                <input id="saveForm" name="saveForm" class="btTxt submit" type="submit" value="Submit" />
            </li>
        </ul>

        <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" type="text/javascript"></script>

        <script src="http://tradeify.org/joav/jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>

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
            function updateTags() {
                selected_tags = $("#tags").val().split(",");
                $.each(selected_tags, function() {
                    this.trim();
                });
                var json_url = build_search_query("/tags_json.aspx");
                $.getJSON(json_url, function(context) {
                    var recommended_tags = context.tags_json.overall
                    var tags = "";
                    $.each(recommended_tags, function() {
                        tags = (tags === "") ? this.tag : tags + " , " + this.tag;
                    });
                    alert(tags + $("#recommended_tags").val());
                    $("#recommended_tags").val(tags);
                    alert($("#recommended_tags").val());
                });
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
