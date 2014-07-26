app.controller('MainCtrl',
    ['$scope', '$window', 'cookieService', '$rootScope',
    function ($scope, $window, cookieService, $rootScope) {
        $scope.info = {
            currentLanguage: (function () {
                var language = cookieService('lang');
                if (language) {
                    return language;
                }

                cookieService('lang', 'he-IL', { path: '/' });
                return 'he-IL'

            })()
        };

        $scope.info.currentLanguageDisplay = (function () {
            switch ($scope.info.currentLanguage) {
                case 'en-US':
                    return 'English';
                case 'ru-RU':
                    return 'Pусский';
                case 'ar-AE':
                    return 'العربية';
                default:
                    return 'עברית';
            }
        })();

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
