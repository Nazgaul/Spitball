module app {
    "use strict";
    var scoreToReach = 0;
    class GamificationBoard {
        static $inject = ["userService", "$interval"];
        data;
        score = 0;
        constructor(private userService: IUserService, private $interval: angular.IIntervalService) {
            this.userService.gamificationBoard()
                .then(response => {
                    this.data = response;
                    //this.data.score = 1073741823;
                    scoreToReach = this.data.score / this.data.nextLevel * 100;
                    var q = $interval(() => {
                        this.score += 1;
                        this.score = Math.min(this.score, scoreToReach);
                        if (this.score === scoreToReach) {
                            this.$interval.cancel(q);
                        }
                    },10);
                });
        }


    }

    angular.module("app.dashboard").controller("gamificationBoard", GamificationBoard);
}