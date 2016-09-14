'use strict';
(function () {
    angular.module('app.chat').controller('ChatController', chat);
    chat.$inject = ['$timeout', '$scope', 'realtimeFactotry',
        'searchService', 'userDetailsFactory', 'chatBus', 'itemThumbnailService',
        '$mdDialog', 'routerHelper', '$document',
        'notificationService', 'resManager', '$rootScope', "$uiViewScroll", "$stateParams"];

    function chat($timeout, $scope, realtimeFactotry, searchService,
        userDetailsFactory, chatBus, itemThumbnailService, $mdDialog, routerHelper, $document,
        notificationService, resManager, $rootScope, $uiViewScroll, $stateParams) {
        var c = this, chunkSize = 50, page = 0,
        connectionStatuses = {
            connected: 1,
            disconnected: 0
        };
        c.states = {
            messages: 1,
            chat: 3
        };

        var lastConnectionStatus = 0;
        c.state = c.states.messages;
        c.connected = true;
        c.resetSearch = resetSearch;
        c.expandSearch = expandSearch;
        c.search = search;
        c.chat = conversation;
        c.send = send;
        c.messages = [];
        c.backFromChat = backFromChat;
        //c.unread = 0;
        c.dialog = dialog;
        c.users = [];
        c.usersPaging = usersPaging;
        c.lastSearch = c.term = '';
        c.loadMoreMessages = loadMoreMessages;
        c.focusSearch = false;
        c.lastPage = false;

        search();


        c.scrollSetting = {
            scrollbarPosition: 'outside',
            scrollInertia: 50
        };




        function backFromChat() {
            c.state = c.states.messages;
            c.newText = '';
            resetChat();
        }
        function resetChat() {
            c.userChat = null;
            page = 0;
        }

        function updateUnread() {
            if (c.users) {
                var x = 0;
                for (var i = 0; i < c.users.length; i++) {
                    x += c.users[i].unread || 0;
                }
                $timeout(function () { //give it some delay
                    chatBus.setUnread(x);
                });
            } //else {
            //c.unread = ++c.unread;
            //chatBus.setUnread(++c.unread);
            //}

        }

        function resetSearch() {
            c.term = '';
            c.search('');
        }

        function search(term, loadNextPage) {
            if (!loadNextPage) {
                page = 0;
            }
            chatBus.messages(term, page).then(function (response) {
                if (loadNextPage) {
                    c.users = makeUniqueAndRemoveMySelf(c.users.concat(response));

                } else {
                    page = 0;
                    c.users = makeUniqueAndRemoveMySelf(response);
                }
                updateUnread();
            });
        }
        function makeUniqueAndRemoveMySelf(array) {

            var flags = [], output = [], l = array.length, i;
            for (i = 0; i < l; i++) {
                if (array[i].id === userDetailsFactory.get().id) continue;
                if (flags[array[i].id]) continue;
                flags[array[i].id] = true;
                output.push(array[i]);
            }
            return output;
        }

        function handleChatMessages(response) {
            response.reverse();
            for (var i = 0; i < response.length; i++) {
                response[i].partner = response[i].userId !== userDetailsFactory.get().id;
                if (response[i].blob) {
                    response[i].thumb = itemThumbnailService.getChat(response[i].blob);
                }
            }
            return response;
        }


        function conversation(user) {
            c.userChat = user;
            c.messages = [];
            c.lastPage = false;
            chatBus.chat(c.userChat.conversation,
                [c.userChat.id, userDetailsFactory.get().id],
                '',
                chunkSize
                ).then(function (response) {
                    c.messages = handleChatMessages(response);
                    scrollToBotton();

                    if (response.length < chunkSize) {
                        c.lastPage = true;
                    }
                });

            if (c.userChat.unread) {
                chatBus.read(c.userChat.conversation);
                c.userChat.unread = 0;
                updateUnread();
            }

            $rootScope.$broadcast("expandChat");
            c.state = c.states.chat;
        }

        function expandSearch() {
            $rootScope.$broadcast("expandChat");
            c.focusSearch = true;
        }

        function loadMoreMessages() {
            var firstMessage = c.messages[0];
            if (!firstMessage.id) {
                return;
            }
            chatBus.chat(c.userChat.conversation,
                [c.userChat.id, userDetailsFactory.get().id],
                c.messages[0].time,
                chunkSize
                ).then(function (response) {
                    c.messages = handleChatMessages(response).concat(c.messages);
                    $timeout(function () {
                        $uiViewScroll(angular.element('#chatMessage_' + firstMessage.id));
                    });
                    if (response.length < chunkSize) {
                        c.lastPage = true;
                    }
                });
        }


        function send() {
            if (!c.newText) {
                return;
            }
            c.messages.push({
                text: c.newText,
                time: new Date().toISOString(),
                partner: false
            });
            realtimeFactotry.sendMsg(c.userChat.id, c.newText, c.userChat.conversation);
            c.newText = '';
        }
        if ($stateParams.conversationData) {
            conversation($stateParams.conversationData);
        }
        $scope.$on('open-chat-user', function (e, args) {
            conversation(args);
        });
        $scope.$on('preview-ready', function (e, args) {
            var message = c.messages.find(function (f) {
                return f.blob === args;
            });
            if (message) {
                message.thumb += '&1=1';
            }
        });
        $scope.$on('connection-state', function (e, args) {
            if (args.status === connectionStatuses.disconnected) {
                if (args.status !== lastConnectionStatus) {
                    c.connected = false;
                }
            }
            else {
                c.connected = true;
            }
            lastConnectionStatus = args.status;

        });
        $scope.$on('hub-chat', function (e, args) {
            //if its me
            if (args.userId !== userDetailsFactory.get().id) {
                //can be from different platform
                if (!c.userChat) {
                    return;
                }
                if (!c.userChat.conversation) {
                    c.userChat.conversation = args.chatRoom;
                }

                var messages = c.messages.filter(function (message) {
                    return message.text === args.message;
                });


                var attachments = c.messages.filter(function (message) {
                    return message.blob === args.blob;
                });
                // if there's no message with same text or there's one - but older than a minute ago
                //(meaning we are connected to the chatroom in other place and sent new message there): 
                if (!args.blob && (!messages.length || messages[messages.length - 1].time < new Date(new Date() - 60000).toISOString())) {
                    c.messages.push({
                        text: args.message,
                        time: new Date().toISOString(),
                        partner: false
                    });
                }
                if (args.blob && !attachments.length) {
                    c.messages.push({
                        blob: args.blob,
                        time: new Date().toISOString(),
                        thumb: itemThumbnailService.getChat(args.blob),
                        partner: false
                    });
                }

                scrollToBotton();
                updateUnread();
                updateScope();
                return;
            }
            // im in the same chat
            if (c.userChat && c.userChat.conversation === args.chatRoom) {
                if (args.blob) {
                    args.thumb = itemThumbnailService.getChat(args.blob);
                }
                c.messages.push({
                    text: args.message,
                    time: new Date().toISOString(),
                    partner: true,
                    blob: args.blob,
                    thumb: args.thumb
                });
                scrollToBotton();
                updateUnread();
                updateScope();
                return;
            }
            //im not in chat at all

            if (!c.users.length) {
                search();
                notificationService.send(resManager.get('toasterChatMessage'), args.message, null, onNotificationClick);
                //soundsService.handlers.chat();
                updateUnread();
                updateScope();
                return;
            }

            //got conversation with that user
            var user = getConversationPartner(args.chatRoom);

            if (user) {
                notificationService.send(user.name, args.message, user.image, onNotificationClick);
                //soundsService.handlers.chat();
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
                notificationService.send(resManager.get('toasterChatMessage'), args.message, null, onNotificationClick);
                //soundsService.handlers.chat();

                //need to refresh data to find it.
                search();
                return;
            }
            notificationService.send(user.name, args.message, user.image);
            //soundsService.handlers.chat();
            user.unread++;
            user.conversation = args.id;
            updateUnread();
            updateScope();

            function getConversationPartner(chatRoomId) {
                return c.users.find(function (f) {
                    return f.conversation === chatRoomId;
                });
            }
            function onNotificationClick() {
                var partner = getConversationPartner(args.chatRoom);
                conversation(partner);
            }
            function updateScope() {
                $scope.$apply();
            }
        });

        $scope.$on('hub-status', function (e, args) {
            if (!c.users) {
                return;
            }
            var user = c.users.find(function (f) {
                return f.id === args.userId;
            });
            if (!user) {
                return;
            }
            user.lastSeen = new Date().toISOString();
            user.online = args.online;
            $scope.$apply();
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
                    },
                        1);
                },

                beforeUpload: function (up, file) {
                    up.settings.multipart_params = {
                        fileName: file.name,
                        fileSize: file.size,
                        users: [c.userChat.id]
                    };
                },
                fileUploaded: function (uploader, file, response) {
                    // cacheFactory.clearAll();
                    // file.complete = true;

                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        c.messages.push({
                            time: new Date().toISOString(),
                            partner: false,
                            blob: obj.payload,
                            thumb: itemThumbnailService.getChat(obj.payload)
                        });
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
        };

        function scrollToBotton() {
            $timeout(function () {
                $scope.$broadcast('chat-scroll');
                //$scope.scrollToBotton();
                //        c.updateScrollbar('scrollTo', 'bottom', { scrollInertia: 0, timeout: 100 });
            });
        }


        //dialog
        //var useFullScreen = ($mdMedia('sm') || $mdMedia('xs')) && $scope.customFullscreen;
        function dialog(blob, ev) {
            $rootScope.$broadcast('disablePaging');
            $mdDialog.show({
                controller: 'previewController',
                controllerAs: 'lc',
                templateUrl: routerHelper.buildUrl('/chat/previewdialog/'),
                parent: angular.element(document.body),
                targetEvent: ev,
                clickOutsideToClose: true,
                resolve: {
                    doc: function () { return chatBus.preview(blob, 0); },
                    blob: function () { return blob; }
                },
                fullscreen: true
            }).finally(function () {
                $rootScope.$broadcast('enablePaging');
            });
        }

        function usersPaging() {
            page++;
            search(c.term, true);
        }
    }

})();



(function () {
    'use strict';
    angular.module('app.chat').controller('previewController', previewController);
    previewController.$inject = ['$mdDialog', 'doc', 'blob', '$rootScope', '$sce'];
    function previewController($mdDialog, doc, blob, $rootScope, $sce) {
        var lc = this;
        //lc.users = users;
        lc.close = close;
        function close() {
            $mdDialog.hide();
        }

        lc.downloadLink = '/chat/download/?blobName=' + blob;
        if (!doc || !doc.viewName) {
            lc.view = 'preview-faild.html';
        }
        else {
            lc.items = doc.content;
            if (doc.viewName === 'Text') {
                lc.items[0] = $sce.trustAsResourceUrl(lc.items[0]);
            }
            lc.view = 'chat-' + doc.viewName + '.html';
        }
        $rootScope.$on('$stateChangeStart', function () {
            $mdDialog.hide();
        });
    }
})();
