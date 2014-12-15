app.controller('ProductCtrl',
    ['$scope', '$timeout', '$controller',
    function ($scope, $timeout, $controller) {

        angular.extend(this, $controller('StoreCtrl', { $scope: $scope }));

        $scope.setCurrentTab();


        "use strict";
        $timeout(function () {
            $scope.$emit('viewContentLoaded');
        });


    }]
);
