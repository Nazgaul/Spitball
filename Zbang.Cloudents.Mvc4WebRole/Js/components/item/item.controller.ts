﻿module app {
    "use strict";
    enum State {
        Regular = 0,
        Rename = 1,
        Flag = 2
    }
    class Item {

        static $inject = ["itemData", "$state", "$stateParams", "$timeout",
            "itemService", "$mdToast", "$sce", "$location", "user", "showToasterService","resManager"];
        details;
        index = 0;
        needLoadMore = false;
        preview = "";
        selectedState = State.Regular;
        document;
        loader = false;
        showRawText = false; // for ui purpose
        renameText;
        constructor(private itemData,
            private $state: angular.ui.IStateService,
            private $stateParams: spitaball.ISpitballStateParamsService,
            private $timeout: angular.ITimeoutService,
            private itemService: IItemService,
            private $mdToast: angular.material.IToastService,
            private $sce: angular.ISCEService,
            private $location: angular.ILocationService,
            private user: IUserData,
            private showToasterService: IShowToasterService,
            private resManager: IResManager) {

            this.details = itemData;
            const href = this.$state.href(this.$state.current.name, this.$state.current.params);
            this.details.downloadUrl = href + 'download/';
            this.details.printUrl = href + 'print/';
            this.details.boxUrl = $state.href("box.items", angular.extend({}, $stateParams));
            this.getPreview();
            this.document = itemData.fileContent;
            $timeout(() => { this.showLikeToaster() }, 1000);
        }







        followBox() {
            this.itemService.followbox();
            //cacheFactory.clearAll();//autofollow issue
        }
        getPreview() {
            this.loader = true;
            return this.itemService.getPreview(this.details.blob, this.index, this.$stateParams.itemId, this.$stateParams.boxId).then(data => {
                data = data || {};
                this.loader = false;
                var preview: string = data.preview;
                if (preview) {
                    if (preview.indexOf('iframe') > 0
                        || preview.indexOf('audio') > 0
                        || preview.indexOf('video') > 0
                        || preview.indexOf('previewFailed') > 0) {
                        this.preview = this.$sce.trustAsHtml(preview);
                    } else {
                        const element = angular.element(preview);
                        this.preview += preview;
                        if (element.find('img,svg').length === 3) {
                            this.needLoadMore = true;
                        }

                    }

                }
            });
        }
        swipeLeft() {
            if (this.details.next) {

                this.$location.url(this.details.next);
            }
        }


        swipeRight() {
            if (this.details.previous) {
                this.$location.url(this.details.previous);
            }
        }

        showRename() {
            this.selectedState = State.Rename;
            this.renameText = this.details.name;
        }

        loadMore() {
            if (this.needLoadMore && this.user.id) {
                this.needLoadMore = false;
                ++this.index;
                return this.getPreview();
            }
        }
        submitDisabled;
        renameItem() {
            if (this.renameText === this.details.name) {
                this.selectedState = State.Regular;
                return;
            }
            this.submitDisabled = true;
            this.itemService.renameItem(this.renameText, this.$stateParams.itemId).then(response => {
                this.details.name = response.name;
                this.$location.path(response.url).replace();
                this.showToasterService.showToaster(this.resManager.get("renameItem"));
            }).finally(() => {
                this.submitDisabled = false;
            });
        }

        flag;
        customFlag;

        flagItem() {
            this.itemService.flag(this.flag, this.customFlag, this.$stateParams.itemId);
            this.showToasterService.showToaster(this.resManager.get("flagItem"));

            this.cancelFlag();
        }

        cancelFlag() {
            this.flag = '';
            this.selectedState = State.Regular;
        }

        showLikeToaster() {
            this.$mdToast.show({
                hideDelay: 0,
                locals: {
                    userLike: this.details.like
                },
                position: 'top right',
                controller: "likeToasterDialog as lt",
                templateUrl: "likeToasterTemplate.html",
                toastClass: 'angular-animate'
                //parent: element
            }).then(() => {
                this.itemService.like(this.$stateParams.itemId, this.$stateParams.boxId);
            });
        }

    }

    angular.module('app.item').controller('ItemController', Item);

}


