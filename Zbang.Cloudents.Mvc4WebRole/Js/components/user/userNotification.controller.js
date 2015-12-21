(function() {
    angular.module('app.user.details').controller('UserNotificationController', notification);

    notification.$inject = ['userDetailsFactory', 'userService'];

    function notification(userDetailsFactory, userService) {
        var un = this;

        userDetailsFactory.init().then(function () {
            if (!userDetailsFactory.isAuthenticated()) {
                return;
            }
            userService.getNotification(function(response) {
            });
        });
    }
})()