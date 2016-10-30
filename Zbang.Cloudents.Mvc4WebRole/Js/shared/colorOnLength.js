(function () {
    'use strict';
    angular.module('app').directive('dColor',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {

                    if (angular.isNumber(attrs.dColor)) {
                        element.addClass('color' + attrs.dColor % 10);
                    } else {
                        element.addClass('color' + attrs.dColor.length % 10);
                    }
                }
            };
        }
    );
})();

(function() {
    angular.module("app")
        .directive("placeholderMobile", placeholderMobile);
    placeholderMobile.$inject = ["$mdMedia"];
    
    function placeholderMobile($mdMedia) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                if ($mdMedia('xs')) {
                    element.attr("placeholder", attrs.placeholderMobile);
                }
            }
        };
    };
})();
