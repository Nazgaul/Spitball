(function () {
    angular.module('app').controller('LeaderboardController', leaderboard);

    leaderboard.$inject = ['dashboardService', '$stateParams', 'boxService'];

    function leaderboard(dashboardService, $stateParams, boxService) {
        var self = this;


        if ($stateParams.boxId) {
            boxService.leaderBoard($stateParams.boxId).then(function (response) {
                self.leaderboard = response;
            });
        }
        else {
            dashboardService.leaderboard().then(function (response) {
                self.leaderboard = response;
            });
        }
    }
})()