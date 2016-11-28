var app;
(function (app) {
    "use strict";
    var LeaderBoard = (function () {
        function LeaderBoard(dashboardService, $filter, $mdDialog) {
            var _this = this;
            this.dashboardService = dashboardService;
            this.$filter = $filter;
            this.$mdDialog = $mdDialog;
            this.hideLeaderBoard = true;
            this.hidePromo = false;
            this.leaderboard = [];
            this.hideBox = false;
            dashboardService.leaderboard().then(function (response) {
                if (response.type === 1) {
                    _this.hidePromo = true;
                    if (response.model.length < 3) {
                    }
                    _this.hideLeaderBoard = false;
                    for (var i = 0; i < response.model.length; i++) {
                        response.model[i].score = $filter("megaNumber")(response.model[i].score);
                    }
                    _this.leaderboard = response.model;
                    console.log(_this.leaderboard);
                }
                else {
                    _this.hideLeaderBoard = true;
                    _this.flashcardPromo = response.model;
                    console.log(_this.flashcardPromo);
                }
                _this.hideBox = _this.hideLeaderBoard && _this.hidePromo;
            });
        }
        LeaderBoard.prototype.details = function (ev) {
            this.$mdDialog.show({
                templateUrl: "/flashcard/promo/",
                targetEvent: ev,
                clickOutsideToClose: true,
                fullscreen: false
            });
            console.log("here");
        };
        LeaderBoard.$inject = ["dashboardService", "$filter", "$mdDialog"];
        return LeaderBoard;
    }());
    angular.module("app.dashboard").controller("DashboardLeaderboard", LeaderBoard);
})(app || (app = {}));
