/// <reference path="~/Scripts/angular.js" />
(function () {
    angular.module('app.user.details').controller('UserDetails', userDetails);
    userDetails.$inject = ['userDetailsService'];

    function userDetails(userDetailsService) {
        var ud = this;
        ud.isLoggedIn = false;

        userDetailsService.getDetails().then(function (response) {
            if (response.id) {
                ud.isLoggedIn = true;
            }
            ud.id = response.id;
            ud.name = response.name;
            ud.image = response.image;
            ud.score = response.score;
        });

        //ud.name = 'Guy Golan';
        //ud.points = 500;
        //ud.image = 'http://lorempixel.com/400/200/';
    }
})();



(function () {
    angular.module('app.user').service('userDetailsService', userDetails);
    userDetails.$inject = ['ajaxService','$q'];

    function userDetails(ajaxservice,$q) {
        var ud = this;

        var serverCall = false;
        var defer = $q.defer();

        ud.getDetails = function () {
            if (ud.details) {
                defer.resolve(ud.details);
                return defer.promise;
            }
            if (!serverCall) {
                serverCall = true;
                ajaxservice.get('/account/details/', null, 1800000).then(function (response) {
                    serverCall = false;
                    ud.details = response;
                    defer.resolve(ud.details);
                });
            }
            return defer.promise;
        }

        ud.getAccountDetails = function() {
            return ajaxservice.get('/account/settingsdata/', null, 1800000);
        }
    }
})();