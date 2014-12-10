
app.directive('scrollToTop',
   ['$rootScope',

   function ($rootScope) {
       "use strict";
       return {
           restrict: "A",
           scope : {
               scrollToTop: '@'
           },
           link: function (scope, elm) {
               var isTop;
               scope.$watch('scrollToTop', function (newValue) {
                   newValue = !!newValue; //to boolean
                   if (!isTop && newValue) {
                       elm.scrollTop(0);
                   }
                   isTop = newValue;
               });

               elm.bind('scroll', function () {
                   if (elm[0].scrollTop !== 0 && isTop) {

                       if (scope.$$phase) {
                           scope.$parent.params.scrollToTop = false;
                           isTop = false;
                           return;
                       }

                       scope.$apply(function () {
                           scope.$parent.params.scrollToTop = false;
                           isTop = false;
                       });
                   }
               });

           }
       };
   }
   ]);
