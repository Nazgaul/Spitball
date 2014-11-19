(function (window, document) {
    "use strict";
    var headerSettings = document.getElementsByClassName('headerSettings')[0],
        headerWrapper = document.getElementsByClassName('headerWpr')[0],
        videoWrapper = document.getElementById('VideoWpr'),
        homeVideo = document.getElementById('homeVideo'),
        mVideo = document.getElementById('mVideo'),
        closeMenu = document.querySelector('.sideBar .closeMenu');

    highlightPage();



    //#region highlightmenu
    function highlightPage() {
        var pages = document.querySelector('aside ul').children,
            url = location.href,
            pageAnchor;

        if (location.pathname.toLowerCase() === '/account/') {
            pages[0].className += ' currentItem';
            return;
        }
        for (var i = 1, l = pages.length; i < l; i++) {
            var pageAnchor = pages[i].querySelector('a');
            if (pageAnchor && pageAnchor.href && pageAnchor.href.toLowerCase() === url) {
                pages[i].className += ' currentItem';
                return;
            }
        }

    }
    //#endregion

    //#region events
    //#region toggle menu
    headerSettings.addEventListener('click', function () {
        var className = 'menuOpen';
        var classString = headerWrapper.className, nameIndex = classString.indexOf(className);
        if (nameIndex == -1) {
            classString += ' ' + className;
        }
        else {
            classString = classString.substr(0, nameIndex) + classString.substr(nameIndex + className.length);
        }
        headerWrapper.className = classString;

    });

    closeMenu.addEventListener('click', function () {
        headerWrapper.className = 'headerWpr';
    });

    //#endregion

    //#region togglevideo
    videoWrapper.addEventListener('click', function () {
        cd.analytics.trackEvent('Homepage', 'Show video', 'Clicking on play the video');
        videoWrapper.className += ' open';
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
        videoWrapper.className = 'vidoeWrapper';
        if (homeVideo.readyState) {
            homeVideo.currentTime = 0;
        }
        homeVideo.pause();

    });

    //#endregion


    //$('.currentItem').removeClass('currentItem');
    //var url = location.pathname.toLowerCase() + location.hash;
    //if (url === "/account/") {
    //    $('aside').find('li:first').addClass('currentItem');
    //} else {
    //    $('aside').find('a[href$="' + url + '"]').parent().addClass('currentItem');
    //}
    //#endregion

})(window, window.document);