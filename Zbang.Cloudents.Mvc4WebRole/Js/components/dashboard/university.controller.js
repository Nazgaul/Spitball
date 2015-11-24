(function () {
    angular.module('app.dashboard').controller('UniversityController', universityMeta);
    universityMeta.$inject = ['dashboardService'];



    function universityMeta(dashboardService) {
        var um = this;

        um.loading = true;
        dashboardService.getUniversityMeta().then(function (response) {
            if (!response.img) {
                response.img = 'https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/universityEmptyState.png';
            }
            um.info = response;
            um.loading = false;
        });
    }
})();