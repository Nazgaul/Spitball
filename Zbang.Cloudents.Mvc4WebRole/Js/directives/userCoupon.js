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
                   return id;
                   //switch(id.length) {
                   //    case 1:
                   //        return "00000" + id;
                   //    case 2:
                   //        return "0000" + id;
                   //    case 3:
                   //        return "000" + id;
                   //    case 4:
                   //        return "00" + id;
                   //    case 5:
                   //        return  "0" + id;
                   //    default:
                   //        return id;
                   //}

               }
                
           }
       }
   }]);