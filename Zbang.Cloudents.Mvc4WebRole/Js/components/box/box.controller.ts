module app {
    "use strict";
    var boxId: number;
    export interface IBoxController {
        canDelete(userId);
        toggleSettings();
        inviteExpand();
        isActiveState(state: string);
        closeCollapse();
        inviteToBox();
        follow();
    }

    class BoxController {
        static $inject = ["$state", "$stateParams", "boxData", "$scope",
            "$rootScope", "user", "resManager", "boxService", "ajaxService2",
            "$timeout", "$window", "userUpdatesService","shareService"];
        data;
        showLeaderboard;
        isAcademic;
        needFollow;
        canInvite;
        canShare;
        owner;
        inviteOpen;


        //TODO: To fix that
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
            private userUpdatesService: IUserUpdatesService,
            private shareService: IShareService
        ) {
           
            boxId = $stateParams.boxId;
            this.data = boxData;
            this.showLeaderboard = this.isAcademic = boxData.boxType === "academic" || boxData.boxType === "academicClosed";
            this.needFollow = boxData.userType === "invite" || boxData.userType === "none";
            this.canInvite = boxData.boxType !== "academicClosed" && this.isAcademic || (boxData.userType === "owner" && !this.isAcademic);
            this.canShare = boxData.boxType !== "academicClosed" && this.isAcademic && !this.needFollow;
            this.owner = boxData.userType === "owner";

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
            $scope.$on("$destroy",
                () => {
                    $window.onbeforeunload = null;
                });
            //if ($state.current.name === "box") {
            //    $state.go(".feed", $stateParams, { location: "replace", notify: false });
            //}

        }
        follow() {
            if (!this.user.id) {
                this.$rootScope.$broadcast("show-unregisterd-box");
                return;
            }
            const appController: IAppController = this.$scope["app"];
            appController.showToaster(this.resManager.get("toasterFollowBox"));
            this.boxService.follow(boxId).then(() => {
                this.followBox();
            });

        }

        private followBox() {
            this.needFollow = false;
            this.$rootScope.$broadcast("refresh-boxes");
        }


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

        inviteToBox() {
            if (!this.user.id) {
                this.$rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            if (this.inviteOpen) {
                this.$rootScope.$broadcast("close-collapse");
                this.inviteOpen = false;
            } else {
                this.$rootScope.$broadcast("close-collapse");
                this.inviteOpen = true;
                this.$scope.$broadcast("open_invite");
            }
        }
        closeCollapse() {
            this.$rootScope.$broadcast("close-collapse");
        }
        isActiveState(state: string) {
            return state === this.$state.current.name;
        }
        inviteExpand() {
            if (this.html) {
                return;
            }
            return this.ajaxService2.getHtml("/share/invitedialog/").then((response: string) => {
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
                if (this.settingsOpen) {
                    this.settingsOpen = false;
                }
                else {
                    this.settingsOpen = true;
                }
                return;
            }
            return this.ajaxService2.getHtml('/box/boxsettings/').then(response => {
                this.settingsHtml = response;
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

        }


        //stuff for child elements
        canDelete(userId) {
            if (this.user.isAdmin || this.user.id === userId) {
                return true;
            }
            return false;
        }

        share() {
            this.settingsOpen = this.inviteOpen = false;
            this.shareService.shareDialog("b", boxId);
        }


    }

    angular.module('app.box').controller('BoxController', BoxController);
}