module app {

    class Gamification {
        constructor() {
            console.log("badge");
        }
    }

    angular.module("app.user").controller("gamification", Gamification);
}