app.controller('SearchHeaderCtrl',
    ['$scope', '$timeout', 'sSearch', 'textDirectionService',
    function ($scope, $timeout, Search, textDirectionService) {
        $scope.params = {
            currentPage: 0,
            isSearching: false,
            showOtherUnis: false
        };

        $scope.data = {};


        if ($location.search()['q']) {
            var query = $location.search()['q'];
            $scope.params.search = query;
            search();
        }

        $scope.$on('$routeUpdate', function () {
            var query = $location.search()['q'];
            if (query) {
                $scope.params.search = query;
                search();
                return;
            }

            $scope.params.search = null;

        });
        function search() {
            var query = $scope.params.search;

            $scope.params.isSearching = true;

            Search.searchByPage({ q: query, page: $scope.params.currentPage }).then(function (response) {
                var data = response.success ? response.payload : {};
                parseData(data);

            }, function () {
                $scope.params.isSearching = false;
            });            
        }

        function getOtherUnisItems() {
            var query = $scope.params.search;

            $scope.params.isSearching = true;

            Search.searchByPage({ q: query, page: $scope.params.currentPage }).then(function (response) {
                var data = response.success ? response.payload : {};
                parseData(data);

            }, function () {
                $scope.params.isSearching = false;
            });
        }

        function parseData(data) {
            $scope.data.items = data.boxes;
            $scope.data.items = data.items;
            $scope.data.items = data.users;
            
            if (data.items.length < 50) {
                if (data.items.length === 0) {
                    getOtherUnisItems();
                }
                $scope.params.showOtherUnis = true;
            }
        }

    }]
);
