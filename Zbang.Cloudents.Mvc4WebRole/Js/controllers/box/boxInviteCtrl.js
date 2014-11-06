
mBox.controller('BoxInviteCtrl',
        ['$scope', '$q', '$modalInstance', 'sShare', 'sFacebook', 'data',
        function ($scope, $q, $modalInstance, sShare, sFacebook, data) {
            "use strict";
            $scope.box = data;

            $scope.next = function () {
                $modalInstance.close();
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
                //TODO analytics
            };

            $scope.invite = function (contact) {
                //TODO analytics
                sShare.invite.box({ recepients: [contact.id], boxId: $scope.box.id }).then(function () {                    
                }, function () {
                    alert('Error');
                });
            };

            $scope.inviteFacebook = function (contact) {
                //TODO analytics
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