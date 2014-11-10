mBox.controller('BoxInviteCtrl',
        ['$scope', '$q', '$modalInstance','$analytics', 'sShare', 'sFacebook', 'data',
        function ($scope, $q, $modalInstance, $analytics, sShare, sFacebook, data) {
            "use strict";
            $scope.box = data;

            $scope.next = function () {
                $modalInstance.close();
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();                
            };

            $scope.invite = function (contact) {
                $analytics.eventTrack('Box Invite', {
                    category: 'Gmail/Cloudents',
                    label: 'User invited a friend to box from google or cloudents'
                });

                sShare.invite.box({ recepients: [contact.id], boxId: $scope.box.id }).then(function () {                    
                }, function () {
                    alert('Error');
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

                        $analytics.eventTrack('Box Invite', {
                            category: 'Facebook',
                            label: 'User invited a friend to box from facebook'
                        });

                    }, function () {
                        dfd.reject();
                    });


                }, function () {
                    alert('Error');
                    dfd.reject();
                });



                return dfd.promise;
            };

        }
        ]);