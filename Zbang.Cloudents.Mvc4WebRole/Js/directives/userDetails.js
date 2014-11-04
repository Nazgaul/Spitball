
app.directive('userDetails', ['sUserDetails', function (sUserDetails) {
    "use strict";
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.text(sUserDetails.getDetails()[attrs.userDetails]);
        }
    };
}]);
