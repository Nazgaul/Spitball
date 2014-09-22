app.directive('rateStar',
[

   function () {
       function rate(elem, attrs) {
           for (var i = 0; i < attrs.rateStar; i++) {
               elem.children().get(4 - i).classList.add('rate');
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
                   var star = elem[0].querySelectorAll('.rate');
                   for (var i = 0; i < star.length; i++) {
                       star[i].classList.remove('rate');
                   }
               }, function () {
                   rate(elem, attrs);
               });

               $(elem).click(function (e) {
                   var userRate = 0,
                   star = elem.children();
                   for (var i = 0; i < star.length; i++) {
                       if (star[i] === e.target.parentElement) {
                           userRate = 5 - i;
                       }
                   }
                   attrs.rateStar = userRate;
                   scope.rateItemCallback({ t: userRate });
               });

               attrs.$observe('rateStar', function (value) {
                   rate(elem, attrs);
               });

           }
       };
   }
]);