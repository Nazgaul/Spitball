/// <reference path="~/Scripts/angular.js" />
(function () {
    angular.module('app.ajaxservice', ['jmdobry.angular-cache']);
})();
(function () {
    angular.module('app.userdetails', ['app.ajaxservice']);
})();
