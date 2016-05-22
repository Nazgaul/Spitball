'use strict';
(function () {
    angular.module('app.chat').controller('ChatController', chat);
    chat.$inject = ['$timeout', '$scope', '$mdSidenav', 'realtimeFactotry',
        'searchService', 'userDetailsFactory', 'chatBus', 'itemThumbnailService'];

    function chat($timeout, $scope, $mdSidenav, realtimeFactotry, searchService,
        userDetailsFactory, chatBus, itemThumbnailService) {
        var c = this;
        c.states = {
            messages: 1,
            // friends: 2,
            chat: 3
        };
        c.state = c.states.messages;
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

        function search() {
            chatBus.messages(c.term).then(function (response) {
                c.users = response;
                updateUnread();
            });
        }

        function conversation(user) {
            c.userChat = user;
            c.messages = [];
            //if (c.userChat.conversation) {
            chatBus.chat(c.userChat.conversation, [c.userChat.id, userDetailsFactory.get().id]).then(function (response) {
                for (var i = 0; i < response.length; i++) {
                    response[i].partner = response[i].userId !== userDetailsFactory.get().id;
                    if (response[i].blob) {
                        response[i].blob = itemThumbnailService.getChat(response[i].blob);
                    }
                }
                c.messages = response;
            });
            //}

            if (c.userChat.unread) {
                chatBus.read(c.userChat.conversation);
                c.userChat.unread = 0;
                updateUnread();
            }
            c.state = c.states.chat;
        }


        function send() {
            if (c.newText === '') {
                return;
            }
            realtimeFactotry.sendMsg(c.userChat.id, c.newText, c.userChat.conversation);
            c.newText = '';
        }

        $scope.$on('open-chat-user', function (e, args) {
            $mdSidenav('chat').open();
            conversation(args);
        });
        $scope.$on('hub-chat', function (e, args) {
            //if its me
            if (args.userId !== userDetailsFactory.get().id) {
                if (!c.userChat.conversation) {
                    c.userChat.conversation = args.chatRoom;
                }
               
                c.messages.push({
                    text: args.message,
                    time: new Date().toISOString(),
                    partner: false,
                    blob: itemThumbnailService.getChat(args.blob)
                });
                updateScope();
                return;
            }
            // im in the same chat
            if (c.userChat && c.userChat.conversation === args.chatRoom) {
                c.messages.push({
                    text: args.message,
                    time: new Date().toISOString(),
                    partner: true,
                    blob: itemThumbnailService.getChat(args.blob)
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


        c.upload = {
            url: '/upload/chatfile/',
            options: {
                chunk_size: '3mb'
            },
            callbacks: {
                filesAdded: function (uploader) {
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },

                beforeUpload: function (up, file) {
                    up.settings.multipart_params = {
                        fileName: file.name,
                        fileSize: file.size
                    };
                },
                fileUploaded: function (uploader, file, response) {
                    // cacheFactory.clearAll();
                    // file.complete = true;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        realtimeFactotry.sendMsg(c.userChat.id, null, c.userChat.conversation, obj.payload);
                        //     u.filesCompleteCount++;
                        //     file.systemId = obj.payload.item.id;
                        //     $rootScope.$broadcast('item_upload', obj.payload);
                    }
                }
                //uploadComplete: function () {
                //    //toasterUploadComplete
                //    //$scope.app.showToaster(resManager.get('toasterUploadComplete'));
                //    //$timeout(closeUpload, 2000);
                //},
                //error: function (uploader, error) {
                //    //error.file.error = true;
                //    //u.filesErrorCount++;
                //}
            }
        }


    };

})();


