<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="twitter_auth.ascx.cs" Inherits="twademe.controls.twitter_auth" %>

<div id="InitalizeAuth" runat="server">
    <asp:ImageButton ID="SignInWithTwitter" runat="server" 
        ImageUrl="~/images/sign_in_with_twitter.png" 
        onclick="SignInWithTwitter_Click" />
</div>
<div id="SignedIn" runat="server">
    <img alt="profile pic" runat="server" id="ProfilePic" />
    Signed in as <span id="ProfileScreenName" runat="server" /> 
</div>