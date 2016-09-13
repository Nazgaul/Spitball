(function () {
    'use strict';
    angular.module('app.dashboard').controller('UniversityController', universityMeta);
    universityMeta.$inject = ['dashboardService', 'itemThumbnailService'];



    function universityMeta(dashboardService, itemThumbnailService) {
        var um = this;
        um.info = {};
        um.loaded = false;
        dashboardService.getUniversityMeta().then(function (response) {
            if (!response.logo) {
                response.logo = itemThumbnailService.logo;
            }
            um.loaded = true;
            um.info = response;

           
        });
    }
})();