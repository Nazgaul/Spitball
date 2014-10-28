"use strict";
app.directive('userDetails', ['sUserDetails', function (sUserDetails) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.text(sUserDetails.getDetails()[attrs.userDetails]);
        }
    };
}]);
