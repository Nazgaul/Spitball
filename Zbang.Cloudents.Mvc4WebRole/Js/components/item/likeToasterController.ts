module app {
    "use strict";
    class LikeToasterDialog {
        details: any;
        static $inject = ["$stateParams", "$mdToast", "itemService", "sbHistory", "$timeout"];
        constructor(private $stateParams: spitaball.ISpitballStateParamsService,
            private $mdToast: angular.material.IToastService,
            private itemService: IItemService,
            private sbHistory: ISbHistory,
            private $timeout: angular.ITimeoutService) {
            this.itemService.getDetails(this.$stateParams.boxId, this.$stateParams.itemId, this.sbHistory.firstState())
                .then((response) => {
                    this.details = response;
                });
        }

        boxid = this.$stateParams.boxId;
        itemId = this.$stateParams.itemId;


        close() {
            this.$mdToast.hide();
        };

        like() {
            this.itemService.like(this.itemId, this.boxid);
            if (this.details.like) {
                this.details.likes--;
            } else {
                this.details.likes++;
                this.$timeout(() => {
                    this.close();
                }, 2000);
            }
            this.details.like = !this.details.like;

        }

    }

    angular.module("app.item").controller("likeToasterDialog", LikeToasterDialog);
}