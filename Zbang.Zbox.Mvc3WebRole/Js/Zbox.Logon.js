function LogOn() {
    registerEvents();
    registerFacebook();
    function registerEvents() {
        $('div.homeForm').on('click','.switchVisiablityButton',function() {
            $('#combinedForms1').toggle();
            $('#combinedForms2').toggle();
        });
        
    }
    function registerFacebook() {
        window.fbAsyncInit = function () {
            FB.init({
                appId: '450314258355338',
                status: true,
                cookie: true,
                xfbml: true,
                oauth: true
            });
        };

        (function (d) {
            var js, id = 'facebook-jssdk'; if (d.getElementById(id)) { return; }
            js = d.createElement('script'); js.id = id; js.async = true;
            js.src = "//connect.facebook.net/en_US/all.js";
            d.getElementsByTagName('head')[0].appendChild(js);
        }(document));
        LogOn.facebookCLick = function () {
            FB.login(function (response) {
                if (response.authResponse) {
                    var request = new ZboxAjaxRequest({
                        url: "/Account/FacebookLogin",
                        data: { token: response.authResponse.accessToken },
                        success: function (data) {
                            if (data) {
                                window.location.replace(data.url);
                                return;
                            }
                            var returnUrl = Zbox.getParameterByName('returnUrl');
                            if (returnUrl === '') {
                                window.location.replace('/');
                            }
                            else {
                                window.location.replace(returnUrl);
                            }
                        },
                        error: function (msg) {
                            $('div.loading').hide();
                            $('#fbError').text(msg.error);
                        }
                    });
                    request.Post();
                }
            }, { scope: 'email' });
        };
    }

    //LogOn.RegisterBeforeSend = function () {
    //    disableSubmit();
    //};
    //LogOn.RegisterSuccess = function (data) {
    //    if (data.Success) {
    //        window.location.replace('/');
    //    }
    //    else {
    //        document.getElementById('combinedForms2').innerHTML = data.Payload.View;
    //    }
    //};
    //function disableSubmit()
    //{
    //    $('input[type="submit"]:visible').attr('disabled', 'disabled');
    //}
    //LogOn.LogInBeforeSend = function () {
    //    disableSubmit();
    //};
    //LogOn.LogInSuccess = function (data) {
    //    if (data.Success) {
    //        var returnUrl = Zbox.getParameterByName('returnurl');
    //        if (returnUrl === '') {
    //            window.location.replace('/');
    //        }
    //        else {
    //            window.location.replace(returnUrl);
    //        }
    //    }
    //    else {
    //        $('input[type="submit"]:visible').removeAttr('disabled');
    //        document.getElementById('combinedForms1').innerHTML = data.Payload.View;
    //    }
    //};
}