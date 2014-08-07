app.controller('CategoryCtrl',
    ['$scope', '$routeParams', '$timeout', '$location', 'Store',
    function ($scope, $routeParams, $timeout, $location, Store) {
        var consts = {
            defaultMaxProducts: 9,
            productsIncrement: 9
        },
        allProducts;     

        $scope.params = {
            maxProducts: consts.defaultMaxProducts,
            universityId: $routeParams.universityId || $routeParams.universityid || null
        };

        Store.products({ categoryId: $routeParams.categoryId,universityId: $scope.params.universityId}).then(function (response) {
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
        if ($location.search()['q']) {
            var query = $location.search()['q'];
            $scope.params.search = query;
            search();
        }

        $scope.search = function (e) {
            e.preventDefault();
            search();
        
        };

        $scope.$on('$routeUpdate', function () {
            var query = $location.search()['q'];
            if (query) {
                $scope.params.search = query;
                search();
                return;
            }

            $scope.params.search = null;
            $scope.products = allProducts;
            $scope.params.isSearching = false;
            $scope.params.maxProducts = consts.defaultMaxProducts;

        });

        function search() {
            var query = $scope.params.search;

            $scope.params.isSearching = true;
            $location.search({ q: $scope.params.search});
            Store.search({ term: query, universityId: $scope.params.universityId }).then(function (response) {
                var data = response.success ? response.payload : {};
                $scope.params.isSearching = false;
                $scope.products = data;
                $scope.params.maxProducts = consts.defaultMaxProducts;
            }, function () {
                $scope.params.isSearching = false;
            });
        }
    }]
);
