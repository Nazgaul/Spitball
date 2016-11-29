var app;
(function (app) {
    "use strict";
    var ShareDialog = (function () {
        function ShareDialog($mdDialog, data, $window) {
            this.$mdDialog = $mdDialog;
            this.data = data;
            this.$window = $window;
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
        ShareDialog.prototype.facebook = function () {
            var shareFb = 'https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(this.url);
            this.$window.open(shareFb, "pop", "width=600, height=400, scrollbars=no");
        };
        ShareDialog.prototype.whatsapp = function () {
            this.$window.open("whatsapp://send?text=" + this.url, "pop", "width=600, height=400, scrollbars=no");
        };
        ShareDialog.prototype.twitter = function () {
            var shareTwiiter = "https://twitter.com/intent/tweet?text=" + encodeURIComponent(this.url);
            this.$window.open(shareTwiiter, "pop", "width=600, height=400, scrollbars=no");
        };
        ShareDialog.$inject = ["$mdDialog", "data", "$window"];
        return ShareDialog;
    }());
    angular.module("app").controller("ShareDialog", ShareDialog);
})(app || (app = {}));
