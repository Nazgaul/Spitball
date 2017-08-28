"use strict";
var app;
(function (app) {
    "use strict";
    var Item = (function () {
        function Item(profileData, itemThumbnailService, userService, $mdDialog, resManager, boxService, $scope) {
            this.profileData = profileData;
            this.itemThumbnailService = itemThumbnailService;
            this.userService = userService;
            this.$mdDialog = $mdDialog;
            this.resManager = resManager;
            this.boxService = boxService;
            this.$scope = $scope;
            this.files = [];
            this.itemsLoading = false;
            this.itemPage = 0;
            this.item();
        }
        Item.prototype.item = function () {
            var _this = this;
            var self = this;
            self.itemsLoading = true;
            return self.userService.files(this.profileData.id, self.itemPage).then(function (response) {
                angular.forEach(response, function (value) {
                    var retVal = self.itemThumbnailService.assignValue(value.source);
                    value.thumbnail = retVal.thumbnail;
                    value.nameExtension = value.name.replace(/\.[^/.]+$/, "");
                    value.downloadLink = value.url + 'download/';
                });
                _this.files = _this.files.concat(response);
                if (response.length) {
                    _this.itemsLoading = false;
                    self.itemPage++;
                }
            });
        };
        Item.prototype.deleteItem = function (ev, item) {
            var _this = this;
            //disablePaging = true;
            var confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('deleteItem'))
                .targetEvent(ev)
                .ok(this.resManager.get('dialogOk'))
                .cancel(this.resManager.get('dialogCancel'));
            this.$mdDialog.show(confirm).then(function () {
                var index = _this.files.indexOf(item);
                _this.files.splice(index, 1);
                _this.$scope["u"].details.numItem--;
                //self.details.numItem--;
                _this.boxService.deleteItem(item.id);
            }).finally(function () {
                //disablePaging = false;
            });
        };
        Item.$inject = ["profileData", "itemThumbnailService", "userService",
            "$mdDialog", "resManager", "boxService", "$scope"];
        return Item;
    }());
    angular.module("app.user").controller("item", Item);
})(app || (app = {}));
//# sourceMappingURL=items.controller.js.map