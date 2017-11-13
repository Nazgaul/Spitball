"use strict";
var app;
(function (app) {
    'use strict';
    var page = 0, needToBringMore = true, disablePaging = false;
    function resetParams() {
        page = 0;
        needToBringMore = true;
        disablePaging = false;
    }
    var ItemsController = (function () {
        function ItemsController(boxService, $stateParams, $rootScope, itemThumbnailService, $mdDialog, $scope, user, $q, resManager, $state, $window, $timeout) {
            var _this = this;
            this.boxService = boxService;
            this.$stateParams = $stateParams;
            this.$rootScope = $rootScope;
            this.itemThumbnailService = itemThumbnailService;
            this.$mdDialog = $mdDialog;
            this.$scope = $scope;
            this.user = user;
            this.$q = $q;
            this.resManager = resManager;
            this.$state = $state;
            this.$window = $window;
            this.$timeout = $timeout;
            this.items = [];
            this.uploadShow = true;
            this.buildItem = function (value) {
                //in search we bring url
                value.url = value.url || _this.$state.href("item", {
                    universityType: _this.$stateParams["universityType"],
                    boxId: _this.$stateParams["boxId"],
                    boxName: _this.$stateParams["boxName"],
                    itemId: value.id,
                    itemName: value.name
                });
                value.downloadLink = value.url + 'download/';
                var retVal = _this.itemThumbnailService.assignValue(value.source);
                value.thumbnail = retVal.thumbnail;
                value.nameExtension = value.name.replace(/\.[^/.]+$/, "");
            };
            resetParams();
            $scope["stateParams"] = $stateParams;
            if ($stateParams["tabId"] && $stateParams["q"]) {
                $state.go("box.items", { tabId: $stateParams["tabId"], q: null });
                return;
            }
            if ($stateParams["tabId"]) {
                this.getItems().then(function () {
                    _this.scrollToPosition();
                });
            }
            else if ($stateParams["q"]) {
                this.getFilter().then(function () {
                    ;
                    _this.scrollToPosition();
                });
            }
            else {
                this.getItems().then(function () {
                    ;
                    _this.scrollToPosition();
                });
            }
            $rootScope.$on('disablePaging', function () {
                disablePaging = true;
            });
            $rootScope.$on('enablePaging', function () {
                disablePaging = false;
            });
            $scope.$on('update-thumbnail', function (e, args) {
                var item = _this.items.find(function (x) { return (x.id === args); });
                if (item) {
                    item.thumbnail += '&1=1';
                }
            });
            $rootScope.$on('item_upload', function (event, response2) {
                var self = _this;
                if (angular.isArray(response2)) {
                    for (var j = 0; j < response2.length; j++) {
                        pushItem(response2[j]);
                    }
                    return;
                }
                pushItem(response2);
                function pushItem(response) {
                    if (!response) {
                        return;
                    }
                    // ReSharper disable once CoercedEqualsUsing
                    if (response.boxId !== $stateParams.boxId) {
                        return;
                    }
                    if (response.item.tabId !== $stateParams["tabId"]) {
                        return; //not the same tab
                    }
                    self.followBox();
                    var item = response.item;
                    self.buildItem(item);
                    self.items.unshift(item);
                }
            });
            $rootScope.$on('close_upload', function () {
                _this.uploadShow = true;
            });
            $rootScope.$on('item_delete', function (e, itemId) {
                var item = _this.items.findIndex(function (x) { return (x.id === itemId); });
                if (item >= 0) {
                    _this.items.splice(item, 1);
                }
            });
            $scope.$watchCollection(function () {
                return [$state.params["tabId"], $state.params["q"]];
            }, function (newParams, oldParams) {
                if ($state.current.name !== "box.items") {
                    return; //happen upon link
                }
                if (newParams[0] !== oldParams[0]) {
                    if ($stateParams["tabId"] && $stateParams["q"]) {
                        $state.go("box.items", { tabId: $stateParams["tabId"], q: null });
                        return;
                    }
                    resetParams();
                    _this.getItems();
                }
                if (newParams[1] !== oldParams[1]) {
                    resetParams();
                    _this.getFilter();
                }
            });
        }
        ItemsController.prototype.myPagingFunction = function () {
            if (this.term) {
                return this.$q.when();
            }
            return this.getItems();
        };
        ;
        ItemsController.prototype.followBox = function () {
            this.$scope.$emit('follow-box');
        };
        ItemsController.prototype.removeItemFromTab = function (item) {
            this.boxService.addItemToTab(this.$stateParams.boxId, null, item.id);
            this.$scope.$broadcast('tab-item-remove');
            var index = this.items.indexOf(item);
            this.items.splice(index, 1);
        };
        ItemsController.prototype.addItemToTab = function ($data, tab) {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            var item = this.items.findIndex(function (x) { return (x.id === $data.id); });
            if (item >= 0) {
                this.items.splice(item, 1);
            }
            tab.count++;
            this.followBox();
            this.boxService.addItemToTab(this.$stateParams.boxId, tab.id, $data.id);
        };
        ItemsController.prototype.openUpload = function () {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            this.$rootScope.$broadcast('open_upload', this.$stateParams["tabId"]);
            this.uploadShow = false;
        };
        ItemsController.prototype.deleteItem = function (ev, item) {
            var _this = this;
            disablePaging = true;
            var confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('deleteItem'))
                .targetEvent(ev)
                .ok(this.resManager.get('dialogOk'))
                .cancel(this.resManager.get('dialogCancel'));
            this.$mdDialog.show(confirm).then(function () {
                var index = _this.items.indexOf(item);
                _this.boxService.deleteItem(item.id).then(function () {
                    _this.$scope.$broadcast('tab-item-remove');
                    _this.items.splice(index, 1);
                });
            }).finally(function () {
                disablePaging = false;
            });
        };
        ItemsController.prototype.getItems = function () {
            var _this = this;
            if (!needToBringMore) {
                return this.$q.when();
            }
            if (disablePaging) {
                return this.$q.when();
            }
            return this.boxService.items(this.$stateParams.boxId, this.$stateParams["tabId"], page).then(function (response) {
                angular.forEach(response, _this.buildItem);
                if (page > 0) {
                    _this.items = _this.items.concat(response);
                }
                else {
                    _this.items = response;
                }
                if (!response.length) {
                    needToBringMore = false;
                }
                page++;
            });
        };
        ItemsController.prototype.filter = function () {
            if (!this.term) {
                this.getItems();
            }
            this.$state.go('box.items', { tabId: null, q: this.term });
        };
        //upload
        ItemsController.prototype.getFilter = function () {
            var _this = this;
            this.term = this.$stateParams["q"];
            return this.boxService.filterItem(this.$stateParams["q"], this.$stateParams.boxId, 0).then(function (response) {
                angular.forEach(response, _this.buildItem);
                _this.items = response;
            });
        };
        ItemsController.prototype.scrollToPosition = function () {
            var _this = this;
            var yOffsetParam = this.$stateParams["pageYOffset"];
            if (yOffsetParam) {
                this.$timeout(function () {
                    _this.$window.scrollTo(0, yOffsetParam);
                });
            }
        };
        return ItemsController;
    }());
    ItemsController.$inject = ['boxService', '$stateParams', '$rootScope',
        'itemThumbnailService', '$mdDialog',
        '$scope', 'user', '$q', 'resManager', '$state', "$window", "$timeout"];
    angular.module('app.box.items').controller('ItemsController', ItemsController);
})(app || (app = {}));
//# sourceMappingURL=item.controller.js.map