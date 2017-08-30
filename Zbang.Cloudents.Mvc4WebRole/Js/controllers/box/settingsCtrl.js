mBox.controller('SettingsCtrl',
    ['$scope', '$modalInstance', '$location', '$analytics', 'sUserDetails', 'sBox', 'data', 'resManager', 'sNotify', 'sModal',

function ($scope, $modalInstance, $location, $analytics, sUserDetails, sBox, data, resManager, sNotify, sModal) {
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
            $analytics.eventTrack('Settings - Save Settings', {
                category: 'Box'
            });

        }, function (response) {
            sNotify.alert(response[0].value[0]);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss();
        $analytics.eventTrack('Settings - Cancel', {
            category: 'Box'
        });
    };

    $scope.delete = function () {

        $modalInstance.dismiss();

        if (sUserDetails.getDetails().isAdmin) {
            sModal.open('leavePrompt', {
                callback: {
                    close: function (response) {
                        if (response.delete) {
                            remove(true);
                            return;
                        }
                        remove();
                    }
                },
                data: {
                    name: $scope.info.name
                }
            });
            return;
        }


        remove();


        $analytics.eventTrack('Settings - Delete/Unfollow', {
            category: 'Box'            
        });

        function remove(isDelete) {
            sBox.remove({ id: $scope.info.boxId, 'delete': isDelete }).then(function () {
                $location.path('/dashboard/');
            });
        }
    };
    $scope.deleteOrUnfollow = function () {

        if (sUserDetails.getDetails().isAdmin) {
            return JsResources.Delete + ' / ' + JsResources.Unfollow;
        }

        if ($scope.info.userType === 'none') {
            return '';
        }

        if ($scope.info.userType === 'owner') {
            return resManager.get('Delete');
        }

        return resManager.get('LeaveGroup');
    };

    $scope.inviteFriends = function () {
        $modalInstance.close({
            invite: true
        });

        $analytics.eventTrack('Settings - Invite Friends', {
            category: 'Box'
        });
    };
}
    ]);