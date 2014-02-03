<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="Zbang.Zbox.ViewModel.DTOs" %>
<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
    ZBox
</asp:Content>
<asp:Content ID="CentertPaneContent" ContentPlaceHolderID="CenterPanePlaceHolder"
    runat="server">
    <div id="center-content-container">    
        <% Html.RenderPartial("~/Views/Storage/ConversationFilesAndFilesUploader.ascx"); %>        
    </div>
</asp:Content>
<asp:Content ID="WestPaneContent" ContentPlaceHolderID="WestPanePlaceHolder" runat="server">
    <div>
        <div id="boxes-container">
            <div id="boxes-content">
                <% Html.RenderPartial("~/Views/Storage/SearchBoxItem.ascx"); %>
                <% Html.RenderPartial("~/Views/Storage/CreateBox.ascx"); %>
                <% Html.RenderPartial("~/Views/Storage/Boxes.ascx"); %>               
            </div>
        </div>
        <a class="downloadZbox1" href="https://zboxstorage.blob.core.windows.net/zboxclient/Zbox.Desktop.exe" target="_blank">
            <img src="/Content/Images/Download_Zbox_Client.png" alt="download Zbox client"  style="position:absolute; left: 4px; bottom: 6px; z-index: 999;"/></a>

    </div>     
</asp:Content>
<asp:Content ID="EastPaneContent" ContentPlaceHolderID="EastPanePlaceHolder" runat="server">
    
    <div id="eastPaneAccordion">
        <h3 id="Privacy">
            <a href="#" class="txt-11 uppernoDecoration blue-dark"><img alt="" src="/Content/Images/icons/icon-privacy.png" /> privacy: <span id="privacy-settings-header" class="black48 txt-12 regularDecoration">
                </span></a></h3>
        <div id="box-privacy-settings">
            <% Html.RenderPartial("~/Views/Collaboration/BoxPrivacySettings.ascx"); %>
        </div>
        <h3 class="top-dotted-border">
            <a href="#" class="txt-11 uppernoDecoration blue-dark"><img alt="" src="/Content/Images/icons/icon-notifacation.png" />notifications: <span id="notification-settings-header"
                class="black48 txt-12 regularDecoration"></span></a></h3>
        <div>
            <% Html.RenderPartial("~/Views/Collaboration/BoxNotificationSettings.ascx"); %></div>
        <h3 class="top-dotted-border">
            <a href="#" class="txt-11 uppernoDecoration blue-dark"><img alt="" src="/Content/Images/icons/upload.png" />uploads: <span id="uploadsCounter" class="black48 txt-12 regularDecoration">0</span></a></h3>
        <div>
            <div id="filelist">
            </div>
        </div>
    </div>
    <div>          
        <% Html.RenderPartial("~/Views/Collaboration/AddFriend.ascx"); %>
        <% Html.RenderPartial("~/Views/Collaboration/Friends.ascx"); %>
    </div>
</asp:Content>
<asp:Content ID="NorthPanePlaceHolderLeftSide" ContentPlaceHolderID="NorthPanePlaceHolderLeftSide"
    runat="server">
    <% Html.RenderPartial("LogOnUserControl"); %>
</asp:Content>
<asp:Content ID="SouthPaneContent" ContentPlaceHolderID="SouthPanePlaceHolder" runat="server">
        <% Html.RenderPartial("~/Views/Storage/FilesAndFilesUploader.ascx"); %>  
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolderDialogs" ID="Dialogs" runat="server">
    <% Html.RenderPartial("~/Views/Home/Dialogs.ascx"); %>
    <% Html.RenderPartial("~/Views/Home/Toaster.ascx"); %>
    <% Html.RenderPartial("~/Views/Home/Templates.ascx"); %>
    <% Html.RenderPartial("~/Views/Home/LightBox.ascx"); %>
    <%  if (Request.IsAuthenticated)
        {
            Html.RenderPartial("~/Views/Account/Settings.ascx");
        }
    %>
</asp:Content>
