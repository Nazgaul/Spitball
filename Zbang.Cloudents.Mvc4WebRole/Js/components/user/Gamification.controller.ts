module app {

    class Gamification {
        static $inject = ["$state", "$scope", "userService", "$timeout", "$anchorScroll"];
        levels;
        doneLevel = false;
        badges;
        badgeInfo;
        leaderboard: Array<any> = [];
        leaderboardMyself = true;
        leaderboardPage = 0;

        constructor(private $state: angular.ui.IStateService,
            private $scope: angular.IScope,
            private userService: IUserService,
            private $timeout: angular.ITimeoutService,
            private $anchorScroll: angular.IAnchorScrollService) {
            $scope["$state"] = this.$state;
            if (!$state.params["type"]) {
                this.$state.go($state.current.name, { type: "level" });
            }
            $scope.$watch(() => { return $state.params["type"] },
                (newVal) => {
                    if (newVal === "level") {
                        this.levelTab();
                    }
                    if (newVal === "badge") {
                        this.badgeTab();
                    }
                    if (newVal === "community") {
                        this.communityTab();
                    }
                });
        }
        isActive(state) {
            return this.$state.params["type"] === state;
        }

        private levelTab() {
            if (this.doneLevel) {
                return;
            }
            this.levels = {};
            this.userService.levels(this.$state.params["userId"])
                .then(response => {
                    var i: number;
                    //var progress = { progress: 0 }
                    for (i = 0; i < response.number; i++) {
                        this.levels["l" + i] = { progress: 100 };
                    }
                    this.levels["l" + response.number] = { progress: response.score / response.nextLevel * 100 };
                    for (i = response.number + 1; i < 5; i++) {
                        this.levels["l" + i] = { progress: 0 };
                    }
                    this.doneLevel = true;
                });
        }

        private badgeTab() {
            if (this.badges) {
                return;
            }
            this.userService.badges(this.$state.params["userId"])
                .then(response => {
                    console.log(response);
                    this.badges = response.badges;
                    angular.forEach(this.badges,
                        (v) => {
                            var badge = response.model.find(f => f.badge === v.name);
                            if (badge) {
                                v.progress = badge.progress;
                            } else {
                                v.progress = 0;
                            }
                        });
                });
        }
        private communityTab() {
            return this.userService.leaderboard(this.$state.params["userId"], this.leaderboardMyself, this.leaderboardPage)
                .then((response: Array<any>) => {
                    for (let i = 0; i < response.length; i++) {
                        const elem = response[i];
                        if (i === 0) {
                            elem.progress = this.leaderboard[this.leaderboard.length - 1] || 100;
                            continue;
                        }
                        elem.progress = elem.score / response[0].score * 100;
                    }
                    if (this.leaderboardPage) {
                        this.leaderboard = this.leaderboard.concat(response);
                    } else {
                        this.leaderboard = response;
                    }
                    if (this.leaderboardMyself) {
                        this.$timeout(() => {
                            this.$anchorScroll("user_" + this.$state.params["userId"]);
                        });
                    }
                });
        }
        goToSelf() {
            this.leaderboardPage = 0;
            this.leaderboardMyself = !this.leaderboardMyself;
            this.communityTab();
        }
        showBadge(badge) {
            if (!badge) {
                return;
            }
            const badgeIndex = this.badges.indexOf(badge);
            this.badgeInfo = {
                data: badge,
                next: this.badges[badgeIndex + 1],
                prev: this.badges[badgeIndex - 1]
            };
        }
        loadMoreLeaderboard() {
            if (!this.leaderboardMyself) {
                this.leaderboardPage++;
                return this.communityTab();
            }
        }
    }
    angular.module("app.user").controller("gamification", Gamification);
}