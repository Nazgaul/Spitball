"use strict";
mAccount.controller('NotificationSettingsCtrl',
    ['$scope', 'sBox', 'sUser',

        function ($scope, sBox, sUser) {
            var jsResources = window.JsResources;

            $scope.params = {
                boxListLength: 20,
                boxListPage: 20
            };

            sUser.notification().then(function (response) {
                var data = response.success ? response.payload : [];

                $scope.boxes = data;
            });

            $scope.addBoxes = function () {
                $scope.params.boxListLength += 20;
            };

            $scope.updateNotification = function (box) {
                sBox.changeNotification({ boxId: box.id, notification: box.notifications }).then(function () { });
            };
        }]);