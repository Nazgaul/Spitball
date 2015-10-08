(function () {
    angular.module('app.dashboard').controller('UniversityController', universityMeta);
    universityMeta.$inject = ['dashboardService'];



    function universityMeta(dashboardService) {
        var um = this;

        um.loading = true;
        dashboardService.getUniversityMeta().then(function (response) {
            um.info = response;
            um.loading = false;
        });
    }
})();