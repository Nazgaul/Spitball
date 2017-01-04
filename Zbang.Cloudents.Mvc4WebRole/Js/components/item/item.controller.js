var app;
(function (app) {
    "use strict";
    var State;
    (function (State) {
        State[State["Regular"] = 0] = "Regular";
        State[State["Rename"] = 1] = "Rename";
        State[State["Flag"] = 2] = "Flag";
    })(State || (State = {}));
    var promise;
    var Item = (function () {
        function Item(itemData, $state, $stateParams, $timeout, itemService, $mdToast, $sce, $location, user, showToasterService, resManager, $scope) {
            var _this = this;
            this.itemData = itemData;
            this.$state = $state;
            this.$stateParams = $stateParams;
            this.$timeout = $timeout;
            this.itemService = itemService;
            this.$mdToast = $mdToast;
            this.$sce = $sce;
            this.$location = $location;
            this.user = user;
            this.showToasterService = showToasterService;
            this.resManager = resManager;
            this.$scope = $scope;
            this.index = 0;
            this.needLoadMore = false;
            this.preview = "";
            this.selectedState = State.Regular;
            this.loader = false;
            this.showRawText = false;
            this.view = "preview-svg.html";
            this.documents = [];
            this.details = itemData;
            var href = this.$state.href(this.$state.current.name, this.$state.current.params);
            this.details.downloadUrl = href + 'download/';
            this.details.printUrl = href + 'print/';
            this.details.boxUrl = $state.href("box.items", angular.extend({}, $stateParams));
            this.getPreview();
            this.document = itemData.fileContent;
            if (!this.details.like && user.id) {
                promise = $timeout(function () { _this.showLikeToaster(); }, 10000);
            }
            this.$scope.$on('$destroy', function () {
                $timeout.cancel(promise);
            });
        }
        Item.prototype.followBox = function () {
            this.itemService.followbox();
        };
        Item.prototype.getPreview = function () {
            var amount = 15;
            var self = this;
            self.loader = true;
            return self.itemService.getPreview(self.details.blob, self.index * amount, self.$stateParams.itemId, self.$stateParams.boxId).then(function (data) {
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
                var preview = data.preview;
                if (preview) {
                    if (preview.indexOf('iframe') > 0
                        || preview.indexOf('audio') > 0
                        || preview.indexOf('video') > 0
                        || preview.indexOf('<a') >= 0) {
                        self.preview = self.$sce.trustAsHtml(preview);
                    }
                    else {
                        self.view = "preview-" + preview.toLowerCase() + ".html";
                        self.documents = self.documents.concat(data.content);
                        if (data.content.length > 15) {
                            self.needLoadMore = true;
                        }
                    }
                }
            });
        };
        Item.prototype.swipeLeft = function () {
            if (this.details.next) {
                this.$location.url(this.details.next);
            }
        };
        Item.prototype.swipeRight = function () {
            if (this.details.previous) {
                this.$location.url(this.details.previous);
            }
        };
        Item.prototype.showRename = function () {
            this.selectedState = State.Rename;
            this.renameText = this.details.name;
        };
        Item.prototype.loadMore = function () {
            if (this.needLoadMore && this.user.id) {
                this.needLoadMore = false;
                ++this.index;
                return this.getPreview();
            }
        };
        Item.prototype.renameItem = function () {
            var _this = this;
            if (this.renameText === this.details.name) {
                this.selectedState = State.Regular;
                return;
            }
            this.submitDisabled = true;
            this.itemService.renameItem(this.renameText, this.$stateParams.itemId).then(function (response) {
                _this.details.name = response.name;
                _this.$location.path(response.url).replace();
                _this.showToasterService.showToaster(_this.resManager.get("renameItem"));
                _this.selectedState = State.Regular;
            }).finally(function () {
                _this.submitDisabled = false;
            });
        };
        Item.prototype.flagItem = function () {
            this.itemService.flag(this.flag, this.customFlag, this.$stateParams.itemId);
            this.showToasterService.showToaster(this.resManager.get("flagItem"));
            this.cancelFlag();
        };
        Item.prototype.cancelFlag = function () {
            this.flag = '';
            this.selectedState = State.Regular;
        };
        Item.prototype.showLikeToaster = function () {
            var _this = this;
            this.$mdToast.show({
                hideDelay: 0,
                locals: {
                    userLike: this.details.like
                },
                position: 'top right',
                controller: "likeToasterDialog as lt",
                templateUrl: "likeToasterTemplate.html",
                toastClass: 'angular-animate'
            }).then(function (res) {
                if (res) {
                    _this.itemService.like(_this.$stateParams.itemId, _this.$stateParams.boxId);
                }
            });
        };
        return Item;
    }());
    Item.$inject = ["itemData", "$state", "$stateParams", "$timeout",
        "itemService", "$mdToast", "$sce", "$location", "user", "showToasterService", "resManager", "$scope"];
    angular.module('app.item').controller('ItemController', Item);
})(app || (app = {}));
//# sourceMappingURL=item.controller.js.map