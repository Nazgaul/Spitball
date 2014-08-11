app.controller('CategoryCtrl',
    ['$scope', '$routeParams', '$timeout', '$location', '$window', 'Store',
    function ($scope, $routeParams, $timeout, $location, $window, Store) {
        var consts = {
            defaultMaxProducts: 9,
            productsIncrement: 9
        },
        allProducts,
        hideBanners = $routeParams.categoryId && $routeParams.categoryId.length > 0;


        $scope.params = {
            maxProducts: consts.defaultMaxProducts,
        };

        Store.products({ categoryId: $routeParams.categoryId, universityId: $scope.params.universityId, producerId: $routeParams.producerId }).then(function (response) {
            allProducts = response.payload;
            $scope.products = allProducts;

            $timeout(function () {
                $scope.$emit('viewContentLoaded');
                //    if ($routeParams.categoryId) {
                //        if ($window.pageYOffset > 0 || $window.pageYOffset < 400) {
                //            $window.scrollTo(0, 400);
                //        }

                //    }
            }, 0);
        });


        $scope.addProducts = function () {
            $scope.params.maxProducts += consts.productsIncrement;
        };

        $scope.hideBanners = function () {
            return hideBanners;
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
            hideBanners = false;
            $scope.products = allProducts;
            $scope.params.isSearching = false;
            $scope.params.maxProducts = consts.defaultMaxProducts;

        });

        $scope.urlQueryString = function () {
            var first = true, qs = '';
            for (var key in $routeParams) {
                if (first) {                    
                    qs = '?' + key.toLowerCase() + '=' + $routeParams[key];
                    first = false;
                    continue;
                }
                qs += '&' + key.toLowerCase() + '=' + $routeParams[key];
            }            
            return qs;
        };

        function search() {
            var query = $scope.params.search;
            $scope.params.isSearching = true;
            hideBanners = true;
            $location.search({ q: $scope.params.search });
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
