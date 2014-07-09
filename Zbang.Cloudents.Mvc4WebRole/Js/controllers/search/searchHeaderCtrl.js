app.controller('SearchHeaderCtrl',
    ['$scope', '$timeout', 'debounce', 'sSearch',
    function ($scope, $timeout, debounce, Search) {
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

        var lastQuery, lastResultCount;
        $scope.search = debounce(function () {
            var query = $scope.formData.query;

            if (!query) {
                $scope.params.showDropdown = false;
                return;
            }

            if (query === lastQuery) {
                return;
            }

            lastQuery = query;

            Search.dropdown({ q: query }).then(function (response) {
                var data = response.success ? response.payload : {};
                $scope.params.showDropdown = true;

                parseData(data);
            });
        }, 150);

        $scope.resultCount = function () {
            return $scope.searchResults.boxes.length + $scope.searchResults.items.length +
                    $scope.searchResults.people.length + $scope.searchResults.otherItems.length;
        };

        $scope.searchFocus = function () {
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


        $scope.$on('$routeChangeStart', function () {
            $scope.params.showDropdown = false;
            $scope.formData.query = null;
            $scope.searchResults = {
                boxes: [],
                items: [],
                people: [],
                otherItems: []
            };
        });

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
                    emptyCategories = data.items.length + data.boxes.length + data.users.length;

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

                $scope.searchResults.boxes = (data.boxes || []).slice(maxCategoryItems);
                $scope.searchResults.items = (data.items || []).slice(maxCategoryItems);
                $scope.searchResults.people = (data.users || []).slice(maxCategoryItems);
                $scope.searchResults.otherItems = (data.otherItems || []).slice(maxOtherItems);
            }

        };
    }
    ]);