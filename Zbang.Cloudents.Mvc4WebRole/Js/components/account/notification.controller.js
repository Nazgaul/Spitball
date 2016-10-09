(function () {
'use strict';
    angular.module('app.account').controller('AccountSettingsNotificationController', notification);

    notification.$inject = ['accountService', '$document', 'resManager', '$scope'];
    function notification(accountService, $document, resManager, $scope) {
        var self = this;

        accountService.getNotification().then(function (response) {
            self.notifications = response.boxNotifications;

            self.emailNotification = response.emailNotification;
            //self.notifications = response.boxNotifiactions;

        });

        self.updateSubsription = function() {
            accountService.setPersonalNotification(self.emailNotification);
            showToast(resManager.get('settingsDone'));
        }

        self.updateNotification = function (box) {
            accountService.setNotification(box.id, box.notifications).then(function () {
                showToast(resManager.get('settingsDone'));
            });
        }

        function showToast(message) {
            $scope.app.showToaster(message, 'notification');

        }

    }
})();