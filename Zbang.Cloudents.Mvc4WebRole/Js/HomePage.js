﻿(function (i, s, o, g, r, a, m) {
    i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
        (i[r].q = i[r].q || []).push(arguments)
    }, i[r].l = 1 * new Date(); a = s.createElement(o),
    m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
})(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

ga('require', 'displayfeatures');

ga('create', 'UA-9850006-3', {
    'siteSpeedSampleRate': 70,
    'cookieDomain': 'spitball.co',
    'alwaysSendReferrer': true
});

window.fbAsyncInit = function () {
    FB.init({
        appId: '786786954776091',
        status: true,
        cookie: true,
        xfbml: true,
        oauth: true        
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
    js.src = "//connect.facebook.net/en_US/all.js";
    d.getElementsByTagName('head')[0].appendChild(js);
}(document));

(function (document) {
    "use strict";
    var //videoWrapper = document.getElementById('VideoWpr'),
        //homeVideo = document.getElementById('homeVideo'),
        //mVideo = document.getElementById('mVideo'),
        userDetails = document.getElementById('userDetails'),
        sidebarCb = document.getElementById('sidebarCb');
        //facebookLogin = document.getElementById('facebookLogin');


    var menu = document.querySelector('aside ul');
    if (menu) {
        highlightPage(menu);
    } else {
        userDetails.addEventListener('click', toggleUserDropdown);
    }
    //#region highlightmenu
    function highlightPage(menu2) {
        var pages = menu2.children,
            url = location.href,
            pageAnchor;
        
        for (var i = 1, l = pages.length; i < l; i++) {
            pageAnchor = pages[i].querySelector('a');
            if (pageAnchor && pageAnchor.href && pageAnchor.href.toLowerCase() === url) {
                pages[i].addClass('currentItem');
                return;
            }
        }

    }
    //#endregion

    //#region events

    //#region toggle menu

    function toggleUserDropdown() {
        var dropdown = this.querySelector('.userMenu');

        if (dropdown.style.display === 'block') {
            dropdown.style.display = 'none';
            return;
        }

        dropdown.style.display = 'block';

    }
    //#endregion

    //function homePageEvents() {
    //    //#region togglevideo

    //    videoWrapper.addEventListener('click', function () {
    //        window.ga('send', 'event', 'Homepage', 'Show Video', 'Clicking on play the video');
    //        mVideo.style.display = 'block';
    //        setTimeout(function () {
    //            videoWrapper.addClass('open');
    //        }, 0);

    //        if (homeVideo.readyState) {
    //            homeVideo.currentTime = 0;
    //        }
    //        setTimeout(function () {
    //            homeVideo.play();
    //        }, 600);

    //    });

    //    mVideo.addEventListener('click', function (e) {
    //        if (e.target.id === 'homeVideo') {
    //            return;
    //        }
    //        mVideo.style.display = 'none';
    //        setTimeout(function () {
    //            videoWrapper.removeClass('open');
    //        }, 0);
    //        //if (homeVideo.readyState) {
    //        //    homeVideo.currentTime = 0;
    //        //}
    //        homeVideo.load();

    //    });

    //    //#endregion
    //}


    //#region menu login
    document.addEventListener('click', function (e) {
        var target = e.target;

        if (target.hasClass('addConnect')) {
            sidebarCb.checked = false;
            connectApi.connect();
            return;
        }

        if (target.hasClass('addRegister')) {
            sidebarCb.checked = false;
            connectApi.register();
            return;
        }
    });
    //#endregion




    //#endregion

})(window.document);