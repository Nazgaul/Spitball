mBox.controller('UploadPopupCtrl',
    ['$scope', '$modalInstance', 'data',
    function ($scope, $modalInstance, data) {
        $scope.boxId = data.boxId;
        $scope.tabId = data.tabId;
        $scope.questionId = data.questionId;
        $scope.newQuestion = data.newQuestion;


        $scope.close = function () {
            $modalInstance.close();
        };

        $scope.dismiss = function () {
            $modalInstance.dismiss();
        };

    }
    ]);
