﻿mDashboard.controller('CreateBoxWizardCtrl',
     ['$scope', '$modalInstance', 'WizardHandler', 'sLibrary', 'sShare', 'sFacebook', 'data','$q','$analytics','sNotify',
        function ($scope, $modalInstance, sWizardHandler, sLibrary, sShare, sFacebook, data, $q, $analytics,sNotify) {
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
                $scope.$broadcast('input:facebook');

            };

            $scope.cancel = function () {
                if (!$scope.box.url) {
                    $modalInstance.dismiss();
                    $analytics.eventTrack( 'Cancel', {
                        category: 'Box Wizard'
                    });
                    return;
                }

           
                wizard.finish();
            };

            $scope.invite = function (contact) {
                $analytics.eventTrack('Invite', {
                    category: 'Box Wizard'
                });

                sShare.invite.box({ recepients: [contact.id], boxId: $scope.box.id }).then(function () {
                }, function () {
                    sNotify.alert('Error');
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
                        $analytics.eventTrack('Facebook Invite', {
                            category: 'Box Wizard'
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

            $scope.completeWizard = function (items) {                
                var url = $scope.box.url;
                $modalInstance.close({
                    url: url,
                    isItems: items
                });
                $analytics.eventTrack('Finish', {
                    category: 'Box Wizard'
                });
            };
        }]
    );
