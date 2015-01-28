﻿
mDashboard.controller('InviteCloudentsCtrl',
        ['$scope', '$q', 'sFacebook', '$modalInstance', '$analytics', 'sShare', 'sNotify',
        function ($scope, $q, sFacebook, $modalInstance, $analytics, sShare, sNotify) {
            "use strict";
            $scope.inviteCloudents = true;
            $scope.next = function () {
                $modalInstance.close();
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
            };

            $scope.invite = function (contact) {
                sShare.invite.cloudents({ recepients: [contact.id] }).then(function () { }, function () {
                    sNotify.alert('Error');
                });
            };

            $scope.inviteFacebook = function (contact) {
                $analytics.eventTrack('Invite', {
                    category: 'Invite to Cloudents'
                });

                var dfd = $q.defer(),
                    data = {
                        //boxId: $scope.box.id,
                        id: contact.id,
                        username: contact.username || contact.id,
                        firstName: contact.firstname,
                        middleName: contact.middlename,
                        lastName: contact.lastname,
                        sex: contact.gender
                    };

                sShare.facebookInvite.cloudents(data).then(function (response) {
                    sFacebook.send({
                        path: response.url,
                        to: contact.id
                    }).then(function () {
                        dfd.resolve();

                        $analytics.eventTrack('Facebook Invite', {
                            category: 'Invite to Cloudents'
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