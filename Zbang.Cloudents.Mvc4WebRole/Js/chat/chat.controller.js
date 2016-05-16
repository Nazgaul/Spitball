'use strict';
(function () {
    angular.module('app.chat').controller('ChatController', chat);
    chat.$inject = ['$timeout', '$scope', '$mdSidenav', 'realtimeFactotry', 'searchService'];

    function chat($timeout, $scope, $mdSidenav, realtimeFactotry, searchService) {
        var c = this;
        c.states = {
            messages: 1,
            friends: 2,
            chat: 3
        };
        c.state = c.states.messages;
        c.friendsState = friendsState;
        c.search = friendsState;
        $scope.$on('open-chat', function () {
            $mdSidenav('chat').toggle();
            $scope.app.chatOpened = !$scope.app.chatOpened;
            if (!$scope.app.chatOpened) {
                return;
            }
            //TODO: add ajax
            c.users = [];
            

           
            $timeout(function () {
                $scope.$broadcast('updateScroll');

            });
        });

        function friendsState() {
            c.state = c.states.friends;
            c.term = '';
            searchService.searchUsers(c.term, 0).then(function (response) {
                c.users = response;
            });
        }
        

    }
})();