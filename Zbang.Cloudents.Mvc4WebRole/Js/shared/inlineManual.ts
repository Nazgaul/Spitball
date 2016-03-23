/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
declare var inline_manual_player: any;
(() => {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout'];
    function inlineManual($rootScope: ng.IRootScopeService, $timeout: ng.ITimeoutService) {
        $rootScope.$on("$stateChangeSuccess", () => {
            // Inline manual fix for angular
            if (angular.isDefined(inline_manual_player)) {
                $timeout(() => {
                    inline_manual_player.manualReinit();
                }, 1000);
            }
        });
    }


})()