mAccount.controller('AccountCtrl',
['$scope','$timeout','sAccount',
function ($scope,$timeout,sAccount) {
    "use strict";

    $timeout(function () {
        $scope.$emit('viewContentLoaded');
    });

    $scope.changeLocale = function(lang){
         $angularCacheFactory.get('htmlCache').removeAll();

                $analytics.eventTrack('Language Change', {
                    category: 'Register Popup',
                    label: 'User changed language to ' +lang
                });

                sAccount.changeLocale({ language: lang}).then(function () {
         
                 
                    $window.location.reload();
                });
    };
        



}
]);