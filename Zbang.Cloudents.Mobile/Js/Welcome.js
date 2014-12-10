/// <reference path="../Views/Account/Index2.cshtml" />
(function (cd, $) {
    "use strict";

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
            if (video.readyState) {
                video.currentTime = 0;
            }
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
            if (video.readyState) {
                video.currentTime = 0;
            }
            video.pause();

        });
    });
    //#endregion    

    $('#dLangSelect').change(function (e) {
        var data = $('#registerForm').serializeArray();
        var x = [];
        for (var d in data) {
            if (data[d].name === 'FirstName' || data[d].name === 'LastName' || data[d].name === 'NewEmail') {
                x.push(data[d]);
            }
        }
        sessionStorage.setItem('registerForm', JSON.stringify(x));
        window.location.href = this.selectedOptions[0].getAttribute('data-href')
    });
    $(function () {
        var data = sessionStorage.getItem('registerForm');
        if (!data) {
            return;
        }
        var arr = JSON.parse(data);
        for (var i = 0; i < arr.length ; i++) {
            $('#registerForm').find('[name="' + arr[i].name + '"]')[0].value = arr[i].value;
        }
        sessionStorage.removeItem('registerForm');
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

    hightLightMenu();
    //menu bar actions 
    function hightLightMenu() {
        $('.currentItem').removeClass('currentItem');
        var url = location.pathname.toLowerCase() + location.hash;
        if (url === "/account/") {
            $('aside').find('li:first').addClass('currentItem');
        } else {
            $('aside').find('a[href$="' + url + '"]').parent().addClass('currentItem');
        }
    }

    $.extend($.validator.messages, {
        email: $('#NewEmail').data('valRegex')
    });
})(cd, jQuery);