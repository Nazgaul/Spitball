<%@ Page MasterPageFile="~/Views/Shared/Site.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<asp:Content ContentPlaceHolderID="HeaderContentPlaceHolder" ID="Header" runat="server">
    <style type="text/css">
    .ui-layout-east{display:none !important}
    .ui-layout-west{display:none !important}
    .ui-layout-south{width:100% !important;}
    .ui-layout-center{width:100% !important;}
    #conversationsHeader{display:none !important;}
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
           $('.ui-layout-east').hide();
           $('.ui-layout-west').hide();
           $('.ui-layout-center').addClass('verifyMail');
        });
    </script>
    <div class="not-Shared-wrap">
            <div class="not-Shared-text">
                <h1 class="style65">
                    Whoops looks like we lost one!
                </h1>
                <div class="style64">
                You are here because the box you requested doesn't exist, or is private. 
                </div>
                <div class="style64 line2">
                But don't worry - it can happen to the best of us. Try going to Zbox Home instead.
                </div>
                <div class="clear"></div>
                <a class="btnZboxHome" href="/">zbox home</a>
            </div>
        </div>
       
        
</asp:Content>
