mAccount.controller('AccountCtrl',
['$scope', '$timeout', 'sAccount',
    //'$angularCacheFactory',
    '$window', '$analytics', '$location',
function ($scope, $timeout, sAccount,
    //$angularCacheFactory,
    $window, $analytics, $location) {
    "use strict";

    $scope.params = {};
    $scope.data = {};
    switch ($location.hash()) {
        case 'login':
            $scope.data.state = 2;
            $scope.params.showLogin = true;
            break;
        case 'register':
            $scope.data.state = 1;
            $scope.params.showLogin = true;
            break;        
    }    
    $timeout(function () {
        $scope.$emit('viewContentLoaded');
    });

    $scope.changeLocale = function (lang) {
        //$angularCacheFactory.get('htmlCache').removeAll();

        $analytics.eventTrack('Language Change', {
            category: 'Home page',
            label: 'User changed language to ' + lang
        });

        sAccount.changeLocale({ language: lang }).then(function () {
            $window.location.reload();
        });
    };

    $scope.close = function () {
        $scope.params.showLogin = false;
    };

    $scope.login = function () {
        $scope.data = {
            state :2 //login
        };
        $scope.params.showLogin = true;
    };

    $scope.register= function () {
        $scope.data = {
            state: 1 //register
        };
        $scope.params.showLogin = true;
    };    
}
]);