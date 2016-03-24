/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    angular.module('app').run(inlineManual);
    inlineManual.$inject = ['$rootScope', '$timeout', '$document'];
    function inlineManual($rootScope, $timeout, $document) {
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
//# sourceMappingURL=inlineManual.js.map