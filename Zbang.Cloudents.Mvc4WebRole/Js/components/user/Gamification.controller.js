var app;
(function (app) {
    var Gamification = (function () {
        function Gamification($state, $scope, userService) {
            var _this = this;
            this.$state = $state;
            this.$scope = $scope;
            this.userService = userService;
            this.doneLevel = false;
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
            });
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
                _this.levels["l" + response.number] = { progress: response.score / response.nextLevel * 100 };
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
                console.log(response);
                _this.badges = response.badges;
                angular.forEach(_this.badges, function (v) {
                    var badge = response.model.find(function (f) { return f.badge === v.name; });
                    if (badge) {
                        v.progress = badge.progress;
                    }
                    else {
                        v.progress = 0;
                    }
                });
            });
        };
        Gamification.prototype.showBadge = function (badge) {
            if (!badge) {
                return;
            }
            var badgeIndex = this.badges.indexOf(badge);
            this.badgeInfo = {
                data: badge,
                next: this.badges[badgeIndex + 1],
                prev: this.badges[badgeIndex - 1]
            };
        };
        Gamification.$inject = ["$state", "$scope", "userService"];
        return Gamification;
    }());
    angular.module("app.user").controller("gamification", Gamification);
})(app || (app = {}));
