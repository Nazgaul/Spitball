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
                    element[0].className += (' color' + num % 10);
                    //element.addClass('color' + num % 10);
                    //if (attrs.colorOn) {
                    //    element.find(attrs.colorOn).addClass('color' + num % 10);
                    //}
                }
            };
        }
    );
    angular.module('app').directive('colorParent',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    var num = 0, attr = attrs.colorParent;
                    for (var i = 0; i < attr.length; i++) {
                        var temp = parseInt(attr[i], 16);
                        if (!isNaN(temp)) {
                            num += temp;
                        }
                    }
                    //element[0].className += (' colorParent' + num % 10);
                    element.addClass('color-parent' + num % 10);
                    //if (attrs.colorOn) {
                    //    element.find(attrs.colorOn).addClass('color' + num % 10);
                    //}
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
