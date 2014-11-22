/// <reference path="../Views/Account/Index2.cshtml" />
(function (document) {
    "use strict";



    var eById = document.getElementById.bind(document),
        registerPopup = eById('register'), registerForm = eById('registerForm'),
        cancelPopup = eById('cancelRegisterPopup'), regPopup = eById('regPopup'),
        connectPopup = eById('connect'), connectForm = eById('login');


    if (!connectPopup) {
        return;
    }

    var connectPopupFb = connectPopup.getElementsByClassName('facebook')[0],
        registerPopupFb = registerPopup.getElementsByClassName('facebook')[0];

    var connectFormOpen = false, registerFormOpen = false;



    //#region Show and Hide popups

    registerPopup.addEventListener('click', function (e) {
        var target = e.target;

        if (!target.hasClass('addConnect')) {
            return;
        }

        connect();

    });
    connectPopup.addEventListener('click', function (e) {
        var target = e.target;

        if (!target.hasClass('addRegister')) {
            return;
        }

        register();
    });
    registerPopup.addEventListener('click', function (e) {
        var target = e.target;
        if (!target.hasClass('emailBtn')) {
            return;
        }

        if (!registerPopup.hasClass('registerFirst')) {
            return;
        }

        registerPopup.addClass('step2');
    });
    registerPopup.addEventListener('click', closePopup);
    connectPopup.addEventListener('click', closePopup);

    addChangeEvent(connectForm);
    addChangeEvent(registerForm);



    function focusOnElement(popup) {
        popup.querySelector('.inputText').focus();
    }


    function closePopup(e) {
        var target = e.target,
         isClose = target.getAttribute('data-closelogin');

        if (isClose) {
            resetPopupView();
        }

        connectFormOpen = registerFormOpen = false;
    }

    function resetPopupView() {
        registerPopup.removeClass('register registerFirst step2');
        connectPopup.removeClass('connect');
        //cd.resetForm($('#registerForm'));
        //cd.resetForm($('#login'));
        if (regPopup) {
            regPopup.style.display = 'none';
        }
    }

    //#endregion

    window.connectApi = {
        connect: connect,
        register: register,
        registerAction: registerAction,
        connectFb: connectFb
    }

    function connect() {
        connectFormOpen = true;
        resetPopupView();
        connectPopup.addClass('connect');
        focusOnElement(connectPopup);
    }

    function register() {
        registerPopup.addClass('register');
        focusOnElement(registerPopup);
        registerFormOpen = true;
    }

    function registerAction() {
        register();
        $registerPopup.addClass('registerFirst');
    }

    connectForm.addEventListener('submit', function (e) {
        e.preventDefault();

        if (!validateForm(connectForm)) {
            return;
        }

        var data = {
            invId: gup('invId')
        };

        var inputs = connectForm.getElementsByTagName('input');
        for (var i = 0, l = inputs.length; i < l; ++i) {
            if (inputs[i].type === 'submit') {
                continue;
            }
            data[inputs[i].name] = inputs[i].value;
        }

        var submit = connectForm.querySelector('input[type="submit"]')
        submit.disabled = true;

        ajax.post(connectForm.action, data, function (data) {
            submit.disabled = false;

            if (!data.success) {
                alert('Something went wrong please try again');
            }
            window.sessionStorage.clear();

            var returnUrl = cd.getParameterByName('returnUrl');
            if (returnUrl.length) {
                window.location = returnUrl;
                return;
            }

            if (window.location.href.indexOf('error') > -1) {
                window.location.href = '/dashboard/';
                return;
            }
            window.location.reload();

            //cd.resetErrors($form);
            //cd.displayErrors($form, data.payload);
            //$submit.removeAttr(c);

        }, true);
        //data: data,
        //type: 'POST',
        //success: function (data) {

        //},
        //error: function () {
        //    $submit.removeAttr(c);
        //    cd.notification('Something went wrong please try again');
        //}    
    });

    //$registerForm.submit(function (e) {
    //    e.preventDefault();

    //    var $form = $(this), $submit = $form.find(':submit');
    //    $form.validate().settings.ignore = ''; //we to this because hidden fields are not validated
    //    if (!(!$form.valid || $form.valid())) {
    //        return;
    //    }

    //    var d = $form.serializeArray();
    //    d.push({ name: 'universityId', value: cd.getParameterByName('universityId') });
    //    d.push({ name: 'returnUrl', value: cd.getParameterByName('returnurl') || (window.location.pathname === '/account' ? null : window.location.pathname) })
    //    d.push({ name: 'invId', value: cd.getParameterByName('invId') });
    //    $submit.attr('disabled', 'disabled');
    //    $.ajax({
    //        url: $form.prop('action'),
    //        data: d,
    //        type: 'POST',
    //        success: function (data) {
    //            if (data.success) {
    //                window.sessionStorage.clear();
    //                if (data.payload) {
    //                    window.location.href = data.payload;
    //                    return;
    //                }

    //                if (window.location.href.indexOf('error') > -1) {
    //                    window.location.href = '/dashboard/';
    //                    return;
    //                }
    //                window.location.reload();
    //                return;
    //            }
    //            cd.resetErrors($form);
    //            cd.displayErrors($form, data.payload);
    //            $submit.removeAttr('disabled');

    //        },
    //        error: function () {
    //            $submit.removeAttr('disabled');
    //            cd.notification('Something went wrong please try again');
    //        }
    //    });
    //});

    connectPopupFb.addEventListener('click', connectFb);
    registerPopupFb.addEventListener('click', connectFb);
    function logInFacebook(accessToken) {
        var facebookText = {
            en: "I have just signed up to Cloudents.\nCloudents is a free online and mobile social studying platform. With a large collection of study material, course notes, summaries and Q&As Cloudents makes my studying easier.",
            he: 'התחברתי ל-Cloudents, המאגר האקדמי של הסטודנטים.\nמחפשים חומרי לימוד?\nסיכומים, מבחנים,מאמרים.\nמעל ל - 50 אלף קבצים במאגר.\nפשוט לחצו כאן, והירשמו. \nהדרך אל התואר, לא הייתה קלה יותר.',
            ar: 'أنا أيضا قمت بالاتصال بشبكة Cloudents. حيث أن Cloudentsلديها أكبر مجموعة من مذكرات المقررات الدراسية، والامتحانات في مدرستك. أنضم إلى Cloudents، كلما زاد عدد الطلاب المنضمين، كلما أصبحت الدراسة أسهل.',
            ru: 'Я тоже подключился к Cloudents. В Cloudents есть крупнейшее собрание конспектов и экзаменов вашего учебного заведения. Присоединяйтесь к Cloudents; чем больше обучающихся присоединятся, тем легче будет учиться.',
            zh: '我也连接到 Cloudents 了。Cloudents 拥有最丰富的课程笔记以及贵校的考卷。加入 Cloudents 吧，越多人参加，学习就变得越容易。',
            nl: 'Check Cloudents! Een plek om samen te werken aan opdrachten, studiemateriaal te vinden, proeftentamens te doen of ideeën en teksten te bespreken.'
        };

        $.ajax({
            type: 'POST',
            url: "/Account/FacebookLogin/",
            data: {
                token: accessToken,
                universityId: cd.getParameterByName('universityId'),
                invId: cd.getParameterByName('invId'),
                returnUrl: cd.getParameterByName('returnurl') || (window.location.pathname.indexOf('/account') > -1 ? null : window.location.pathname)
            },
            success: function (data) {
                if (!data.success) {
                    alert('there is a problem signing you in with facebook');
                    return;
                }
                window.sessionStorage.clear();
                var obj = data.payload;
                if (obj.isnew) {
                    FB.api('/me', function (response) {
                        var locale = response.locale.substr(0, response.locale.indexOf('_')),
                            text = facebookText[locale];
                        if (!text) {
                            text = facebookText.en;
                        }
                        FB.api('/me/feed', 'post', { message: text, link: 'https://www.cloudents.com' }, function () {
                            if (obj.url) {
                                window.location.href = obj.url;
                                return;
                            }

                            if (window.location.href.indexOf('error') > -1) {
                                window.location.href = '/dashboard/';
                                return;
                            }

                            window.location.reload();
                        });
                    });

                }
                else {
                    window.location.reload();
                }
            },
            error: function (msg) {
                alert(msg);
            }
        });
    }

    function connectFb() {
        FB.login(function (response) {
            if (response.authResponse) {
                var accessToken = response.authResponse.accessToken;
                FB.api('/me/permissions', function (response) {

                    var perms = response.data[0];
                    //console.log(perms)
                    // Check for publish_stream permission in this case....
                    if (perms.email) {
                        //console.log(accessToken);
                        logInFacebook(accessToken);
                        // User has permission
                        cd.analytics.trackEvent('Homepage', 'Facebook signup', 'Successful login useing facebook');
                    } else {
                        cd.notification('you need to give email permission');
                        cd.analytics.trackEvent('Homepage', 'Facebook signup', 'Failed login useing facebook');
                    }
                });
            }

        }, { scope: 'email,publish_stream,user_friends' });
    }

    var ajax = {};
    ajax.send = function (url, callback, method, data, sync) {
        var x = new XMLHttpRequest();
        x.open(method, url, sync);
        x.onreadystatechange = function () {
            if (x.readyState == 4) {
                if (x.status === 200) {
                    callback(JSON.parse(x.responseText));
                }
            }

        };
        if (method == 'POST') {
            x.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
            x.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        }
        x.send(data)
    };

    ajax.get = function (url, data, callback, sync) {
        var query = [];
        for (var key in data) {
            query.push(encodeURIComponent(key) + '=' + encodeURIComponent(data[key]));
        }
        ajax.send(url + '?' + query.join('&'), callback, 'GET', null, sync)
    };

    ajax.post = function (url, data, callback, sync) {
        var query = [];
        for (var key in data) {
            query.push(encodeURIComponent(key) + '=' + encodeURIComponent(data[key]));
        }
        ajax.send(url, callback, 'POST', query.join('&'), sync)
    };

    function gup(name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(window.location.href);
        if (results == null) {
            return null;
        } else {
            return results[1];
        }
    }

    function addChangeEvent(form) {
        var inputs = form.getElementsByTagName("input");
        for (var i = 0, l = inputs.length; i < l; i++) {
            inputs[i].oninput = function (e) {
                var target = e.target;
                validateInput(form, target);
            };
        }
    }
    function validateForm(form) {
        var inputs = form.getElementsByTagName('input'),
            valid = true;
        for (var i = 0, l = inputs.length; i < l && valid; ++i) {
            valid = validateInput(input[i]);
        }
        return valid;



    }
    function validateInput(form, input) {
        var error;
        var errorElement = form.querySelector('[data-valmsg-for="' + input.name + '"]');
        if (!errorElement) {
            return;
        }
        resetError(errorElement);

        if (input.type === 'text') {
            error = input.getAttribute('data-val-required');
            if (!valRequired(input, errorElement, error)) {
                return false;
            }


            return true;
        }

        if (input.type === 'email') {
            //regex

            error = input.getAttribute('data-val-regex');
            if (!valRegex(input, errorElement, error)) {
                return false;
            }

            //required
            error = input.getAttribute('data-val-required');
            if (!valRequired(input, errorElement, error)) {
                return false;
            }


            return true;
        }


        if (input.type === 'password') {
            //length
            error = input.getAttribute('data-val-length');
            if (!valLength(input, errorElement, error)) {
                return false;
            }

            //required
            error = input.getAttribute('data-val-required');
            if (!valRequired(input, errorElement, error)) {
                return false;
            }
            return true;
        }

        function valRegex(input, errorElement, error) {
            var pattern = input.getAttribute('data-val-regex-pattern');
            if (error && error.length && input.value && pattern.length) {
                var patternExp = new RegExp(pattern);
                if (!patternExp.test(input.value)) {
                    appendError(input.name, errorElement, error);
                    return false;
                }
            }
            return true;
        }

        function valRequired(input, errorElement, error) {
            if (error && error.length) {
                if (!input.value) {
                    appendError(input.name, errorElement, error);
                    return false;
                }
            }

            return true;
        }

        function valLength(input, errorElement, error) {
            var minLength = parseInt(input.getAttribute('data-val-length-min'), 10);
            if (error && error.length && minLength) {
                if (input.value && (input.value.length > 0 && input.value.length < minLength)) {
                    appendError(input.name, errorElement, error);
                    return false;
                }
            }

            return true;
        }

        function resetError(element) {
            errorElement.innerHTML = '';
            errorElement.removeClass('field-validation-error').addClass('field-validation-valid');
        }
        function appendError(inputName, errorElement, error) {
            errorElement.removeClass('field-validation-valid').addClass('field-validation-error');
            errorElement.insertAdjacentHTML('beforeend', '<span for=' + inputName + '>' + error + '</span>');
        }

    }

    //$(function () {
    //    var data = sessionStorage.getItem('registerForm');
    //    if (!data) {
    //        return;
    //    }
    //    var arr = JSON.parse(data);
    //    for (var i = 0; i < arr.length ; i++) {
    //        $('#registerForm').find('[name="' + arr[i].name + '"]')[0].value = arr[i].value;
    //    }
    //    $('.addRegister').click();
    //    sessionStorage.removeItem('registerForm');
    //});
})(window.document);