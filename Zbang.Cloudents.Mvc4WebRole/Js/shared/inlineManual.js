/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="userDetails.ts" />
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
        });
        $rootScope.$on("$stateChangeSuccess", function () {
            // Inline manual fix for angular
            //var element = $document.find('[ui-view][animation-class]');
        });
        function injectInlineManual() {
            $document.ready(function () {
                var e = document.createElement("script"), t = document.getElementsByTagName("script")[0];
                e.async = true, e.src = "https://inlinemanual.com/embed/player.48877e35a515f4d5093914d5e9e51176.js",
                    e.charset = "UTF-8", t.parentNode.insertBefore(e, t);
            });
        }
    }
})();
