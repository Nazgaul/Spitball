var app;
(function (app) {
    "use strict";
    var ShareDialog = (function () {
        function ShareDialog() {
            this.url = "hi";
            console.log("here");
        }
        return ShareDialog;
    }());
    angular.module("app").controller("ShareDialog", ShareDialog);
})(app || (app = {}));
