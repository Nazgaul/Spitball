(function () {
    angular.module('app.account').controller('AccountSettingsNotificationController', notification);

    notification.$inject = ['accountService', '$mdToast', '$document', 'resManager'];
    function notification(accountService, $mdToast, $document, resManager) {
        var self = this;


        accountService.getNotification().then(function(response) {
            self.notifications = response;
        });

        self.updateNotification = function (box) {
            accountService.setNotification(box.id, box.notifications).then(function () {
                showToast(resManager.get('settingsDone'));
            });
        }

        function showToast(messae) {
            $mdToast.show(
                   $mdToast.simple()
                   .textContent(messae)
                   .position('top')
                   .parent($document[0].querySelector('#accountPage'))
                   .hideDelay(3000));
        }

    }
})();