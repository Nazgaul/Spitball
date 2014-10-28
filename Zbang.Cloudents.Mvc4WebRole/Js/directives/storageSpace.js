﻿"use strict";
app.directive('storageSpace',
   [
   function () {
       return {
           restrict: "A",           
           link: function (scope,elem,attrs) {
               var percent = elem.width() / 100;
               attrs.$observe('usedSpace', function (value) {
                   elem.find('.your').width(value  * percent);
               });
               
           }
       };
   }
   ]);