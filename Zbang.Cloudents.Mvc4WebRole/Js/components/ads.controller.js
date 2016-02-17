﻿(function() {
    angular.module('app').controller('adController', ads);
    ads.$inject = ['ajaxService', 'userDetailsFactory'];

    function ads(ajaxService, userDetailsFactory) {
        var ad = this;
        ad.show = true;
        if (!userDetailsFactory.isAuthenticated()) {
            ad.show = false;
            return;
        }
        if (userDetailsFactory.get().university.country.toLowerCase() !== 'il') {
            ad.show = false;
            return;
        }
        ajaxService.get('home/studentad').then(function (response) {
            if (!response) {
                ad.show = false;
            }
            console.log(response);
            ad.content = response;
        });
    }

})();