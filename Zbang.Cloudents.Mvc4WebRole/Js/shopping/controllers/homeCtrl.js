app.controller('HomeCtrl',
    ['$scope', 'Shopping',
    function ($scope, Shopping) {

        $scope.params = {
            maxProducts: 9,
            maxProductsIncrement : 9
        };

        Shopping.products().then(function (response) {
            $scope.products = response.payload;
        });

        $scope.addProducts = function () {
            $scope.params.maxProducts += $scope.params.maxProductsIncrement;
        };


    }]
);
