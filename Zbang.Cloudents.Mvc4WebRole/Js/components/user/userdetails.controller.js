/// <reference path="~/Scripts/angular.js" />
(function () {
    angular.module('app.user.details').controller('UserDetails', userDetails);
    userDetails.$inject = ['accountService', '$scope'];

    function userDetails(accountService, $scope) {
        var ud = this;
        ud.isLoggedIn = false;

        accountService.getDetails().then(function (response) {
            if (response.id) {
                ud.isLoggedIn = true;
            }
            assignValues(response);
        });

        $scope.$on('userDetailsChange', function () {
            accountService.getDetails().then(function (response) {
                assignValues(response);
            });
        });

        function assignValues(response) {
            ud.id = response.id;
            ud.name = response.name;
            ud.image = response.image;
            ud.score = response.score;
        }
    }
})();



