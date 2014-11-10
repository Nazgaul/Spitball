mDashboard.controller('CreateBoxCtrl',
        ['$scope',
         'sBox',
         '$analytics',
         function ($scope, sBox, $analytics) {
             "use strict";
             var createDisabled = false;
             $scope.create = function (isValid) {
                 if (createDisabled) {
                     return;
                 }

                 if (!isValid) {
                     return;
                 }

                 createDisabled = true;

                 sBox.createPrivate($scope.formData).then(function (data) {
                     $scope.box.url = data.url;
                     $scope.box.id = data.id;
                     $scope.next();
                     $analytics.eventTrack('Box Wizard', {
                         category: 'Create Private'
                     });
                 }, function (response) {
                     $scope.formData.error = response[0].value[0];
                 }).finally(function () {
                     createDisabled = false;
                 });


             };
         }
        ]);
