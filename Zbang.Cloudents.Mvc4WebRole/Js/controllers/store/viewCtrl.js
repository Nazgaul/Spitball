app.controller('ViewCtrl',
    ['$scope', '$timeout',
    function ($scope, $timeout) {
        $timeout(function () {
            $scope.$emit('viewContentLoaded');
        });


    }]
);
