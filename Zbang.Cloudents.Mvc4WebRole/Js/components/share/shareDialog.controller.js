var app;
(function (app) {
    "use strict";
    var ShareDialog = (function () {
        function ShareDialog($mdDialog, data) {
            this.$mdDialog = $mdDialog;
            this.data = data;
            this.url = location.origin + "/" + data.what + "/" + encodeBase64(data.id);
            function encodeBase64(integer) {
                var characterSet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                if (integer === 0) {
                    return '0';
                }
                var s = '';
                while (integer > 0) {
                    s = characterSet[integer % 62] + s;
                    integer = Math.floor(integer / 62);
                }
                return s;
            }
            ;
        }
        ShareDialog.prototype.close = function () {
            this.$mdDialog.cancel();
        };
        ShareDialog.$inject = ["$mdDialog", "data"];
        return ShareDialog;
    }());
    angular.module("app").controller("ShareDialog", ShareDialog);
})(app || (app = {}));
