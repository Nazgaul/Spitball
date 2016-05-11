var Login = function () {
    var forgotPasswordState = 0,
        loginState = 1,
        signupState = 2;

    var handleLogin = function () {
        $('.login-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            rules: {
                email: {
                    required: true,
                    email: true
                },
                password: {
                    required: true,
                    minlength: 6
                },
                remember: {
                    required: false
                }
            },
            //invalidHandler: function (event, validator) { //display error alert on form submit   	                
            //},

            highlight: function (element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            success: function (label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function (error, element) {
                error.insertAfter(element);
            },

            submitHandler: function (form) {
                signin(form);
            }
        });

        $('.login-form input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.login-form').validate().form()) {
                    $('.login-form').submit();
                }
                return false;
            }
        });
    }


    var handleForgetPassword = function () {
        var serializedEmail;
        var $forgotPassword = $('#f-forgot-password');

        $('.forgot-password-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            rules: {
                email: {
                    required: true,
                    email: true
                }
            },

            highlight: function (element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            success: function (label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function (error, element) {
                error.insertAfter(element);
            }
        });


        $forgotPassword.submit(function (ev) {
            ev.preventDefault();
            $forgotPassword.find(':submit').attr('disabled', 'disabled');
            serializedEmail = $(this).serialize();
            sendRequest();

        });
        $('#resendRequest').click(function () {
            var requestSent = sendRequest();
            if (requestSent) {
                alert($(this).data("response"));
            }
        });

        function sendRequest() {
            $.post('/account/resetpassword/', serializedEmail).done(function (data) {
                $forgotPassword.find(':submit').removeAttr('disabled');
                if (!data.success) {
                    var text = data.payload;
                    alert(text);
                    return;
                }
                $('.check-email-message').removeClass('hidden').show();
                $('.forgot-password-form').hide();
            });
            return true;
        }
    }

    var handleRegister = function () {

        $('.register-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "",
            rules: {
                firstName: {
                    required: true
                },
                lastName: {
                    required: true
                },
                NewEmail: {
                    required: true,
                    email: true
                },
                sex: {
                    required: true
                },
                Password: {
                    required: true,
                    minlength: 6
                }
            },
            //invalidHandler: function (event, validator) { //display error alert on form submit   

            //},

            highlight: function (element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            success: function (label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function (error, element) {
                if (element.attr("name") == "tnc") { // insert checkbox errors after the container                  
                    error.insertAfter($('#register_tnc_error'));
                    error.insertAfter(element);
                    return;
                }
                if (element.attr("name") == "sex") {
                    console.log('here');
                    error.insertAfter($('label[for="female"]'));
                    return;
                }
                error.insertAfter(element);
            },

            submitHandler: function (form) {
                signup(form);
            }
        });

        $('.register-form input').keypress(function (e) {
            var $form = $('.register-form');
            if (e.which == 13) {
                if ($form.validate().form()) {
                    $form.submit();
                }
                return false;
            }
        });


        $('#register-btn, .login-option.signup').click(function (e) {
            e.preventDefault();
            pushState(signupState);
            ga('send', 'pageview', '/account/signup');
        });

        $('#register-btn').click(function (e) {
            document.body.scrollTop = 0;
            $('.login-form, .forgot-password-form').hide();
            $('.register-form').removeClass('hidden').fadeIn();
            //trackConversion();
        });

        //for forgot password
        $('.signin-btn, #login-btn, .forgot-password-form .cancel').click(function (e) {
            e.preventDefault();
            document.body.scrollTop = 0;
            $('.register-form, .forgot-password-form').hide();
            $('.login-form').removeClass('hidden').fadeIn();
            pushState(loginState);
            ga('send', 'pageview', '/account/signin');

        });


        $('#forget-password').click(function () {

            document.body.scrollTop = 0;
            $('.login-form, .register-form').hide();
            $('.forgot-password-form').removeClass('hidden').fadeIn();
            pushState(forgotPasswordState);
            ga('send', 'pageview', '/account/resetpassword');
            //trackConversion();
        });

    }

    var handleExtenalLogin = function () {
        $('.facebook').click(function () {
            facebookLogin();
        });

        $('.google').click(function () {
            googleLogIn();
        });
        window.Intercom('boot', {
            app_id: "njmpgayv",
        });
    }

    function disableState(elem) {
        $(elem).attr('disabled', 'disabed').addClass('disabled');
    }


    function signup(form) {
        var submitBtn = $(form).find('.btn-primary');
        disableState(submitBtn);
        var t = setTimeout(function () {
            $('.home-page-body .loader').addClass('active');
        }, 200);
        var values = $(form).serialize();
        var returnUrl = getUrlVars()['returnUrl'];
        var universityId = getUrlVars()['universityid'];
        if (returnUrl) {
            values += "&returnUrl=" + returnUrl;
        }
        if (universityId) {
            values += "&universityId=" + universityId;
        }


        $.post('/account/register', values).done(function (data) {
            if (!data.success) {

                var text;
                try {
                    text = data.payload;
                } catch (e) {
                    text = 'Unspecified error';
                }
                $('#register_alert_text span').text(text);
                $('#register_alert_text').show();
                submitBtn.removeClass('disabled').removeAttr('disabled');
                return;
            }

            ga('send', 'event', 'Signup', 'Email');

            clearStorage();
            //setNewUser();
            if (returnUrl) {
                window.location.href = decodeURIComponent(returnUrl);
                return;
            }

            window.location.href = data.payload;
        }).always(function () {
            clearTimeout(t);
            $('.home-page-body .loader').removeClass('active');
        });
    }

    function signin(form) {
        var submitBtn = $(form).find('.btn-primary');
        disableState(submitBtn);
        var t = setTimeout(function () {
            $('.home-page-body .loader').addClass('active');
        }, 200);
        var values = $(form).serialize();
        var returnUrl = getUrlVars()['returnUrl'];


        $.post('/account/login', values).done(function (data) {
            if (!data.success) {
                var text;
                try {
                    text = data.payload;
                } catch (e) {
                    text = 'Unspecified error';
                }
                $('#login_alert_text span').text(text);
                $('#login_alert_text').show();
                submitBtn.removeClass('disabled').removeAttr('disabled');
                return;
            }

            clearStorage();

            ga('send', 'event', 'Signin', 'Email');


            if (returnUrl) {
                window.location.href = decodeURIComponent(returnUrl);
                return;
            }
            window.location.href = data.payload;

        }).always(function () {
            clearTimeout(t);
            $('.home-page-body .loader').removeClass('active');
        });
    }

    function googleLogIn() {
        var authInstance = gapi.auth2.getAuthInstance();

        authInstance.signIn().then(function (googleUser) {
            var idToken = googleUser.getAuthResponse().id_token;
            $.post('/account/GoogleLogin', {
                token: idToken,
                universityId: getUrlVars()['universityid'],
                returnUrl: getUrlVars()['returnUrl']
            }).done(function (data) {
                externalLogIn(data, 'Google');
            });
        });
    }

    function facebookLogin() {
        FB.login(function (response) {
            if (response.authResponse) {
                var accessToken = response.authResponse.accessToken;
                processLogin(accessToken);
            }

        }, { scope: 'email,user_friends' });

        function processLogin(accessToken) {
            //trackConversion();
            $.post('/account/facebookLogin', {
                token: accessToken,
                universityId: getUrlVars()['universityid'],
                returnUrl: getUrlVars()['returnUrl']
            }).done(function (data) {
                externalLogIn(data, 'Facebook');
            });
        }
    }
    // this is a hack for using angular cache element in the next page
    //function setNewUser() {
    //    var date = new Date().getTime();
    //    sessionStorage.setItem('angular-cache.caches.points.keys', '["register"]');
    //    sessionStorage.setItem('angular-cache.caches.points.data.register', '{"key":"register","value":true,"created":' + date + ',"accessed":' + date + ',"expires":' + (date + 600000) + '}');

    //}

    function externalLogIn(data, type) { //Type google or facebook
        if (!data.success) {
            alert('there is a problem signing you in');
            return;
        }

        clearStorage();
        var obj = data.payload;
        if (obj.isnew) {
            if (obj.url) {
                window.location.href = obj.url;
                return;
            }
            ga('send', 'event', 'Signup', type);
            return;
        }
        ga('send', 'event', 'Signin', type);
        var returnUrl = getUrlVars()['returnUrl'];
        if (returnUrl) {
            window.location.href = decodeURIComponent(returnUrl);
            return;
        }
        window.location.href = '/dashboard/';
    }

    function clearStorage() {
        if (window.sessionStorage) {
            window.sessionStorage.clear();
        }
    }
    function pushState(state) {
        if (!window.history) {
            return;
        }
        var universityId = getUrlVars()['universityid'];
        var ext = '';
        if (universityId) {
            ext = '?universityid=' + universityId;
        }
        if (state === loginState) {

            window.history.replaceState(null, "Signin", "/account/signin/" + ext);
            return;
        }
        if (state === signupState) {
            window.history.replaceState(null, "Sign up", "/account/signup/" + ext);
            return;
        }
        window.history.replaceState(null, "Forgot password", "/account/resetpassword/");
        $(window).scrollTop(0);
    }

    function getUrlVars() {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    }


    return {
        //main function to initiate the module
        init: function () {

            handleLogin();
            handleForgetPassword();
            handleRegister();
            //handleLanguage();
            //handleExtenalLogin();
            if (window.location.pathname.indexOf('/signin') > -1) {
                handleLoginElements('login-form');
                $('.signin-btn').addClass('hidden', true);

            }

            if (window.location.pathname.indexOf('/signup') > -1) {
                handleLoginElements('register-form');
                $('.signin-btn').addClass('hidden', true);
            }

            if (window.location.pathname.indexOf('/resetpassword') > -1) {
                handleLoginElements('forgot-password-form', true);
                $('.signin-btn').addClass('hidden');
            }

            $.each($.validator.messages, function (key, value) {
                if ($.type(value) == 'string') { $.validator.messages[key] = value.replace('.', ''); }
            });



            $('.content').show();
        },
        initExternalLogin: function () {
            handleExtenalLogin();
          //  svg4everybody();

        }
    };

}();


