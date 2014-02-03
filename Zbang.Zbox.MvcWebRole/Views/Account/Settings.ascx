<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%/*%><script src="~/Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script><%*/%>
<script type="text/javascript">
    $(document).ready(function () {
        $('#settings').click(function (event) {
            $('#divSettings').show();
            $('.fadeMe').show();
            return false;
        });

        $('#btnCancel').click(function (event) {
            CloseSettings()
            return false;
        });

        // #btnSettingsClose  -> Setting popup - X (button)
        $('#btnSettingsClose').click(function (event) {
            CloseSettings();
            return false;
        });

        function CloseSettings() {
            $('#divSettings #spanChangePasswordFeedback').text('');
            $('#divSettings #txtEmailPassResetNewText').val('');
            $('#divSettings #txtEmailPassResetNew').val('');
            $('#divSettings #txtEmailPassReset').val('');
            $('#divSettings').hide();
            $('.fadeMe').hide();
        }

        $('#divSettings #SettingsCheckbox').click(function () {
            Zbox.PasswordShow('#divSettings #SettingsCheckbox', '#divSettings #txtEmailPassResetNewText', '#divSettings #txtEmailPassResetNew', 'inline');
        });
        $('#divSettings #btnOk').click(function () {
            var passwd = '';
            if ($('#divSettings #SettingsCheckbox').attr('checked')) {
                passwd = $('#divSettings #txtEmailPassResetNewText').val();
            }
            else {
                passwd = $('#divSettings #txtEmailPassResetNew').val();
            }
            if (passwd.length < 6) {
                $('#divSettings #spanChangePasswordFeedback').text(' Your password should be at least 6 characters.');
                return false;
            }
            var currentPassword = $('#divSettings #txtEmailPassReset').val();

            if (currentPassword.length < 6) {
                $('#divSettings #spanChangePasswordFeedback').text(' Your current password should be at least 6 characters.');
                return false;
            }

            var ChangePasswordSettingsRequest = new ZboxAjaxRequest({
                data: { oldPassword: currentPassword, newPassword: passwd },
                url: '/Account/ChangePassword',
                success: function (data) {
                    Zbox.toaster('Password changed');
                    $('#divSettings').hide();
                    $('.fadeMe').hide();
                },
                error: function (err) {
                    $('#divSettings #spanChangePasswordFeedback').text(err);
                }
            });

            ChangePasswordSettingsRequest.Post();
        });
    });
</script>
<div id="divSettings" class="baloon123 boxShadow shadow123  " style="display: none;">
   <%-- <img class="settingPopTop" src="/Content/Images/settingsPopup/box-top.png" width="451" height="13" />--%>
    <div id="settingsPop-bg">

    <div class="BallonContent123">
        <img class="float-left settingsIcon" src="../../Content/Images/settingsPopup/settingsIcon.png" alt="Settings" title="Settings" />
        <div class="txt-18 blue-3 float-left">
            <!-- <img src="/Content/Account/Images/forgot_large.png" alt="Forgot" class="ForgotPassTitle" />-->
            <label class="ForgotPassTitle">
                Reset Password</label></div>
                <br class="clear" />
                <hr class="settingPopLine" />
        <div class="BallonContentSmallLetter">
            <label class="txt-13 blue-5">
                Your password should be at least 6 characters.</label>
        </div>
        <div class="txt-13 blue-5 BallonContentBoldLetter">
            Current password:
        </div>
        <div class="resetPassInputText Gradiant RoundCorner4px">
            <input type="password" id="txtEmailPassReset" />
        </div>
        
        <div class="txt-13 blue-5 BallonContentBoldLetter">
            New password:
        </div>

        <div class="resetPassInputText Gradiant RoundCorner4px float-left">
            <input type="text" style="display:none;" id="txtEmailPassResetNewText" />
             <input type="password" id="txtEmailPassResetNew" />
                
        </div>
        <div class="showPass">
            <div class="BallonContentBoldLetter txt-12 back4d">
                <input id="SettingsCheckbox" type="checkbox" />
                Show password
            </div>
        </div>
        <div class="clear">
        </div>
        <br />
        <div id="divBallonButtons">
            <div class="BallonButtons" style="margin:0 0 0 15px;">
                <input class="button-container rounded style27 blue-bg" type="button" id="btnOk"
                    value="Change" />
            </div>
            <div class="BallonButtons" >
                <input class="button-container rounded style29 blue-border" type="button" id="btnCancel"
                    value="Cancel" />
            </div>
            <span id="spanChangePasswordFeedback" class="baloon-feedback"></span>
            <div class="clear">
            </div>
        </div>
    </div>

     <img id="btnSettingsClose" src="/Content/Images/close.png" />
    </div>
<%--    <img src="/Content/Images/settingsPopup/box-bottom.png" />--%>
   
</div>
<div class="fadeMe" style="display: none;"></div>