(function () {
    angular.module('app.ajaxservice', ['angular-cache']);
})();
(function () {
    angular.module('app.dashboard', ['app.ajaxservice']);
})();
(function () {
    angular.module('app.flashcard', ['app.ajaxservice']);
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
    angular.module('app.quiz', ['app.user','app.quiz.score', 'app.quiz.challenge', 'app.ajaxservice', 'app.quiz.stopwatch']);
})();




(function () {
    angular.module('app.upload', ['app.ajaxservice']);
})();

(function () {
    angular.module('app.account', ['app.ajaxservice']);
})();

(function () {
    angular.module('app.chat', ['SignalR']);
})();


