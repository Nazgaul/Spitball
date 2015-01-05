angular.module('app.events', []).
    directive('cursorEnd',
        function () {
            return function (scope, element, attr) {
                element.on('focus', function () {
                    setTimeout(function () {
                        element[0].setSelectionRange(element[0].value.length, element[0].value.length);
                    }, 10);
                });

                scope.$on('destroy', function () {
                    element.off('focus');
                });
            }
        }


    ).directive('dragView',
   function () {

       return {
           restrict: "A",
           link: function (scope, element, attr) {
               var posX;

               element.on('touchstart', function (e) {
                   e.preventDefault();
                   posX = e.touches[0].clientX;
               });
               element.on('touchmove', function (e) {
                   e.preventDefault();
                   var curPosX = e.touches[0].clientX;
                   
                   if (curPosX > posX) {
                       return;
                   }

                   if (Math.abs(curPosX - posX) >= 115) {
                       return;
                   }

                   element.css({ marginLeft: curPosX - posX + 'px' });

               });
               element.on('touchend', function (e) {
                   e.preventDefault();
                   element.css({ marginLeft: 0 });
               });

               scope.$on('$destroy', function () {
                   element.off('touchstart');
                   element.off('tocuhmove');
                   element.off('touchend');
               });

           }
       }
   });
