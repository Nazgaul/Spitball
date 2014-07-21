app.controller('MainCtrl',
    ['$scope', '$window', '$cookies',
    function ($scope, $window, $cookies) {

        console.log($cookies.lang);

        $scope.info = {
            currentLanguage: $cookies.lang || 'en-US'
        };

        $scope.setLanguage = function (val) {
            if ($cookies.lang === val) {
                return;
            }
            $cookies.lang = val;
            $window.location.reload();
        };

    }]
);
