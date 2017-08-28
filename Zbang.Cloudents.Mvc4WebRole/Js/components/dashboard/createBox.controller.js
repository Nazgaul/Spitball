"use strict";
var app;
(function (app) {
    "use strict";
    var CreateBoxController = (function () {
        function CreateBoxController(dashboardService, $location, $scope, $rootScope, resManager, showToasterService) {
            this.dashboardService = dashboardService;
            this.$location = $location;
            this.$scope = $scope;
            this.$rootScope = $rootScope;
            this.resManager = resManager;
            this.showToasterService = showToasterService;
            this.submitDisabled = false;
        }
        CreateBoxController.prototype.create = function (myform) {
            var _this = this;
            this.submitDisabled = true;
            this.dashboardService.createPrivateBox(this.boxName).then(function (response) {
                _this.$scope["d"].createBoxOn = false;
                _this.cancel(myform);
                _this.$rootScope.$broadcast("refresh-boxes");
                var appController = _this.$scope["app"];
                _this.showToasterService.showToaster(_this.resManager.get('toasterCreateBox'));
                _this.$location.url(response.url);
            }, function (response) {
                myform["name"].$setValidity('server', false);
                _this.error = response;
            }).finally(function () {
                _this.submitDisabled = false;
            });
        };
        ;
        CreateBoxController.prototype.cancel = function (myform) {
            this.boxName = '';
            myform.$setPristine();
            myform.$setUntouched();
            this.$scope["d"].createBoxOn = false;
        };
        CreateBoxController.$inject = ['dashboardService', '$location', '$scope', '$rootScope', 'resManager', 'showToasterService'];
        return CreateBoxController;
    }());
    angular.module("app.dashboard").controller("CreateBoxController", CreateBoxController);
})(app || (app = {}));
//# sourceMappingURL=createBox.controller.js.map