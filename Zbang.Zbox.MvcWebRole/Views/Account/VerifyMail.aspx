<%@ Page MasterPageFile="~/Views/Shared/Site.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<asp:Content ContentPlaceHolderID="HeaderContentPlaceHolder" ID="Header" runat="server">
    <style type="text/css">
    .ui-layout-east{display:none !important}
    .ui-layout-west{display:none !important}
    .ui-layout-south{width:100% !important;}
    .ui-layout-center{width:100% !important;}
    </style>
</asp:Content>

<asp:Content ID="NorthPanePlaceHolderLeftSide" ContentPlaceHolderID="NorthPanePlaceHolderLeftSide"
    runat="server">
    <% Html.RenderPartial("LogOnUserControl"); %>
</asp:Content>

<asp:Content ID="CentertPaneContent" ContentPlaceHolderID="CenterPanePlaceHolder"
    runat="server">    
    <script type="text/javascript">
        $(document).ready(function () {           
            $('.ui-layout-center').addClass('verifyMail');
        });
    </script>
    <div class="not-Shared-wrap">
            <div class="thanks-text">
                <h1 class="style65">
                    Thanks!
                </h1>
                <div class="style64">
                Your email has been <%= this.ViewData["VerificationResult"] %> Please sign-in. 
                </div>
                <a class="btnSignIn" href="/">Sign-In</a>
            </div>
        </div>
       
        
</asp:Content>

