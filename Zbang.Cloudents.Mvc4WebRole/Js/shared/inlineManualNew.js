
!function () {
    var e = document.createElement("script"),
        t = document.getElementsByTagName("script")[0];
    e.async = 1,
        e.src = "https://inlinemanual.com/embed/player.48877e35a515f4d5093914d5e9e51176.bare.js",
        e.charset = "UTF-8", t.parentNode.insertBefore(e, t)
}();


(function () {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', 'userDetailsFactory'];
    function inlineManual($rootScope, userDetailsFactory) {
        userDetailsFactory.init().then(function (userData) {
            if (window.id < 0) {
                return;
            }
            var interval = window.setInterval(function () {
                if (window.createInlineManualPlayer !== undefined) {
                    inlineManualTracking = {
                        uid: userData.id,
                        email: userData.email,
                        username: userData.name,
                        created: Math.round(userData.createTime.getTime() / 1000)
                    };
                    inlineManualOptions = {
                        language: userData.culture.substring(0, 2) === 'he' ? 'he' : 'en'
                    };
                    createInlineManualPlayer(inlineManualPlayerData);
                    inline_manual_player.setCallbacks({
                        onProfileLoad: function (/*topic_id, step_id*/) {
                            inline_manual_player.setMetadata(inlineManualTracking);
                        }
                    });
                    window.clearInterval(interval);
                }
            }, 20);
        });


        $rootScope.$on("$stateChangeSuccess", function () {
            if (angular.isDefined(window.inline_manual_player)) {
                inline_manual_player.getProfile();
            }
        });
    }
})();