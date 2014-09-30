/// <reference path="../Views/Account/Index2.cshtml" />
(function (cd, $) {
    "use strict";

    if (window.scriptLoaded.isLoaded('log')) {
        return;
    }
    var eById = document.getElementById.bind(document),
        $registerPopup = $(eById('register')), $registerForm = $(eById('registerForm')),
        $cancelPopup = $(eById('cancelRegisterPopup')), regPopup = eById('regPopup'),
        $connectPopup = $(eById('connect')), $connectForm = $(eById('login'));

    //#region Show and Hide popups
    $(document).on('click', '.addConnect', function (e) {
        e.preventDefault();
        resetPopupView();
        $connectPopup.addClass('connect');
        focusOnElement($connectPopup);
    })
    .on('click', '.addRegister', function (e) {

        e.preventDefault();
        //resetPopupView(); // bring this back on we cant populate data
        $registerPopup.addClass('register');
        focusOnElement($registerPopup);
    })
    .on('click', '.registerFirst .emailBtn', function () {
        $registerPopup.addClass('step2');
    });

    $('[data-closelogin]').click(resetPopupView);

    cd.pubsub.subscribe('register', function (data) {
        resetPopupView();
        $registerPopup.addClass('register');
        if (data.action) {
            $registerPopup.addClass('registerFirst');
        }

        focusOnElement($registerPopup);
    });

    $cancelPopup.click(function () {
        regPopup.style.display = 'none';
    });

    function focusOnElement($popup) {
        $popup.find('.inputText').first().focus();
    }

    cd.pubsub.subscribe('resetLoginPopup',resetPopupView);
    function resetPopupView() {
        $registerPopup.removeClass('register registerFirst step2');
        $connectPopup.removeClass('connect');
        cd.resetForm($('#registerForm'));
        cd.resetForm($('#login'));
        if (regPopup) {
            regPopup.style.display = 'none';
        }
    }

    //#endregion
   
    $connectForm.submit(function (e) {
        e.preventDefault();
        var $form = $(this), $submit = $form.find(':submit');
        if (!(!$form.valid || $form.valid())) {
            return;
        }
        var c = 'disabled';
        var d = $form.serializeArray();

        $submit.attr(c, c);
        $.ajax({
            url: $form.prop('action'),
            data: d,
            type: 'POST',
            success: function (data) {
                if (data.success) {
                    var returnUrl = cd.getParameterByName('returnUrl');
                    if (returnUrl.length) {
                        window.location = returnUrl;
                        return;
                    }
                    window.location.reload();
                    return;
                }
                cd.resetErrors($form);
                cd.displayErrors($form, data.payload);
                $submit.removeAttr(c);
            },
            error: function () {
                $submit.removeAttr(c);
                cd.notification('Something went wrong please try again');
            }
        });
    });

    $registerForm.submit(function (e) {
        e.preventDefault();

        var $form = $(this), $submit = $form.find(':submit');
        $form.validate().settings.ignore = ''; //we to this because hidden fields are not validated
        if (!(!$form.valid || $form.valid())) {
            return;
        }

        var d = $form.serializeArray();
        d.push({ name: 'universityId', value: cd.getParameterByName('universityId') });
        d.push({ name: 'returnUrl', value: cd.getParameterByName('returnurl') || (window.location.pathname === '/account' ? null : window.location.pathname)})
        $submit.attr('disabled', 'disabled');
        $.ajax({
            url: $form.prop('action'),
            data: d,
            type: 'POST',
            success: function (data) {
                if (data.success) {
                    if (data.payload) {
                        window.location.href = data.payload;
                        return;
                    }
                    window.location.reload();
                    return;
                }
                cd.resetErrors($form);
                cd.displayErrors($form, data.payload);
                $submit.removeAttr('disabled');

            },
            error: function () {
                $submit.removeAttr('disabled');
                cd.notification('Something went wrong please try again');
            }
        });
    });

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
                returnUrl: cd.getParameterByName('returnurl') || (window.location.pathname === '/account' ? null : window.location.pathname)
            },
            success: function (data) {
                if (!data.success) {
                    cd.notification('there is a problem signing you in with facebook');
                    return;
                }
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
                            window.location.reload();
                        });
                    });

                }
                else {
                    window.location.reload();
                }
            },
            error: function (msg) {
                $('div.loading').hide();
                $('#fbError').text(msg);
            }
        });
    }

    var fbElement = $('.facebook');
    if (fbElement.length) {
        fbElement.click(function () {
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
        });
        cd.loader.registerFacebook();

    }

    //cd.sessionStorageWrapper.clear();
    //cd.localStorageWrapper.removeItem('history');//remove history
    $.extend($.validator.messages, {
        email: $('#NewEmail').data('valRegex'),
    });
})(cd, jQuery);