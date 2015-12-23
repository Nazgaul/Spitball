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
            invalidHandler: function (event, validator) { //display error alert on form submit   	                
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
                error.insertAfter(element.closest('.input-icon'));
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
        $('#f-forgot-password').submit(function (ev) {
            ev.preventDefault();
            var btn = $('#f-forgot-password').find(':submit').attr('disabled', 'disabled');
            serializedEmail = $(this).serialize();
            sendRequest();

        });
        $('#resendRequest').click(function () {
            sendRequest();
        });

        function sendRequest() {
            $.post('/account/resetpassword/', serializedEmail).done(function (data) {
                $('#f-forgot-password').find(':submit').removeAttr('disabled');
                if (!data.success) {
                    var text = data.payload;
                    alert(text);
                    return;
                }
                $('.check-email-message').show();
                $('.forgot-password-form').hide();
            });
        }
    }

    var handleRegister = function () {

        $('.register-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "",
            rules: {

                firstname: {
                    required: true
                },
                lastname: {
                    required: true
                },
                email: {
                    required: true,
                    email: true
                },
                confirmemail: {
                    required: true,
                    email: true,
                    equalTo: "#register_email"
                },
                password: {
                    required: true,
                    minlength: 6
                }
            },
            invalidHandler: function (event, validator) { //display error alert on form submit   

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
                if (element.attr("name") == "tnc") { // insert checkbox errors after the container                  
                    error.insertAfter($('#register_tnc_error'));
                } else if (element.closest('.input-icon').size() === 1) {
                    error.insertAfter(element.closest('.input-icon'));
                } else {
                    error.insertAfter(element);
                }
            },

            submitHandler: function (form) {
                signup(form);
            }
        });

        $('.register-form input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.register-form').validate().form()) {
                    $('.register-form').submit();
                }
                return false;
            }
        });

        jQuery('#register-btn').click(function () {
            jQuery('.login-form, .forgot-password-form').hide();
            jQuery('.register-form').show();
            pushState(signupState);
            ga('send', 'pageview', '/account/signup');
            trackConversion();
        });

        jQuery('#login-btn, .forgot-password-form .cancel').click(function () {
            jQuery('.login-form').show();
            jQuery('.register-form, .forgot-password-form').hide();
            pushState(loginState);
            ga('send', 'pageview', '/account/signin');
            trackConversion();
        });


        jQuery('#forget-password').click(function () {
            jQuery('.forgot-password-form').show();
            jQuery('.login-form, .register-form').hide();
            pushState(forgotPasswordState);
            ga('send', 'pageview', '/account/resetpassword');
            trackConversion();
        });

        //jQuery('#lang_select').change(function (e) {
        //    saveRegisterData();
        //    window.location.href = jQuery(this).find(':selected').attr('data-href');
        //});
    }

    var handleExtenalLogin = function () {
        jQuery('.facebook').click(function () {
            facebookLogin();
        });

        jQuery('.google').click(function () {
            googleLogIn();
        });
    }



    function signup(form) {
        var submitBtn = $(form).find('.btn-primary');
        submitBtn.addClass('disabled');
        var values = $(form).serialize();
        var returnUrl = getUrlVars()['returnUrl'];
        var universityId = getUrlVars()['universityid'];
        if (returnUrl) {
            values += "&returnUrl=" + encodeURIComponent(returnUrl);
        }
        if (universityId) {
            values += "&universityId=" + encodeURIComponent(universityId);
        }


        $.post('/account/register', values).done(function (data) {
            if (!data.success) {

                var text;
                try {
                    text = data.payload[0].value[0];
                } catch (e) {
                    text = 'Unspecified error';
                }
                $('#register_alert_text span').text(text);
                $('#register_alert_text').show();
                submitBtn.removeClass('disabled');
                return;
            }

            ga('send', 'event', 'Signup', 'Email');

            clearStorage();
            //setNewUser();
            if (returnUrl) {
                window.location.href = returnUrl;
                return;
            }

            window.location.href = data.payload;
        });
    }

    function signin(form) {
        var submitBtn = $(form).find('.btn-primary');
        submitBtn.addClass('disabled');
        var values = $(form).serialize();
        var returnUrl = getUrlVars()['returnUrl'];
        if (returnUrl) {
            values += "&returnUrl=" + encodeURIComponent(getUrlVars()['returnUrl']);
        }


        $.post('/account/login', values).done(function (data) {
            if (!data.success) {
                var text;
                try {
                    text = data.payload[0].value[0];
                } catch (e) {
                    text = 'Unspecified error';
                }
                $('#login_alert_text span').text(text);
                $('#login_alert_text').show();
                submitBtn.removeClass('disabled');
                return;
            }

            clearStorage();

            ga('send', 'event', 'Signin', 'Email');


            if (returnUrl) {
                window.location.href = returnUrl;
                return;
            }
            window.location.href = data.payload;

        });
    }

    function googleLogIn() {
        var authInstance = gapi.auth2.getAuthInstance();
        authInstance.signIn().then(function (googleUser) {
            var id_token = googleUser.getAuthResponse().id_token;
            trackConversion();
            $.post('/account/GoogleLogin', {
                token: id_token,
                universityId: getUrlVars()['universityid']
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
            trackConversion();
            $.post('/account/facebookLogin', {
                token: accessToken,
                universityId: getUrlVars()['universityid']
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
            alert('there is a problem signing you in with facebook');
            return;
        }

        clearStorage();
        var obj = data.payload;
        if (obj.isnew) {
            //setNewUser();
            //FB.api('/me', function () {	                    
            if (obj.url) {
                window.location.href = obj.url;
                return;
            }
            //});

            ga('send', 'event', 'Signup', type);
            return;
        }


        ga('send', 'event', 'Signin', type);

        var returnUrl = getUrlVars()['returnUrl'];
        if (returnUrl) {
            window.location.href = returnUrl;
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
            handleExtenalLogin();
            if (window.location.pathname.indexOf('/signup') > -1) {
                jQuery('.login-form, .forgot-password-form').hide();
                jQuery('.register-form').show();
                //loadRegisterData();
            }

            if (window.location.pathname.indexOf('/resetpassword') > -1) {
                jQuery('.login-form, .register-form').hide();
                jQuery('.forgot-password-form').show();
                //loadRegisterData();
            }

            $.each(jQuery.validator.messages, function (key, value) {
                if ($.type(value) == 'string') { jQuery.validator.messages[key] = value.replace('.', ''); }
            });



            $('.content').show();
        },
        initExternalLogin: function () {
            handleExtenalLogin();

        }
    };

}();

function trackConversion() {
    window.google_trackConversion({
        conversion_id: 939226062,
        conversion_language: "en",
        conversion_format: "3",
        conversion_color: "ffffff",
        conversion_label: "KiNvCODoqWAQzuftvwM",
        remarketing_only: false
    });
}

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