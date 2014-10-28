"use strict";
mDashboard.controller('InviteCloudentsCtrl',
        ['$scope','$q', 'sFacebook', '$modalInstance',
        function ($scope, $q, sFacebook, $modalInstance) {

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
                sFacebook.send({
                    path: '',
                    to: contact.id
                }).then(function () {
                    var data = {
                        boxId: $scope.box.id,
                        id: contact.id,
                        username: contact.username || contact.id,
                        firstName: contact.firstname,
                        middleName: contact.middlename,
                        lastName: contact.lastname,
                        sex: contact.gender
                    };

                    sShare.facebookInvite.box(data).then(function (response1) {
                        if (!response1.success) {
                            alert('Error');
                            dfd.reject();
                        }

                        dfd.resolve();
                    });


                }, function () {
                    dfd.reject();
                });
                return dfd.promise;
            };

        }
        ]);