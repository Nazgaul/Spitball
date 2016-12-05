var app;
(function (app) {
    var Gamification = (function () {
        function Gamification() {
            console.log("badge");
        }
        return Gamification;
    }());
    angular.module("app.user").controller("gamification", Gamification);
})(app || (app = {}));
