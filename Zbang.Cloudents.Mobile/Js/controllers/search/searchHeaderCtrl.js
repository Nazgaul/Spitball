
app.controller('SearchHeaderCtrl',
    ['$scope', '$timeout', '$location', 'debounce', 'sSearch', 'sUserDetails', 'textDirectionService', '$analytics', 'sLogin',
    function ($scope, $timeout, $location, debounce, Search, sUserDetails, textDirectionService, $analytics, sLogin) {
        "use strict";
        $scope.params = {
            maxItems: 6,
            minItems: 3,
            showDropdown: false
        };

        $scope.searchResults = {
            boxes: [],
            items: [],
            people: [],
            otherItems: []
        };

        $scope.formData = {};


        //$timeout(function () {
        //    if ($location.search()['q']) {
        //        var query = $location.search()['q'];
        //        if (query) {
        //            $scope.formData.query = query;
        //            $scope.params.preventDropDown = true;
        //            $scope.search();
        //        }
        //    }
        //});


        var lastQuery, lastResultCount;
        $scope.search = debounce(function () {
            var query = $scope.formData.query;

            if (!query) {
                $scope.params.showDropdown = false;
                lastQuery = null;
                return;
            }

            if (query === lastQuery) {
                return;
            }

            lastQuery = query;

            $analytics.eventTrack('Search Header', {
                category: 'Search',
                label: 'User searched for ' + query
            });
            Search.dropdown({ q: query }).then(function (data) {
                parseData(data);
                if ($scope.params.preventDropDown) {
                    $scope.params.preventDropDown = false;
                    return;
                }

                $scope.params.showDropdown = true;

            });
        }, 150);

        $scope.fullSearch = function (isValid) {
            if (!isValid) {
                return;
            }

            $analytics.eventTrack('Search', {
                category: 'Full Search'
            });

            $location.url('/search/?q=' + $scope.formData.query);

        };


        $scope.resultCount = function () {
            return $scope.searchResults.boxes.length + $scope.searchResults.items.length +
                    $scope.searchResults.people.length + $scope.searchResults.otherItems.length;
        };

        $scope.searchFocus = function () {
            if (!sUserDetails.isAuthenticated()) {
                sLogin.registerAction();
                return;
            }

            if ($scope.formData.query && $scope.formData.query.length) {
                $scope.params.showDropdown = true;
            }
        };

        $scope.searchFocusout = function () {
            if ($scope.params.hover) {
                return;
            }

            $scope.params.showDropdown = false;
        };

        $scope.$watch('formData.query', function (v) {
            if (!v) {
                return;
            }
            $scope.params.textDirection = textDirectionService.isRTL(v) ? 'rtl' : 'ltr';
        });

        //$scope.$on('$routeChangeStart', function () {
        //    $scope.params.showDropdown = false;
        //    $scope.formData.query = null;
        //});

        //$scope.$on('$routeUpdate', function () {
        //    $scope.params.showDropdown = false;
        //    $scope.params.preventDropDown = true;
        //    $scope.searchResults = {
        //        boxes: [],
        //        items: [],
        //        people: [],
        //        otherItems: []
        //    };
        //});

        $scope.openSearch = function () {
            $scope.params.isOpen = true;
        };

        $scope.cancel = function () {
            $scope.params.isOpen = false;
        };
        function parseData(data) {
            if (!lastResultCount) {                
                lastResultCount = $scope.resultCount();                
                appendData();
                return;
            }

            if (!lastResultCount === 0 && $scope.resultCount() === 0 && $scope.params.showDropdown) {
                return;
            }

            appendData();

            function appendData() {


                var maxCategoryItems = 0,
                    maxOtherItems = 0,
                    emptyCategories = (data.items.length === 0) + (data.boxes.length === 0) + (data.users.length === 0);


                switch (emptyCategories) {
                    case 0:
                        maxCategoryItems = $scope.params.minItems;
                        break;
                    case 2:
                        maxCategoryItems = $scope.params.maxItems;
                        maxOtherItems = $scope.params.minItems;
                        break;
                    case 3:
                        maxOtherItems = $scope.params.maxItems;
                        break;
                    default:
                        maxCategoryItems = $scope.params.minItems;
                        maxOtherItems = $scope.params.minItems;
                        break;
                };


                $scope.searchResults.boxes = minmizeItems(data.boxes, maxCategoryItems);
                $scope.searchResults.items = minmizeItems(data.items, maxCategoryItems);
                $scope.searchResults.people = minmizeItems(data.users, maxCategoryItems);
                $scope.searchResults.otherItems = minmizeItems(data.otherItems, maxOtherItems);

                function minmizeItems(array, maxItems) {
                    array = array || [];
                    return array.slice(0, maxItems);

                }
            }

        };
    }
    ]);