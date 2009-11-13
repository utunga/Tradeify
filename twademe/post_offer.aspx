<%@ Page Title="" Language="C#" MasterPageFile="~/masters/site.Master" AutoEventWireup="true" CodeBehind="post_offer.aspx.cs" Inherits="twademe.post_offer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.js"></script>
    
   <%-- <!-- twademe.org key --><script src="http://maps.google.com/jsapi?key=ABQIAAAABEpdHyPr3QztCREcH5edthQypDhEeaS-lwZnXa8YYYptMMZM5xTv8mOncJRz0T03-h1yE09ZY6daEw" type="text/javascript"></script> --%> 
   <!-- twooooby.org key --><script src="http://maps.google.com/jsapi?key=ABQIAAAABEpdHyPr3QztCREcH5edthTaUEcZrrSdLPsGRmAPjnLD6mzdjRRmQoKpGS1a_BMeq5GbhgxcOOVJBg" type="text/javascript"></script>

   <script language="Javascript" type="text/javascript">
       //<![CDATA[
       google.load("maps", "2");

       var geocoder;
       var map;

       function address_geocoded(point) {
           if (!point) {
               $("#twooby #search_location .infobox").html(".. unable to understand that address..");
               map.clearOverlays();
           }
           else {
               $("#twooby #search_location .infobox").html("<nobr>" + point + "</nobr>");
               map.setCenter(point, 13);
               var marker = new GMarker(point);
               map.addOverlay(marker);
               var address = $("#twooby #search_location #location").val();
               $("#twooby #search_location a").html("<nobr>" + address + "</nobr>");
           }

       }

       function open_search_for_edit() {
           var currentAddr = $("#twooby #search_location a.first").text();
           $("#twooby #search_location a").hide();
           $("#twooby #search_location input").val(currentAddr);
           $("#twooby #search_location input").show();
           $("#twooby #search_location #location").change(geo_code_search_location).change(); // set up change event, then trigger it
           $("#twooby #search_location #location").keyup(function(event) {
               if ((event.keyCode == 27) || // escape key
                    (event.keyCode == 13))  // enter key
               {
                   //FIXME can't seem to capture tab key
                   //FIXME should do one last lookup in case of impatience
                   close_search_for_edit();
               }
               else {
                   var timeoutID;
                   window.clearTimeout(timeoutID);
                   timeoutID = window.setTimeout(geo_code_search_location, 200);
               }
           });
           $("#twooby #search_location input").select();
           $("#twooby #search_location input").focus();
       }

       function geo_code_search_location() {
           $("#twooby #search_location .infobox").hide();
           $("#map_canvas").show();
           var address = $("#twooby #search_location #location").val();
           if (geocoder) {
               geocoder.getLatLng(address, address_geocoded)
           }
           $("#map_canvas").click(close_search_for_edit);
       }

       function close_search_for_edit() {
           $("#map_canvas").hide();
           $("#twooby #search_location input").hide();
           $("#twooby #search_location a").show();
           $("#twooby #search_location .infobox").show();
       }

       // This function called after google api has been loaded
       function google_initialize() {
           $("#map_canvas").show();
           map = new GMap2(document.getElementById("map_canvas"));
           $("#map_canvas").hide();
           $("#twooby #search_location input").hide()
           if (google.loader.ClientLocation) {
               currentLocation = google.loader.ClientLocation;
               $("#twooby #search_location a").html(currentLocation.address.city + ", " + currentLocation.address.region + ", " + currentLocation.address.country_code);
               $("#twooby #search_location .infobox").html("(" + currentLocation.latitude + "," + currentLocation.longitude + ")");
               map.setCenter(new GLatLng(currentLocation.latitude, currentLocation.longitude), 12);
           }
           $("#twooby #search_location a").click(open_search_for_edit);
           geocoder = new GClientGeocoder();
       }

       //]]>
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<asp:PlaceHolder ID="AuthRequiredWarning" runat="server" Visible="true">
     <h4>You need to sign in to Twitter first</h4>
     <h5>Authorize this website using the 'sign in with twitter' button at top right then you can get started</h5>
</asp:PlaceHolder>

<asp:PlaceHolder ID="PostMessage" runat="server" Visible="false">
            <h4>What are you offering?</h4>
           
            <div style="float:left;">
            <div class="info">
              <textarea accesskey="u" id="Status" runat="server" class="part_of_message" rows="2" cols="40">#offr_test </textarea>
            </div>
           
            </div>
            <div>

                <span class="numeric" id="chars_left_notice">
                    <strong class="char-counter" id="status-field-char-counter" style="color: rgb(204, 204, 204);">140</strong>
                </span>
                 <%--<h5>The message is not yet valid but keep typing</h5>--%>
                <input type="submit" value="TWEET IT!"/>
            </div>
            <div class="clear"></div>
           <hr />

            <asp:PlaceHolder ID="PostedMessageStatus" runat="server" Visible="false">
                <h4>Successfully Posted</h4>
            </asp:PlaceHolder>
            <%--<h5>Or use the form below to gaurantee your message is valid</h5>
            
            <input type="hidden" id="twitterUserName" name="twitterUserName" />
            <div class="input_box">
            	<label for="offer">What are you offering?</label>
            	  <textarea accesskey="o" id="offer" class="block_input part_of_message" name="offer" rows="2" cols="40">I am offering</textarea>
                  <div class="example">e.g.  "I have some surplus cabbages" / "I am offering gardening services"</div>
            </div>

            <div class="clear" ></div>
            <div class="input_box">
           		<div class="part_of_message">
                    in <span id="search_location">L:<a class="first" href="#" >you</a>
                   <span id="map_canvas"></span>
                   <input type="text" id="location" value="" />
                </div>

                <div class="clear" ></div>
                <div class="example">e.g.  "in L:Wellington, NZ" (click to change)</div>
            </div>
            <div class="input_box">
            	<div id="currencies" class="part_of_message">
            	    for <span id="included"><a href="#" >#free</a></span>
            	    <input id="rate" type="hidden" value="#free" />

                </div>
                <div class="example">e.g.  "for #free", "for #barter", "for #NZD or #barter"</div>
                <div class="example"> Click to add/remove
                    <ul class="currency_choice">
                            <li><a href="#">#free</a></li>
                            <li><a href="#">#barter</a></li>
                            <li><a href="#">#pledge</a></li>

                            <li><a href="#">#well_talents</a></li>
                            <li><a href="#">#NZD</a></li>
                     </ul>
          	    </div>
               
          	  <div class="input_box">
            	<label for="rate" >Where can they go for more information?</label>
                <input id="url_part" class="block_input part_of_message" name="detailUrl" value="" />

          	  </div>--%>
            
        
        </div>
</asp:PlaceHolder>
    
       
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="RightContent" runat="server">
</asp:Content>
