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
            this.hidePromo = true;
            this.leaderboard = [];
            this.hideBox = false;
            dashboardService.leaderboard().then(function (response) {
                if (response.type === 1) {
                    if (response.model.length < 3) {
                        _this.hideBox = true;
                    }
                    _this.hideLeaderBoard = false;
                    for (var i = 0; i < response.model.length; i++) {
                        response.model[i].score = $filter("megaNumber")(response.model[i].score);
                    }
                    _this.leaderboard = response.model;
                    console.log(_this.leaderboard);
                }
                else {
                    _this.flashcardPromo = response.model;
                    _this.dashboardService.getUniversityMeta()
                        .then(function (response) {
                        _this.hidePromo = false;
                        _this.color1 = response.btnColor;
                        _this.color2 = response.stripColor;
                    });
                }
            });
        }
        LeaderBoard.prototype.details = function (ev) {
            this.$mdDialog.show({
                templateUrl: "/flashcard/promo/",
                targetEvent: ev,
                clickOutsideToClose: true,
                locals: {
                    color1: this.color1,
                    color2: this.color2
                },
                controller: "DialogPromo",
                controllerAs: "dp",
                fullscreen: false
            });
        };
        LeaderBoard.$inject = ["dashboardService", "$filter", "$mdDialog"];
        return LeaderBoard;
    }());
    var DialogController = (function () {
        function DialogController(color1, color2, $mdDialog) {
            this.color1 = color1;
            this.color2 = color2;
            this.$mdDialog = $mdDialog;
            this.colorA = color1;
            this.colorB = color2;
        }
        DialogController.prototype.close = function () {
            this.$mdDialog.hide();
        };
        DialogController.$inject = ["color1", "color2", "$mdDialog"];
        return DialogController;
    }());
    angular.module("app.dashboard").controller("DialogPromo", DialogController);
    angular.module("app.dashboard").controller("DashboardLeaderboard", LeaderBoard);
})(app || (app = {}));
