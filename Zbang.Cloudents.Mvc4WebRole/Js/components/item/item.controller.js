var app;
(function (app) {
    "use strict";
    var State;
    (function (State) {
        State[State["Regular"] = 0] = "Regular";
        State[State["Rename"] = 1] = "Rename";
        State[State["Flag"] = 2] = "Flag";
    })(State || (State = {}));
    var Item = (function () {
        function Item(itemData, $state, $stateParams, $timeout, itemService, $mdToast, $sce, $location, user, showToasterService, resManager) {
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
            this.index = 0;
            this.needLoadMore = false;
            this.preview = "";
            this.selectedState = State.Regular;
            this.loader = false;
            this.showRawText = false;
            this.details = itemData;
            var href = this.$state.href(this.$state.current.name, this.$state.current.params);
            this.details.downloadUrl = href + 'download/';
            this.details.printUrl = href + 'print/';
            this.details.boxUrl = $state.href("box.items", angular.extend({}, $stateParams));
            this.getPreview();
            this.document = itemData.fileContent;
            $timeout(function () { _this.showLikeToaster(); }, 1000);
        }
        Item.prototype.followBox = function () {
            this.itemService.followbox();
        };
        Item.prototype.getPreview = function () {
            var _this = this;
            this.loader = true;
            return this.itemService.getPreview(this.details.blob, this.index, this.$stateParams.itemId, this.$stateParams.boxId).then(function (data) {
                data = data || {};
                _this.loader = false;
                var preview = data.preview;
                if (preview) {
                    if (preview.indexOf('iframe') > 0
                        || preview.indexOf('audio') > 0
                        || preview.indexOf('video') > 0
                        || preview.indexOf('previewFailed') > 0) {
                        _this.preview = _this.$sce.trustAsHtml(preview);
                    }
                    else {
                        var element = angular.element(preview);
                        _this.preview += preview;
                        if (element.find('img,svg').length === 3) {
                            _this.needLoadMore = true;
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
            }).then(function () {
                _this.itemService.like(_this.$stateParams.itemId, _this.$stateParams.boxId);
            });
        };
        Item.$inject = ["itemData", "$state", "$stateParams", "$timeout",
            "itemService", "$mdToast", "$sce", "$location", "user", "showToasterService", "resManager"];
        return Item;
    }());
    angular.module('app.item').controller('ItemController', Item);
})(app || (app = {}));
