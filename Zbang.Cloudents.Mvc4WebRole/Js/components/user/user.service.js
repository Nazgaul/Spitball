var app;
(function (app) {
    'use strict';
    var UserService = (function () {
        function UserService(ajaxService2) {
            this.ajaxService2 = ajaxService2;
        }
        UserService.prototype.getDetails = function (userid) {
            return this.ajaxService2.get("/user/profilestats/", { id: userid });
        };
        UserService.prototype.boxes = function (userid, page) {
            return this.ajaxService2.get("/user/boxes/", { id: userid, page: page });
        };
        UserService.prototype.friends = function (userid, page) {
            return this.ajaxService2.get("/user/friends/", { id: userid, page: page });
        };
        UserService.prototype.files = function (userid, page) {
            return this.ajaxService2.get("/user/items/", { id: userid, page: page });
        };
        UserService.prototype.feed = function (userid, page) {
            return this.ajaxService2.get("/user/comment/", { id: userid, page: page });
        };
        UserService.prototype.quiz = function (userid, page) {
            return this.ajaxService2.get("/user/quiz/", { id: userid, page: page });
        };
        UserService.prototype.flashcard = function (userid, page) {
            return this.ajaxService2.get("/user/flashcard/", { id: userid, page: page });
        };
        UserService.prototype.getNotification = function () {
            return this.ajaxService2.get("/share/notifications/");
        };
        UserService.prototype.gamificationBoard = function () {
            return this.ajaxService2.get("/user/gamificationboard");
        };
        UserService.prototype.levels = function (userid) {
            return this.ajaxService2.get("/user/levels", { userid: userid });
        };
        UserService.prototype.badges = function (userid) {
            return this.ajaxService2.get("/user/userbadges/", { userid: userid });
        };
        UserService.prototype.leaderboard = function (userid, page) {
            return this.ajaxService2.get("/user/leaderboard/", {
                userid: userid,
                page: page
            });
        };
        UserService.$inject = ['ajaxService2'];
        return UserService;
    }());
    angular.module("app.user").service("userService", UserService);
})(app || (app = {}));
//# sourceMappingURL=user.service.js.map