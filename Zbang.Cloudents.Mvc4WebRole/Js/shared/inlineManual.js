/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="userDetails.ts" />
(function () {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout', '$document', 'userDetailsFactory'];
    function inlineManual($rootScope, $timeout, $document, userDetailsFactory) {
        userDetailsFactory.init().then(function (userData) {
            inlineManualTracking = {
                uid: userData.id,
                email: userData.email,
                username: userData.name,
                created: userData.createTime.getTime(),
            };
            !function () { var e = document.createElement("script"), t = document.getElementsByTagName("script")[0]; e.async = true, e.src = "https://inlinemanual.com/embed/player.48877e35a515f4d5093914d5e9e51176.js", e.charset = "UTF-8", t.parentNode.insertBefore(e, t); }();
        });
        $rootScope.$on("$stateChangeSuccess", function () {
            // Inline manual fix for angular
            var element = $document.find('[ui-view][animation-class]');
            if (angular.isDefined(inline_manual_player)) {
                $timeout(function () {
                    inline_manual_player.manualReinit(element[0]);
                }, 1000);
            }
        });
    }
})();
