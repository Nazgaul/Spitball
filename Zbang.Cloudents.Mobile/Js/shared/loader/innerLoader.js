﻿angular.module('app').directive('innerScroll',
   [function () {

       return {
           restrict: "A",

           link: function (scope, element, attr) {

               var loader = angular.element('<svg class="svg-loading"><use xlink:href="#loading" /></svg>');
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