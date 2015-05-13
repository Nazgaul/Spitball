angular.module('dashboard').directive('userCoupon',
   ['userDetails', function (userDetails) {
       return {
           restrict: "A",
           scope: false,
           link: function (scope, element, attr) {

               element.text(formatUserId(userDetails.getId()))

               function formatUserId(userId) {
                   var id = userId.toString();                   
                   switch(id.length) {
                       case 1:
                           return "00000" + id;
                       case 2:
                           return "0000" + id;
                       case 3:
                           return "000" + id;
                       case 4:
                           return "00" + id;
                       case 5:
                           return  "0" + id;
                       default:
                           return id;
                   }

               }
                
           }
       }
   }]);