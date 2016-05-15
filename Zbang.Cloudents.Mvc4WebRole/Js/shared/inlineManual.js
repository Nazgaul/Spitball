'use strict';
(function () {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout', '$document', 'userDetailsFactory'];
    function inlineManual($rootScope, $timeout, $document, userDetailsFactory) {
        var registeredUser = false;
        userDetailsFactory.init().then(function (userData) {
            if (!userData.id) {
                return;
            }
            registeredUser = true;
            inlineManualTracking = {
                uid: userData.id,
                email: userData.email,
                username: userData.name,
                created: Math.round(userData.createTime.getTime() / 1000)
            };
            $timeout(injectInlineManual, 0, false);
        });
        $rootScope.$on("$stateChangeSuccess", function () {
            $timeout(function () {
                if (registeredUser && angular.isDefined(inline_manual_player)) {
                    //inline_manual_player.manualReinit();
                    //inline_manual_player.setMetadata(inlineManualTracking);

                    inline_manual_player.setCallbacks({
                        onProfileLoad: function (player, topic_id, step_id) {
                            player.setMetadata(inlineManualTracking);
                        }
                    });

                }
            }, 1000);
        });
        function injectInlineManual() {
            $document.ready(function () {
                var e = document.createElement("script"), t = document.getElementsByTagName("script")[0];
                e.async = true, e.src = "https://inlinemanual.com/embed/player.48877e35a515f4d5093914d5e9e51176.js",
                    e.charset = "UTF-8", t.parentNode.insertBefore(e, t);
            createInlineManualPlayer(inlineManualTracking);
            });
        }
    }
})();
