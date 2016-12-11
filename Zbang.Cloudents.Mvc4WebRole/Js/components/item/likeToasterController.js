var app;
(function (app) {
    "use strict";
    var LikeToasterDialog = (function () {
        function LikeToasterDialog($stateParams, $mdToast, itemService, sbHistory, $timeout) {
            var _this = this;
            this.$stateParams = $stateParams;
            this.$mdToast = $mdToast;
            this.itemService = itemService;
            this.sbHistory = sbHistory;
            this.$timeout = $timeout;
            this.boxid = this.$stateParams.boxId;
            this.itemId = this.$stateParams.itemId;
            this.itemService.getDetails(this.$stateParams.boxId, this.$stateParams.itemId, this.sbHistory.firstState())
                .then(function (response) {
                _this.details = response;
            });
        }
        LikeToasterDialog.prototype.close = function () {
            this.$mdToast.hide();
        };
        ;
        LikeToasterDialog.prototype.like = function () {
            var _this = this;
            this.itemService.like(this.itemId, this.boxid);
            if (this.details.like) {
                this.details.likes--;
            }
            else {
                this.details.likes++;
                this.$timeout(function () {
                    _this.close();
                }, 2000);
            }
            this.details.like = !this.details.like;
        };
        LikeToasterDialog.$inject = ["$stateParams", "$mdToast", "itemService", "sbHistory", "$timeout"];
        return LikeToasterDialog;
    }());
    angular.module("app.item").controller("likeToasterDialog", LikeToasterDialog);
})(app || (app = {}));
//# sourceMappingURL=likeToasterController.js.map