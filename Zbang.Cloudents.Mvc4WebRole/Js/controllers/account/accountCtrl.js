mAccount.controller('AccountCtrl',
['$scope',
function ($scope) {
    "use strict";

    $timeout(function () {
        $scope.$emit('viewContentLoaded');
    });

}
]);