app.controller('CategoryCtrl',
    ['$scope', '$routeParams', '$timeout','Store',
    function ($scope, $routeParams,$timeout, Store) {
        var consts = {
            defaultMaxProducts: 9,
            productsIncrement: 9
        },
        allProducts;

        $scope.params = {
            maxProducts: consts.defaultMaxProducts,
        };

        Store.products({ categoryId: $routeParams.categoryId }).then(function (response) {
            allProducts = response.payload;
            $scope.products = allProducts;

            $timeout(function () {
                $scope.$emit('viewContentLoaded');
            });
        });


        $scope.addProducts = function () {
            $scope.params.maxProducts += consts.productsIncrement;
        };


        //var lastQuery;
        //$scope.search = debounce(function () {
        //    var query = $scope.params.search;

        //    if (!query) {
        //        $scope.products = allProducts;
        //        $scope.params.maxProducts = consts.defaultMaxProducts;
        //        return;
        //    }

        //    if (query === lastQuery) {
        //        return;
        //    }

        //    lastQuery = query;

        //    Store.search({ term: query }).then(function (response) {
        //        var data = response.success ? response.payload : {};
        //        $scope.products = data;
        //        $scope.params.maxProducts = consts.productsIncrement;
        //    });
        //}, 150);
        $scope.search = function (e) {
            e.preventDefault();

            var query = $scope.params.search;

            $scope.params.isSearching = true;

            Store.search({ term: query }).then(function (response) {
                var data = response.success ? response.payload : {};
                $scope.params.isSearching = false;
                $scope.products = data;
                $scope.params.maxProducts = consts.productsIncrement;
            }, function () {
                $scope.params.isSearching = false;
            });
        };

    }]
);
