app.controller('MainCtrl',
    ['$scope', '$window', '$cookies', '$rootScope',
    function ($scope, $window, $cookies, $rootScope) {

        $scope.info = {
            currentLanguage: $cookies.lang || 'en-US',            
        };

        $scope.setLanguage = function (val) {
            if ($cookies.lang === val) {
                return;
            }
            $cookies.lang = val;
            $window.location.reload();
        };

        $scope.$on('$routeChangeSuccess', function (event, current, previous) {
            if (!current.$$route) {
                return;
            }

            if (current.$$route.type === 'products' && current.params.categoryId === '646') {
                $scope.info.currentTab = 'sales';
                return;
            }
            $scope.info.currentTab = current.$$route.type;
        });

    }]
);
