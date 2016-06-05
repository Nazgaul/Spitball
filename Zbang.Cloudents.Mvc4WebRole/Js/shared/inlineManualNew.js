setTimeout(function () {
    inlineManualTracking = {
        uid: window.id,
        username: "inlinemanual inlinemanual"
    };
    inlineManualOptions = {
        language: $('html').attr('lang')
    };
    !function () {
        var e = document.createElement("script"),
            t = document.getElementsByTagName("script")[0];
        e.async = 1,
        e.src = "https://inlinemanual.com/embed/player.48877e35a515f4d5093914d5e9e51176.bare.js",
        e.charset = "UTF-8", t.parentNode.insertBefore(e, t),
        e.onload = function () {
            createInlineManualPlayer(inlineManualPlayerData);

            inline_manual_player.setCallbacks({
                onProfileLoad: function (topic_id, step_id) {
                    inline_manual_player.setMetadata(inlineManualTracking);
                }
            });
        }
    }();

}, 1000);

(function () {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout'];
    function inlineManual($rootScope, $timeout) {
        $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
            if (angular.isDefined(window.inline_manual_player)) {
                inline_manual_player.getProfile();
            }
        });
    }
})();