angular.module('dashboard').directive('boxLetter',
   function () {
       return {
           restrict: 'A',
           link: function (scope, element, attrs) {
               var random = Math.floor(Math.random() * 7) + 1;

               var char;
               if (scope.box) {
                   char = scope.box.name[0].toUpperCase();
               } else if (scope.recommendedBox) {
                   char = scope.recommendedBox.name[0].toUpperCase();
               }
               code = char.charCodeAt(0).toString(),
               sum = 0;
               for (var i = 0; i < code.length; i++) {
                   sum += parseInt(code[i]);
               }
               sum = sum % 8;

               if (sum == 0) {
                   sum = 1;
               }

               attrs.$set('dataBoxLetter', char);
               element.addClass('color' + sum);
           }
       };
   }
   );