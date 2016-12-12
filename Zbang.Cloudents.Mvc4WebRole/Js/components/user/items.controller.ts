module app {
    "use strict";
    class Item {
        files = [];
        itemsLoading = false;
        itemPage = 0;
        static $inject = ["profileData", "itemThumbnailService", "userService",
            "$mdDialog", "resManager", "boxService","$scope"];
        constructor(
            private profileData: IUserData,
            private itemThumbnailService: IItemThumbnailService,
            private userService: IUserService,
            private $mdDialog: angular.material.IDialogService,
            private resManager: IResManager,
            private boxService: IBoxService,
            private $scope: angular.IScope
        ) {
            this.item();
        }

        item() {
            var self = this;
            self.itemsLoading = true;
            return self.userService.files(this.profileData.id, self.itemPage).then(response => {
                angular.forEach(response, value => {
                    var retVal = self.itemThumbnailService.assignValue(value.source);
                    value.thumbnail = retVal.thumbnail;
                    value.nameExtension = value.name.replace(/\.[^/.]+$/, "");
                    value.downloadLink = value.url + 'download/';
                });
                this.files = this.files.concat(response);
                if (response.length) {
                    this.itemsLoading = false;
                    self.itemPage++;
                }

            });
        }
        deleteItem(ev, item) {
            //disablePaging = true;
            const confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('deleteItem'))
                .targetEvent(ev)
                .ok(this.resManager.get('dialogOk'))
                .cancel(this.resManager.get('dialogCancel'));

            this.$mdDialog.show(confirm).then(() => {
                var index = this.files.indexOf(item);
                this.files.splice(index, 1);
                this.$scope["u"].details.numItem--;
                //self.details.numItem--;
                this.boxService.deleteItem(item.id);
            }).finally(function () {
                //disablePaging = false;
            });
        }
    }

    angular.module("app.user").controller("item", Item);
}