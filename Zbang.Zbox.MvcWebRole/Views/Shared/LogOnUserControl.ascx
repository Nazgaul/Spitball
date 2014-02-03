<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="<%  if (Request.IsAuthenticated) { %>LogOnUserControl<% }else{ %>LogOnUserControlNotAuth<% } %>">
    <img src="/Content/Images/default-user-avatar.png" id="user-avatar" alt="avatar" />
    <%
        if (Request.IsAuthenticated)
        {
    %>
    <script type="text/javascript">

        $(document).ready(function () {
            // $('.VerificationStatus>a').click(function (e) {
            $('#emailVerificationStatus').find('a').click(function (e) {
                e.preventDefault();
                var ResendEmailVerification = new ZboxAjaxRequest({
                    url: '/Account/ResendEmailVerification',
                    success: function (data) {
                        Zbox.toaster('Verification email sent');
                    },
                    error: function (error) {
                    }
                });
                ResendEmailVerification.Post();

            });




            // changes dropdown arrow + right side of dropdown bg
            $('#usrDropdonRight').hide();
            $('#LogOnUserControl').hover(  //  
                           function () {
                               $('#usrdropArrow').attr("src", "/Content/Images/user-arrow-down-hover.png");
                               $('#usrDropdonRight').show();
                           },
                          function () {
                              $('#usrdropArrow').attr("src", "/Content/Images/user-arrow-down.png");
                              $('#usrDropdonRight').hide();
                          }
                        );
        });
    </script>
    <div id="usrnameBtn" class="white txt-13 bold nobr ">
        Hello
        <%: Page.User.Identity.Name%>
        <img alt="" id="usrdropArrow" src="/Content/Images/user-arrow-down.png" width="9"
            height="5" />
    </div>
    <div id="usrDropdown" class="style4 nobr">
        <a href="#" id="settings">settings</a>
        <%: Html.ActionLink("Sign out", "LogOff", "Account")%>
    </div>
    <%
        }
        else
        {
    %>
    <%--can be only when user on share box--%>
    <div id="style56 nobr" style="width: 280px;">
        <div class="style56 nobr float-left nonAuthLink ">
            Sign-up
            <%:Html.ActionLink("here", "SubscribeToSharedBox", "Collaboration", new { boxId = ViewData["shortUid"] }, new { @style = "text-decoration:underline;" })%></div>
        <div class="style56 nobr float-left nonAuthLinkSep ">
            |</div>
        <div class="style4 nobr float-left nonAuthLinkUsr">
            Already a user?
            <%:Html.ActionLink("log in", "SubscribeToSharedBox", "Collaboration", new { boxId = ViewData["shortUid"] }, new { @style = "text-decoration:underline;" })%>
        </div>
        <br class="clear" />
    </div>
    <%} %>
</div>
<%
    if (Request.IsAuthenticated)
    {
%>
<div id="emailVerificationStatus" class="style28" style="display: none;">
    <div class="VerificationStatus">
        Please check your email and verify your account. Click <a class="style23 underlined"
            href="#">here</a> to re-send verification email.</div>
    <div id="emailVerificationStatusBorder">
        &nbsp;
    </div>
</div>
<img id="usrDropdonRight" src="/Content/Images/user-logout-settings-bg-right.png"
    width="7px" height="109px" />
<%} %>
