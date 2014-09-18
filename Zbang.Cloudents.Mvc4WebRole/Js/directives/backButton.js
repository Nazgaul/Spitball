app.directive('backButton',
   ['$rootScope',

   function ($rootScope) {
       return {
           restrict: "A",
           link: function (scope, elem, attrs) {
               $location.path($rootScope.previousUrl);
           }
       };
   }
   ]);





