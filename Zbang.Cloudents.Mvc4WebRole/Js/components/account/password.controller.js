(function () {
    'use strict';
    angular.module('app.account').controller('AccountSettingsPasswordController', password);

    password.$inject = ['accountService', '$mdToast', '$document', '$scope', 'resManager'];
    function password(accountService, $mdToast, $document, $scope, resManager) {
        var self = this;


        self.submit = function (myform) {
            accountService.updatePassword(self.old, self.new).then(function () {
         
                self.old = '';
                self.new = '';
                $scope.app.resetForm(myform);
                $scope.app.showToaster(resManager.get('passwordChangeSuccess'), 'accountPage');
            }, function (response) {
                myform.old.$setValidity('server',false);
                self.error = response;
            });
        }

        self.cancel = cancel;

        function cancel(myform) {
            self.old = '';
            self.new = '';
            $scope.app.resetForm(myform);
        }
    }
})();