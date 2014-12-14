app.controller('ItemRegCtrl',
['$scope', '$modalInstance',
function ($scope, $modalInstance) {
    "use strict";

    
    $scope.register = function () {
        $modalInstance.close();
    };

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };


}
]);