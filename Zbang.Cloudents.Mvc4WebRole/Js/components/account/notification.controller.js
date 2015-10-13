(function () {
    angular.module('app.account').controller('AccountSettingsNotificationController', notification);

    notification.$inject = ['accountService'];
    function notification(accountService) {
        var self = this;


        accountService.getNotification().then(function(response) {
            self.notifications = response;
        });

        self.updateNotification = function (box) {
            accountService.setNotification(box.id, box.notifications).then(function () {
                alert('complete');
            });
        }

    }
})();