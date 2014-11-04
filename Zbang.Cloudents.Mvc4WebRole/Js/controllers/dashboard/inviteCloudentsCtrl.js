"use strict";
mDashboard.controller('InviteCloudentsCtrl',
        ['$scope','$q', 'sFacebook', '$modalInstance','sShare',
        function ($scope, $q, sFacebook, $modalInstance, sShare) {

            $scope.inviteCloudents = true;
            $scope.next = function () {
                $modalInstance.close();
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
            };

            $scope.invite = function (contact) {
                sShare.invite.cloudents({ recepients: [contact.id]}).then(function (response) {
                    if (!response.success) {
                        alert('Error');
                    }
                });
            };

            $scope.inviteFacebook = function (contact) {
                var dfd = $q.defer();

                var data = {
                    //boxId: $scope.box.id,
                    id: contact.id,
                    username: contact.username || contact.id,
                    firstName: contact.firstname,
                    middleName: contact.middlename,
                    lastName: contact.lastname,
                    sex: contact.gender
                };

                sShare.facebookInvite.box(data).then(function (response) {
                    if (!response.success) {
                        alert('Error');
                        dfd.reject();
                        return;
                    }

                    sFacebook.send({
                        path: response.payload.url,
                        to: contact.id
                    }).then(function () {

                    }, function () {
                        dfd.reject();
                    });
                    
                });
                return dfd.promise;
            };

        }
        ]);