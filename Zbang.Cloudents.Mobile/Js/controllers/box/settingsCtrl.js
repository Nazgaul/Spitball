mBox.controller('SettingsCtrl',
    ['$scope', '$modalInstance', '$location', '$analytics', 'sUserDetails', 'sBox', 'data', 'resManager', 'sNotify',

function ($scope, $modalInstance, $location, $analytics, sUserDetails, sBox, data, resManager, sNotify) {
    "use strict";
    //Settings

    $scope.info = data.info;
    $scope.info.boxId = data.boxId;


    $scope.info.inviteUrl = $location.path() + 'invite/';
    $scope.info.user = sUserDetails.getDetails();

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
            $scope.formData.queryString = response.queryString;
            $modalInstance.close($scope.formData);
            $analytics.eventTrack('Box Settings', {
                category: 'Save Settings'
            });

        }, function (response) {
            sNotify.alert(response[0].value[0]);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss();
        $analytics.eventTrack('Box Settings', {
            category: 'Cancel'
        });
    };

    $scope.delete = function () {
        $modalInstance.dismiss();
        sBox.remove({ id: $scope.info.boxId }).then(function () {
            $location.path('/dashboard/');
        });

        $analytics.eventTrack('Box Settings', {
            category: 'Delete/Unfollow',
            label: 'User deleted or unfollowed a box'
        });
    };
    $scope.deleteOrUnfollow = function () {

        if ($scope.info.userType === 'none') {
            return '';
        }
        if ($scope.info.userType === 'owner') {
            return resManager.get('Delete');
        }

        if ($scope.info.membersLength <= 2 && $scope.info.comments < 2 && !$scope.info.itemsLength) {
            return resManager.get('Delete');
        }

        return resManager.get('LeaveGroup');
    };

    $scope.inviteFriends = function () {
        $modalInstance.close({
            invite: true
        });

        $analytics.eventTrack('Box Settings', {
            category: 'Invite Friends'
        });
    };
}
    ]);