//setTimeout(function () {
//    !function () {
//        var e = document.createElement("script"),
//            t = document.getElementsByTagName("script")[0];
//        e.async = 1,
//        e.src = "https://inlinemanual.com/embed/player.48877e35a515f4d5093914d5e9e51176.js",
//        e.charset = "UTF-8", t.parentNode.insertBefore(e, t)
//    }();

//    inlineManualTracking = {
//        uid: window.id,
//        username: "inlinemanual inlinemanual"
//    }
//}, 1000);

//(function () {
//    angular.module('app').run(inlineManual);
//    inlineManual.$inject = ['$rootScope','$timeout'];
//    function inlineManual($rootScope, $timeout) {
//        $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
//            if (angular.isDefined(window.inline_manual_player)) {
//                $timeout(function () {
//                    console.log('reiniting');
//                    inline_manual_player.getProfile();
//                    inline_manual_player.setCallbacks({
//                        onProfileLoad: function (player, topic_id, step_id) {
//                            console.log('onProfileLoad');
//                            inlineManualTracking = {
//                                uid: window.id,
//                                username: "inlinemanual inlinemanual"
//                            }
//                        }
//                    });
//                }, 1500);
//            }
//        });
//    }
//})();