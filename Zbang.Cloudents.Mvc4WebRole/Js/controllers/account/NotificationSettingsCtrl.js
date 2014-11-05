
mAccount.controller('NotificationSettingsCtrl',
    ['$scope', 'sBox', 'sUser',
        function ($scope, sBox, sUser) {
            "use strict";

            var jsResources = window.JsResources;

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
                sBox.changeNotification({ boxId: box.id, notification: box.notifications }).then(function () { });
            };
        }]);