module app {
    "use strict";

    class CreateBoxController {

        static $inject = ['dashboardService', '$location', '$scope', '$rootScope', 'resManager'];

        submitDisabled = false;
        boxName: string;
        error: string;

        constructor(
            private dashboardService: IDashboardService,
            private $location: angular.ILocationService,
            private $scope: angular.IScope,
            private $rootScope: angular.IRootScopeService,
            private resManager: IResManager) {

        }


        create(myform: angular.IFormController) {
            this.submitDisabled = true;
            this.dashboardService.createPrivateBox(this.boxName).then(response => {
                this.$scope["d"].createBoxOn = false;
                this.cancel(myform);
                this.$rootScope.$broadcast("refresh-boxes");
                const appController: IAppController = this.$scope["app"];
                appController.showToaster(this.resManager.get('toasterCreateBox'));
                this.$location.url(response.url);
            }, response => {
                myform["name"].$setValidity('server', false);
                this.error = response;
            }).finally(() => {
                this.submitDisabled = false;
            });
        };

        cancel(myform: angular.IFormController) {
            this.boxName = '';
            const appController: IAppController = this.$scope["app"];
            appController.resetForm(myform);
            this.$scope["d"].createBoxOn = false;
        }
    }
    angular.module("app.dashboard").controller("CreateBoxController", CreateBoxController);
}