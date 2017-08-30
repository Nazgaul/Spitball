app.directive('letter',
   function () {
       return {
           restrict: 'A',
           link: function (scope, element, attrs) {
     
               attrs.$observe('letter', function () {
                   if (_.isEmpty(attrs.letter)) {
                       return;
                   }

                   set();
               });

               function set() {
                   var char = attrs.letter.toUpperCase();
                   code = char.charCodeAt(0).toString(),
                   sum = 0;
                   for (var i = 0; i < code.length; i++) {
                       sum += parseInt(code[i]);
                   }
                   sum = sum % 8;

                   if (sum == 0) {
                       sum = 1;
                   }

                   var c = 'color' + sum;
                   var hebrewChars = new RegExp("^[\u0590-\u05FF]+$");
                   if (hebrewChars.test(char)) {
                       c += ' heb';
                   }
                   element.addClass(c);
               }
           }
       };
   }
   );