module app {
    'use strict';


    
    var page = 0, needToBringMore = true, disablePaging = false;

    function resetParams() {
        page = 0;
        needToBringMore = true;
        disablePaging = false;
    }

    class ItemsController {
        static $inject = ['boxService', '$stateParams', '$rootScope',
            'itemThumbnailService', '$mdDialog',
            '$scope', 'user', '$q', 'resManager', '$state', "$window", "$timeout"];

        items = [];
        uploadShow = true;
        term;
        constructor(
            private boxService: IBoxService,
            private $stateParams: spitaball.ISpitballStateParamsService,
            private $rootScope: angular.IRootScopeService,
            private itemThumbnailService: IItemThumbnailService,
            private $mdDialog: angular.material.IDialogService,
            private $scope: angular.IScope,
            private user: IUserData,
            private $q: angular.IQService,
            private resManager: IResManager,
            private $state: angular.ui.IStateService,
            private $window: angular.IWindowService,
            private $timeout: angular.ITimeoutService) {
            resetParams();
            $scope["stateParams"] = $stateParams;

            if ($stateParams["tabId"] && $stateParams["q"]) {
                $state.go("box.items", { tabId: $stateParams["tabId"], q: null });
                return;
            }
            if ($stateParams["tabId"]) {
                this.getItems().then(() => {
                    this.scrollToPosition();
                });
            }
            else if ($stateParams["q"]) {
                this.getFilter().then(() => {
                    ;
                    this.scrollToPosition();
                });
            } else {
                this.getItems().then(() => {
                    ;
                    this.scrollToPosition();
                });
            }
            $rootScope.$on('disablePaging', () => {
                disablePaging = true;
            });
            $rootScope.$on('enablePaging', () => {
                disablePaging = false;
            });
            $scope.$on('update-thumbnail', (e, args) => {
                var item = this.items.find(x => (x.id === args));
                if (item) {
                    item.thumbnail += '&1=1';
                }
            });
            $rootScope.$on('item_upload', (event, response2) => {
                var self = this;
                if (angular.isArray(response2)) {
                    for (let j = 0; j < response2.length; j++) {
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
                    const item = response.item;
                    self.buildItem(item);
                    self.items.unshift(item);
                }
            });
            $rootScope.$on('close_upload', () => {
                this.uploadShow = true;
            });
            $rootScope.$on('item_delete', (e, itemId) => {
                var item = this.items.findIndex(x => (x.id === itemId));
                if (item >= 0) {
                    this.items.splice(item, 1);
                }
            });

            $scope.$watchCollection(() => {
                return [$state.params["tabId"], $state.params["q"]];
            }, (newParams, oldParams) => {
                if ($state.current.name !== "box.items") {
                    return; //happen upon link
                }
                if (newParams[0] !== oldParams[0]) {
                    if ($stateParams["tabId"] && $stateParams["q"]) {
                        $state.go("box.items", { tabId: $stateParams["tabId"], q: null });
                        return;
                    }
                    resetParams();
                    this.getItems();
                }
                if (newParams[1] !== oldParams[1]) {
                    resetParams();
                    this.getFilter();
                }

            });
        }
        myPagingFunction() {
            if (this.term) {
                return this.$q.when();
            }
            return this.getItems();
        };
        followBox() {
            this.$scope.$emit('follow-box');
        }
        //i.filter = filter;
        //i.openUpload = openUpload;
        //i.deleteItem = deleteItem;
        //i.addItemToTab = addItemToTab;
        //i.downloadItem = followBox;
        //i.removeItemFromTab = removeItemFromTab;

        removeItemFromTab(item) {
            this.boxService.addItemToTab(this.$stateParams.boxId, null, item.id);
            this.$scope.$broadcast('tab-item-remove');
            const index = this.items.indexOf(item);
            this.items.splice(index, 1);
        }
        addItemToTab($data, tab) {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            const item = this.items.findIndex(x => (x.id === $data.id));
            if (item >= 0) {
                this.items.splice(item, 1);
            }
            tab.count++;
            this.followBox();
            this.boxService.addItemToTab(this.$stateParams.boxId, tab.id, $data.id);

        }


        openUpload() {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            this.$rootScope.$broadcast('open_upload', this.$stateParams["tabId"]);
            this.uploadShow = false;
        }

        deleteItem(ev, item) {
            disablePaging = true;
            const confirm = this.$mdDialog.confirm()
                .title(this.resManager.get('deleteItem'))
                .targetEvent(ev)
                .ok(this.resManager.get('dialogOk'))
                .cancel(this.resManager.get('dialogCancel'));

            this.$mdDialog.show(confirm).then(() => {
                var index = this.items.indexOf(item);
                this.boxService.deleteItem(item.id).then(() => {
                    this.$scope.$broadcast('tab-item-remove');
                    this.items.splice(index, 1);
                });
            }).finally(() => {
                disablePaging = false;
            });
        }


        getItems() {
            if (!needToBringMore) {
                return this.$q.when();
            }
            if (disablePaging) {
                return this.$q.when();
            }
            return this.boxService.items(this.$stateParams.boxId, this.$stateParams["tabId"], page).then(response => {
                angular.forEach(response, this.buildItem);
                if (page > 0) {
                    this.items = this.items.concat(response);
                } else {
                    this.items = response;
                }

                if (!response.length) {
                    needToBringMore = false;
                }
                page++;
            });

        }
        private buildItem = (value) => {
            value.url = this.$state.href("item",
                {
                    universityType: this.$stateParams["universityType"],
                    boxId: this.$stateParams["boxId"],
                    boxName: this.$stateParams["boxName"],
                    itemId: value.id,
                    itemName: value.name
                });
            value.downloadLink = value.url + 'download/';
            const retVal = this.itemThumbnailService.assignValue(value.source);
            value.thumbnail = retVal.thumbnail;
            value.nameExtension = value.name.replace(/\.[^/.]+$/, "");
        }
        filter() {
            if (!this.term) {
                this.getItems();
            }
            this.$state.go('box.items', { tabId: null, q: this.term });

        }




        //upload

        getFilter() {
            this.term = this.$stateParams["q"];
            return this.boxService.filterItem(this.$stateParams["q"], this.$stateParams.boxId, 0).then(response => {
                angular.forEach(response, this.buildItem);
                this.items = response;
            });
        }

        private scrollToPosition() {
            var yOffsetParam = this.$stateParams["pageYOffset"];
            if (yOffsetParam) {
                this.$timeout(() => {
                    this.$window.scrollTo(0, yOffsetParam);
                });
            }
        }
    }
    angular.module('app.box.items').controller('ItemsController', ItemsController);
}
