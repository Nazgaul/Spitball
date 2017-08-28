"use strict";
var app;
(function (app) {
    "use strict";
    var boxId;
    var BoxController = (function () {
        function BoxController($state, $stateParams, boxData, $scope, $rootScope, user, resManager, boxService, ajaxService2, $timeout, $window, userUpdatesService, shareService, showToasterService, $mdDialog) {
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
            this.shareService = shareService;
            this.showToasterService = showToasterService;
            this.$mdDialog = $mdDialog;
            boxId = $stateParams.boxId;
            this.data = boxData;
            this.isAcademic = boxData.boxType === 'academic' || boxData.boxType === 'academicClosed';
            this.needFollow = boxData.userType === "invite" || boxData.userType === "none";
            this.canInvite = boxData.boxType !== "academicClosed" &&
                this.isAcademic || (boxData.userType === "owner" && !this.isAcademic);
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
            $window.onbeforeunload = function () {
                if (!_this.user.id) {
                    return;
                }
                userUpdatesService.deleteUpdates(boxId);
            };
            $scope.$on("$destroy", function () {
                $window.onbeforeunload = null;
            });
        }
        BoxController.prototype.follow = function () {
            var _this = this;
            if (!this.user.id) {
                this.$rootScope.$broadcast("show-unregisterd-box");
                return;
            }
            var appController = this.$scope["app"];
            this.showToasterService.showToaster(this.resManager.get("toasterFollowBox"));
            this.boxService.follow(boxId).then(function () {
                _this.followBox();
            });
        };
        BoxController.prototype.followBox = function () {
            this.needFollow = false;
            this.$rootScope.$broadcast("refresh-boxes");
        };
        //updateBox(updateBoxForm: angular.IFormController) {
        //    if (this.settings.needFollow) {
        //        this.boxService.unfollow(boxId).then(() => {
        //            this.$rootScope.$broadcast("remove-box", boxId);
        //            this.$state.go("dashboard");
        //        });
        //        return;
        //    }
        //    var needToSave = false;
        //    angular.forEach(updateBoxForm, (value, key) => {
        //        if (key[0] === "$") return;
        //        if (!needToSave) {
        //            needToSave = !value.$pristine;
        //        }
        //    });
        //    if (needToSave) {
        //        this.data.name = this.settings.name;
        //        this.data.privacySetting = this.settings.privacy;
        //        if (this.isAcademic) {
        //            this.data.courseId = this.settings.courseId;
        //            this.data.professorName = this.settings.professorName;
        //        }
        //        this.settings.submitDisabled = true;
        //        this.boxService.updateBox(boxId, this.data.name, this.settings.courseId, this.settings.professorName, this.settings.privacy, this.settings.notificationSettings).then(response => {
        //            this.settingsOpen = false;
        //            this.$stateParams["boxName"] = response.queryString;
        //            const appController: IAppController = this.$scope["app"];
        //            appController.showToaster(this.resManager.get("toasterBoxSettings"));
        //            this.$state.go('box.feed', this.$stateParams, { location: "replace" });
        //        }).finally(() => {
        //            this.settings.submitDisabled = false;
        //        });
        //    }
        //}
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
                //        this.$timeout(() => {
                //            this.$rootScope.$broadcast('close-collapse');
                //            this.settingsOpen = true;
                //            this.settings = this.settings || {};
                //            //boxType
                //            //privacySetting
                //            this.settings.name = this.data.name;
                //            this.settings.needFollow = this.needFollow;
                //            this.settings.submitDisabled = false;
                //            if (this.isAcademic) {
                //                this.settings.courseId = this.data.courseId;
                //                this.settings.professorName = this.data.professorName;
                //            } else if (this.owner) {
                //                this.settings.privacy = this.data.privacySetting;
                //            }
                //            if (!this.settings.notificationSettings) {
                //                this.boxService.notification(boxId).then(response2 => {
                //                    this.settings.notificationSettings = response2;
                //                });
                //            }
                //        });
            });
        };
        //stuff for child elements
        BoxController.prototype.canDelete = function (userId) {
            if (this.user.isAdmin || this.user.id === userId) {
                return true;
            }
            return false;
        };
        BoxController.prototype.share = function () {
            this.settingsOpen = this.inviteOpen = false;
            this.shareService.shareDialog("b", boxId);
        };
        BoxController.prototype.showLeaderboard = function () {
            this.$mdDialog.show({
                templateUrl: "/box/leaderboardpartial/",
                clickOutsideToClose: true,
                controller: "BoxLeaderboard",
                controllerAs: "g",
                fullscreen: false
            });
        };
        BoxController.$inject = ["$state", "$stateParams", "boxData", "$scope",
            "$rootScope", "user", "resManager", "boxService", "ajaxService2",
            "$timeout", "$window", "userUpdatesService", "shareService",
            "showToasterService", "$mdDialog"];
        return BoxController;
    }());
    angular.module('app.box').controller('BoxController', BoxController);
})(app || (app = {}));
//# sourceMappingURL=box.controller.js.map