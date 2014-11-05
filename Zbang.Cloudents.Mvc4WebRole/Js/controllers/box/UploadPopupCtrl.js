
mBox.controller('UploadPopupCtrl',
    ['$scope', '$modalInstance', 'data',
    function ($scope, $modalInstance, data) {
        "use strict";
        $scope.boxId = data.boxId;
        $scope.tabId = data.tabId;
        $scope.questionId = data.questionId;
        $scope.newQuestion = data.newQuestion;


        $scope.close = function () {
            $modalInstance.close();
            //TODO analytics close for box with uploaded files
        };

        $scope.dismiss = function () {
            $modalInstance.dismiss();
            //TODO analytics close fox box no files
        };

    }
    ]);
