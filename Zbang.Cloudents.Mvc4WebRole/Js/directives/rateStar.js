
app.directive('rateStar',
[

   function () {
       "use strict";
       var cRate = 'rate';
       function rate(elem, attrs) {
           var children = elem.children();
           for (var i = 0; i < attrs.rateStar; i++) {
               var star = children.get(4 - i);
               if (star.classList) {
                   star.classList.add(cRate);
               } else {
                   $(star).attr('class', function (index, classNames) {
                       return classNames + ' ' + cRate;
                   });
               }
               
               
           }
       };
       
       return {
           scope: {
               rateItemCallback: '&rateItemCallback'
           },
           restrict: "A",
           link: function (scope, elem, attrs) {
               rate(elem, attrs);

               $(elem).hover(function () {
                   var star = elem[0].querySelectorAll('.' + cRate);
                   for (var i = 0; i < star.length; i++) {
                       if (star[i].classList) {
                           star[i].classList.remove(cRate);
                       } else {
                           $(star[i]).attr('class', function (index, classNames) {
                               return classNames.replace(cRate, '');
                           });
                       }
                   }
               }, function () {
                   rate(elem, attrs);
               });

               $(elem).click(function (e) {
                   var target = e.target;
                   if (!target.tagName) { //fix for safari and maybe ie
                       target = target.correspondingUseElement;
                   }
                   var userRate = 0,
                   star = elem.children();
                   for (var i = 0; i < star.length; i++) {
                       while (target.tagName.toLowerCase() !== 'svg') {
                           target = target.parentNode;
                       }
                       if (star[i] === target) {
                           userRate = 5 - i;
                       }
                   }
                   attrs.rateStar = userRate;
                   scope.rateItemCallback({ t: userRate });
               });

               attrs.$observe('rateStar', function () {
                   rate(elem, attrs);
               });

           }
       };
   }
]);