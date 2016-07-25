﻿'use strict';
(function () {
    angular.module('app.dashboard').controller('UniversityController', universityMeta);
    universityMeta.$inject = ['dashboardService'];



    function universityMeta(dashboardService) {
        var um = this;
        dashboardService.getUniversityMeta().then(function (response) {
            if (!response.logo) {
                response.logo = 'https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/universityEmptyState.png';
            }
            um.info = response;
        });
    }
})();