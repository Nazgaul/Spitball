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
            // friends: 2,
            chat: 3
        };
        // c.open = false;
        c.state = c.states.messages;
        //c.friendsState = friendsState;
        c.search = search;
        c.chat = conversation;
        c.send = send;
        c.close = close;
        c.messages = [];
        c.backFromChat = backFromChat;
        c.unread = 0;


        chatBus.unread().then(function (response) {
            updateUnread(response);
        });

        $scope.$watch(function () {
            return $mdSidenav('chat').isOpen();
        }, function (val) {
            if (!val) {
                resetChat();
                return;
            }

            messageState();
        });
        function backFromChat() {
            c.state = c.states.messages;
            resetChat();
        }
        function resetChat() {
            c.userChat = null;
        }

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
            chatBus.messages().then(function (response) {
                c.users = response;
            });

        }

        //function friendsState() {
        //    c.state = c.states.friends;
        //    c.term = '';
        //    search();
        //}
        function search() {
            chatBus.messages(c.term).then(function (response) {
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
            c.userChat = c.userChat || {};
            if (args.chatRoom !== c.userChat.conversation) {
                return;
            }
            c.messages.push({
                text: args.message,
                time: new Date().toISOString(),
                partner: true
            });
            $scope.$apply();
        });

        $scope.$on('hub-chat-roomid', function (e, args) {
            if (!c.userChat.conversation) {
                c.userChat.conversation = args.message;
            }
        });
        $scope.$on('hub-chat-room', function (e, args) {
            //if (!c.userChat.conversation) {
            //    c.userChat.conversation = args.message;
            //}
        });
    }
})();

(function () {
    angular.module('app.chat').controller('chatIndicatorController', chatIndicator);

    chatIndicator.$inject = ['chatBus', '$mdSidenav'];

    function chatIndicator(chatBus, $mdSidenav) {
        var cc = this;

        cc.unread = chatBus.getUnread;
        cc.openChat = function () {
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
(function () {
    angular.module('app.chat').factory('chatBus', chatBus);
    chatBus.$inject = ['ajaxService'];

    function chatBus(ajaxService) {
        var unreadCount = 0;
        var chatService = {};

        chatService.setUnread = function (count) {
            unreadCount = count;
        };
        chatService.getUnread = function () {
            return unreadCount;
        };

        chatService.messages = function (q) {
            return ajaxService.get('/chat/conversation', { q: q });
        }
        chatService.chat = function (id) {
            return ajaxService.get('/chat/messages', {
                chatRoom: id
            });
        }
        chatService.unread = function () {
            return ajaxService.get('chat/unreadcount');
        }


        return chatService;
    }
})()