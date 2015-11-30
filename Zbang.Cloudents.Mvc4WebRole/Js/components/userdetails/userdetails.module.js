/// <reference path="~/Scripts/angular.js" />
(function () {
    angular.module('app.ajaxservice', ['jmdobry.angular-cache']);
})();
//(function () {
//    angular.module('app.userdetails', ['app.ajaxservice']);
//})();
(function () {
    angular.module('app.dashboard', ['app.ajaxservice']);
})();
(function () {
    angular.module('app.box.feed', ['app.ajaxservice']);
})();
(function () {
    angular.module('app.box.quizzes', ['app.ajaxservice']);
})();
(function () {
    angular.module('app.box.items', ['app.ajaxservice']);
})();

(function () {
    angular.module('app.box.members', ['app.ajaxservice']);
})();
(function () {
    angular.module('app.box', ['app.ajaxservice', 'app.box.feed', 'app.box.quizzes', 'app.box.items', 'app.box.members']);
})();


(function () {
    angular.module('app.user.details', ['app.ajaxservice']);
})();
(function () {
    angular.module('app.user.account', ['app.ajaxservice']);
})();
(function () {
    angular.module('app.user', ['app.ajaxservice', 'app.user.details', 'app.user.account']);
})();


(function () {
    angular.module('app.library', ['app.ajaxservice']);
})();

(function () {
    angular.module('app.search', ['app.ajaxservice']);
})();

(function () {
    angular.module('app.item', ['app.ajaxservice']);
})();


(function () {
    angular.module('app.quiz', ['app.user','app.quiz.score', 'app.quiz.challenge', 'app.ajaxservice', 'app.quiz.stopwatch', 'ui.bootstrap.modal']);
})();


(function () {
    angular.module('app.upload', ['app.ajaxservice']);
})();

(function () {
    angular.module('app.account', ['app.ajaxservice']);
})();


