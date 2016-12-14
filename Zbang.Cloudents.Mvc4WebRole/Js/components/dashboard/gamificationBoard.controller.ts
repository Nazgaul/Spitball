module app {
    "use strict";
    var scoreToReach = 0;
    var badgesToReach = 0;
    class GamificationBoard {
        static $inject = ["userService", "$interval", "userDetailsFactory"];
        data;
        score = 0;
        badges = 0;
        totalBadgs = 5; //TODO: Get total num of badges - not hardcode it to 5
        constructor(private userService: IUserService, private $interval: angular.IIntervalService, private userDetailsFactory: IUserDetailsFactory) {

            this.userService.gamificationBoard()
                .then(response => {
                    this.data = response;
                    //TODO: Add userName and userImage to the userService.gamificationBoard() response instead calling userDetailsFactory.get()
                    var userData = userDetailsFactory.get();
                    this.data.userName = userData.name;
                    this.data.userImage = userData.image;
                    this.data.userId = userData.id;
                    //this.data.score = 1073741823;
                    scoreToReach = this.data.score / this.data.nextLevel * 100;
                    var q = $interval(() => {
                        this.score += 1;
                        this.score = Math.min(this.score, scoreToReach);
                        if (this.score === scoreToReach) {
                            this.$interval.cancel(q);
                        }
                    }, 10);
                    badgesToReach = this.data.badgeCount / this.totalBadgs * 100;
                    var b = $interval(() => {
                        this.badges += 1;
                        this.badges = Math.min(this.badges, badgesToReach);
                        if (this.badges === badgesToReach) {
                            this.$interval.cancel(b);
                        }
                    }, 10);
                });
        }


    }

    angular.module("app.dashboard").controller("gamificationBoard", GamificationBoard);
}