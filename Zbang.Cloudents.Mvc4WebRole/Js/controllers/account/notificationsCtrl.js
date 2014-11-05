﻿
mAccount.controller('NotificationsCtrl',
    ['$scope', 'sShare', '$analytics',
        function ($scope, sShare, $analytics) {
            "use strict";


            $scope.params = {
                notificationsListLength: 12,
                notificationsListPage: 12
            };

            sShare.getNotifications().then(function (notifications) {                
                $scope.notifications = notifications;
                countNewNotifications();
            });


            $scope.openNotifications = function () {

                //TODO analytics

                if ($scope.params.wasOpened) {
                    return;
                }

                $scope.params.wasOpened = true;

                //sShare.markNotificationsAsOld().then(function () {
                //    _.forEach($scope.notifications, function (notification) {
                //        notification.isNew = false;
                //    });

                //    $scope.params.newNotifications = 0;
                //});
            };
            $scope.markAsRead = function (notification) {
                //sShare.markNotificationAsRead({ messageId: notification.msgId }).then(function (response) {
                //    notification.isRead = true;
                //});

                $analytics.eventTrack('Site header', {
                    category: 'Notifications',
                    label: 'User clicked a notification'
                });
            };

            $scope.$on('followedBox', function (e, boxId) {
                var notification = _.find($scope.notifications, function (notification2) {
                    return notification2.boxId === boxId;
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