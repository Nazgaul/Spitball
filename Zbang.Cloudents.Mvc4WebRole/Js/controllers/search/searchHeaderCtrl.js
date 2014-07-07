define('searchHeaderCtrl', ['app'], function (app) {
    app.controller('SearchHeaderCtrl',
        ['$scope','Search', 'debounce','$timeout',
        function ($scope, debounce, Search,$timeout) {
            $scope.params = {
                maxItems: 6,
                minItems: 3,
                showDropdown: false
            };

            var lastQuery;
            $scope.search = debounce(function () {
                var query = $scope.formData.query;

                $scope.params.showDropdown = false;

                if (!query) {
                    return;
                }

                if (query === lastQuery) {
                    return;
                }

                lastQuery = query;

                Search.dropdown({ q: query }).then(function (response) {
                    response = response.payload|| {};
                    $scope.params.showDropdown = true;

                    parseData(data);                    
                });
            }, 150);

            $scope.emptyResults = function () {
                return $scope.searchResults.boxes.length + $scope.searchResults.items.length + $scope.searchResults.users.length + $scope.searchResults.otherItems.length
            };

            $scope.searchFocus = function () {
                if (formData.query.length) {
                    $scope.params.showDropdown = true;
                }
            };

            $scope.searchFocusout = function () {
                $timeout(function () {
                    $scope.params.showDropdown = false;
                });
            };

            function parseData(data) {
                $scope.searchResults = {
                    boxes: data.boxes,
                    items: data.items,
                    people: data.users,
                    otherItems: data.otherItems
                };
            }
        }
    ]);
});