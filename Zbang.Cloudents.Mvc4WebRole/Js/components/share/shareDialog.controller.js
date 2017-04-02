var app;
(function (app) {
    "use strict";
    var ShareDialog = (function () {
        function ShareDialog($mdDialog, data, $window, analytics) {
            this.$mdDialog = $mdDialog;
            this.data = data;
            this.$window = $window;
            this.analytics = analytics;
            this.what = data.what;
            switch (this.what) {
                case 'f':
                    this.socialAction = 'share-flashcard';
                    break;
                case 'b':
                    this.socialAction = 'share-box';
                    break;
                default:
                    this.socialAction = 'share';
            }
            this.socialAction = this.what == 'f' ? 'share-flashcard' : 'share-box';
            this.url = location.origin + "/" + data.what + "/" + encodeBase64(data.id);
            this.whatappLink = "whatsapp://send?text=" + encodeURIComponent(this.url);
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
            this.analytics["send"]('social', "facebook", this.socialAction, this.url);
            var shareFb = 'https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(this.url);
            this.$window.open(shareFb, "pop", "width=600, height=400, scrollbars=no");
        };
        ShareDialog.prototype.whatsapp = function () {
            this.analytics["send"]('social', "whatsApp", this.socialAction, this.url);
        };
        ShareDialog.prototype.twitter = function () {
            this.analytics["send"]('social', "twitter", this.socialAction, this.url);
            var shareTwiiter = "https://twitter.com/intent/tweet?text=" + encodeURIComponent(this.url);
            this.$window.open(shareTwiiter, "pop", "width=600, height=400, scrollbars=no");
        };
        ShareDialog.prototype.onSuccess = function () {
            this.analytics["send"]('social', "copied-link", this.socialAction, this.url);
        };
        return ShareDialog;
    }());
    ShareDialog.$inject = ["$mdDialog", "data", "$window", "Analytics"];
    angular.module("app").controller("ShareDialog", ShareDialog);
})(app || (app = {}));
//# sourceMappingURL=shareDialog.controller.js.map