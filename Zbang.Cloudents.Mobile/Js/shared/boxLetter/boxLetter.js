angular.module('app').directive('boxLetter',
   function () {
       return {
           restrict: 'A',
           link: function (scope, element, attrs) {
               var random = Math.floor(Math.random() * 7) + 1;

               var char = attrs.boxLetter.toUpperCase();                                                
               code = char.charCodeAt(0).toString(),
               sum = 0;
               for (var i = 0; i < code.length; i++) {
                   sum += parseInt(code[i]);
               }
               sum = sum % 8;

               if (sum == 0) {
                   sum = 1;
               }
               element.addClass('color' + sum);
           }
       };
   }
   );