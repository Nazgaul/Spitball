/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
declare var inline_manual_player: any;
(() => {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout', '$document'];
    function inlineManual($rootScope: ng.IRootScopeService,
        $timeout: ng.ITimeoutService,
        $document: ng.IDocumentService) {
        $rootScope.$on("$stateChangeSuccess", () => {
            // Inline manual fix for angular
            var element = $document.find('[ui-view][animation-class]');
            if (angular.isDefined(inline_manual_player)) {
                $timeout(() => {
                    inline_manual_player.manualReinit(element[0]);
                }, 1000);
            }
        });
    }


})()