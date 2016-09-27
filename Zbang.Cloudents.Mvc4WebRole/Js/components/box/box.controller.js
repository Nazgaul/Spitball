var app;
(function (app) {
    'use strict';
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
            if ($state.current.name === 'box') {
                $state.go('box.feed', $stateParams, { location: "replace" });
            }
            boxId = $stateParams.boxId;
            this.data = boxData;
            this.showLeaderboard = this.isAcademic = boxData.boxType === 'academic' || boxData.boxType === 'academicClosed';
            this.needFollow = boxData.userType === 'invite' || boxData.userType === 'none';
            this.canInvite = boxData.boxType !== 'academicClosed' && this.isAcademic || (boxData.userType === 'owner' && !this.isAcademic);
            this.canShare = boxData.boxType !== 'academicClosed' && this.isAcademic && !this.needFollow;
            this.owner = boxData.userType === 'owner';
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
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            var appController = this.$scope["app"];
            appController.showToaster(this.resManager.get("toasterFollowBox"));
            this.boxService.follow(boxId);
            this.followBox();
        };
        BoxController.prototype.followBox = function () {
            this.needFollow = false;
            this.$rootScope.$broadcast("refresh-boxes");
        };
        BoxController.prototype.updateBox = function (updateBoxForm) {
            var _this = this;
            if (this.settings.needFollow) {
                this.boxService.unfollow(boxId).then(function () {
                    _this.$rootScope.$broadcast("remove-box", boxId);
                    _this.$state.go('dashboard');
                });
                return;
            }
            var needToSave = false;
            angular.forEach(updateBoxForm, function (value, key) {
                if (key[0] === "$")
                    return;
                if (!needToSave) {
                    needToSave = !value.$pristine;
                }
            });
            if (needToSave) {
                this.data.name = this.settings.name;
                this.data.privacySetting = this.settings.privacy;
                if (this.isAcademic) {
                    this.data.courseId = this.settings.courseId;
                    this.data.professorName = this.settings.professorName;
                }
                this.settings.submitDisabled = true;
                this.boxService.updateBox(boxId, this.data.name, this.settings.courseId, this.settings.professorName, this.settings.privacy, this.settings.notificationSettings).then(function (response) {
                    _this.settingsOpen = false;
                    _this.$stateParams["boxName"] = response.queryString;
                    var appController = _this.$scope["app"];
                    appController.showToaster(_this.resManager.get("toasterBoxSettings"));
                    _this.$state.go('box.feed', _this.$stateParams, { location: "replace" });
                }).finally(function () {
                    _this.settings.submitDisabled = false;
                });
            }
        };
        BoxController.prototype.inviteToBox = function () {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            this.$rootScope.$broadcast('close-collapse');
            this.inviteOpen = true;
            this.$scope.$broadcast('open_invite');
        };
        BoxController.prototype.closeCollapse = function () {
            this.$rootScope.$broadcast('close-collapse');
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
                this.settingsOpen = true;
                return;
            }
            return this.ajaxService2.getHtml('/box/boxsettings/').then(function (response) {
                _this.settingsHtml = response;
                _this.$timeout(function () {
                    _this.$rootScope.$broadcast('close-collapse');
                    _this.settingsOpen = true;
                    _this.settings = _this.settings || {};
                    _this.settings.name = _this.data.name;
                    _this.settings.needFollow = _this.needFollow;
                    _this.settings.submitDisabled = false;
                    if (_this.isAcademic) {
                        _this.settings.courseId = _this.data.courseId;
                        _this.settings.professorName = _this.data.professorName;
                    }
                    else if (_this.owner) {
                        _this.settings.privacy = _this.data.privacySetting;
                    }
                    if (!_this.settings.notificationSettings) {
                        _this.boxService.notification(boxId).then(function (response2) {
                            _this.settings.notificationSettings = response2;
                        });
                    }
                });
            });
        };
        BoxController.prototype.canDelete = function (userId) {
            if (this.user.isAdmin || this.user.id === userId) {
                return true;
            }
            return false;
        };
        BoxController.$inject = ["$state", "$stateParams", "boxData", "$scope",
            "$rootScope", "user", "resManager", "boxService", "ajaxService2", "$timeout", "$window", "userUpdatesService"];
        return BoxController;
    }());
    angular.module('app.box').controller('BoxController', BoxController);
})(app || (app = {}));
