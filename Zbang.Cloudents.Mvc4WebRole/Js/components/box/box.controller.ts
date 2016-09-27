module app {
    "use strict";
    var boxId: number;
    class BoxController {
        static $inject = ["$state", "$stateParams", "boxData", "$scope",
            "$rootScope", "user", "resManager", "boxService", "ajaxService2", "$timeout", "$window", "userUpdatesService"];
        data;
        showLeaderboard;
        isAcademic;
        needFollow;
        canInvite;
        canShare;
        owner;
        inviteOpen;

        //TODO: remove to different controller
        settings;
        settingsOpen;
        settingsHtml;

        html;

        constructor(
            private $state: angular.ui.IStateService,
            private $stateParams: spitaball.ISpitballStateParamsService,
            private boxData,
            private $scope: angular.IScope,
            private $rootScope: angular.IRootScopeService,
            private user: IUserData,
            private resManager: IResManager,
            private boxService: IBoxService,
            private ajaxService2: IAjaxService2,
            private $timeout: angular.ITimeoutService,
            private $window: angular.IWindowService,
            private userUpdatesService: IUserUpdatesService
        ) {
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

            $scope.$on("close_invite", () => {
                this.inviteOpen = false;

            });

            $scope.$on("follow-box",
                () => {
                    this.followBox();
                });
            $scope.$on("close-collapse", () => {
                this.inviteOpen = false;
                this.settingsOpen = false;
            });

            $scope.$on("hide-leader-board", () => {
                this.showLeaderboard = false;
            });

            $window.onbeforeunload = () => {
                if (!this.user.id) {
                    return;
                }
                userUpdatesService.deleteUpdates(boxId);

            };
        }
        follow() {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            const appController: IAppController = this.$scope["app"];
            appController.showToaster(this.resManager.get("toasterFollowBox"));
            this.boxService.follow(boxId);
            this.followBox();

        }

        private followBox() {
            this.needFollow = false;
            this.$rootScope.$broadcast("refresh-boxes");
        }


        updateBox(updateBoxForm: angular.IFormController) {
            if (this.settings.needFollow) {
                this.boxService.unfollow(boxId).then(() => {
                    this.$rootScope.$broadcast("remove-box", boxId);
                    this.$state.go('dashboard');
                });
                return;
            }
            var needToSave = false;
            angular.forEach(updateBoxForm, (value, key) => {
                if (key[0] === "$") return;
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
                this.boxService.updateBox(boxId, this.data.name, this.settings.courseId, this.settings.professorName, this.settings.privacy, this.settings.notificationSettings).then(response => {
                    this.settingsOpen = false;
                    this.$stateParams["boxName"] = response.queryString;
                    const appController: IAppController = this.$scope["app"];
                    appController.showToaster(this.resManager.get("toasterBoxSettings"));
                    this.$state.go('box.feed', this.$stateParams, { location: "replace" });

                }).finally(() => {
                    this.settings.submitDisabled = false;
                });

            }
        }

        inviteToBox() {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            this.$rootScope.$broadcast('close-collapse');
            this.inviteOpen = true;
            this.$scope.$broadcast('open_invite');
        }
        closeCollapse() {
            this.$rootScope.$broadcast('close-collapse');
        }
        isActiveState(state: string) {
            return state === this.$state.current.name;
        }
        inviteExpand() {
            if (this.html) {
                return;
            }
            return this.ajaxService2.getHtml("/share/invitedialog/").then(response => {
                this.html = response;
                this.$timeout(() => {
                    this.$scope.$broadcast("open_invite");
                });
            });
        }
        toggleSettings() {
            if (this.needFollow) {
                return;
            }

            if (this.settingsHtml) {
                this.settingsOpen = true;
                return;
            }
            return this.ajaxService2.getHtml('/box/boxsettings/').then(response => {
                this.settingsHtml = response;
                this.$timeout(() => {
                    this.$rootScope.$broadcast('close-collapse');
                    this.settingsOpen = true;

                    this.settings = this.settings || {};
                    //boxType
                    //privacySetting

                    this.settings.name = this.data.name;
                    this.settings.needFollow = this.needFollow;
                    this.settings.submitDisabled = false;
                    if (this.isAcademic) {
                        this.settings.courseId = this.data.courseId;
                        this.settings.professorName = this.data.professorName;
                    } else if (this.owner) {
                        this.settings.privacy = this.data.privacySetting;
                    }

                    if (!this.settings.notificationSettings) {
                        this.boxService.notification(boxId).then(response2 => {
                            this.settings.notificationSettings = response2;
                        });
                    }
                });
            });

        }
        //stuff for child elements
        canDelete(userId) {
            if (this.user.isAdmin || this.user.id === userId) {
                return true;
            }
            return false;
        }


    }

    angular.module('app.box').controller('BoxController', BoxController);
}

//(function () {
//    //angular.module('app.box').controller('BoxController', box);
//    //box.$inject = ['boxService', 'boxData', '$stateParams', '$scope',
//    //    '$state', '$rootScope', 'userDetailsFactory',
//    //    'resManager', '$timeout', 'userUpdatesService', '$window', 'ajaxService2'];
//    //function box(boxService, boxData, $stateParams, $scope, $state,
//    //    $rootScope, userDetailsFactory, resManager, $timeout, userUpdatesService, $window, ajaxService) {

//    //if ($state.current.name === 'box') {
//    //    $state.go('box.feed', $stateParams, { location: "replace" });
//    //}
//    //var b = this, boxId = $stateParams.boxId;
//    // b.data = boxData;
//    //b.showLeaderboard = b.isAcademic = boxData.boxType === 'academic' || boxData.boxType === 'academicClosed';
//    //b.needFollow = boxData.userType === 'invite' || boxData.userType === 'none';
//    //b.canInvite = boxData.boxType !== 'academicClosed' && b.isAcademic || (boxData.userType === 'owner' && !b.isAcademic);
//    //b.canShare = boxData.boxType !== 'academicClosed' && b.isAcademic && !b.needFollow;
//    //b.owner = boxData.userType === 'owner';
//    //b.follow = follow;
//    //b.updateBox = updateBox;
//    //b.inviteToBox = inviteToBox;
//    //b.closeCollapse = closeCollapse;
//    //b.user = userDetailsFactory.get();
//    ////b.url = b.user.url;
//    ////b.image = b.user.image;
//    //b.isActiveState = isActiveState;
//    //b.inviteExpand = inviteExpand;
//    //b.toggleSettings = toggleSettings;
//    //
//    ////stuff for child elements
//    //b.canDelete = canDelete;



















//}
//})();


