app.controller('ProductCtrl',
    ['$scope','$timeout',
    function ($scope, $timeout) {
        $timeout(function () {
            $scope.$emit('viewContentLoaded');
        });
        

    }]
);
