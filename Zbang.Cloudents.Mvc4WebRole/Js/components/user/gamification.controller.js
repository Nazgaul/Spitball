"use strict";
var app;
(function (app) {
    var Gamification = (function () {
        function Gamification($state, $scope, userService) {
            var _this = this;
            this.$state = $state;
            this.$scope = $scope;
            this.userService = userService;
            this.doneLevel = false;
            this.leaderboard = [];
            //leaderboardMyself = true;
            this.leaderboardPage = 0;
            $scope["$state"] = this.$state;
            if (!$state.params["type"]) {
                this.$state.go($state.current.name, { type: "level" });
            }
            $scope.$watch(function () { return $state.params["type"]; }, function (newVal) {
                if (newVal === "level") {
                    _this.levelTab();
                }
                if (newVal === "badge") {
                    _this.badgeTab();
                }
                if (newVal === "community") {
                    _this.communityTab();
                }
            });
            //const user = this.userDetailsFactory.get();
        }
        Gamification.prototype.isActive = function (state) {
            return this.$state.params["type"] === state;
        };
        Gamification.prototype.levelTab = function () {
            var _this = this;
            if (this.doneLevel) {
                return;
            }
            this.levels = {};
            this.userService.levels(this.$state.params["userId"])
                .then(function (response) {
                var i;
                for (i = 0; i < response.number; i++) {
                    _this.levels["l" + i] = { progress: 100 };
                }
                _this.levels["l" + response.number] = { progress: (response.score / response.nextLevel * 100).toFixed(0) };
                for (i = response.number + 1; i < 5; i++) {
                    _this.levels["l" + i] = { progress: 0 };
                }
                _this.doneLevel = true;
            });
        };
        Gamification.prototype.badgeTab = function () {
            var _this = this;
            if (this.badges) {
                return;
            }
            this.userService.badges(this.$state.params["userId"])
                .then(function (response) {
                _this.badges = {};
                angular.forEach(response, function (v) {
                    _this.badges[v.badge] = {};
                    _this.badges[v.badge].progress = v.progress;
                });
            });
        };
        Gamification.prototype.communityTab = function () {
            var _this = this;
            return this.userService.leaderboard(this.$state.params["userId"], this.leaderboardPage)
                .then(function (response) {
                //var progress = 100;
                //if (this.leaderboard.length) {
                //    progress = this.leaderboard[this.leaderboard.length - 1].progress;
                //}
                var highScore = response[0].score;
                if (_this.leaderboard.length) {
                    highScore = _this.leaderboard[0].score;
                }
                for (var i = 0; i < response.length; i++) {
                    var elem = response[i];
                    if (i === 0) {
                        if (!_this.leaderboard.length) {
                            elem.progress = 100;
                            continue;
                        }
                        // elem.progress = this.leaderboard[this.leaderboard.length - 1].progress || 100;
                    }
                    elem.progress = elem.score / highScore * 100;
                }
                if (_this.leaderboardPage) {
                    _this.leaderboard = _this.leaderboard.concat(response);
                }
                else {
                    _this.leaderboard = response;
                }
                //if (this.leaderboardMyself) {
                //    this.$timeout(() => {
                //        this.$anchorScroll("user_" + this.$state.params["userId"]);
                //    });
                //}
            });
        };
        //goToSelf() {
        //    this.leaderboardPage = 0;
        //    this.leaderboardMyself = !this.leaderboardMyself;
        //    this.communityTab();
        //}
        //showBadge(badge) {
        //    if (!badge) {
        //        return;
        //    }
        //    const badgeIndex = this.badges.indexOf(badge);
        //    this.badgeInfo = {
        //        data: badge,
        //        next: this.badges[badgeIndex + 1],
        //        prev: this.badges[badgeIndex - 1]
        //    };
        //}
        Gamification.prototype.loadMoreLeaderboard = function () {
            //if (!this.leaderboardMyself) {
            this.leaderboardPage++;
            return this.communityTab();
            //}
        };
        return Gamification;
    }());
    Gamification.$inject = ["$state", "$scope", "userService", "user"];
    angular.module("app.user").controller("gamification", Gamification);
})(app || (app = {}));
//# sourceMappingURL=gamification.controller.js.map