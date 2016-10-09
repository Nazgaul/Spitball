(function () {
    'use strict';
    angular.module('app').directive('menuAd', menuAd);
    menuAd.$inject = ['$compile', '$timeout'];

    function menuAd($compile, $timeout) {
        return {
            restrict: 'E',
            link: function (scope, elem) {
                var adHtml = '<li class="ad nav-item" hide-xs data-ng-dfp-ad-container>' +
                '<div ng-if="app.showBoxAd" data-ng-dfp-ad="div-gpt-ad-1461243129238-1" data-ng-dfp-ad-hide-when-empty></div>' +
                '<div ng-if="!app.showBoxAd" data-ng-dfp-ad="div-gpt-ad-1461243129238-2" data-ng-dfp-ad-hide-when-empty></div>' +
                '</li>'
                var adElem = angular.element(adHtml);

                function handleAd() {
                    var el = $compile(adElem)(scope);
                    elem.after(el);
                    elem.remove();
                }

                $timeout(function () {
                    handleAd();
                });
            }
        };
    }
})();