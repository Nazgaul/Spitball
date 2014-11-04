
mBox.controller('SettingsCtrl',
    ['$scope', '$modalInstance', '$location', 'sUserDetails', 'sBox', 'data',

     function ($scope, $modalInstance, $location, sUserDetails, sBox, data) {
         "use strict";
         //Settings
         var states = {
             settings: 1,
             //members: 2
         };
         $scope.info = data.info;
         $scope.info.boxId = data.boxId;


         $scope.info.inviteUrl = $location.path() + 'invite/';
         $scope.info.user = sUserDetails.getDetails();

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
                 if (response.success) {
                     $scope.formData.queryString = response.payload.queryString;
                     $modalInstance.close($scope.formData);
                 } else {
                     alert(response.payload[0].value[0]);
                 }
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
         var jsResources = window.JsResources;
         $scope.deleteOrUnfollow = function () {

             if ($scope.info.userType === 'none') {
                 return '';
             }
             if ($scope.info.userType === 'owner') {
                 return jsResources.Delete;
             }

             if ($scope.info.membersLength <= 2 && $scope.info.comments < 2 && !$scope.info.itemsLength) {
                 return jsResources.Delete;
             }

             return jsResources.LeaveGroup;
         };

         $scope.inviteFriends = function () {
             $modalInstance.close({
                 invite:true
             });
         };
     }
    ]);