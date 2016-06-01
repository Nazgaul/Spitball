//setTimeout(function () {
//    !function () {
//        var e = document.createElement("script"),
//            t = document.getElementsByTagName("script")[0];
//        e.async = 1,
//        e.src = "https://inlinemanual.com/embed/player.48877e35a515f4d5093914d5e9e51176.js",
//        e.charset = "UTF-8", t.parentNode.insertBefore(e, t)
//    }();

//    inlineManualTracking = {
//        //uid: window.id,
//        username: "inlinemanual inlinemanual"
//    }
//    console.log('here');

//}, 1000);


//'use strict';
//(function () {
//    angular.module('app').run(inlineManual);
//    inlineManual.$inject = ['$rootScope'];
//    function inlineManual($rootScope) {
//        $rootScope.$on("$stateChangeSuccess", function () {
//            if (angular.isDefined(window.inline_manual_player)) {
//                setTimeout(function () {
//                    console.log('reiniting');
//                    inline_manual_player.manualReinit();
//                }, 1500);
//            }
//        });
//    }
//})();