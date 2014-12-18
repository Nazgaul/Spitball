mAccount.controller('AccountCtrl',
['$scope', '$timeout', 'sAccount', '$angularCacheFactory', '$window', '$analytics', '$location',
function ($scope, $timeout, sAccount, $angularCacheFactory, $window, $analytics, $location) {
    "use strict";

    $scope.params = {};

    if ($location.hash().indexOf('register') > -1 || $location.hash().indexOf('login') > -1 || $location.hash().indexOf('facebook') > -1) {
        $scope.params.showLogin = true;
    }

    $timeout(function () {
        $scope.$emit('viewContentLoaded');
    });

    $scope.changeLocale = function (lang) {
        $angularCacheFactory.get('htmlCache').removeAll();

        $analytics.eventTrack('Language Change', {
            category: 'Register Popup',
            label: 'User changed language to ' + lang
        });

        sAccount.changeLocale({ language: lang }).then(function () {
            $window.location.reload();
        });
    };
}
]);