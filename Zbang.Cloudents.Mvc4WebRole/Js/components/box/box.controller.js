var app;
(function (app) {
    "use strict";
    var boxId;
    var BoxController = (function () {
        function BoxController($state, $stateParams, boxData, $scope, $rootScope, user, resManager, boxService, ajaxService2, $timeout, $window, userUpdatesService) {
            var _this = this;
            this.$state = $state;
            this.$stateParams = $stateParams;
            this.boxData = boxData;
            this.$scope = $scope;
            this.$rootScope = $rootScope;
            this.user = user;
            this.resManager = resManager;
            this.boxService = boxService;
            this.ajaxService2 = ajaxService2;
            this.$timeout = $timeout;
            this.$window = $window;
            this.userUpdatesService = userUpdatesService;
            if ($state.current.name === "box") {
                $state.go("box.feed", $stateParams, { location: "replace" });
            }
            boxId = $stateParams.boxId;
            this.data = boxData;
            this.showLeaderboard = this.isAcademic = boxData.boxType === "academic" || boxData.boxType === "academicClosed";
            this.needFollow = boxData.userType === "invite" || boxData.userType === "none";
            this.canInvite = boxData.boxType !== "academicClosed" && this.isAcademic || (boxData.userType === "owner" && !this.isAcademic);
            this.canShare = boxData.boxType !== "academicClosed" && this.isAcademic && !this.needFollow;
            this.owner = boxData.userType === "owner";
            $scope.$on("close_invite", function () {
                _this.inviteOpen = false;
            });
            $scope.$on("follow-box", function () {
                _this.followBox();
            });
            $scope.$on("close-collapse", function () {
                _this.inviteOpen = false;
                _this.settingsOpen = false;
            });
            $scope.$on("hide-leader-board", function () {
                _this.showLeaderboard = false;
            });
            $window.onbeforeunload = function () {
                if (!_this.user.id) {
                    return;
                }
                userUpdatesService.deleteUpdates(boxId);
            };
        }
        BoxController.prototype.follow = function () {
            var _this = this;
            if (!this.user.id) {
                this.$rootScope.$broadcast("show-unregisterd-box");
                return;
            }
            var appController = this.$scope["app"];
            appController.showToaster(this.resManager.get("toasterFollowBox"));
            this.boxService.follow(boxId).then(function () {
                _this.followBox();
            });
        };
        BoxController.prototype.followBox = function () {
            this.needFollow = false;
            this.$rootScope.$broadcast("refresh-boxes");
        };
        BoxController.prototype.inviteToBox = function () {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            if (this.inviteOpen) {
                this.$rootScope.$broadcast("close-collapse");
                this.inviteOpen = false;
            }
            else {
                this.$rootScope.$broadcast("close-collapse");
                this.inviteOpen = true;
                this.$scope.$broadcast("open_invite");
            }
        };
        BoxController.prototype.closeCollapse = function () {
            this.$rootScope.$broadcast("close-collapse");
        };
        BoxController.prototype.isActiveState = function (state) {
            return state === this.$state.current.name;
        };
        BoxController.prototype.inviteExpand = function () {
            var _this = this;
            if (this.html) {
                return;
            }
            return this.ajaxService2.getHtml("/share/invitedialog/").then(function (response) {
                _this.html = response;
                _this.$timeout(function () {
                    _this.$scope.$broadcast("open_invite");
                });
            });
        };
        BoxController.prototype.toggleSettings = function () {
            var _this = this;
            if (this.needFollow) {
                return;
            }
            if (this.settingsHtml) {
                if (this.settingsOpen) {
                    this.settingsOpen = false;
                }
                else {
                    this.settingsOpen = true;
                }
                return;
            }
            return this.ajaxService2.getHtml('/box/boxsettings/').then(function (response) {
                _this.settingsHtml = response;
            });
        };
        BoxController.prototype.canDelete = function (userId) {
            if (this.user.isAdmin || this.user.id === userId) {
                return true;
            }
            return false;
        };
        BoxController.$inject = ["$state", "$stateParams", "boxData", "$scope",
            "$rootScope", "user", "resManager", "boxService", "ajaxService2",
            "$timeout", "$window", "userUpdatesService"];
        return BoxController;
    }());
    angular.module('app.box').controller('BoxController', BoxController);
})(app || (app = {}));
