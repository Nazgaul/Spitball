
app.controller('ProductCtrl',
    ['$scope','$timeout',
    function ($scope, $timeout) {
        "use strict";
        $timeout(function () {
            $scope.$emit('viewContentLoaded');
        });
        

    }]
);
