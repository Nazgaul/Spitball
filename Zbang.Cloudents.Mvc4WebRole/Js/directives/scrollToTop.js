app.directive('scrollToTop',
   ['$rootScope',

   function ($rootScope) {
       return {
           restrict: "A",
           scope : {
               scrollToTop: '@'
           },
           link: function (scope, elm, attr) {
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
                       scope.$apply(function () {
                           //we should use $parse service here
                           scope.$parent.params.scrollToTop = false;
                           isTop = false;
                       });
                   }
               });

           }
       };
   }
   ]);
