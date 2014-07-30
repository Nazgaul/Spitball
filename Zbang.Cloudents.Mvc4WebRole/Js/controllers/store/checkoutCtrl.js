app.controller('CheckoutCtrl',
    ['$scope', '$timeout',
    function ($scope, $timeout) {
        $timeout(function () {
            $scope.$emit('viewContentLoaded');
        });


    }]
);
