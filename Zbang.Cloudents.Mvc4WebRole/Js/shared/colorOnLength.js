(function () {
    'use strict';
    angular.module('app').directive('dColor',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    var length = attrs.dColor % 10;
                    element.addClass('color' + length);
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
