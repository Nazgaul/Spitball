
app.controller('HomeCtrl',
    ['$scope', /*'debounce',*/'Store',
    function ($scope, /*debounce,*/ Store) {
        "use strict";
        var consts = {
            defaultMaxProducts: 9,
            productsIncrement: 9
        },
        allProducts;

        $scope.params = {
            maxProducts: consts.defaultMaxProducts,            
        };

        Store.products().then(function (response) {
            allProducts = response;
            $scope.products = allProducts;
        });

        $scope.addProducts = function () {
            $scope.params.maxProducts += consts.productsIncrement;
        };


        //var lastQuery;
        //$scope.search = debounce(function () {
        //    var query = $scope.params.search;

        //    if (!query) {
        //        $scope.products = allProducts;
        //        $scope.params.maxProducts = consts.productsIncrement;
        //        return;
        //    }

        //    if (query === lastQuery) {
        //        return;
        //    }

        //    lastQuery = query;

        //    Store.search({ term : query }).then(function (response) {
        //        var data = response.success ? response.payload : {};
        //        $scope.products = data;
        //        $scope.params.maxProducts = consts.productsIncrement;
        //    });
        //}, 150);
        $scope.search = function (e) {
            e.preventDefault();

            var query = $scope.params.search;
            $scope.params.isSearching = true;
            Store.search({ term: query }).then(function () {
                $scope.products = data;
                $scope.params.maxProducts = consts.productsIncrement;
            }).finally(function () {
                $scope.params.isSearching = false;
            });
        };
    }]
);
