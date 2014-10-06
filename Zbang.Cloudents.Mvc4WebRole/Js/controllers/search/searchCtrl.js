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
function ($scope, $timeout, $location, sSearch, textDirectionService, constants) {
    $scope.params = {
        currentPage: 0,
        currentTab: constants.tabs.boxes,
        otherItemsPage: 0,
        showOtherUnis: false,
        isOtherItems: false,
        itemsLoading: false,
        boxesLoading: false,
        usersLoading: false
    };

    $scope.data = {};


    if ($location.search()['q']) {
        var query = $location.search()['q'];
        $scope.params.query = query;
        search();
    }

    $scope.$on('$routeUpdate', function () {
        var query = $location.search()['q'];
        $scope.params.currentPage = $scope.params.otherItemsPage = 0;
        $scope.params.showOtherUnis = false;
        $scope.params.isOtherItems = false;
        $scope.data.boxes = $scope.data.items = $scope.data.otherItems = $scope.data.users = [];
        if (query) {
            $scope.params.query = query;
            $scope.params.textDirection = textDirectionService.isRTL(query) ? 'rtl' : 'ltr';
            $timeout(search, 300);
            return;
        }

        $scope.params.query = null;
    });

    $scope.showOtherUnisItems = function () {
        getOtherUnisItems();
        $scope.params.showOtherUnis = false;

    };

    $scope.setCurrentTab = function (tab) {
        var length;
        $scope.params.currentTab = tab;
        switch (tab) {
            case constants.tabs.boxes:
                length = $scope.data.boxes.length;
                break;
            case constants.tabs.items:
                length = $scope.data.items.length || $scope.data.otherItems.length;
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

    $scope.addToList = function () {
        console.log('here');
        if ($scope.data.loading) {
            return;
        }

        var showMore = false;
        switch ($scope.params.currentTab) {
            case constants.tabs.boxes:
                if ($scope.data.boxes.length % 50 === 0) {
                    $scope.params.currentPage++;
                    $scope.params.boxesLoading = true;
                    showMore = true;
                }

                break;
            case constants.tabs.items:
                if ($scope.params.isOtherItems) {
                    if ($scope.data.otherItems.length % 50 === 0) {
                        $scope.params.otherItemsPage++;
                        $scope.params.itemsLoading = true;
                        getOtherUnisItems();
                        return;
                    }

                    break;
                }


                if ($scope.data.items.length % 50 === 0) {
                    $scope.params.itemsLoading = true;
                    $scope.params.currentPage++;
                    showMore = true;

                }
                break;
            case constants.tabs.users:
                if ($scope.data.users.length % 50 === 0) {
                    $scope.params.usersLoading = true;
                    $scope.params.currentPage++;
                    showMore = true;

                }

                break;
        }

        if (!showMore) {
            return;
        }

        $timeout(function () {
            sSearch.searchByPage({ q: $scope.params.query, page: $scope.params.currentPage }).then(function (response) {
                var data = response.success ? response.payload : {};
                parseData(data);
                $scope.params.itemsLoading = false;
                $scope.params.boxesLoading = false;
                $scope.params.usersLoading = false;


            }, function () {
                $scope.params.itemsLoading = false;
                $scope.params.boxesLoading = false;
                $scope.params.usersLoading = false;
            });
        }, 500);

    };


    $timeout(function () {
        $scope.$emit('viewContentLoaded');
    });

    function search() {
        var query = $scope.params.query;

        $scope.data.loading = true;
        $scope.params.noResults = false;

        $timeout(function () {

            sSearch.searchByPage({ q: query, page: $scope.params.currentPage }).then(function (response) {
                var data = response.success ? response.payload : {};
                parseData(data);
                setInitTab();
                $scope.data.loading = false;
            }, function () {
                $scope.data.loading = false;
            });
        }, 500);
    }

    function getOtherUnisItems() {
        var query = $scope.params.query;

        $scope.params.isOtherItems = true;

        sSearch.searchOtherUnis({ q: query, page: $scope.params.otherItemsPage }).then(function (response) {
            var data = response.success ? response.payload : {};
            $scope.data.otherItems = $scope.data.otherItems ? $scope.data.otherItems.concat(data) : data;
            $scope.params.itemsLoading = false;
        }, function () {
            $scope.params.itemsLoading = false;
        });
    }

    function parseData(data) {
        $scope.data.boxes = $scope.data.boxes ? $scope.data.boxes.concat(data.boxes) : data.boxes;
        $scope.data.items = $scope.data.items ? $scope.data.items.concat(data.items) : data.items;
        $scope.data.users = $scope.data.users ? $scope.data.users.concat(data.users) : data.users;

        if ($scope.params.currentPage === 0 && data.items.length < 50) {
            if (data.items.length === 0) {
                getOtherUnisItems();
                return;
            }

            $scope.params.showOtherUnis = true;
        }
    }

    function setInitTab() {
        var tab;

        if ($scope.params.currentTab !== constants.tabs.boxes) {
            return;
        }

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
