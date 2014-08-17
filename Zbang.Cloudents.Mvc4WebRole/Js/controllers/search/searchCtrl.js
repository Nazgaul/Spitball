angular.module('mSearch', []).
    constant({
        constants: {
            tabs: {
                boxes: 'sTab1',
                items: 'sTab2',
                users: 'sTab3',
            },
            stars: {
                width: 69,
                count: 5
            }
        }
    }).
controller('SearchCtrl',
['$scope',
'$timeout',
'$location',
'sSearch',
'textDirectionService',
'constants',
function ($scope, $timeout, $location, Search, textDirectionService, constants) {
    $scope.params = {
        currentPage: 0,
        otherItemsPage: 0,
        isSearching: false,
        showOtherUnis: false
    };

    $scope.data = {};


    if ($location.search()['q']) {
        var query = $location.search()['q'];
        $scope.params.query = query;
        search();
    }

    $scope.$on('$routeUpdate', function () {
        var query = $location.search()['q'];
        if (query) {
            $scope.params.query = query;
            search();
            return;
        }

        $scope.params.query = null;
    });

    $scope.showOtherUnisItems = function () {
        getOtherUnisItems();
    };

    $scope.setCurrentTab = function (tab) {
        var length;
        $scope.params.currentTab = tab;
        switch (tab) {
            case constants.tabs.boxes:
                length = $scope.data.boxes.length;
                break;
            case constants.tabs.items:
                length = $scope.data.items.length;
                break;
            case constants.tabs.users:
                length = $scope.data.users.length;
                break;
        }
        $scope.params.noResults = (length === 0);

    };

    $scope.itemRating = function (rate) {
        if (rate) {
            return constants.stars.width / constants.stars.count * rate;
        }

        return 0;
    };

    $timeout(function () {
        $scope.$emit('viewContentLoaded');
    });

    function search() {
        var query = $scope.params.query;

        $scope.params.isSearching = true;

        Search.searchByPage({ q: query, page: $scope.params.currentPage }).then(function (response) {
            var data = response.success ? response.payload : {};
            parseData(data);
            setInitTab();
            $scope.params.currentPage++;

        }, function () {
            $scope.params.isSearching = false;
        });
    }

    function getOtherUnisItems() {
        var query = $scope.params.query;

        $scope.params.isSearching = true;

        Search.searchByPage({ q: query, page: $scope.params.currentPage }).then(function (response) {
            var data = response.success ? response.payload : {};
            parseData(data);
            $scope.params.otherItemsPage++;
        }, function () {
            $scope.params.isSearching = false;
        });
    }

    function parseData(data) {
        $scope.data.boxes = data.boxes;
        $scope.data.items = data.items;
        $scope.data.users = data.users;

        if (data.items.length < 50) {
            if (data.items.length === 0 && $scope.params.currentPage === 0) {
                getOtherUnisItems();
            }

            $scope.params.showOtherUnis = true;
        }
    }

    function setInitTab() {
        var tab;
        if ($scope.data.boxes.length === 0) {
            if ($scope.data.items.length) {
                tab = constants.tabs.items;
            }

            if ($scope.data.users.length === 0) {
                tab = constants.tabs.users;
            }
        }

        tab = constants.tabs.boxes;

        $scope.setCurrentTab(tab);
    }

}]
);
