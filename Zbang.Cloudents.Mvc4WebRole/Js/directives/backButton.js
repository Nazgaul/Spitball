"use strict";
app.directive('backButton',
   ['$rootScope','$location',

   function ($rootScope, $location) {
       return {
           restrict: "A",
           link: function () {
               $location.path($rootScope.previousUrl);
           }
       };
   }
   ]);