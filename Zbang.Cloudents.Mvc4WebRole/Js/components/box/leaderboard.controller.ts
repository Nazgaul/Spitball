module app {
    "use strict";
    export interface IUserGamification {
        rank?: number;
        name: string;
        image: string;
        points: number;
        levelName: string;
        progress: number;
    }

    class Leaderboard {
        static $inject = ["boxService", "$stateParams", "$mdDialog", "userDetailsFactory"];
        leaderboard;
        leaderboardUser: IUserGamification;
        //leaderboardMyself = true;
        constructor(
            private boxService: IBoxService,
            private $stateParams: spitaball.ISpitballStateParamsService,
            private $mdDialog: angular.material.IDialogService,
            private userDetailsFactory: IUserDetailsFactory) {
            this.leaderBoard();
        }
        private leaderBoard() {
            const user = this.userDetailsFactory.get();
            this.leaderboardUser = {
                name: user.name,
                image: user.image,
                levelName: user.levelName,
                progress: user.score / user.nextLevel * 100,
                points: user.score
            };
            this.boxService.leaderBoard(this.$stateParams.boxId)
                .then(response => {
                    var leaderBoard = response.model;
                    for (let i = 0; i < leaderBoard.length; i++) {
                        const elem = leaderBoard[i];
                        if (i === 0) {
                            elem.progress = 100;
                            continue;
                        }
                        elem.progress = elem.score / leaderBoard[0].score * 100;
                    }
                    this.leaderboardUser.rank = response.rank;
                    this.leaderboard = leaderBoard;
                });
        }
        close() {
            this.$mdDialog.hide();
        }
    }

    angular.module("app.box").controller("BoxLeaderboard", Leaderboard);
}