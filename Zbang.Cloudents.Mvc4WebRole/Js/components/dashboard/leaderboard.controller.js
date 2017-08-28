"use strict";
var app;
(function (app) {
    "use strict";
    var LeaderBoard = (function () {
        function LeaderBoard(dashboardService, $filter, //meganumber is need defined
            $mdDialog) {
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
                    //this.hidePromo = true;
                    if (response.model.length < 3) {
                        _this.hideBox = true;
                        //this.hideLeaderBoard = true;
                    }
                    _this.hideLeaderBoard = false;
                    for (var i = 0; i < response.model.length; i++) {
                        response.model[i].score = $filter("megaNumber")(response.model[i].score);
                    }
                    _this.leaderboard = response.model;
                }
                else {
                    //this.hideLeaderBoard = true;
                    _this.flashcardPromo = response.model || {};
                    _this.flashcardPromo.count = _this.flashcardPromo.count || 0;
                    _this.dashboardService.getUniversityMeta()
                        .then(function (response2) {
                        _this.hidePromo = false;
                        _this.color1 = response2.btnColor;
                        _this.color2 = response2.stripColor;
                        _this.university = response2.name;
                    });
                }
                //                this.hideBox = this.hideLeaderBoard && this.hidePromo;
            });
        }
        LeaderBoard.prototype.details = function (ev) {
            this.$mdDialog.show({
                templateUrl: "/flashcard/promo/",
                targetEvent: ev,
                clickOutsideToClose: true,
                locals: {
                    color1: this.color1,
                    color2: this.color2,
                    university: this.university
                },
                controller: "DialogPromo",
                controllerAs: "dp",
                //controller: ["color1","color2",function(color1, color2) {
                //}],
                fullscreen: false // Only for -xs, -sm breakpoints.
            });
        };
        LeaderBoard.$inject = ["dashboardService", "$filter", "$mdDialog"];
        return LeaderBoard;
    }());
    var DialogController = (function () {
        function DialogController(color1, color2, university, $mdDialog) {
            this.color1 = color1;
            this.color2 = color2;
            this.university = university;
            this.$mdDialog = $mdDialog;
            this.colorA = color1;
            this.colorB = color2;
            this.universityName = university;
        }
        DialogController.prototype.close = function () {
            this.$mdDialog.hide();
        };
        DialogController.$inject = ["color1", "color2", "university", "$mdDialog"];
        return DialogController;
    }());
    angular.module("app.dashboard").controller("DialogPromo", DialogController);
    angular.module("app.dashboard").controller("DashboardLeaderboard", LeaderBoard);
})(app || (app = {}));
//# sourceMappingURL=leaderboard.controller.js.map