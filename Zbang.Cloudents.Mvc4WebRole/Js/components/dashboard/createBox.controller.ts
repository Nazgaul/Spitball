module app {
    "use strict";

    class CreateBoxController {

        static $inject = ['dashboardService', '$location', '$scope', '$rootScope', 'resManager', 'showToasterService'];

        submitDisabled = false;
        boxName: string;
        error: string;

        constructor(
            private dashboardService: IDashboardService,
            private $location: angular.ILocationService,
            private $scope: angular.IScope,
            private $rootScope: angular.IRootScopeService,
            private resManager: IResManager,
            private showToasterService: IShowToasterService
        ) {

        }


        create(myform: angular.IFormController) {
            this.submitDisabled = true;
            this.dashboardService.createPrivateBox(this.boxName).then(response => {
                this.$scope["d"].createBoxOn = false;
                this.cancel(myform);
                this.$rootScope.$broadcast("refresh-boxes");
                const appController: IAppController = this.$scope["app"];
                this.showToasterService.showToaster(this.resManager.get('toasterCreateBox'));
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

            myform.$setPristine();
            myform.$setUntouched();
            
            this.$scope["d"].createBoxOn = false;
        }
    }
    angular.module("app.dashboard").controller("CreateBoxController", CreateBoxController);
}