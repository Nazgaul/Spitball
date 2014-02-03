<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Zbang.Zbox.Domain.Common" %>
<%@ Import Namespace="Zbang.Zbox.Infrastructure.Enums" %>

<input type="hidden" id="PrivacyId" />
<div id="BoxPrivacySettings">
    <div class="privacy-settings-option-container" id="privacy-settings-<%:BoxPrivacySettings.NotShared %>">
        <input type="radio" name="privacy-settings" value="<%:BoxPrivacySettings.NotShared %>" />
        <span class="style25">Just me</span>
    </div>
    <div class="privacy-settings-option-container" id="privacy-settings-<%:BoxPrivacySettings.Public %>">
        <input type="radio" name="privacy-settings" value="<%:BoxPrivacySettings.Public %>" />
        <span class="style25">Public</span>
    </div>
    <div class="privacy-settings-option-container" id="privacy-settings-<%=BoxPrivacySettings.InvitationOnly %>">
        <input type="radio" name="privacy-settings" value="<%:BoxPrivacySettings.InvitationOnly %>" />
        <span class="style25">Members only</span>
    </div>
    <div class="privacy-settings-option-container" id="privacy-settings-<%:BoxPrivacySettings.PasswordProtected %>">
        <input type="radio" name="privacy-settings" value="<%:BoxPrivacySettings.PasswordProtected %>" />
        <span class="style25">Password protected</span>
        <div>
            <div>
                <span class="style68">password:</span>
                <input type="password" id="share-password" />
                <input type="text" style="display:none" id="share-password1" />
            </div>
            <div class="privacypass">               
                <input style="border: 0;" type="checkbox" id="share-password-confirm" />
                <label class="style66">
                    Show password</label>
            </div>
        </div>
    </div>
    <div id="privacy-buttons-container" style="visibility: hidden;">
        <div class="button-container rounded style29 blue-border">
            <div class="my-button">
                <a id="btnPrivacyCancel"  href="#">Cancel</a></div>
        </div>
        <div class="button-container rounded style27 blue-bg">
            <div class="my-button">
                <a id="btnPrivacyOk" href="#">Save</a></div>
        </div>
    </div>
    <div id="privacy-change-feedback">
        <label>
        </label>
    </div>
</div>
