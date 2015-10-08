(function () {
    angular.module('app').controller('LeaderboardController', leaderboard);

    leaderboard.$inject = ['dashboardService', '$stateParams', 'boxService'];

    function leaderboard(dashboardService, $stateParams, boxService) {
        var self = this;

        self.loading = true;

        if ($stateParams.boxId) {
            boxService.leaderBoard($stateParams.boxId).then(function (response) {
                self.leaderboard = response;
                self.loading = false;
            });
        }
        else {
            dashboardService.leaderboard().then(function (response) {
                self.leaderboard = response;
                self.loading = false;
            });
        }
    }
})()