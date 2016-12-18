var app;
(function (app) {
    "use strict";
    var GamificationBoard = (function () {
        function GamificationBoard(userService, $interval, userDetailsFactory, $scope) {
            var _this = this;
            this.userService = userService;
            this.$interval = $interval;
            this.userDetailsFactory = userDetailsFactory;
            this.$scope = $scope;
            var user = userDetailsFactory.get();
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
            $scope.$on('userDetailsChange', function () {
                var userData = _this.userDetailsFactory.get();
                _this.data = {
                    progress: userData.score / userData.nextLevel * 100,
                    badgesProgress: userData.badges / _this.badges * 100
                };
                $scope.$apply();
            });
        }
        GamificationBoard.$inject = ["userService", "$interval", "userDetailsFactory", "$scope"];
        return GamificationBoard;
    }());
    angular.module("app.dashboard").controller("gamificationBoard", GamificationBoard);
})(app || (app = {}));
//# sourceMappingURL=gamificationBoard.controller.js.map