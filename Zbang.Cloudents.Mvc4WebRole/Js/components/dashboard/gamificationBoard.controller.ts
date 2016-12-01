module app {
    "use strict";

    class GamificationBoard {
        static $inject = ["userService"];
        data;
        constructor(private userService: IUserService) {
            this.userService.gamificationBoard()
                .then(response => {
                    this.data = response;
                    console.log(this.data);
                });
        }


    }

    angular.module("app.dashboard").controller("gamificationBoard", GamificationBoard);
}