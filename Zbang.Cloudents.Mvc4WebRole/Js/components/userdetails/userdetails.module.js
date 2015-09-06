/// <reference path="~/Scripts/angular.js" />
(function () {
    angular.module('app.ajaxservice', ['jmdobry.angular-cache']);
})();
(function () {
    angular.module('app.userdetails', ['app.ajaxservice']);
})();
(function () {
    angular.module('app.dashboard', ['app.ajaxservice']);
})();
(function () {
    angular.module('app.box', ['app.ajaxservice']);
})();
(function () {
    angular.module('app.box.feed', ['app.ajaxservice']);
})();

