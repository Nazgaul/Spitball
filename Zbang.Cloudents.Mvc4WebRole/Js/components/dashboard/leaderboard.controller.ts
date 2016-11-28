module app {
    "use strict";

    class LeaderBoard {
        static $inject = ["dashboardService", "$filter"];
        hideLeaderBoard = true;
        hidePromo = false;
        leaderboard = [];
        flashcardPromo;
        hideBox = false;
        constructor(private dashboardService: IDashboardService,
            private $filter: any //meganumber is need defined
        ) {
            dashboardService.leaderboard().then((response: any) => {
                if (response.type === 1) {
                    this.hidePromo = true;
                    if (response.model.length < 3) {
                        //this.hideLeaderBoard = true;
                    }
                    this.hideLeaderBoard = false;
                    for (let i = 0; i < response.model.length; i++) {
                        response.model[i].score = $filter("megaNumber")(response.model[i].score);
                    }
                    
                    this.leaderboard = response.model;
                    console.log(this.leaderboard);
                } else {
                    this.hideLeaderBoard = true;
                    this.flashcardPromo = response.model;
                    console.log(this.flashcardPromo);
                }
                this.hideBox = this.hideLeaderBoard && this.hidePromo;

            });
        }
    }

    angular.module("app.dashboard").controller("DashboardLeaderboard", LeaderBoard);
}