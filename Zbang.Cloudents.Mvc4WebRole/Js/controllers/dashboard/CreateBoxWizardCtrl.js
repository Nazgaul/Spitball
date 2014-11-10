mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope', '$modalInstance', 'WizardHandler', 'sLibrary', 'sShare', 'sFacebook', 'data','$q','$analytics',
        function ($scope, $modalInstance, sWizardHandler, sLibrary, sShare, sFacebook, data, $q, $analytics) {
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
                    $analytics.eventTrack('Box Wizard', {
                        category: 'Cancel'
                    });
                    return;
                }

           
                wizard.finish();
            };

            $scope.invite = function (contact) {
                $analytics.eventTrack('Box Wizard', {
                    category: 'Invite'
                });

                sShare.invite.box({ recepients: [contact.id], boxId: $scope.box.id }).then(function () {
                }, function () {
                    alert('Error');
                });
            };

            $scope.inviteFacebook = function (contact) {
            

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
                        $analytics.eventTrack('Box Wizard', {
                    category: 'Facebook Invite'
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

            $scope.completeWizard = function (items) {                
                var url = $scope.box.url;
                $modalInstance.close({
                    url: url,
                    isItems: items
                });
                $analytics.eventTrack('Box Wizard', {
                    category: 'Finish'
                });
            };
        }]
    );
