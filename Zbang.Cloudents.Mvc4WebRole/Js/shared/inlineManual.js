/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout'];
    function inlineManual($rootScope, $timeout) {
        $rootScope.$on("$stateChangeSuccess", function () {
            // Inline manual fix for angular
            if (angular.isDefined(inline_manual_player)) {
                $timeout(function () {
                    inline_manual_player.manualReinit();
                }, 1000);
            }
        });
    }
})();
