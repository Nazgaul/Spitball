app.controller('CongratsCtrl',
['$scope', '$modalInstance', 'data', 
function ($scope, $modalInstance,data) {
    "use strict";

    $scope.params = {
        points: data
    };    

    $scope.cancel = function () {
        $modalInstance.close();
    };        
        
  
}
]);