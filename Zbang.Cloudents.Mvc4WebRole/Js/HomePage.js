window.fbAsyncInit = function () {
    FB.init({
        appId: '450314258355338',
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
    var headerSettings = document.getElementsByClassName('headerSettings')[0],
        headerWrapper = document.getElementsByClassName('headerWpr')[0],
        videoWrapper = document.getElementById('VideoWpr'),
        homeVideo = document.getElementById('homeVideo'),     
        mVideo = document.getElementById('mVideo'),
        closeMenu = document.querySelector('.sideBar .closeMenu'),
        facebookLogin = document.getElementById('facebookLogin');

    var isHomePage = location.pathname.toLowerCase().indexOf('/account') > -1;
    highlightPage();


    if (isHomePage) {
        homeVideo.load();
        homePageEvents();
    }

    //#region highlightmenu
    function highlightPage() {
        var pages = document.querySelector('aside ul').children,
            url = location.href,
            pageAnchor;

        if (isHomePage) {
            pages[0].addClass('currentItem');
            return;
        }
        for (var i = 1, l = pages.length; i < l; i++) {
            var pageAnchor = pages[i].querySelector('a');
            if (pageAnchor && pageAnchor.href && pageAnchor.href.toLowerCase() === url) {
                pages[i].addClass('currentItem');
                return;
            }
        }

    }
    //#endregion

    //#region events

    //#region toggle menu
    headerSettings.addEventListener('click', toggleMenu);

    closeMenu.addEventListener('click', toggleMenu);

    function toggleMenu() {
        headerWrapper.toggleClass('menuOpen');
    };
    //#endregion

    function homePageEvents() {
        //#region togglevideo

        videoWrapper.addEventListener('click', function () {
            cd.analytics.trackEvent('Homepage', 'Show video', 'Clicking on play the video');
            mVideo.style.display = 'block';
            setTimeout(function () {
                videoWrapper.addClass('open');
            }, 0);

            if (homeVideo.readyState) {
                homeVideo.currentTime = 0;
            }
            setTimeout(function () {
                homeVideo.play();
            }, 600);

        });

        mVideo.addEventListener('click', function (e) {
            if (e.target.id === 'homeVideo') {
                return;
            }
            mVideo.style.display = 'none';
            setTimeout(function () {
                videoWrapper.removeClass('open');
            }, 0);
            //if (homeVideo.readyState) {
            //    homeVideo.currentTime = 0;
            //}
            homeVideo.load();

        });

        //#endregion

        //#region login
      
        facebookLogin.addEventListener('click', function (e) {
            connectApi.connectFb();
        });
        //#endregion
    }
    
  
    //#region menu login
    document.addEventListener('click', function (e) {
        var target = e.target;

        if (target.hasClass('addConnect')) {
            headerWrapper.removeClass('menuOpen');
            connectApi.connect();
            return;
        }

        if (target.hasClass('addRegister')) {
            headerWrapper.removeClass('menuOpen');
            connectApi.register();
            return;
        }
    });
    //#endregion
  

   

    //#endregion

})(window.document);