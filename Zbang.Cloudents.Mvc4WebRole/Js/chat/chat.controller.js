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
            c.unread = response;
            chatBus.setUnread(response);
        });

        $scope.$watch(function () {
            return $mdSidenav('chat').isOpen();
        }, function (val) {
            if (!val) {
                resetChat();
                return;
            }

            search();
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
        function updateUnread() {
            if (c.users) {
                var x = 0;
                for (var i = 0; i < c.users.length; i++) {
                    x += c.users[i].unread || 0;
                }
                c.unread = x;
                chatBus.setUnread(x);
            } else {
                c.unread = ++c.unread;
                chatBus.setUnread(c.unread);
            }

        }
        //function messageState() {
        //    chatBus.messages().then(function (response) {
        //        c.users = response;
        //    });

        //}

        //function friendsState() {
        //    c.state = c.states.friends;
        //    c.term = '';
        //    search();
        //}
        function search() {
            chatBus.messages(c.term).then(function (response) {
                c.users = response;
                updateUnread();
            });
        }

        function conversation(user) {
            c.userChat = user;
            c.messages = [];
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
            //if its me
            if (args.userId !== userDetailsFactory.get().id) {
                if (!c.userChat.conversation) {
                    c.userChat.conversation = args.chatRoom;
                }
                c.messages.push({
                    text: args.message,
                    time: new Date().toISOString(),
                    partner: false
                });
                updateScope();
                return;
            }
            // im in the same chat
            if (c.userChat && c.userChat.conversation === args.chatRoom) {
                c.messages.push({
                    text: args.message,
                    time: new Date().toISOString(),
                    partner: true
                });
                updateScope();
                return;
            }
            //im not in chat at all
            if (!c.users) {
                updateUnread();
                updateScope();
                return;
            }
            //got conversation with that user
            var user = c.users.find(function (f) {
                return f.conversation === args.chatRoom;
            });
            if (user) {
                user.unread++;
                updateUnread();
                updateScope();
                return;
            }
            //got no conversation with that user
            user = c.users.find(function (f) {
                return f.id === args.user;
            });
            if (!user) {
                //need to refresh data to find it.
                search();
                return;
            };
            user.unread++;
            user.conversation = args.id;
            updateUnread();
            updateScope();


            function updateScope() {
                $scope.$apply();
            }
        });
        //c.userChat = c.userChat || {};
        //// we are in the conversation
        //if (c.userChat.conversation === args.chatRoom) {
        //    c.messages.push({
        //        text: args.message,
        //        time: new Date().toISOString(),
        //        partner: true
        //    });
        //need to remove unread







        //c.userChat = c.userChat || {};
        //if (args.chatRoom !== c.userChat.conversation) {
        //    if (!c.users) {
        //        updateUnread();
        //        $scope.$apply();
        //        return;
        //    }
        //    var user = c.users.find(function(f) {
        //        return f.conversation === args.chatRoom;
        //    });
        //    if (!user) {
        //        //TODO: not sure how
        //        return;
        //    };
        //    user.unread++;
        //    updateUnread();
        //    $scope.$apply();
        //    //user.conversation = 
        //    //return;
        //} else {
        //    c.messages.push({
        //        text: args.message,
        //        time: new Date().toISOString(),
        //        partner: true
        //    });
        //    $scope.$apply();
        //}

        //});

        // the caller gets
        //$scope.$on('hub-chat-roomid', function (e, args) {
        //    if (!c.userChat.conversation) {
        //        c.userChat.conversation = args.message;
        //    }
        //    $scope.$apply();
        //});
        // the cally gets
        //$scope.$on('hub-chat-room', function (e, args) {
        //    if (!c.users) {
        //        updateUnread();
        //        $scope.$apply();
        //        return;
        //    }
        //    var user = c.users.find(function (f) {
        //        return f.id === args.user;
        //    });
        //    if (!user) {
        //        search();
        //        return;
        //    };
        //    user.unread++;
        //    user.conversation = args.id;
        //    updateUnread();
        //    $scope.$apply();
        //    //if (!c.userChat.conversation) {
        //    //    c.userChat.conversation = args.message;
        //    //}
    };

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
            return ajaxService.get('/chat/conversation', { q: q }, 0);
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