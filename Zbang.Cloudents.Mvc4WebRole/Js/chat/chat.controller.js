'use strict';
(function () {
    angular.module('app.dashboard').controller('ChatController', chat);
    chat.$inject = ['$rootScope', '$timeout', '$scope', '$mdSidenav'];

    function chat($rootScope, $timeout, $scope, $mdSidenav) {
        $scope.$on('open-chat', function () {
            $mdSidenav('chat').toggle();
            $scope.app.chatOpened = !$scope.app.chatOpened;
            $timeout(function () {
                $rootScope.$broadcast('updateScroll');
            });
        });

    }
})();