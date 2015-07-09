﻿mBox.controller('BoxInviteCtrl',
        ['$scope', '$q', '$modalInstance', '$analytics', 'sShare', 'sFacebook', 'data', 'sNotify',
        function ($scope, $q, $modalInstance, $analytics, sShare, sFacebook, data, sNotify) {
            "use strict";
            $scope.box = data;

            $scope.next = function () {
                $modalInstance.close();
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
            };

            $scope.invite = function (contact) {
                $analytics.eventTrack('Invite Gmail/Spitball', {
                    category: 'Box'
                });

                sShare.invite.box({ recepients: [contact.id], boxId: $scope.box.id }).then(function () {
                }, function () {
                    sNotify.alert('Error');
                });
            };

            $scope.inviteFacebook = function (contact) {


                var dfd = $q.defer();

                var data2 = {
                    boxId: $scope.box.id,
                    id: contact.id,
                    username: contact.username || contact.id,
                    firstName: contact.firstname,
                    middleName: contact.middlename,
                    lastName: contact.lastname,
                    sex: contact.gender
                };

                sShare.facebookInvite.box(data2).then(function (response) {
                    sFacebook.send({
                        path: response.url,
                        to: contact.id
                    }).then(function () {
                        dfd.resolve();

                        $analytics.eventTrack('Invite Facebook', {
                            category: 'Box',
                            label: 'User invited a friend to box from facebook'
                        });

                    }, function () {
                        dfd.reject();
                    });


                }, function (response) {
                    sNotify.alert(response);
                    dfd.reject();
                });



                return dfd.promise;
            };

        }
        ]);