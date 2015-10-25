/// <reference path="~/Scripts/angular.js" />
(function () {
    angular.module('app.user.details').controller('UserDetails', userDetailsController);
    userDetailsController.$inject = ['accountService', '$scope', 'userDetails'];

    function userDetailsController(accountService, $scope, userDetails) {
        var ud = this;
        ud.isLoggedIn = false;

        userDetails.get().then(function (response) {
            assignValues(response);
        });
        userDetails.isAuthenticated().then(function(response) {
            ud.isLoggedIn = response;
        });

        $scope.$on('userDetailsChange', function () {
            userDetails.get().then(function (response) {
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



