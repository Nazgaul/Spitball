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



//'use strict';
//(function () {
//    angular.module('app').directive('loadSvg',
//        function () {
//            return {
//                restrict: 'A',
//                link: function (scope, element, attrs) {
//                    svg4everybody(element[0]);
//                    //var length = attrs.dColor % 10;
//                    //element.addClass('color' + length);
//                }
//            };
//        }
//    );
//})();




