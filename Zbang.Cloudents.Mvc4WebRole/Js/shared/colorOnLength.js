(function () {
    'use strict';
    angular.module('app').directive('dColor',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    
                    var x = parseInt(attrs.dColor, 10);
                    if (!isNaN(x)){
                        element.addClass('color' + x % 10);
                    } else {
                        element.addClass('color' + attrs.dColor.length % 10);
                    }
                }
            };
        }
    );
    angular.module('app').directive('dColorScope',
        function () {
            return {
                scope: {
                    dColorScope: '='
                },
                restrict: 'A',
                link: function (scope, element) {
                    scope.$watch("dColorScope",
                        function (newVal, oldVal) {
                            if (newVal === oldVal) {
                                return;
                            }
                            if (oldVal) {
                                assignColor(newVal, oldVal);
                            }
                        });
                    assignColor(scope.dColorScope);
                    function assignColor(num,oldVal) {
                        element.removeClass("color" + oldVal % 10).addClass('color' + num % 10);
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
