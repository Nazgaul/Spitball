
app.directive('backButton',
   ['$rootScope','$location',

   function ($rootScope, $location) {
       "use strict";
       return {
           restrict: "A",
           link: function () {
               $location.path($rootScope.previousUrl);
           }
       };
   }
   ]);