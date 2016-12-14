module app {
    "use strict";
    interface IDashboardGamification extends IUserGamification {
        badges: number;
        badgesProgress: number;
        nextLevel: number;
    }
    //var scoreToReach = 0;
    //var badgesToReach = 0;
    class GamificationBoard {
        static $inject = ["userService", "$interval", "userDetailsFactory"];
        data;

        //score = 0;
        nextLevel: number;
        badges: number;
        constructor(private userService: IUserService,
            private $interval: angular.IIntervalService,
            private userDetailsFactory: IUserDetailsFactory) {

            const user = userDetailsFactory.get();
            this.data = {};
            this.data.progress = 0;
            this.userService.gamificationBoard()
                .then(response => {
                    this.badges = response.badgeCount;
                    var d: IDashboardGamification =
                        {
                            badges: user.badges,
                            image: user.image,
                            levelName: user.levelName,
                            name: user.name,
                            points: user.score,
                            progress: user.score / user.nextLevel * 100,
                            badgesProgress: user.badges / this.badges * 100,
                            rank: response.location,
                            nextLevel: user.nextLevel

                        };
                    this.data = d;
                    this.data.id = user.id;
                    this.nextLevel = this.data.nextLevel - this.data.points;
                    console.log(this.data, response);

                });
        }
    }

    angular.module("app.dashboard").controller("gamificationBoard", GamificationBoard);
}