'use strict';
(function () {
    angular.module('app.chat').controller('ChatController', chat);
    chat.$inject = ['$timeout', '$scope', '$mdSidenav', 'realtimeFactotry', 'searchService','userService','userDetailsFactory'];

    function chat($timeout, $scope, $mdSidenav, realtimeFactotry, searchService,userService,userDetailsFactory) {
        var c = this;
        c.states = {
            messages: 1,
            friends: 2,
            chat: 3
        };
        c.open = false;
        c.state = c.states.messages;
        c.friendsState = friendsState;
        c.search = search;
        c.chat = conversation;
        c.send = send;
        c.messages = [];
        $scope.$on('open-chat', function () {
            $mdSidenav('chat').toggle();
            $scope.app.chatOpened = !$scope.app.chatOpened;
            //if (!$scope.app.chatOpened) {
            //    return;
            //}
            messageState();
            //TODO: add ajax
            //c.users = [];
            $timeout(function () {
                $scope.$broadcast('updateScroll');

            });
        });

        function messageState() {
            userService.messages().then(function (response) {
                c.users = response;
            });
        }

        function friendsState() {
            c.state = c.states.friends;
            c.term = '';
            search();
        }
        function search() {
            searchService.searchUsers(c.term, 0).then(function (response) {
                c.users = response;
            });
        }

        function conversation(user) {
            c.userChat = user;
            if (c.userChat.conversation) {
                userService.chat(c.userChat.conversation).then(function (response) {
                    for (var i = 0; i < response.length; i++) {
                        response[i].partner = response[i].userId !== userDetailsFactory.get().id;
                    }
                    c.messages = response;
                });
            }
            c.state = c.states.chat;
        }


        function send(myform) {
            if (c.newText === '') {
                return;
            }
            //c.messages.push({
            //    text: c.newText,
            //    time: new Date().toISOString()
            //});
            realtimeFactotry.sendMsg(c.userChat.id, c.newText, c.userChat.conversation);
            c.newText = '';
        }

        $scope.$on('hub-chat', function (e, args) {
            c.messages.push({
                text: args,
                time: new Date().toISOString(),
                partner: true
            });
            $scope.$apply();
        });

        $scope.$on('hub-chat-roomid', function(e, args) {
            c.userChat.conversation = args;

        });
    }
})();