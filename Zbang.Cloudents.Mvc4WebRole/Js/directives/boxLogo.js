app.directive('boxLogo',
   ['$filter', 'sVerChecker', function ($filter, sVerChecker) {
       var template = '<svg class="svg-{0} color{1}"><use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href="/images/boxicons.svg?{2}#{0}"></use></svg>';
       return {
           restrict: 'E',
           scope: {
               name: '=',
               type: '='
           },
           link: function (scope, element, attrs) {

               var code, icon, color, sum = 0, version = sVerChecker.currentVersion();               

               //class
               var length = scope.name.length;
               color = normalize(length % 8);

               if (scope.type === 'box') {
                   icon = 'privateBox';
                   append();
                   return;
               }

               //svg
               for (var i = 0; i < scope.name.length; i++) {
                   code = scope.name.charCodeAt(i).toString();
                   sum += parseInt(code);
               }

               icon = 'icon' + normalize(sum % 10);

               
               append();

               function normalize(sum) {
                   if (sum == 0) {
                       return 1;
                   }

                   return sum;
               }

               function append() {
                   var formatted = $filter('stringFormat')(template, [icon, color, version]);
                   angular.element(formatted);
                   element.after(formatted);
                   element.remove();
               }
           }
       };
   }
   ]);