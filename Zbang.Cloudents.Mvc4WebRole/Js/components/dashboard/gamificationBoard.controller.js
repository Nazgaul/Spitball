var app;
(function (app) {
    "use strict";
    var GamificationBoard = (function () {
        function GamificationBoard(userService) {
            var _this = this;
            this.userService = userService;
            this.userService.gamificationBoard()
                .then(function (response) {
                _this.data = response;
                console.log(_this.data);
            });
        }
        GamificationBoard.$inject = ["userService"];
        return GamificationBoard;
    }());
    angular.module("app.dashboard").controller("gamificationBoard", GamificationBoard);
})(app || (app = {}));
