module app {
    "use strict";

    class Leaderboard {
        static $inject = ["boxService", "$stateParams"];
        leaderboard;
        constructor(
            private boxService: IBoxService,
            private $stateParams: spitaball.ISpitballStateParamsService) {
            boxService.leaderBoard($stateParams.boxId)
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
    }

    angular.module("app.box").controller("Leaderboard", Leaderboard);
}