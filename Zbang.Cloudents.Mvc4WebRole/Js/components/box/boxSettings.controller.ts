module app {
    "use strict";
    var boxId: number;
    class BoxSettingsController {
        static $inject = ["$state", "$stateParams", "$scope",
            "$rootScope", "resManager", "boxService", "ajaxService2", "$timeout"];

        //TODO: remove to different controller
        settings;
        

        html;

        constructor(
            private $state: angular.ui.IStateService,
            private $stateParams: spitaball.ISpitballStateParamsService,
            private $scope: angular.IScope,
            private $rootScope: angular.IRootScopeService,
            private resManager: IResManager,
            private boxService: IBoxService,
            private ajaxService2: IAjaxService2,
            private $timeout: angular.ITimeoutService
        ) {

            boxId = $stateParams.boxId;

            $scope.$on("close-collapse", () => {
                const boxController: IBoxController = this.$scope["b"];
                boxController["inviteOpen"] = false;
                boxController["settingsOpen"] = false;
            });


            this.$timeout(() => {
                const boxController: IBoxController = this.$scope["b"];
                this.$rootScope.$broadcast('close-collapse');
                boxController["settingsOpen"] = true;

                this.settings = this.settings || {};
                //boxType
                //privacySetting

                this.settings.name = boxController["data"].name;
                this.settings.needFollow = boxController["needFollow"];
                this.settings.submitDisabled = false;
                if (boxController["isAcademic"]) {
                    this.settings.courseId = boxController["data"].courseId;
                    this.settings.professorName = boxController["data"].professorName;
                } else if (boxController["owner"]) {
                    this.settings.privacy = boxController["data"].privacySetting;
                }

                if (!this.settings.notificationSettings) {
                    this.boxService.notification(boxId).then(response2 => {
                        this.settings.notificationSettings = response2;
                    });
                }
            });
        }

        updateBox(updateBoxForm: angular.IFormController) {
            if (this.settings.needFollow) {
                this.boxService.unfollow(boxId).then(() => {
                    this.$rootScope.$broadcast("remove-box", boxId);
                    this.$state.go("dashboard");
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

                const boxController: IBoxController = this.$scope["b"];
                boxController["data"].name = this.settings.name;
                boxController["data"].privacySetting = this.settings.privacy;
                if (boxController["isAcademic"]) {
                    boxController["data"].courseId = this.settings.courseId;
                    boxController["data"].professorName = this.settings.professorName;
                }
                this.settings.submitDisabled = true;
                this.boxService.updateBox(boxId, boxController["data"].name, this.settings.courseId, this.settings.professorName, this.settings.privacy, this.settings.notificationSettings).then(response => {
                    const appController: IAppController = this.$scope["app"];
                    boxController["settingsOpen"] = false;
                    this.$stateParams["boxName"] = response.queryString;
                    appController.showToaster(this.resManager.get("toasterBoxSettings"));
                    this.$state.go('box.feed', this.$stateParams, { location: "replace" });

                }).finally(() => {
                    this.settings.submitDisabled = false;
                });

            }
        }



    }

    angular.module('app.box').controller('BoxSettingsController', BoxSettingsController);
}