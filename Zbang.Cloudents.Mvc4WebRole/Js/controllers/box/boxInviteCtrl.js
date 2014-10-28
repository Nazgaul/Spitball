"use strict";
mBox.controller('BoxInviteCtrl',
        ['$scope', '$q', '$modalInstance', 'sShare', 'sFacebook', 'data',
        function ($scope, $q, $modalInstance, sShare, sFacebook, data) {

            $scope.box = data;

            $scope.next = function () {
                $modalInstance.close();
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
            };

            $scope.invite = function (contact) {
                sShare.invite.box({ recepients: [contact.id], boxId: $scope.box.id }).then(function (response) {
                    if (!response.success) {
                        alert('Error');
                    }
                });
            };

            $scope.inviteFacebook = function (contact) {
                var dfd = $q.defer();
                sFacebook.send({
                    path: $scope.box.url,
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