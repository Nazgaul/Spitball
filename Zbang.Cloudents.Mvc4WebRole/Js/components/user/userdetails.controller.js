/// <reference path="~/Scripts/angular.js" />
(function () {
    angular.module('app.user.details').controller('UserDetailsController', userDetailsController);
    userDetailsController.$inject = ['accountService', '$scope', 'userDetailsFactory'];

    function userDetailsController(accountService, $scope, userDetails) {
        var ud = this;
        ud.isLoggedIn = false;
        userDetails.init().then(function() {
            assignValues(userDetails.get());

            ud.isLoggedIn = userDetails.isAuthenticated();
        });
        $scope.$on('userDetailsChange', function () {
            assignValues(userDetails.get());
        });

        function assignValues(response) {
            ud.id = response.id;
            ud.name = response.name;
            ud.image = response.image;
            ud.score = response.score;
            ud.url = response.url;
        }
    }
})();



