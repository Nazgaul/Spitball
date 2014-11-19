mAccount.controller('NotificationsCtrl',
    ['$scope', 'sNotification', '$analytics', '$window', '$location', '$route',
        function ($scope, sNotification, $analytics, $window, $location, $route) {
            "use strict";

            $scope.params = {
                notificationsListLength: 7,
                notificationsListPage: 7,
            };            
         
            getDetails();

            $scope.openNotifications = function () {

                $analytics.eventTrack('Notifications', {
                    category: 'Notifications',
                    label: 'User ' + ($scope.params.wasOpened ? 'closed' : 'opened') + ' notifications'
                });

            };


            $scope.markAsRead = function (notification) {

                if (notification.url === $location.path()) {
                    $route.reload();
                }


                if (notification.isRead) {
                    return;
                }

                notification.isRead = true;
                sNotification.setRead(notification.msgId);

                $analytics.eventTrack('Notifications', {
                    category: 'Notifications',
                    label: 'User clicked a notification'
                });
            };

            $scope.$on('followedBox', function (e, boxId) {
                var notification = _.find($scope.notifications, function (notification2) {
                    return notification2.boxId === boxId;
                });

                if (!notification) {
                    return;
                }

                var index = $scope.notifications.indexOf(notification);
                $scope.notifications.splice(index, 1);
            });

            $scope.$on('newNotifications', getDetails);

            $scope.addNotifications = function () {
                $scope.params.notificationsListLength += $scope.params.notificationsListPage;
            };

            $scope.updateState = function (isOpen) {
                if (!isOpen) {
                    return;
                }

                $scope.$broadcast('update-scroll');
            }

            function getDetails() {
                $scope.notifications = sNotification.getAll();
                $scope.params.newNotifications = sNotification.getUnreadLength();              
            }

        }]);