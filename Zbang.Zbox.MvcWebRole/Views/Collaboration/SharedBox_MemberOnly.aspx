<%@ Page MasterPageFile="~/Views/Shared/Site.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

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
        #conversationsHeader
        {
            display: none !important;
        }
    </style>
</asp:Content>
<asp:Content ID="CentertPaneContent" ContentPlaceHolderID="CenterPanePlaceHolder"
    runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('div#dialog-PasswordProtected').dialog({
                title: '<div class="style57"><img src="/Content/Images/locker.png" /> This box is for members only</div>',
                width: 'auto',
                height: 'auto',
                modal: true,
                resizable: false,
                open: function (event, ui) {
                    $('div#dialog-share-Box #privacySelect').selectbox();
                    $('div#dialog-share-Box #privacySelect').bind('change', function (event) {
                        IsPasswordShow();
                    });
                    $('#recipients-display').css('max-height', Math.round($(this).height() * 0.3));
                    $('#recipients-display').css('max-width', $('#recipients-display').width());
                },
                close: function (event, ui) {
                    $('div#dialog-share-Box #privacySelect').unbind('change');
                },
                buttons: {
                    Okay: function () {
                        $('#dialog-PasswordProtected').find('form').submit();
                    }

                    //Cancel: function () {
                        //window.location.href = "/";
                    //}
                }
            });

        });

    </script>
    <div id="center-content-container">
        <% Html.RenderPartial("~/Views/Storage/ConversationFilesAndFilesUploader.ascx"); %>
        <div id="dialog-PasswordProtected" class="container">
            <h1 class="style74">
                This box is for members only</h1>
            <% using (Html.BeginForm("SubscribeToSharedBox", "Collaboration", new { boxId = this.ViewData["shortUid"] }))
               { %>
            <div>
                <input type="hidden" name="boxId" value="<%:this.ViewData["shortUid"] %>" />               
                <div class="clear">
                </div>
                <span class="style66">to view the content of this box you must verify your invitation
                    by logging into zbox using the email you were invited with </span>
                <div class="style28">
                    <% if (this.ViewData.Keys.Contains("error")) Response.Write(ViewData["error"]); %>
                </div>
            </div>
            <% } %>
        </div>
    </div>
</asp:Content>
<asp:Content ID="SouthPaneContent" ContentPlaceHolderID="SouthPanePlaceHolder" runat="server">
    <% Html.RenderPartial("~/Views/Storage/FilesAndFilesUploader.ascx"); %>
</asp:Content>
