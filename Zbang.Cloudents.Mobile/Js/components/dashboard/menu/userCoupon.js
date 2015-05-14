angular.module('dashboard').directive('userCoupon',
   ['userDetails', function (userDetails) {
       return {
           restrict: "A",
           scope: false,
           link: function (scope, element) {

               element.text(formatUserId(userDetails.getId()));

               function formatUserId(userId) {
                   var id = userId.toString();
                   while (id.length < 6) {
                       id = "0" + id;
                   }
                   return id;

               }
                
           }
       }
   }]);