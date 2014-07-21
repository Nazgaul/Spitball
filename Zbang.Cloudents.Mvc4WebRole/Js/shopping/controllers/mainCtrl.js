app.controller('MainCtrl',
    ['$scope','$window','$cookies',
    function ($scope, $window,$cookies) {

        $scope.params = {
            maxProducts: 9,
            maxProductsIncrement: 9
        };

        $scope.setLanguage = function (val) {
            $cookies.lang = val;
            $window.reload();
        }

    }]
);
