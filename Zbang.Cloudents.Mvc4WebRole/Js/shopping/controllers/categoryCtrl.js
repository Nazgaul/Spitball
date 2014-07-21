app.controller('CategoryCtrl',
    ['$scope', '$routeParams', 'Shopping',
    function ($scope, $routeParams, Shopping) {

        $scope.params = {
            maxProducts: 9,
            maxProductsIncrement: 9
        };

        Shopping.products({categoryId : $routeParams.categoryId}).then(function (response) {
            $scope.products = response.payload;
        });

        $scope.addProducts = function () {
            $scope.params.maxProducts += $scope.params.maxProductsIncrement;
        };


    }]
);
