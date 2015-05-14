app.directive('userCoupon',
   ['sUserDetails', function (sUserDetails) {
       return {
           restrict: "A",
           scope: false,
           link: function (scope, element) {

               element.text(formatUserId(sUserDetails.getDetails().id));

               function formatUserId(userId) {
                   var id = userId.toString();
                   while (id.length < 6) {
                       id = "0" + id;
                   }

                   id = sUserDetails.getDetails().name.toUpperCase()[0] + id;

                   return id;    
               }
                
           }
       }
   }]);