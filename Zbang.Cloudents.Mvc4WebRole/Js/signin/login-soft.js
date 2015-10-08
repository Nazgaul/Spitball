var Login = function () {

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

    //var handleForgetPassword = function () {
    //	$('.forget-form').validate({
    //            errorElement: 'span', //default input error message container
    //            errorClass: 'help-block', // default input error message class
    //            focusInvalid: false, // do not focus the last invalid input
    //            ignore: "",
    //            rules: {
    //                email: {
    //                    required: true,
    //                    email: true
    //                }
    //            },

    //            messages: {
    //                email: {
    //                    required: "Email is required."
    //                }
    //            },

    //            invalidHandler: function (event, validator) { //display error alert on form submit   

    //            },

    //            highlight: function (element) { // hightlight error inputs
    //                $(element)
    //                    .closest('.form-group').addClass('has-error'); // set error class to the control group
    //            },

    //            success: function (label) {
    //                label.closest('.form-group').removeClass('has-error');
    //                label.remove();
    //            },

    //            errorPlacement: function (error, element) {
    //                error.insertAfter(element.closest('.input-icon'));
    //            },

    //            submitHandler: function (form) {
    //                form.submit();
    //            }
    //        });

    //        $('.forget-form input').keypress(function (e) {
    //            if (e.which == 13) {
    //                if ($('.forget-form').validate().form()) {
    //                    $('.forget-form').submit();
    //                }
    //                return false;
    //            }
    //        });

    //        jQuery('#forget-password').click(function () {
    //            jQuery('.login-form').hide();
    //            jQuery('.forget-form').show();
    //        });

    //        jQuery('#back-btn').click(function () {
    //            jQuery('.login-form').show();
    //            jQuery('.forget-form').hide();
    //        });

    //}

    var handleRegister = function () {

        //function format(state) {
        //    if (!state.id) return state.text; // optgroup
        //    return "<img class='flag' src='../../assets/global/img/flags/" + state.id.toLowerCase() + ".png'/>&nbsp;&nbsp;" + state.text;
        //}


        //$("#select2_sample4").select2({
        //  	placeholder: '<i class="fa fa-map-marker"></i>&nbsp;Select a Country',
        //    allowClear: true,
        //    formatResult: format,
        //    formatSelection: format,
        //    escapeMarkup: function (m) {
        //        return m;
        //    }
        //});


        //$('#select2_sample4').change(function () {
        //    $('.register-form').validate().element($(this)); //revalidate the chosen dropdown value and show error or success message for the input
        //});



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
            jQuery('.login-form').hide();
            jQuery('.register-form').show();
            pushState(false);
            ga('send', 'pageview', '/account/signup');
            trackConversion();
        });

        jQuery('#login-btn').click(function () {
            jQuery('.login-form').show();
            jQuery('.register-form').hide();
            pushState(true);
            ga('send', 'pageview', '/account/signin');
            trackConversion();
        });


        jQuery('.facebook').click(function () {
            facebookLogin();
        });

        jQuery('.google').click(function () {
            googleLogIn();
        });

        jQuery('#lang_select').change(function (e) {
            saveRegisterData();
            window.location.href = jQuery(this).find(':selected').attr('data-href');
        });
    }

    var handleLanguage = function () {
        var lang = $.cookie('l2');
        if (!lang) {
            return;
        }
        $('#lang_select').val(lang.toLowerCase());
    }

    function saveRegisterData() {
        if (!window.sessionStorage) {
            return;
        }

        var signupData = {
            firstname: $('.register-form input[name="firstname"]').val(),
            lastname: $('.register-form input[name="lastname"]').val(),
            email: $('.register-form input[name="email"]').val(),
            confirmEmail: $('.register-form input[name="confirmemail"]').val(),
            password: $('.register-form input[name="password"]').val()
        };

        window.sessionStorage.setItem('signup', JSON.stringify(signupData));

    }

    function loadRegisterData() {
        if (!window.sessionStorage) {
            return;
        }

        var signupData = window.sessionStorage.getItem('signup');
        if (!signupData) {
            return;
        }

        var signupObj;
        try {
            signupObj = JSON.parse(signupData);
        } catch (e) {
            signupObj = {};
        }

        for (var key in signupObj) {
            if (signupObj.hasOwnProperty(key)) {
                $('.register-form input[name="' + key.toLowerCase() + '"]').val(signupObj[key]);
            }
        }
        window.sessionStorage.removeItem('signup');
    }

    function signup(form) {
        //var firstname = $('.register-form input[name="firstname"]').val(),
        //    lastname = $('.register-form input[name="lastname"]').val(),
        //    email = $('.register-form input[name="email"]').val(),
        //    confirmEmail = $('.register-form input[name="confirmemail"]').val(),
        //    password = $('.register-form input[name="password"]').val(),
        //        returnUrl = getUrlVars()['returnUrl'];

        var values = $(form).serialize();
        var returnUrl = getUrlVars()['returnUrl'];
        if (returnUrl) {
            values += "&returnUrl=" + encodeURIComponent(getUrlVars()['returnUrl']);
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
                return;
            }

            ga('send', 'event', 'Signup', 'Email');

            clearStorage();
            setNewUser();
            if (returnUrl) {
                window.location.href = returnUrl;
                return;
            }

            window.location.href = data.payload;
        });
    }

    function signin(form) {

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
                token: id_token
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
                token: accessToken
                //returnUrl: getUrlVars()['returnUrl']
            }).done(function (data) {
                externalLogIn(data, 'Facebook');
            });
        }
    }
    // this is a hack for using angular cache element in the next page
    function setNewUser() {
        var date = new Date().getTime();
        sessionStorage.setItem('angular-cache.caches.points.keys', '["register"]');
        sessionStorage.setItem('angular-cache.caches.points.data.register', '{"key":"register","value":true,"created":' + date + ',"accessed":' + date + ',"expires":' + (date + 600000) + '}');

    }
    function externalLogIn(data, type) { //Type google or facebook
        if (!data.success) {
            alert('there is a problem signing you in with facebook');
            return;
        }

        clearStorage();
        var obj = data.payload;
        if (obj.isnew) {
            setNewUser();
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
    function pushState(isLogin) {
        if (!window.history) {
            return;
        }
        if (isLogin) {
            window.history.replaceState(null, "Signin", "/account/signin");
            return;
        }
        window.history.replaceState(null, "Sign up", "/account/signup");
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
            //handleForgetPassword();
            handleRegister();
            handleLanguage();
            if (window.location.pathname.indexOf('/signup') > -1) {
                jQuery('.login-form').hide();
                jQuery('.register-form').show();
                loadRegisterData();
            }

            $.each(jQuery.validator.messages, function (key, value) {
                if ($.type(value) == 'string') { jQuery.validator.messages[key] = value.replace('.', ''); }
            });



            $('.content').show();
        }
    };

}();

+function trackConversion() {
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
jQuery(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    Login.init();
    Demo.init();
    // init background slide images
    $.backstretch([
     "/images/signin/1a.png",
     "/images/signin/2a.png",
     "/images/signin/3a.png"
    ], {
        fade: 1000,
        duration: 8000
    });

    window.google_trackConversion({
        conversion_id: 939226062,
        custom_params: window.google_tag_params,
        remarketing_only: true
    });
});