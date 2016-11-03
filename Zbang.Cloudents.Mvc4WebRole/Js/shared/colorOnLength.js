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
    angular.module('app').directive('color',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    var num = 0, attr = attrs.color;
                    for (var i = 0; i < attr.length; i++) {
                        var temp = parseInt(attr[i], 16);
                        if (!isNaN(temp)) {
                            num += temp;
                        }
                    }
                    element.addClass('color' + num % 10);
                    if (attrs.colorOn) {
                        element.find(attrs.colorOn).addClass('color' + num % 10);
                    }
                    //var x = parseInt(attrs.dColor, 10);
                    //if (!isNaN(x)) {
                    //    element.addClass('color' + x % 10);
                    //} else {
                    //    element.addClass('color' + attrs.dColor.length % 10);
                    //}
                }
            };
        }
    );

    //angular.module('app').directive('colorScope',
    //    function () {
    //        return {
    //            scope: {
    //                colorScope: '='
    //            },
    //            restrict: 'A',
    //            link: function (scope, element) {
    //                scope.$watch("colorScope",
    //                    function (newVal, oldVal) {
    //                        if (newVal === oldVal) {
    //                            return;
    //                        }
    //                        if (oldVal) {
    //                            assignColor(newVal, oldVal);
    //                        }
    //                    });
    //                assignColor(scope.dColorScope);
    //                function assignColor(num, oldVal) {
    //                    element.removeClass("color" + calculatenum(oldVal) % 10).addClass('color' + calculatenum(num) % 10);
    //                }
    //                function calculatenum(guid) {
    //                    var num = 0;
    //                    for (var i = 0; i < guid.length; i++) {
    //                        var temp = parseInt(guid[i], 16);
    //                        if (!isNaN(temp)) {
    //                            num += temp;
    //                        }
    //                    }
    //                    return num;

    //                }
    //            }
                
    //        };
    //    }
    //);
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
