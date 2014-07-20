app.controller('HomeCtrl',
    ['$scope','Shopping',
    function ($scope,Shopping) {
        Shopping.products().then(function (response) {
            $scope.products = response;
        });

    }]
);
