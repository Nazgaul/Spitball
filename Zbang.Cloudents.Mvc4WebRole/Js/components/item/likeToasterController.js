var app;
(function (app) {
    "use strict";
    var LikeToasterDialog = (function () {
        function LikeToasterDialog($mdToast, $timeout, userLike) {
            this.$mdToast = $mdToast;
            this.$timeout = $timeout;
            this.userLike = userLike;
            this.detail = userLike;
        }
        LikeToasterDialog.prototype.close = function () {
            this.$mdToast.cancel();
        };
        ;
        LikeToasterDialog.prototype.like = function () {
            var _this = this;
            this.detail = !this.detail;
            this.$timeout(function () {
                _this.$mdToast.hide();
            }, 2000);
        };
        LikeToasterDialog.$inject = ["$mdToast", "$timeout", "userLike"];
        return LikeToasterDialog;
    }());
    angular.module("app.item").controller("likeToasterDialog", LikeToasterDialog);
})(app || (app = {}));
