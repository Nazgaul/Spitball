
mBox.controller('QuizCloseCtrl',
    ['$scope', '$modalInstance',
        function ($scope, $modalInstance) {
            "use strict";

            $scope.publish = function () {
                $modalInstance.close('publish');
            };

            $scope.deleteQuiz = function () {
                $modalInstance.close('delete');

            };

            $scope.saveDraft = function () {
                $modalInstance.close('draft');

            };

            $scope.cancel = function () {
                $modalInstance.dismiss();

            };
        }

    ]);

