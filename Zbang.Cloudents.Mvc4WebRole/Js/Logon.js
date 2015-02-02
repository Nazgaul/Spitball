﻿/// <reference path="../Views/Account/Index2.cshtml" />
(function (document) {
    "use strict";
    window.staticScript = true;
    var eById = document.getElementById.bind(document),
        registerPopup = eById('register'), registerForm = eById('registerForm'),
        //cancelPopup = eById('cancelRegisterPopup'),
        regPopup = eById('regPopup'),
        connectPopup = eById('connect'), connectForm = eById('login'),
        langSelect = eById('dLangSelect'),
        currentForm, validatinator;


    if (!connectPopup) {
        return;
    }

    document.getElementById('maleRadio').checked = true;

    var connectPopupFb = connectPopup.getElementsByClassName('facebook')[0],
        registerPopupFb = registerPopup.getElementsByClassName('facebook')[0];


    (function () {
        var data = sessionStorage.getItem('registerForm');
        if (!data) {
            return;
        }
        var arr = JSON.parse(data), input;
        for (var i = 0; i < arr.length ; i++) {
            input = registerForm.querySelector('input[name="' + arr[i].name + '"]').value = arr[i].value;
        }

        register();


        sessionStorage.removeItem('registerForm');
    })();

    //document.getElementById('maleRadio').checked = true;
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


    langSelect.addEventListener('change', function () {

        var x = [], obj;
        obj = registerForm.querySelector('input[name="FirstName"]');
        x.push({ name: obj.name, value: obj.value });
        obj = registerForm.querySelector('input[name="LastName"]');
        x.push({ name: obj.name, value: obj.value });
        obj = registerForm.querySelector('input[name="NewEmail"]');
        x.push({ name: obj.name, value: obj.value });

        window.sessionStorage.setItem('registerForm', JSON.stringify(x));
        window.location.href = this.selectedOptions ? this.selectedOptions[0].getAttribute('data-href') : this.options[this.selectedIndex].getAttribute('data-href');
    });

    //addChangeEvent(connectForm);
    //addChangeEvent(registerForm);

    validatinator = new Validatinator(
        {
            loginForm: {
                Email: 'required|email',
                Password: 'required',
            },
            registerForm: {
                FirstName: 'required',
                LastName: 'required',
                NewEmail: 'required|email',
                ConfirmEmail: 'required|email|same:NewEmail',
                Password: 'required|minLength:6'
            }
        },
        {
            required: JsResources.FieldRequired,
            minLength: JsResources.PwdAtLeast6Chars,
            email: JsResources.InvalidEmail,
            same: JsResources.ConfirmEmailCompare
        }
    );

    //document.addEventListener('blur', function (e) {
    //    var target = e.target;

    //    if (!currentForm) {
    //        return;
    //    }

    //    if (target.nodeName !== 'INPUT') {
    //        return;
    //    }
    //    if (target.type == 'submit') {
    //        return;
    //    }

    //    var errorElement = currentForm.querySelector('[data-valmsg-for="' + target.name + '"]');

    //    if (!errorElement) {
    //        return;
    //    }

    //    resetError(errorElement);

    //    if (validatinator.passes(currentForm.name)) {
    //        return;
    //    }

    //    var errorObj = validatinator.errors[currentForm.name][target.name];
    //    if (!errorObj) {
    //        return;

    //    }

    //    var errorText = errorObj[Object.keys(errorObj)[0]];
    //    appendError(target.name, errorElement, errorText);

    //}, true);

    document.addEventListener('input', function (e) {
        var target = e.target;

        if (!currentForm) {
            return;
        }

        if (target.nodeName !== 'INPUT') {
            return;
        }

        var errorElement = currentForm.querySelector('[data-valmsg-for="' + target.name + '"]');

        if (!errorElement) {
            return;
        }

        if (validatinator.passes(currentForm.name)) {
            resetError(errorElement);
            return;
        }

        if (!validatinator.errors[currentForm.name][target.name]) {
            resetError(errorElement);
        }

    }, true);



    function focusOnElement(popup) {
        popup.querySelector('.inputText').focus();
    }


    function closePopup(e) {
        var target = e.target,
         isClose = target.getAttribute('data-closelogin');

        if (isClose) {
            resetPopupView();
            resetErrors(connectForm);
            resetErrors(registerForm);
        }
    }

    function resetPopupView() {
        registerPopup.removeClass('register registerFirst step2');
        connectPopup.removeClass('connect');
        resetErrors(connectForm);
        resetErrors(registerForm);

        if (regPopup) {
            regPopup.style.display = 'none';
        }
    }

    //#endregion

    //#region public
    window.connectApi = {
        connect: connect,
        register: register,
        registerAction: registerAction,
        connectFb: connectFb,
        reset: resetPopupView
    }

    function connect() {
        resetPopupView();
        connectPopup.addClass('connect');
        focusOnElement(connectPopup);
        currentForm = connectForm;
    }

    function register() {
        registerPopup.addClass('register');
        focusOnElement(registerPopup);
        currentForm = registerForm;
    }

    function registerAction() {
        register();
        registerPopup.addClass('registerFirst');
    }
    //#endregion

    connectForm.addEventListener('submit', function (e) {
        e.preventDefault();
        //var submit2 = connectForm.querySelector('input[type="submit"]');
        //submit2.disabled = true;
        if (validatinator.fails('loginForm')) {
            displayLocalErrors(currentForm, validatinator.errors[currentForm.name]);
            return;
        }

        var data = {
            invId: gup('invId')
        };

        submit(connectForm, data);
    });

    registerForm.addEventListener('submit', function (e) {
        e.preventDefault();
        //var submit2 = connectForm.querySelector('input[type="submit"]');
        //submit2.disabled = true;
        if (validatinator.fails('registerForm')) {
            displayLocalErrors(currentForm, validatinator.errors[currentForm.name]);
            return;
        }


        var data = {
            universityId: gup('universityId'),
            returnUrl: gup('returnUrl'),
            invId: gup('invId')
        };

        submit(registerForm, data);
    });

    function submit(form, data) {


        var inputs = form.querySelectorAll('input');
        for (var i = 0, l = inputs.length; i < l; ++i) {
            if (inputs[i].type === 'submit') {
                continue;
            }
            data[inputs[i].name] = inputs[i].value;
        }

        var submit2 = form.querySelector('input[type="submit"]');
        submit2.disabled = true;

        var obj = {};
        for (var x in data) {
            if (data[x] != null) {
                obj[x]  = data[x];
            }
        }

        ajax.post(form.action, obj, function (data2) {
            

            if (!data2.success) {
                resetErrors(form);
                displayErrors(form, data2.payload);
                submit2.disabled = false;
                return;
            }
            window.sessionStorage.clear();

            if (data2.payload) {
                window.location.href = data2.payload;
                return;
            }

            if (window.location.href.indexof('error') > -1) {
                window.location.href = '/dashboard/';
                return;
            }
            window.location.reload();
            return;

        }, true);
    }

    //#region facebook
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

        ajax.post('/Account/FacebookLogin/', {
            token: accessToken,
            universityId: gup('universityId'),
            invId: gup('invId'),
            returnUrl: gup('returnurl') || (window.location.pathname.indexOf('/account') > -1 ? null : window.location.pathname)
        }, function (data) {
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
        });
    }
    function connectFb() {
        FB.login(function (response) {
            if (response.authResponse) {
                var accessToken = response.authResponse.accessToken;
                FB.api('/me/permissions', function (response2) {

                    var perms = response2.data[0];
                    if (perms.email) {
                        logInFacebook(accessToken);
                        // User has permission
                        window.ga('send', 'event', 'Homepage', 'Facebook signup', 'Successful login useing facebook');
                    } else {
                        alert('you need to give email permission');
                        window.ga('send', 'event', 'Homepage', 'Facebook signup', 'Failed login useing facebook');
                    }
                });
            }

        }, { scope: 'email,publish_stream,user_friends' });
    }
    //#endregion


    //#region utils
    var ajax = {};
    ajax.send = function (url, callback, method, data, sync) {
        var x = new XMLHttpRequest();
        x.open(method, url, sync);
        x.onreadystatechange = function () {
            if (x.readyState == 4) {
                if (x.status === 200) {
                    callback(JSON.parse(x.responseText));
                } else {
                    alert('Something went wrong please try again');
                }
            }

        };
        if (method == 'POST') {
            x.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
            x.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        }
        x.send(data);
    };

    ajax.get = function (url, data, callback, sync) {
        var query = [];
        for (var key in data) {
            query.push(encodeURIComponent(key) + '=' + encodeURIComponent(data[key]));
        }
        ajax.send(url + '?' + query.join('&'), callback, 'GET', null, sync);
    };

    ajax.post = function (url, data, callback, sync) {
        var query = [];
        for (var key in data) {
            query.push(encodeURIComponent(key) + '=' + encodeURIComponent(data[key]));
        }
        ajax.send(url, callback, 'POST', query.join('&'), sync);
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
    //#endregion

    function appendError(inputName, errorElement, error) {
        errorElement.removeClass('field-validation-valid').addClass('field-validation-error');
        errorElement.insertAdjacentHTML('beforeend', '<span for=' + inputName + '>' + error + '</span>');
    }

    function resetError(element) {
        element.innerHTML = '';
        element.removeClass('field-validation-error').addClass('field-validation-valid');
    }

    function resetErrors(form) {
        var inputs = form.querySelectorAll('input'), input, errorElement;
        for (var i = 0, l = inputs.length; i < l; ++i) {
            input = inputs[i];
            errorElement = form.querySelector('[data-valmsg-for="' + input.name + '"]');
            if (!errorElement) {
                continue;
            }
            resetError(errorElement);
        }

        var summary = form.querySelector('[data-error-msg="true"]');
        if (summary) {
            summary.parentElement.removeChild(summary);
        }
    }

    function displayLocalErrors(form, errors) {
        var errorElement, errorText, errorObj;

        for (var field in errors) {
            errorObj = errors[field];
            errorText = errorObj[Object.keys(errorObj)[0]];
            errorElement = form.querySelector('[data-valmsg-for="' + field + '"]');
            resetError(errorElement);
            appendError(field, errorElement, errorText);
        }
    }

    function displayErrors(form, payload) {
        for (var i = 0, l = payload.length ; i < l; i++) {

            var summary = payload[i].value[0],
              errorElement = form.querySelector('[data-valmsg-for="' + payload[i].key + '"]');
            if (!errorElement) {
                form.insertAdjacentHTML('afterbegin', '<div data-error-msg="true" class="validation-summary-errors">' + summary + '</div>');
                continue;
            }
            resetError(errorElement);
            var error = payload[i].value[0];
            appendError(payload[i].key, errorElement, error);


            

        }
    }
})(window.document);