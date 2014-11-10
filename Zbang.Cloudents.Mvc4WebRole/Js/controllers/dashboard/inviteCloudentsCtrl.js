
mDashboard.controller('InviteCloudentsCtrl',
        ['$scope', '$q', 'sFacebook', '$modalInstance','$analytics', 'sShare',
        function ($scope, $q, sFacebook, $modalInstance, $analytics, sShare) {
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
                    alert('Error');
                });
            };

            $scope.inviteFacebook = function (contact) {
                $analytics.eventTrack('Invite to Cloudents', {
                    category: 'Invite'
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

                        $analytics.eventTrack('Invite to Cloudents', {
                            category: 'Facebook Invite'
                        });
                    }, function () {
                        dfd.reject();
                    });

                }, function (response) {
                    alert(response);
                    dfd.reject();
                });
                return dfd.promise;
            };

        }
        ]);