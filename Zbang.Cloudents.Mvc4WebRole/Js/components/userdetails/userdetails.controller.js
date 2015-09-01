/// <reference path="~/Scripts/angular.js" />
(function () {
    angular.module('app.userdetails').controller('UserDetails',userDetails);
    userDetails.$inject = ['ajaxService'];

    function userDetails(ajaxservice) {

        ajaxservice.get('/account/details', null).then(function(response) {
            console.log(response)
        });
        var ud = this;
        ud.name = 'Guy Golan';
        ud.points = 500;
        ud.image = 'http://lorempixel.com/400/200/';
    }
})();