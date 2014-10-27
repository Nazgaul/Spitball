mAccount.controller('NotificationsCtrl',
    ['$scope', 'sShare', '$analytics',

        function ($scope, sShare, $analytics) {
            var jsResources = window.JsResources;

            $scope.params = {
                notificationsListLength: 12,
                notificationsListPage: 12
            };

            sShare.getNotifications().then(function (response) {
                var data = response.success ? response.payload : [];

                $scope.notifications = data;

                countNewNotifications();
            });


            $scope.openNotifications = function () {
                if ($scope.params.wasOpened) {
                    return;
                }

                $scope.params.wasOpened = true;

                sShare.markNotificationsAsOld().then(function () {
                    _.forEach($scope.notifications, function (notification) {
                        notification.isNew = false;
                    });

                    $scope.params.newNotifications = 0;
                });
            };
            $scope.markAsRead = function (notification) {
                sShare.markNotificationAsRead({ messageId: notification.msgId }).then(function (response) {
                    notification.isRead = true;
                });

                $analytics.eventTrack('Site header', {
                    category: 'Notifications',
                    label: 'User clicked an invitation'
                });
            };

            $scope.$on('followedBox', function (e, boxId) {
                var notification = _.find($scope.notifications, function (notification) {
                    return notification.boxId === boxId;
                });

                var index = $scope.notifications.indexOf(notification);
                $scope.notifications.splice(index, 1);
            });

            $scope.addNotifications = function () {
                $scope.params.notificationsListLength += $scope.params.notificationsListPage;
            };
            function countNewNotifications() {
                var newNotifications = _.filter($scope.notifications, function (notification) {
                    return notification.isNew;
                });

                $scope.params.newNotifications = newNotifications.length;
            }


        }]);