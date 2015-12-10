(function () {
    angular.module('app').controller('LeaderboardController', leaderboard);

    leaderboard.$inject = ['dashboardService', '$stateParams', 'boxService', '$scope'];

    function leaderboard(dashboardService, $stateParams, boxService, $scope) {
        var self = this;

        self.loading = true;
        self.hide = false;
        if ($stateParams.boxId) {
            if ($stateParams.boxtype.toLowerCase() === 'box') {
                self.hide = true;
            } else {
                boxService.leaderBoard($stateParams.boxId).then(function(response) {
                    self.leaderboard = response;
                    if (!response.length) {
                        self.hide = true;
                        $scope.$emit('hide-leader-board');
                    }
                    self.loading = false;
                });
            }
        }
        else {
            dashboardService.leaderboard().then(function (response) {
                self.leaderboard = response;
                self.loading = false;
            });
        }

       
    }
})()