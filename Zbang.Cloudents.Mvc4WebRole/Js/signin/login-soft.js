var Login = function () {
    var queryString = getUrlVars();
    var forgotPasswordState = 0,
        loginState = 1,
        signupState = 2;

    var handleLogin = function () {
        $('.forms .login').validate({
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

        $('.forms .login input').keypress(function (e) {
            if (e.which === 13) {
                if ($('.forms .login').validate().form()) {
                    $('.forms .login').submit();
                }
                return false;
            }
        });
    }


    var handleForgetPassword = function () {
        var serializedEmail;
        var $forgotPassword = $('.forgot-password');

        $forgotPassword.validate({
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
                $('.check-email-message-wrapper').removeClass('hidden').show();
                $('.forgot-password-wrapper').hide();
            });
            return true;
        }
    }

    var handleRegister = function () {

        var $form = $('.register');
        $form.validate({
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
                if (element.attr("name") === "tnc") { // insert checkbox errors after the container                  
                    error.insertAfter($('#register_tnc_error'));
                    error.insertAfter(element);
                    return;
                }
                if (element.attr("name") === "sex") {
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

        $('.register input').keypress(function (e) {
            if (e.which === 13) {
                if ($form.validate().form()) {
                    $form.submit();
                }
                return false;
            }
        });


        $('#register-btn, .login-option.signup').click(function (e) {
            e.preventDefault();
            pushState(signupState);

        });

        $('#register-btn').click(function () {
            document.body.scrollTop = 0;
            //$('.login-wrapper, .forgot-password-form').hide();
            $('.form-wrapper:not(.register-wrapper)').hide();
            $('.register-wrapper').removeClass('hidden').fadeIn();
        });

        //for forgot password
        $('.signin-btn, #login-btn, .forgot-password-wrapper .cancel').click(function (e) {
            e.preventDefault();
            document.body.scrollTop = 0;
            $('.register-wrapper, .forgot-password-wrapper').hide();
            $('.login-wrapper').removeClass('hidden').fadeIn();
            pushState(loginState);
        });


        $('#forgot-password-link').click(function () {

            document.body.scrollTop = 0;
            $('.login-wrapper, .register-wrapper').hide();
            $('.forgot-password-wrapper').removeClass('hidden').fadeIn();
            pushState(forgotPasswordState);

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
            app_id: "njmpgayv"
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
        //var returnUrl = queryString['returnUrl'];
        //var universityId = queryString['universityid'];
        //if (returnUrl) {
        //values += "&returnUrl=" + returnUrl;
        //}
        //if (universityId) {
        //    values += "&universityId=" + universityId;
        //}


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
            if (queryString['returnUrl']) {
                window.location.href = decodeURIComponent(queryString['returnUrl']);
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

            var returnUrl = queryString['returnUrl'];
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
                token: idToken
                //universityId: queryString['universityid'],
                //returnUrl: queryString['returnUrl']
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
                token: accessToken
                //universityId: queryString['universityid'],
                //returnUrl: queryString['returnUrl']
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
            alert(data.payload.error);
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
        var returnUrl = queryString['returnUrl'];
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
        //var universityId = queryString['universityid'];
        //var ref = queryString['ref'];
        //var ext = '';
        //if (universityId && ref) {
        //    ext = '?universityid=' + universityId;
        //}
        if (state === loginState) {
            updateQueryStringParam('action', 'signin');
            ga('send', 'pageview', '/account/signin');
            $(window).scrollTop(0);
            return;
        }
        if (state === signupState) {
            updateQueryStringParam('action', 'signup');
            ga('send', 'pageview', '/account/signup');
            $(window).scrollTop(0);
            return;
        }
        updateQueryStringParam('action', 'resetpassword');
        ga('send', 'pageview', '/account/resetpassword');
        $(window).scrollTop(0);
    }

    function updateQueryStringParam(key, value) {
        var baseUrl = [location.protocol, '//', location.host, location.pathname].join(''),
            urlQueryString = document.location.search,
            newParam = key + '=' + value,
            params = '?' + newParam;

        // If the "search" string exists, then build params from it
        if (urlQueryString) {
            keyRegex = new RegExp('([\?&])' + key + '[^&]*');

            // If param exists already, update it
            if (urlQueryString.match(keyRegex) !== null) {
                params = urlQueryString.replace(keyRegex, "$1" + newParam);
            } else { // Otherwise, add it to end of query string
                params = urlQueryString + '&' + newParam;
            }
        }
        window.history.replaceState({}, "", baseUrl + params);
    };

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
            if (window.location.search.indexOf('action=signin') > -1) {
                showForm($loginWrapper);
            }

            if (window.location.search.indexOf('action=signup') > -1) {
                showForm($registerWrapper);
            }

            if (window.location.search.indexOf('action=resetpassword') > -1) {
                showForm($forgotPWWrapper);
            }

            $.each($.validator.messages, function (key, value) {
                if ($.type(value) === 'string') { $.validator.messages[key] = value.replace('.', ''); }
            });



            $('.content').show();
        },
        initExternalLogin: function () {
            handleExtenalLogin();
        }
    };

}();


var animatonSpeed = 500;

$('.login-wrapper input').focus(function () {
    $('.alert-danger').slideUp(animatonSpeed);
});


var $main = $('.homePage');
var $loginWrapper = $('.login-wrapper');
var $registerWrapper = $('.register-wrapper');
var $forgotPWWrapper = $('.forgot-password-wrapper');
var $formWrapper = $('.form-wrapper');
var $checkEmailWrapper = $('.check-email-message-wrapper');
var $navBar = $('.static-page-header .toggle');

function showForm($form) {
    $formWrapper.hide();
    $form.show();
    $main.addClass('show-forms');
}


$('.signup-btn').click(function () {
    showForm($registerWrapper);
});

$('.signin-btn').click(function () {
    showForm($loginWrapper);
});

$('.close-form').click(function () {
    $main.removeClass('show-forms');
    setTimeout(function () {
        $formWrapper.hide();
    }, animatonSpeed);
    window.history.replaceState(null, "Home", "/");
});

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