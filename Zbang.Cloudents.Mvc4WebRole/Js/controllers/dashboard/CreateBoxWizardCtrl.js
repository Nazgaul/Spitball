
mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope', '$modalInstance', 'WizardHandler', 'sLibrary', 'sShare', 'sFacebook', 'data','$q',
        function ($scope, $modalInstance, sWizardHandler, sLibrary, sShare, sFacebook, data, $q) {
            "use strict";
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
                    wizard = sWizardHandler.wizard();
                }


                wizard.next();
                $scope.$broadcast('update-scroll');

            };

            $scope.cancel = function () {
                if (!$scope.box.url) {
                    $modalInstance.dismiss();
                    //TODO analytics
                    return;
                }

                wizard.finish();
            };

            $scope.invite = function (contact) {
                //TODO analytics
                sShare.invite.box({ recepients: [contact.id], boxId: $scope.box.id }).then(function() {                    
                }, function () {
                    alert('Error');
                });
            };

            $scope.inviteFacebook = function (contact) {
                //TODO analytics

                var dfd = $q.defer(),
                    data2 = {
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

            $scope.completeWizard = function (items) {
                //TODO analytics
                var url = $scope.box.url;
                $modalInstance.close({
                    url: url,
                    isItems: items
                });
            };
        }]
    );
