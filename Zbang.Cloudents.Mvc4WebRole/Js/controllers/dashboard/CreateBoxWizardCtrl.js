mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope', '$modalInstance', 'WizardHandler', 'sLibrary', 'sShare', 'sFacebook', 'data',
        function ($scope, $modalInstance, WizardHandler, sLibrary, sShare, sFacebook, data) {

            var wizard;

            if (data.isAcademic) {
                $scope.isAcademic = true;
                $scope.boxType = 'academic';
                $scope.department = data.department;
            }

            if (data.isPrivate) {
                $scope.isPrivate = true;
                $scope.boxType = 'private';
            }

            $scope.formData = {};
            $scope.box = {};

            $scope.next = function () {
                if (!wizard) {
                    wizard = WizardHandler.wizard();
                }

                wizard.next();
            };

            $scope.cancel = function () {
                if (!$scope.box.url) {
                    $modalInstance.dismiss();
                    return;
                }

                wizard.finish();
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

            $scope.completeWizard = function (items) {
                var url = $scope.box.url;
                $modalInstance.close({
                    url: url,
                    isItems: items
                });
            };
        }]
    );
