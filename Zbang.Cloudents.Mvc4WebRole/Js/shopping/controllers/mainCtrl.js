app.controller('MainCtrl',
    ['$scope', '$window', 'cookieService', '$rootScope',
    function ($scope, $window, cookieService, $rootScope) {

        $scope.info = {
            currentLanguage: cookieService('lang') || cookieService('lang', 'en-US', { path: '/' }),
        };

        $scope.setLanguage = function (val) {
            if ($scope.info.currentLanguage === val) {
                return;
            }
            cookieService('lang', val, { path: '/' });
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
