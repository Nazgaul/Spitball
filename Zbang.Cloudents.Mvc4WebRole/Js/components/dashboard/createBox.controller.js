'use strict';
(function () {
    angular.module('app.dashboard').controller('CreateBoxController', controller);

    controller.$inject = ['dashboardService', '$location', '$scope', '$rootScope', 'resManager'];
    function controller(dashboardService, $location, $scope, $rootScope, resManager) {
        var self = this;
        self.create = create;
        self.cancel = cancel;
        self.submitDisabled = false;

        function create(myform) {
            self.submitDisabled = true;
            dashboardService.createPrivateBox(self.boxName).then(function (response) {
                $scope.d.createBoxOn = false;
                cancel(myform);
                $rootScope.$broadcast("refresh-boxes");
                $scope.app.showToaster(resManager.get('toasterCreateBox'));
                $location.url(response.url);
            }, function (response) {
                myform.name.$setValidity('server', false);
                self.error = response;
            }).finally(function() {
                self.submitDisabled = false;
            });
        };

        function cancel(myform) {
            self.boxName = '';
            $scope.app.resetForm(myform);
            $scope.d.createBoxOn  = false;
        }
    }

})()