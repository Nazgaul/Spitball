
app.controller('CategoryCtrl',
    ['$scope', '$routeParams', '$timeout', '$location', '$window', 'Store',
    function ($scope, $routeParams, $timeout, $location, $window, sStore) {
        "use strict";
        var consts = {
            defaultMaxProducts: 9,
            productsIncrement: 9
        },
        allProducts,
        hideBanners = $routeParams.categoryId && $routeParams.categoryId.length > 0;


        $scope.params = {
            maxProducts: consts.defaultMaxProducts,

        };

        if ($location.search()['q']) {
            var query = $location.search()['q'];
            $scope.params.search = query;
            search();
        }
        else {

            sStore.products({ categoryId: $routeParams.categoryId, universityId: $routeParams.universityid, producerId: $routeParams.producerid }).then(function (response) {
                allProducts = response;
                $scope.products = allProducts;
            });


        }

        $timeout(function () {
            $scope.$emit('viewContentLoaded');
            //    if ($routeParams.categoryId) {
            //        if ($window.pageYOffset > 0 || $window.pageYOffset < 400) {
            //            $window.scrollTo(0, 400);
            //        }

            //    }
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

        //    sStore.search({ term: query }).then(function (response) {
        //        var data = response.success ? response.payload : {};
        //        $scope.products = data;
        //        $scope.params.maxProducts = consts.productsIncrement;
        //    });
        //}, 150);


        $scope.search = function (e) {
            e.preventDefault();
            search();

        };
        $scope.$watch('params.search', function (newValue) {
            if (newValue === '') {
                $location.search('q', newValue || null);
            }
        });
        $scope.$on('$routeUpdate', function () {
            var query2 = $location.search()['q'];
            if (query2) {
                $scope.params.search = query2;
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
            var query2 = $scope.params.search;
            $scope.params.isSearching = true;
            hideBanners = true;
            $location.search('q', $scope.params.search);

            //TODO analytics

            sStore.search({ term: query2, universityId: $scope.params.universityId }).then(function (data) {                
                $scope.products = data;
                $scope.params.maxProducts = consts.defaultMaxProducts;
            }, function () {
            
            }).finally(function () {
                $scope.params.isSearching = false;
            });
        }
    }]
);
