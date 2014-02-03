<%@ Page MasterPageFile="~/Views/Shared/Site.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="Zbang.Zbox.Infrastructure.Blob" %>
<asp:Content ContentPlaceHolderID="HeaderContentPlaceHolder" ID="Header" runat="server">
    <style type="text/css">
        .ui-layout-east
        {
            display: none !important;
        }
        .ui-layout-west
        {
            display: none !important;
        }
        .ui-layout-south
        {
            width: 100% !important;
        }
        .ui-layout-center
        {
            width: 100% !important;
        }
    </style>
</asp:Content>
<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
    Zbox |
    <%: ViewData["BoxName"] %>
    
</asp:Content>
<asp:Content ID="NorthPanePlaceHolderLeftSide" ContentPlaceHolderID="NorthPanePlaceHolderLeftSide"
    runat="server">
    <% Html.RenderPartial("~/Views/Shared/LogOnUserControl.ascx"); %>
</asp:Content>
<asp:Content ID="CentertPaneContent" ContentPlaceHolderID="CenterPanePlaceHolder"
    runat="server">
    <script type="text/javascript">
        $(function () {
            $('#selectedBoxName').text('<%: ViewData["BoxName"] %>');
            $('#ownerLabel').text('<%: ViewData["BoxOwner"] %>');                        
            var boxitem = {
            BoxId : <%:ViewData["boxid"] %>,
            BoxName: '<%: ViewData["BoxOwner"] %>',
            BoxOwner : '<%: ViewData["BoxOwner"] %>',
            shortUid : '<%:  ViewData["shortUid"] %>'
            };
            var boxes = [];
            boxes.push(boxitem);
            var boxesEmpty = [];
            Zbox.Box.boxes = new Boxes(boxes,boxesEmpty,boxesEmpty);
            

            $('#askForInvitation').click(function (event) {
                var boxId = <%:ViewData["boxid"] %>;
                var request = new ZboxAjaxRequest({
                    url: '/Collaboration/RequestSubscription',
                    data: { boxId: boxId },
                    success: function (status) {

                    },
                    error: function (error) {

                    }
                });

                request.Post();
            });
            


            <%  if (Request.IsAuthenticated && this.ViewData.Keys.Contains("Error"))
                {  %>
            Zbox.ShowConfirmDialog({
                title: 'Invitation',
                message: 'You need to ask for Invitation.',
                ok: function () {
                    var boxId = <%:ViewData["boxid"] %>

                    var request = new ZboxAjaxRequest({
                        url: '/Collaboration/RequestSubscription',
                        data: { boxId: boxId },
                        success: function (status) {
                            window.location.href = "/";
                        },
                        error: function (error) {

                        },
                        complete: function (text) {

                        }
                    });
                    //return true;

                    request.Post();
                },
                cancel: function () {
                    window.location.href = "/";
                }
            });
          <%} else {%>
            Zbox.BoxItem.loadBoxFiles('<%:ViewData["shortUid"] %>', 0, 1);
            loadBoxConversations('<%:ViewData["shortUid"] %>');           
          <%} %>
        }); 
    </script>
    <div id="center-content-container">
        <% Html.RenderPartial("~/Views/Storage/ConversationFilesAndFilesUploader.ascx"); %>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolderDialogs" ID="Dialogs" runat="server">
    <div id="dialog-confirm">
        <div id="dialog-content">
        </div>
    </div>
    <% Html.RenderPartial("~/Views/Home/Toaster.ascx"); %>
    <% Html.RenderPartial("~/Views/Home/LightBox.ascx"); %>
    <% Html.RenderPartial("~/Views/Home/Templates.ascx"); %>
</asp:Content>
<asp:Content ID="SouthPaneContent" ContentPlaceHolderID="SouthPanePlaceHolder" runat="server">
    <% Html.RenderPartial("~/Views/Storage/FilesAndFilesUploader.ascx"); %>
    <input type="hidden" id="selecteBoxId" value="<%:  ViewData["boxid"] ?? 0 %>" />
    <input type="hidden" id="selectedBoxSUID" value="<%:  ViewData["shortUid"] ?? 0 %>" />
</asp:Content>
