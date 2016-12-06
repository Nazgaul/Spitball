var app;
(function (app) {
    var Gamification = (function () {
        function Gamification($state, $scope, userService) {
            var _this = this;
            this.$state = $state;
            this.$scope = $scope;
            this.userService = userService;
            this.doneLevel = false;
            this.communityUsers = [
                {
                    name: "Irena Dorfman",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 50800
                },
                {
                    name: "user 2",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 800
                },
                {
                    name: "user 3",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 800000
                },
                {
                    name: "user 4",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 800000
                },
                {
                    name: "user 5",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 800000
                },
                {
                    name: "user 6",
                    image: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/f2338ed9-d5be-4d39-8fca-2896ef836ef6.jpg",
                    points: 800000
                }];
            $scope["$state"] = this.$state;
            if (!$state.params["type"]) {
                this.$state.go($state.current.name, { type: "level" });
            }
            $scope.$watch(function () { return $state.params["type"]; }, function (newVal, oldVal) {
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
            console.log(this.badges);
        };
        Gamification.$inject = ["$state", "$scope", "userService"];
        return Gamification;
    }());
    angular.module("app.user").controller("gamification", Gamification);
})(app || (app = {}));
