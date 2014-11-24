/// <reference path="../Views/Account/Index2.cshtml" />
(function (document) {
    "use strict";



    var eById = document.getElementById.bind(document),
        registerPopup = eById('register'), registerForm = eById('registerForm'),
        cancelPopup = eById('cancelRegisterPopup'), regPopup = eById('regPopup'),
        connectPopup = eById('connect'), connectForm = eById('login'),
        langSelect = eById('dLangSelect');


    if (!connectPopup) {
        return;
    }

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

    document.getElementById('maleRadio').checked = true;
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
            resetErrors(connectForm);
            resetErrors(registerForm);
        }
    }

    function resetPopupView() {
        registerPopup.removeClass('register registerFirst step2');
        connectPopup.removeClass('connect');

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
        connectFb: connectFb
    }

    function connect() {
        resetPopupView();
        connectPopup.addClass('connect');
        focusOnElement(connectPopup);
    }

    function register() {
        registerPopup.addClass('register');
        focusOnElement(registerPopup);
    }

    function registerAction() {
        register();
        $registerPopup.addClass('registerFirst');
    }
    //#endregion

    connectForm.addEventListener('submit', function (e) {
        e.preventDefault();

        if (!validateForm(connectForm)) {
            return;
        }

        var data = {
            invId: gup('invId')
        };

        submit(connectForm, data);
    });

    registerForm.addEventListener('submit', function (e) {
        e.preventDefault();


        if (!validateForm(registerForm)) {
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

        var submit = form.querySelector('input[type="submit"]')
        submit.disabled = true;


        ajax.post(form.action, data, function (data) {
            submit.disabled = false;

            if (!data.success) {
                resetErrors(form);
                displayErrors(form, data.payload);
                return;
            }
            window.sessionStorage.clear();

            if (data.payload) {
                window.location.href = data.payload;
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

        $.ajax({
            type: 'POST',
            url: "/Account/FacebookLogin/",
            data: {
                token: accessToken,
                universityId: gup('universityId'),
                invId: gup('invId'),
                returnUrl: gup('returnurl') || (window.location.pathname.indexOf('/account') > -1 ? null : window.location.pathname)
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
    //#endregion

    function addChangeEvent(form) {
        var inputs = form.querySelectorAll("input");
        for (var i = 0, l = inputs.length; i < l; i++) {
            placeHolder(inputs[i]);
            inputs[i].oninput = function (e) {
                var target = e.target;
                validateInput(form, target);
            };
        }
    }
    function validateForm(form) {
        var inputs = form.querySelectorAll('input'),
            valid = true;
        for (var i = 0, l = inputs.length; i < l; ++i) {
            valid = validateInput(form, inputs[i]);
        }

        return valid;



    }
    function validateInput(form, input) {


        if (input.type === 'hidden' || input.type === 'submit') {
            return true;
        }

        var error,
            errorElement = form.querySelector('[data-valmsg-for="' + input.name + '"]');
        if (!errorElement) {
            return true;
        }

        //equal to other direction
        var equalInput = form.querySelector('input[data-val-equalto-other="*.' + input.name + '"]');
        if (equalInput) {
            validateInput(form, equalInput);
        }

        //equal to
        error = input.getAttribute('data-val-equalto');
        if (error && !equalTo(input, errorElement, error)) {
            return false;
        }                   

        //regex
        error = input.getAttribute('data-val-regex');
        if (error && !valRegex(input, errorElement, error)) {
            return false;
        }

        //length
        error = input.getAttribute('data-val-length');        
        if (error && !valLength(input, errorElement, error)) {
            return false;
        }

        //required
        error = input.getAttribute('data-val-required');
        if (error && !valRequired(input, errorElement, error)) {
            return false;
        }


        return true;

        function valRegex(input, errorElement, error) {
            var pattern = input.getAttribute('data-val-regex-pattern');
            if (error.length && input.value && pattern.length) {
                var patternExp = new RegExp(pattern);
                if (!patternExp.test(input.value)) {
                    resetError(errorElement);
                    appendError(input.name, errorElement, error);
                    return false;
                }
            }
            return true;
        }

        function valRequired(input, errorElement, error) {
            if (error.length) {

                resetError(errorElement);

                if (input.type === 'radio') {
                    var radioBtns = document.querySelectorAll('input[name="' + input.name + '"]'),
                        checked = false;
                    for (var i = 0, l = radioBtns.length; i < l && !checked; i++) {
                        checked = radioBtns[i].checked;
                    }

                    if (!checked) {
                        appendError(input.name, errorElement, error);
                    }

                    return checked;
                }
                if (!input.value) {
                    appendError(input.name, errorElement, error);
                    return false;
                }
            }

            return true;
        }

        function valLength(input, errorElement, error) {
            var minLength = parseInt(input.getAttribute('data-val-length-min'), 10);
            if (error.length && minLength) {
                if (input.value && (input.value.length > 0 && input.value.length < minLength)) {
                    resetError(errorElement);
                    appendError(input.name, errorElement, error);
                    return false;
                }
            }

            return true;
        }

        function equalTo(input, errorElement, error) {
            var otherInput = form.querySelector('input[name="' + input.getAttribute('data-val-equalto-other').substring(2) + '"]');
            if (error.length && input && otherInput && input.value !== otherInput.value) {
                resetError(errorElement);
                appendError(input.name, errorElement, error);
                return false;
            }
            return true;
        }




    }

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
    }

    function displayErrors(form, payload) {
        var errorElement, error;
        for (var i = 0, l = payload.length ; i < l; i++) {
            errorElement = form.querySelector('[data-valmsg-for="' + payload[i].key + '"]');
            if (!errorElement) {
                continue;
            }
            resetError(errorElement);
            error = payload[i].value[0];
            appendError(payload[i].key, errorElement, error);

        }
    }

    function setCookie(cname, cvalue,sPath) {
        var d = new Date();
        d.setTime(d.getTime() + (30 * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + "; " + expires + ';' +  (sPath ? 'path="' + sPath : "");
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