app.controller('CheckoutCtrl',
    ['$scope','$timeout',
    function ($scope,$timeout) {

        $timeout(function () {
            $scope.product.categories = _.groupBy($scope.product.features, 'category');

            $scope.$emit('viewContentLoaded');
        });
    }]
);
