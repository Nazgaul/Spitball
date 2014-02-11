/// <reference path="../Views/Account/Index2.cshtml" />
(function (cd, $) {
    if (window.scriptLoaded.isLoaded('l')) {
        return;
    }

    "use strict";
    function registerEvents() {
        cd.putPlaceHolder();
        //drop downs
        var slideSpeed = 150;
        var $userMenu = $('ul.userMenu');
        $('[data-menu="user"]').click(function (e) {
            $('#invitesList').slideUp(); //close invite - maybe should be in class
            if ($userMenu.is(':visible')) {
                $userMenu.slideUp(slideSpeed);
                return;
            }
            e.stopPropagation();
            $userMenu.slideDown(slideSpeed);
        });
        $('body').click(function () {
            $userMenu.slideUp(slideSpeed);
        });
        //headerSettings
        $('.headerSettings').click(function () {
            $('.headerWpr').toggleClass('menuOpen');
            cd.analytics.trackEvent('Homepage', 'Click on menu open');
        });
        $('.closeMenu').click(function () {
            $('.headerWpr').removeClass('menuOpen');
        });
        //#region Video in homePage
        var videoElement = document.getElementById('homeVideo'), needToClacHeight = true;
        if (videoElement) {
            videoElement.addEventListener('loadedmetadata', function () {
                needToClacHeight = false;

            }, false);
        }
        $('#homeRegister').click(function () {
            cd.analytics.trackEvent('Homepage', 'Click on Email');
        });
        $('#homeLogin').click(function () {
            cd.analytics.trackEvent('Homepage', 'Click on Login');

        });

        $('#VideoWpr').click(function () {
            if (needToClacHeight) { //ipad issue
                var ratio = 1.77777777777,
                width = cd.orientationDetection() ? screen.height : screen.width;
                videoElement.style.height = (width * 0.75 / ratio) + 'px';
            }

            cd.analytics.trackEvent('Homepage', 'Show video', 'Clicking on play the video');
            $('#mVideo').fadeIn(300);
            $('#homeVideo').animate({ opacity: 1 }, 600, function () {

                var video = $(this)[0];
                video.currentTime = 0;
                video.play();
            });
            calculateTopMargin();
        });
        function calculateTopMargin() {
            $('#homeVideo').css('marginTop', ($(window).height() - $('#homeVideo').height()) / 2 + 'px');
        }

        $('.sideBarSignupBtn').click(function () {
            cd.analytics.trackEvent('Homepage', 'Sidebar signup', 'Clicking on signup in the sidebar');

        });

        $('#mVideo').click(function (e) {
            if (e.target.id === 'homeVideo') {
                return;
            }
            $('#mVideo').fadeOut(300);
            $('#homeVideo').animate({ opacity: 0 }, 300, function () {
                var video = $(this)[0];
                video.currentTime = 0;
                video.pause();
            });
        });
        //#endregion

        $('[data-language]').click(function () {
            cd.setCookie('lang', $(this).data('language'), 10);
        });
        //mobile view
        $('#langSelect').val(cd.getCookie('lang') || 'en-US').change(function () {
            cd.setCookie('lang', $(this).val(), 10);
            location.reload();
        });
        cd.orientationDetection();

        //Welcome page
        function changeState() {

            var $currentShow = $(window.location.hash);
            if ($currentShow.is(':visible')) {
                return;
            }

            $('.enter').hide();
            $currentShow.show();
            $currentShow.find('input[type="text"]:first').focus();
            hightLightMenu();
        }

        if ($('.enter').length) {
            window.onhashchange = function () {
                changeState();
            };
            if (location.hash === '') {
                location.hash = 'connect';
            }
            changeState();
        }
        if ($('#register').length) {
            logInRegisterEvents();
        }


        hightLightMenu();
        //menu bar actions 
        function hightLightMenu() {
            $('.currentItem').removeClass('currentItem');
            var url = location.pathname.toLowerCase() + location.hash;
            if (url === "/account") {
                $('aside').find('li:first').addClass('currentItem');
            } else {
                $('aside').find('a[href$="' + url + '"]').parent().addClass('currentItem');
            }
            $('aside').find('[data-language|="' + $('html').attr('lang') + '"]').parent().addClass('currentItem');
        }



        function logInRegisterEvents() {
            $('form').submit(function (e) {
                e.preventDefault();
                var $form = $(this), $submit = $form.find(':submit');
                if (!$form.valid || $form.valid()) {
                    var d = $form.serializeArray();
                    d.push({ name: 'universityId', value: cd.getParameterByName('universityId') });
                    $submit.attr('disabled', 'disabled');
                    $.ajax({
                        url: $form.prop('action'),
                        data: d,
                        type: 'POST',
                        success: function (data) {
                            if (data.Success) {
                                window.location.href = data.Payload || "/";
                                return;
                            }
                            cd.resetErrors($form);
                            cd.displayErrors($form, data.Payload);
                            $submit.removeAttr('disabled');
                        },
                        error: function () {
                            cd.notification('Something went wrong please try again');
                            $submit.removeAttr('disabled');
                            window.location.reload(); // if cscf was occure - reloading the page to refresh the token
                        }
                    });

                }
            });

        }


    }

    function logInFacebook(accessToken) {
        var facebookText = {
            en: "I have just signed up to Cloudents.\nCloudents is a free online and mobile social studying platform. With a large collection of study material, course notes, summaries and Q&As Cloudents makes my studying easier.",
            he: 'התחברתי ל-Cloudents, המאגר האקדמי של הסטודנטים.\nמחפשים חומרי לימוד?\nסיכומים, מבחנים,מאמרים.\nמעל ל - 50 אלף קבצים במאגר.\nפשוט לחצו כאן, והירשמו. \nהדרך אל התואר, לא הייתה קלה יותר.',
            ar: 'أنا أيضا قمت بالاتصال بشبكة Cloudents. حيث أن Cloudentsلديها أكبر مجموعة من مذكرات المقررات الدراسية، والامتحانات في مدرستك. أنضم إلى Cloudents، كلما زاد عدد الطلاب المنضمين، كلما أصبحت الدراسة أسهل.',
            ru: 'Я тоже подключился к Cloudents. В Cloudents есть крупнейшее собрание конспектов и экзаменов вашего учебного заведения. Присоединяйтесь к Cloudents; чем больше обучающихся присоединятся, тем легче будет учиться.',
            zh: '我也连接到 Cloudents 了。Cloudents 拥有最丰富的课程笔记以及贵校的考卷。加入 Cloudents 吧，越多人参加，学习就变得越容易。'
        };

        $.ajax({
            type: 'POST',
            url: "/Account/FacebookLogin",
            data: {
                token: accessToken,
                universityId: cd.getParameterByName('universityId')
            },
            success: function (data) {
                if (!data.Success) {
                    //location.href = '/';
                    alert('there is a problem signing you in with facebook');
                    return;
                }
                var obj = data.Payload;
                if (obj.isnew) {
                    FB.api('/me', function (response) {
                        var locale = response.locale.substr(0, response.locale.indexOf('_')),
                            text = facebookText[locale];
                        if (!text) {
                            text = facebookText.en;
                        }
                        FB.api('/me/feed', 'post', { message: text, link: 'https://www.cloudents.com' }, function () {
                            location.href = obj.url;
                        });
                        //console.log('Good to see you, ' + response.locale + '.');
                    });

                }
                else {
                    location.href = obj.url;
                }

                //if (data.Success) {
                //    location.href = data.Payload || "/";
                //    return;
                //}
                //location.href = '/';
            },
            error: function (msg) {
                $('div.loading').hide();
                $('#fbError').text(msg);
            }
        });
    }
    registerEvents();
    var fbElement = $('.facebook');
    if (fbElement.length) {
        fbElement.click(function () {
            FB.login(function (response) {
                if (response.authResponse) {
                    var accessToken = response.authResponse.accessToken;
                    FB.api('/me/permissions', function (response) {
                        var perms = response.data[0];
                        // Check for publish_stream permission in this case....
                        if (perms.email) {
                            logInFacebook(accessToken);
                            // User has permission
                            cd.analytics.trackEvent('Homepage', 'Facebook signup', 'Successful login useing facebook');
                        } else {
                            cd.notification('you need to give email permission');
                            cd.analytics.trackEvent('Homepage', 'Facebook signup', 'Failed login useing facebook');
                        }
                    });
                }

            }, { scope: 'email,publish_stream' });
        });
        cd.loader.registerFacebook();
        //window.fbAsyncInit = function () {
        //cd.pubsub.subscribe('fbInit', function () {
        //    FB.getLoginStatus(function (response) {
        //        if (response.status === 'connected') {
        //            logInFacebook(response.authResponse.accessToken);
        //        }
        //    });
        //});

    }
    sessionStorage.clear();
    localStorage.removeItem('history');//remove history
    $.extend($.validator.messages, {
        email: $('#NewEmail').data('valRegex'),
    });
})(cd, jQuery);