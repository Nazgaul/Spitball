//define('manageCtrl',['app'], function (app) {
mBox.controller('SettingsCtrl',
    ['$scope', '$modalInstance', '$location', 'sUserDetails', 'sBox', 'data',

     function ($scope, $modalInstance, $location, sUserDetails, sBox, data) {
         //Settings
         var states = {
             settings: 1,
             //members: 2
         };
         $scope.info = data.info;
         $scope.info.boxId = data.boxId;
         //TODO: this is a temp solution
         $scope.info.inviteUrl = '/invite/' + data.boxId + "/";
         $scope.info.user = sUserDetails.getDetails();

         $scope.partials = {
             shareEmail: '/Share/MessagePartial/',
         };

         //$scope.params = {
         //    state: data.tab === 'settings' ? states.settings : states.members,
         //    search: ''
         //};
         $scope.params = {
             state: states.settings
         };

         $scope.formData = {
             name: $scope.info.name,
             notification: data.notification,
             boxUid: data.boxId
         };

         if ($scope.info.boxType === 'academic') {
             $scope.formData.courseCode = $scope.info.courseId,
             $scope.formData.professor = $scope.info.professor;
         }

         if ($scope.info.boxType === 'box') {
             $scope.formData.boxPrivacy = $scope.info.privacy;
         }


         $scope.save = function () {
             sBox.updateInfo($scope.formData).then(function (response) {
                 $scope.formData.queryString = response.payload.queryString;
                 $modalInstance.close($scope.formData);
             });
         };

         $scope.cancel = function () {
             $modalInstance.dismiss();
         };

         $scope.delete = function () {
             $modalInstance.dismiss();
             sBox.remove({ id: $scope.info.boxId }).then(function () {
                 $location.path('/dashboard/');
             });
         };

         $scope.deleteOrUnfollow = function () {

             if ($scope.info.userType === 'none') {
                 return;
             }
             if ($scope.info.userType === 'owner') {
                 return JsResources.Delete;
             }

             if ($scope.info.membersLength <= 2 && $scope.info.comments < 2 && !$scope.info.itemsLength) {
                 return JsResources.Delete;
             }

             return JsResources.LeaveGroup;
         };

     }
    ]);