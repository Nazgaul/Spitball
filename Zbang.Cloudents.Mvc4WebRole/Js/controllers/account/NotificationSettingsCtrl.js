mAccount.controller('NotificationSettingsCtrl',
    ['$scope', 'sBox', 'sUser', '$analytics',
        function ($scope, sBox, sUser, $analytics) {
            "use strict";

            $scope.params = {
                boxListLength: 20,
                boxListPage: 20
            };

            sUser.notification().then(function (notificationBoxList) {
                $scope.boxes = notificationBoxList;
            });

            $scope.addBoxes = function () {
                $scope.params.boxListLength += 20;
            };

            $scope.updateNotification = function (box) {
                sBox.changeNotification({ boxId: box.id, notification: box.notifications });
                $analytics.eventTrack('Update Notification', {
                    category: 'Account settings',
                    label: 'User updated a notification'
                });
            };
        }]);