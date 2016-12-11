module app {
    "use strict";

    class Leaderboard {
        static $inject = ["boxService", "$stateParams","$mdDialog"];
        leaderboard;
        leaderboardMyself = true;
        constructor(
            private boxService: IBoxService,
            private $stateParams: spitaball.ISpitballStateParamsService,
            private $mdDialog: angular.material.IDialogService) {
            this.leaderBoard();
        }
        private leaderBoard() {
            this.boxService.leaderBoard(this.$stateParams.boxId, this.leaderboardMyself)
                .then(response => {
                    for (let i = 0; i < response.length; i++) {
                        const elem = response[i];
                        if (i === 0) {
                            elem.progress = 100;
                            continue;
                        }
                        elem.progress = elem.score / response[0].score * 100;
                    }
                    this.leaderboard = response;
                });
        }
        goToSelf() {
            this.leaderboardMyself = !this.leaderboardMyself;
            this.leaderBoard();
        }
        close() {
            this.$mdDialog.hide();
        }
    }

    angular.module("app.box").controller("Leaderboard", Leaderboard);
}