'use strict';
(function () {
    angular.module('app.account').controller('AccountSettingsNotificationController', notification);

    notification.$inject = ['accountService', '$document', 'resManager', '$scope'];
    function notification(accountService, $document, resManager, $scope) {
        var self = this;

        accountService.getNotification().then(function (response) {
            self.notifications = response;
            //self.emailNotification = response.emailNotifiactions;
            //self.notifications = response.boxNotifiactions;

        });

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