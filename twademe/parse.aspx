<%@ Page Title="Parse" MasterPageFile="~/masters/site.Master" Language="C#" AutoEventWireup="true" CodeBehind="parse.aspx.cs" Inherits="twademe.parse" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h4>For developers</h4>
<h3>Message Parser</h3>
<p><b>To get a version of a plain text message 'parsed' as JSON, http post or get this page passing a parameter 'message' contain unparsed message text.</b></p>
<p><b>Or just use the form below</b></p>
<form action="parse.aspx" method="get">
<textarea accesskey="u" name="message" rows="5" cols="60">Type your message here</textarea>
<input type="submit" value="Go!" />
</form>
</asp:Content>