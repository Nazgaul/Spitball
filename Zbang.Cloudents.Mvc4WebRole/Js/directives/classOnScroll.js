app.directive('classOnScroll',
   [

   function () {
       return {
           restrict: "A",
           link: function (scope, elem, attrs) {
               console.log(attrs.classOnScroll);

               elem.bind('scroll', function () {
                   if (elem[0].scrollTop) {
                       elem.addClass(attrs.classOnScroll);
                   } else {
                       elem.removeClass(attrs.classOnScroll);
                   }

               });
           }
       };
   }
   ]);