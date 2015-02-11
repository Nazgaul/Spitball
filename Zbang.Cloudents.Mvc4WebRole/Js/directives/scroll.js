angular.module('scroll',[]).directive('scrollPaging',
   ['$window', function ($window) {

       return {
           restrict: "A",
           scope: {
               onScroll: '&'
           },
           link: function (scope, element, attr) {
               var $win = angular.element($window);

               $win.on('scroll', isTriggerFunc);

               scope.$on('$destroy', function () {
                   $win.off('scroll', isTriggerFunc);
               });

               function isTriggerFunc() {
                   var scrollTop = window.pageYOffset,
                       scrollHeight = document.body.scrollHeight,
                       windowHeight = window.innerHeight;

                   if (scrollTop + windowHeight >= scrollHeight * 0.8) {
                       scope.$apply(scope.onScroll);
                   }
               }
           }
       }
   }]);