var animatonSpeed = 300;

$('.login-wrapper input').focus(function () {
    $('.alert-danger').slideUp(animatonSpeed);
});

$('.login-option.signup').click(function (e) {
    e.preventDefault();
    handleLoginElements('register-form', false);
});
$('.signin-btn').click(function (e) {
    e.preventDefault();
    handleLoginElements('login-form', false);
});

$('.close-form').click(function () {
    $.each($('.home-page-body > *:not(.main)'),
        function () {
            $(this).slideDown(animatonSpeed);
        });
    $('.login-wrapper, #main-wrapper .welcome-text').slideDown(animatonSpeed).removeClass('hidden');
    $('.login-wrapper').slideUp(animatonSpeed);
    $('.toggle').removeClass('hidden');
    $('#main-wrapper').css('min-height', 0).height('auto');
    window.history.replaceState(null, "Home", "/");

});

function handleLoginElements(formClass, fromUrl) {
    var $mainSection = $('.home-page-body > *:not(.main)');
    if (fromUrl) {
        $.each($mainSection, function () {
            $(this).hide();
        });
        $('.toggle, #main-wrapper .welcome-text, .login-wrapper .content > :not(.' + formClass + ')').toggleClass('hidden');
        $('.login-wrapper, .login-wrapper .content > .' + formClass).fadeIn();
    } else {
        document.body.scrollTop = document.documentElement.scrollTop = 0;
        $.each($mainSection, function () {
            $(this).slideToggle(animatonSpeed);
        });
        $('.login-wrapper, #main-wrapper .welcome-text').slideToggle(animatonSpeed);
        $('.login-wrapper .content > form:not(.' + formClass + ')').each(function () {
            $(this).addClass('hidden');
        });
        $('.login-wrapper .content > form.' + formClass).removeClass('hidden').show();
        $('.toggle').toggleClass('hidden');
    }
    $('#main-wrapper').css('min-height', 'calc(100vh - 150px)');

}

//function trackConversion() {
//    window.google_trackConversion({
//        conversion_id: 939226062,
//        conversion_language: "en",
//        conversion_format: "3",
//        conversion_color: "ffffff",
//        conversion_label: "KiNvCODoqWAQzuftvwM",
//        remarketing_only: false
//    });
//}

function googleLoad() {
    gapi.load('auth2', function () {
        gapi.auth2.init();
    });
}

window.fbAsyncInit = function () {
    FB.init({
        appId: '450314258355338',
        status: true,
        cookie: true,
        xfbml: true,
        version: 'v2.4'
    });
};
(function (d) {
    var js, id = 'facebook-jssdk';
    if (d.getElementById(id)) {
        return;
    }
    js = d.createElement('script');
    js.id = id;
    js.async = true;
    js.src = "//connect.facebook.net/en_US/sdk.js";
    d.getElementsByTagName('head')[0].appendChild(js);
}(document));


Login.initExternalLogin();