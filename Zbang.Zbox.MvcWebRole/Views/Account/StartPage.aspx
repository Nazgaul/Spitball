<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE HTML>
<html>
<head>
    <title>Log In</title>
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery-1.6.2.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.watermark.min.js") %>"></script>    
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/Zbox/Zbang.Zbox.js") %>"></script>
    <link href="<%= Url.Content("~/Content/Site.Master.css") %>" rel="stylesheet" type="text/css" />
    <link href="<%= Url.Content("~/Content/Site.InternalDesign.css") %>" rel="stylesheet"
        type="text/css" />
    <link href="<%= Url.Content("~/Content/Account/Account.Master.css") %>" rel="stylesheet"
        type="text/css" />
    <!--[if lt IE 9]>
        <link href="<%= Url.Content("~/Content/Account/Account.Master.IE.css") %>" rel="stylesheet"
<![endif]-->
    <!--[if lte IE 7]>
        <link rel="stylesheet" type="text/css" href="<%= Url.Content("~/Content/Account/Account.Master.IE7.css") %>" />
<![endif]-->
    <script type="text/javascript">
        var currentPic = 1;
        var interval;
        $(document).ready(function () {
            $('#UserName input').watermark('username or email');
            $('#Password input').watermark('password');

            $('#NewUser #NewUserName').watermark('Username', 'watermarkStyle');
            $('#NewUser #NewEmail').watermark('Email', 'watermarkStyle');
            $('#NewUser #NewPassword').watermark('Password', 'watermarkStyle');
            interval = setInterval('swapImages()', 5000);

            $('#ForgotPassword').click(function (event) {

                $('#divNewBox').show();
                //curvyCorners.redraw();
                return false;
            });

            $('#btnOk').click(function (event) {

                var UserNameOremailAddress = $('#txtEmailPassReset').val();
                if (UserNameOremailAddress != '') {
                    $('#spanCreateBoxMessage').text('Resetting password...');
                    var forgotPassword = new ZboxAjaxRequest({
                        url: '/Account/ResetPassword',
                        data: { UserDetail: UserNameOremailAddress },
                        success: function (success) {

                            $('#divNewBox').hide();
                        },
                        error: function (error) {
                            $('#spanCreateBoxMessage').text('Invalid user name or password');
                        }
                    })

                    forgotPassword.Post();
                }
                else
                    $('#spanCreateBoxMessage').text('Enter a valid user name or password');
            });


            $('#btnCancel').click(function (event) {
                $('#txtEmailPassReset').val('');
                $('#divNewBox').hide();
                return false;
            });



        });



        function clickPhoto(number) {
            clearInterval(interval);
            interval = setInterval('swapImages()', 5000);
            $("#roller img").attr("src", "/Content/Account/Images/website_photo_empty.png");
            currentPic = number;
            $("#LookPhoto").attr("src", "/Content/Account/Images/website_image" + number + ".png");
            $("#roller #" + number).attr("src", "/Content/Account/Images/website_photo_selected.png");

        }

        function PasswordShowClick(checkBox) {
            if ($("#CheckBoxPasswordShow").attr('checked')) {
                $('#NewUser #NewPassword:last').val($('#NewUser #NewPassword:first').val());
                $('#NewUser #NewPassword:last').css('display', 'inline');
                $('#NewUser #NewPassword:first').css('display', 'none');
            }
            else {
                $('#NewUser #NewPassword:first').val($('#NewUser #NewPassword:last').val());
                $('#NewUser #NewPassword:first').css('display', 'inline');
                $('#NewUser #NewPassword:last').css('display', 'none');
            }

            
        }

        function swapImages() {
            currentPic = (currentPic + 1) > 4 ? 1 : currentPic + 1;
            clickPhoto(currentPic);
        }

        

    </script>
