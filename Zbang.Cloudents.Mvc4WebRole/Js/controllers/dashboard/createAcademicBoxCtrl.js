﻿mWizardBoxCreate.controller('createAcademicBoxCtrl',
        ['$scope',
         'sBox',
         'WizardHandler',
         function ($scope, sBox, WizardHandler) {
             $scope.create = function (isValid) {
                 WizardHandler.wizard().next();
                 //TODO: add disabled state
                 //if (!isValid) {
                 //    return;
                 //}
                 //sBox.createAcademic($scope.formData).then(function (box) {

                 //    //$modalInstance.close(box.payload || box.Payload);
                 //});
             };

             $scope.cancel = function () {
                 // WizardHandler.wizard().finish();
                 WizardHandler.wizard().finish();
             };
         }
        ]);
