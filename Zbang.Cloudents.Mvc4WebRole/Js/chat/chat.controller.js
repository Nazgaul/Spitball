'use strict';
(function () {
    angular.module('app.chat').controller('ChatController', chat);
    chat.$inject = ['$rootScope', '$timeout', '$scope', '$mdSidenav', 'realtimeFactotry'];

    function chat($rootScope, $timeout, $scope, $mdSidenav, realtimeFactotry) {
        $scope.$on('open-chat', function () {
            $mdSidenav('chat').toggle();
            $scope.app.chatOpened = !$scope.app.chatOpened;
            $timeout(function () {
                $rootScope.$broadcast('updateScroll');
            });


        });

    }
})();