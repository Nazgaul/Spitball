module app {
    "use strict";
    enum State {
        Regular = 0,
        Rename = 1,
        Flag = 2
    }
    var promise;
    class Item {

        static $inject = ["itemData", "$state", "$stateParams", "$timeout",
            "itemService", "$mdToast", "$sce", "$location", "user", "showToasterService", "resManager", "$scope"];
        details;
        index = 0;
        needLoadMore = false;
        preview = "";
        selectedState = State.Regular;
        document;
        loader = false;
        showRawText = false; // for ui purpose
        renameText;

        view = "preview-svg.html";
        documents = [];

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
            private resManager: IResManager,
            private $scope: angular.IScope) {

            this.details = itemData;
            const href = this.$state.href(this.$state.current.name, this.$state.current.params);
            this.details.downloadUrl = href + 'download/';
            this.details.printUrl = href + 'print/';
            this.details.boxUrl = $state.href("box.items", angular.extend({}, $stateParams));
            this.getPreview();
            this.document = itemData.fileContent;
            if (!this.details.like && user.id) {
                promise = $timeout(() => { this.showLikeToaster() }, 10000);
            }
            this.$scope.$on('$destroy', () => {
                $timeout.cancel(promise);
            });
        }


        followBox() {
            this.itemService.followbox();
        }
        getPreview() {
            const amount = 15;
            var self = this;
            self.loader = true;
            return self.itemService.getPreview(self.details.blob, self.index * amount, self.$stateParams.itemId, self.$stateParams.boxId).then(data => {
                data = data || {};
                self.loader = false;
                if (data.template) {
                    self.view = "preview-" + data.template.toLowerCase() + ".html";
                    if (data.content) {
                        self.documents = self.documents.concat(data.content);
                        if (data.content.length >= amount) {
                            self.needLoadMore = true;
                        }
                    }
                    return;
                }
                var preview: string = data.preview;
                if (preview) {
                    if (preview.indexOf('iframe') > 0
                        || preview.indexOf('audio') > 0
                        || preview.indexOf('video') > 0
                        || preview.indexOf('<a') >= 0
                        /*|| preview.indexOf('previewFailed') > 0*/) {
                        //self.document.push(self.$sce.trustAsHtml(preview));
                        self.preview = self.$sce.trustAsHtml(preview);
                    } else {
                        // TODO: check why
                        self.view = "preview-" + preview.toLowerCase() + ".html";
                        self.documents = self.documents.concat(data.content);
                        if (data.content.length > 15) {
                            self.needLoadMore = true;
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
                this.selectedState = State.Regular;
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
            }).then((res) => {
                if (res) {
                    this.itemService.like(this.$stateParams.itemId, this.$stateParams.boxId);
                }
            });

        }
    }

    angular.module('app.item').controller('ItemController', Item);

}


