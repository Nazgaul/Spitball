angular.module('app').directive('innerScroll',
   [function () {

       return {
           restrict: "A",

           link: function (scope, element, attr) {

               var loader = angular.element('<div style="display:none;">Loading</div>');
               element.append(loader);

               attr.$observe('innerScroll', function () {
                   if (attr.innerScroll === 'true') {
                       loader.css({ display: 'block' });
                       return;
                   }
                   loader.css({ display: 'none' });
               });

               scope.$on('$destroy', function () {
               });
           }
       }
   }]);