module app {
    "use strict";
    class LikeToasterDialog {
        detail;
        static $inject = ["$mdToast", "$timeout", "userLike"];
        constructor(
            private $mdToast: angular.material.IToastService,
            private $timeout: angular.ITimeoutService,
            private userLike: boolean) {
            this.detail = userLike;
        }
        close() {
            this.$mdToast.cancel();
        };
        like() {

            // this.itemService.like(this.itemId, this.boxid);
            //if (this.details.like) {
            //    //this.details.likes--;
            //} else {
            //this.details.likes++;
            this.detail = !this.detail;
            this.$timeout(() => {
                this.$mdToast.hide();
            }, 2000);
            //}
            //this.details.like = !this.details.like;

        }

    }

    angular.module("app.item").controller("likeToasterDialog", LikeToasterDialog);
}