app.controller('ViewCtrl',
    ['$scope', '$timeout','$controller',
    function ($scope, $timeout, $controller) {
        "use strict";

        angular.extend(this, $controller('StoreCtrl', { $scope: $scope }));

        $scope.setCurrentTab();

        $timeout(function () {
            $scope.$emit('viewContentLoaded');
        });


    }]
);
