module app {
    "use strict";

    class LeaderBoard {
        static $inject = ["dashboardService", "$filter", "$mdDialog"];
        hideLeaderBoard = true;
        hidePromo = true;
        leaderboard = [];
        flashcardPromo;
        color1: string;
        color2: string;
        hideBox = false;
        constructor(private dashboardService: IDashboardService,
            private $filter: any, //meganumber is need defined
            private $mdDialog: angular.material.IDialogService
        ) {
            dashboardService.leaderboard().then((response: any) => {
                if (response.type === 1) {
                    //this.hidePromo = true;
                    if (response.model.length < 3) {
                        this.hideBox = true;
                        //this.hideLeaderBoard = true;
                    }
                    this.hideLeaderBoard = false;
                    for (let i = 0; i < response.model.length; i++) {
                        response.model[i].score = $filter("megaNumber")(response.model[i].score);
                    }

                    this.leaderboard = response.model;
                    console.log(this.leaderboard);
                } else {
                    //this.hideLeaderBoard = true;
                   
                    this.flashcardPromo = response.model;
                    this.dashboardService.getUniversityMeta()
                        .then(response => {
                            console.log(response);
                            this.hidePromo = false;
                            this.color1 = response.btnColor;
                            this.color2 = response.stripColor;
                        });
                }
//                this.hideBox = this.hideLeaderBoard && this.hidePromo;

            });
        }
        details(ev) {
            this.$mdDialog.show({
                templateUrl: "/flashcard/promo/",
                targetEvent: ev,
                clickOutsideToClose: true,
                fullscreen: false // Only for -xs, -sm breakpoints.
            });
            console.log("here");
        }
    }

    angular.module("app.dashboard").controller("DashboardLeaderboard", LeaderBoard);
}