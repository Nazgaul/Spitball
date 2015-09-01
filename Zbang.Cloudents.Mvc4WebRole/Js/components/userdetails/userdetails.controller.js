/// <reference path="~/Scripts/angular.js" />
(function () {
    angular.module('app.userdetails').controller('UserDetails', userDetails);
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
    angular.module('app.userdetails').service('userDetailsService', userDetails);
    userDetails.$inject = ['ajaxService'];

    function userDetails(ajaxservice) {
        var ud = this;
        ud.isLoggedIn = false;
        ud.getDetails = function () {
            return ajaxservice.get('/account/details/', null, 1800000);
        }

        //ud.name = 'Guy Golan';
        //ud.points = 500;
        //ud.image = 'http://lorempixel.com/400/200/';
    }
})();