</head>
<body>
    <% Html.EnableClientValidation(); %>
    <div>
        <div class="account-header">
            <div class="widthStandart">
                <div class="ZboxTitle">
                    <img src="/Content/Images/web_zbox_logo.png" alt="ZBOX" />
                </div>
                <div class="ZboxLogIn">
                    <%using (Html.BeginForm("LogOn", "Account", new { returnUrl = Request.QueryString["returnUrl"] }))
                      { %>
                    <div class="style41 errorLogOn" style="height: 17px;">
                        <%if (ViewData["ErrorLogOn"] != null)
                          { %>
                          <img src="/Content/Images/emailpassword_incorrect.png" class="" alt="" />                      
                        <%} %>
                    </div>
                    <div>
                        <div id="UserName" class="ZboxLogInParams">
                            <div class="LogIninput RoundCorner4px">
                                <%: Html.TextBox("UserName", null, new { @class = "style32", tabindex = 1 })%>
                            </div>
                            <div class="style4 SmallLogInParams">
                                <label>
                                    <%: Html.CheckBox("RememberMe", new { tabindex = 3 })%>
                                    <%: Html.Label("Remember Me")%>
                                </label>
                            </div>
                        </div>
                        <div id="Password" class="ZboxLogInParams">
                            <div class="LogIninput RoundCorner4px">
                                <%: Html.Password("Password", null, new { @class = "style32 RoundCorner4px", tabindex = 2 })%></div>
                            <div class="style4 SmallLogInParams" id="ForgotPassword" tabindex="4">
                                <img src="/Content/Account/Images/forgot.png" alt="forgot" />&nbsp<span>Forgot?</span></div>
                        </div>
                        <div>
                            <div>
                                <input class="SubmitLogIn style33 RoundCorner4px" type="submit" id="UserSignIn" value="Sign In"
                                    tabindex="3" /></div>
                        </div>
                    </div>
                    <%} %>
                    <div id="divNewBox" class="baloon boxShadow" style="display: none;">
                        <div class="baloon-arrow">
                        </div>
                        <div class="BallonContent">
                            <div class="style43">
                                <img src="/Content/Account/Images/forgot_large.png" alt="Forgot" class="ForgotPassTitle" />
                                <label class="ForgotPassTitle">
                                    Forgot your password</label></div>
                            <div class="BallonContentSmallLetter">
                                <label class="style42">
                                    Zbox will send you password reset instructions to the email address associated with
                                    you account</label>
                            </div>
                            <div class="style44" class="BallonContentBoldLetter">
                                Enter your email address or username below
                            </div>
                            <div>
                                <input type="text" id="txtEmailPassReset" class="Gradiant RoundCorner4px" />
                            </div>
                        </div>
                        <div id="divBallonButtons">
                            <div class="BallonButtons">
                                <input class="RoundCorner4px style46" type="button" id="btnCancel" value="Cancel" />
                            </div>
                            <div class="BallonButtons">
                                <input class="RoundCorner4px style45" type="button" id="btnOk" value="Reset password" />
                            </div>
                            <span id="spanCreateBoxMessage" class="baloon-feedback"></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="MainContentBody">
            <div class="NorthMainContentBody">
                <div class="BodyDescription">
                    <div class="style60">
                        The perfect space</div>
                    <div class="style61 txt-18">
                        Upload your music, photos, videos, and documents into a secure box and share your
                        world with family & friends from anywhere around the globe.</div>
                    <div class="ImageDescription">
                        <img id="LookPhoto" src="/Content/Account/Images/website_image1.png" alt="Description" />
                        <div id="roller">
                            <img alt="roller" id="1" src="/Content/Account/Images/website_photo_selected.png"
                                onclick="clickPhoto(1);" />
                            <img alt="roller" id="2" src="/Content/Account/Images/website_photo_empty.png" onclick="clickPhoto(2);" />
                            <img alt="roller" id="3" src="/Content/Account/Images/website_photo_empty.png" onclick="clickPhoto(3);" />
                            <img alt="roller" id="4" src="/Content/Account/Images/website_photo_empty.png" onclick="clickPhoto(4);" />
                        </div>
                    </div>
                </div>
                <div class="NewUser">
                    <div class="style36">
                        New to Zbox? Join today!</div>
                    <div>
                        <%using (Html.BeginForm("Register", "Account", new { returnUrl = Request.QueryString["returnUrl"] }))
                          { %>
                        <div class="style41">
                            <%if (ViewData["ErrorRegister"] != null)
                              { %>
                            <%: ViewData["ErrorRegister"]%>
                            <%} %>
                        </div>
                        <div id="NewUser">
                            <div class="NewUserinputText RoundCorner4px Gradiant">
                                <%: Html.TextBox("NewUserName", null, new { @class = "style37 Gradiant", tabindex = 5 })%></div>
                            <div class="NewUserinputText RoundCorner4px Gradiant">
                                <%: Html.TextBox("NewEmail", null, new { @class = "style37 Gradiant", tabindex = 6 })%></div>
                            <div class="NewUserinputText RoundCorner4px Gradiant">
                                <%: Html.Password("NewPassword", null, new { @class = "style37 Gradiant", tabindex = 7 })%>
                                <%: Html.TextBox("NewPassword", null, new { @class = "style37 Gradiant", tabindex = 7, @style = "display: none;" })%>
                                </div>
                            </div>
                            
                               
                        </div>
                        <div style="margin-top: 7px;">
                            <input style="vertical-align: middle;" type="checkbox" onclick="PasswordShowClick(this)"
                                id="CheckBoxPasswordShow" tabindex="8" />&nbsp
                            <label class="style42" style="vertical-align: middle;" for="CheckBoxPasswordShow">
                                Show password</label></div>
                        <div id="divNewUserSignIn">
                            <input id="NewUserSignIn" class="style55 RoundCorner4px" type="submit" tabindex="9"
                                value="Sign up" /></div>
                        <%} %>
                    </div>
                </div>
            </div>
            <div class="NorthMainContentBody SouthContentBodyPadding">
                <div class="SouthMainContentBodyLeftIem" id="left">
                    <img alt="avilable" src="/Content/Account/Images/website_avilable.png" />
                    <label class="style62">
                        Your files available anywhere</label>
                    <p class="style63 SouthMainContentBodyPara1">
                        Upload you files using Zbox's elegant interface and have easy access to them<br />
                        from work or home.</p>
                </div>
                <div class="SouthMainContentBody" id="center">
                    <img alt="secured" src="/Content/Account/Images/website_secured.png" />
                    <label class="style62">
                        Your files are yours</label>
                    <p class="style63 SouthMainContentBodyPara2">
                        Feel comfortable sharing your files with flexible privacy controls that make sharing
                        your files simple and secure.</p>
                </div>
                <div class="SouthMainContentBody" id="right">
                    <img alt="share" src="/Content/Account/Images/website_share.png" />
                    <label class="style62">
                        Share your boxes</label>
                    <p class="style63 SouthMainContentBodyPara3">
                        Easily share your files with people who are important to you. Keep them updated
                        and manage conversation though comments.</p>
                </div>
            </div>
        </div>
   
</body>
</html>
