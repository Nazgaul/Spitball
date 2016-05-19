'use strict';
(function () {
    angular.module('app.chat').controller('ChatController', chat);
    chat.$inject = ['$timeout', '$scope', '$mdSidenav', 'realtimeFactotry',
        'searchService', 'userDetailsFactory', 'chatBus'];

    function chat($timeout, $scope, $mdSidenav, realtimeFactotry, searchService,
        userDetailsFactory, chatBus) {
        var c = this;
        c.states = {
            messages: 1,
            friends: 2,
            chat: 3
        };
        // c.open = false;
        c.state = c.states.messages;
        c.friendsState = friendsState;
        c.search = search;
        c.chat = conversation;
        c.send = send;
        c.close = close;
        c.messages = [];
        c.unread = 0;


        chatBus.unread().then(function(response) {
            updateUnread(response);
        });

        $scope.$watch(function() {
            return $mdSidenav('chat').isOpen();
        }, function (val) {
            if (!val) {
                return;
            }
           
            messageState();
        });

        function close() {
            $mdSidenav('chat').close();
        }
        //$scope.$on('toggle-chat', function () {
        //    $mdSidenav('chat').toggle();
        //    if (!$mdSidenav('chat').isOpen()) {
        //        return;
        //    }
        //    //$scope.app.chatOpened = !$scope.app.chatOpened;
        //    //if (!$scope.app.chatOpened) {
        //    //    return;
        //    //}
        //    //chatBus.set (100);
        //    //messageState();
        //    //TODO: add ajax
        //    //c.users = [];
        //    //$timeout(function () {
        //    //    $scope.$broadcast('updateScroll');
        //    //
        //    //});
        //});
        function updateUnread(count) {
            c.unread = count;
            chatBus.setUnread(count);
        }
        function messageState() {
            //var count = 0;
            chatBus.messages().then(function (response) {
                //for (var i = 0; i < response.length; i++) {
                //    count += response[i].unread;
                //}
                //updateUnread(count);
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
                chatBus.chat(c.userChat.conversation).then(function (response) {
                    for (var i = 0; i < response.length; i++) {
                        response[i].partner = response[i].userId !== userDetailsFactory.get().id;
                    }
                    c.messages = response;
                });
            }
            c.state = c.states.chat;
        }


        function send() {
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

        $scope.$on('hub-chat-roomid', function (e, args) {
            c.userChat.conversation = args;

        });
    }
})();

(function () {
    angular.module('app.chat').controller('chatIndicatorController', chatIndicator);

    chatIndicator.$inject = ['chatBus', '$mdSidenav'];

    function chatIndicator(chatBus, $mdSidenav) {
        var cc = this;

        cc.unread = chatBus.getUnread;
        cc.openChat = function() {
            $mdSidenav('chat').open();
        }
        //return {
        //    restrict: 'A',
        //    scope: true,
        //    link: function (scope, element, attrs) {

        //        scope.x = userService.getUnreadCount();
        //        //element.text(userService.getUnreadCount());


        //    }
        //}
    }
})();
(function() {
    angular.module('app.chat').factory('chatBus', chatBus);
    chatBus.$inject = ['ajaxService'];

    function chatBus(ajaxService) {
        var unreadCount = 0;
        var chatService = {};

        chatService.setUnread = function(count) {
            unreadCount = count;
        };
        chatService.getUnread = function() {
            return unreadCount;
        };

        chatService.messages = function() {
            return ajaxService.get('/chat/conversation');
        }
        chatService.chat = function(id) {
            return ajaxService.get('/chat/messages', {
                chatRoom: id
            });
        }
        chatService.unread = function() {
            return ajaxService.get('chat/unreadcount');
        }


        return chatService;
    }
})()