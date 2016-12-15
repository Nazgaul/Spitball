var app;
(function (app) {
    "use strict";
    var GamificationBoard = (function () {
        function GamificationBoard(userService, $interval, userDetailsFactory) {
            var _this = this;
            this.userService = userService;
            this.$interval = $interval;
            this.userDetailsFactory = userDetailsFactory;
            var user = userDetailsFactory.get();
            this.data = {};
            this.data.progress = 0;
            this.userService.gamificationBoard()
                .then(function (response) {
                _this.badges = response.badgeCount;
                var d = {
                    badges: user.badges,
                    image: user.image,
                    levelName: user.levelName,
                    name: user.name,
                    points: user.score,
                    progress: user.score / user.nextLevel * 100,
                    badgesProgress: user.badges / _this.badges * 100,
                    rank: response.location,
                    nextLevel: user.nextLevel
                };
                _this.data = d;
                _this.data.id = user.id;
                _this.nextLevel = _this.data.nextLevel - _this.data.points;
                console.log(_this.data, response);
            });
        }
        GamificationBoard.$inject = ["userService", "$interval", "userDetailsFactory"];
        return GamificationBoard;
    }());
    angular.module("app.dashboard").controller("gamificationBoard", GamificationBoard);
})(app || (app = {}));
//# sourceMappingURL=gamificationBoard.controller.js.map