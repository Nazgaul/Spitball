angular.module('app').directive('innerScroll',
   [function () {

       return {
           restrict: "A",

           link: function (scope, element, attr) {

               var loader = angular.element('<svg class="svg-loading"><use xlink:href="#loading" /></svg>');
               element.append(loader);

               attr.$observe('innerScroll', function () {
                   if (attr.innerScroll === 'true') {
                       loader.css({ display: 'block' });
                       if (window.pageYOffset > 0) {
                           loader.css({ position: 'fixed', top: 0 });
                       }
                       return;
                   }
                   //loader.css({ display: 'none' });
               });

               scope.$on('$destroy', function () {
               });
           }
       }
   }]);