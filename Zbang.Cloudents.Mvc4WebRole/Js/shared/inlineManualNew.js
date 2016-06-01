setTimeout(function () {
    !function () {
        var e = document.createElement("script"),
            t = document.getElementsByTagName("script")[0];
        e.async = 1,
        e.src = "https://inlinemanual.com/embed/player.48877e35a515f4d5093914d5e9e51176.js",
        e.charset = "UTF-8", t.parentNode.insertBefore(e, t)
    }();

    inlineManualTracking = {
        uid: window.id
    }
}, 1000);


//'use strict';
//(function () {
//    angular.module('app').run(inlineManual);
//    inlineManual.$inject = ['$rootScope', '$timeout'];
//    function inlineManual($rootScope, $timeout) {
//        inlineManualTracking = {
//            username: "inlinemanual inlinemanual"
//        }

//        if (angular.isDefined(inline_manual_player)) {
//            inline_manual_player.manualReinit();
//        }
//    }
//})();