app.directive('userDetails', ['sUserDetails', function (sUserDetails) {
    "use strict";
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.text(sUserDetails.getDetails()[attrs.userDetails]);
        }
    };
}]);

app.directive('showBanner', function () {
    "use strict";
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var removeCloudentsBanner = 'removeCloudentsBanner';
            element.on('click','.closeButton', function() {
                element.remove();
                localStorage.setItem(removeCloudentsBanner, '1');
            });

            var willShow = localStorage.getItem(removeCloudentsBanner);
            if (willShow == null) {
                element.addClass('moveToSpitBallShow');
            }
        }
    };
});
