'use strict';
(function () {
    angular.module('app.chat').controller('ChatController', chat);
    chat.$inject = ['$timeout', '$scope', '$mdSidenav', 'realtimeFactotry',
        'searchService', 'userDetailsFactory', 'chatBus', 'itemThumbnailService', '$mdDialog', 'routerHelper', '$document'];

    function chat($timeout, $scope, $mdSidenav, realtimeFactotry, searchService,
        userDetailsFactory, chatBus, itemThumbnailService, $mdDialog, routerHelper, $document) {
        var c = this, chinkSize = 10, top = 0, fromid;
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
        c.dialog = dialog;
        //c.loadMore = loadMore;

        userDetailsFactory.init().then(function (response) {
            c.unread = response.unread;
            chatBus.setUnread(response.unread);
        });


        // Required.
        //c.getItemAtIndex = function (index) {
        //    if (index > this.numLoaded_) {
        //        this.fetchMoreItems_(index);
        //        return null;
        //    }
        //    return index;
        //}
        //// Required.
        //// For infinite scroll behavior, we always return a slightly higher
        //// number than the previously loaded items.
        //c.getLength = function () {
        //    return this.numLoaded_ + 5;
        //}


        $scope.$watch(function () {
            return $mdSidenav('chat').isOpen();
        }, function (val) {
            if (!val) {
                resetChat();
                return;
            }
            $document.find('.md-sidenav-backdrop').hide();
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
            chatBus.chat(c.userChat.conversation,
                [c.userChat.id, userDetailsFactory.get().id],
                fromid,
                chinkSize,
                top
                ).then(function (response) {
                    response.reverse();
                    for (var i = 0; i < response.length; i++) {
                        response[i].partner = response[i].userId !== userDetailsFactory.get().id;
                        if (response[i].blob) {
                            response[i].thumb = itemThumbnailService.getChat(response[i].blob);
                        }
                    }
                    c.messages = response;
                });

            if (c.userChat.unread) {
                chatBus.read(c.userChat.conversation);
                c.userChat.unread = 0;
                updateUnread();
            }
            c.state = c.states.chat;
        }

        //function loadMore() {
        //    console.log('here');
        //}


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
        $scope.$on('preview-ready', function (e, args) {
            var message = c.messages.find(function (f) {
                return f.blob === args;
            });
            if (message) {
                message.thumb += '&1=1';
            }
            //c.messages
        });
        $scope.$on('hub-chat', function (e, args) {
            //if its me
            if (args.userId !== userDetailsFactory.get().id) {
                if (!c.userChat.conversation) {
                    c.userChat.conversation = args.chatRoom;
                }

                if (args.blob) {
                    args.thumb = itemThumbnailService.getChat(args.blob);
                }
                c.messages.push({
                    text: args.message,
                    time: new Date().toISOString(),
                    partner: false,
                    blob: args.blob,
                    thumb: args.thumb
                });
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

        $scope.$on('hub-status', function(e, args) {
            if (!c.users) {
                return;
            }
            var user = c.users.find(function (f) {
                return f.id === args.userId;
            });
            if (!user) {
                return;
            }
            user.lastSeen = new Date();
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


        //dialog
        //var useFullScreen = ($mdMedia('sm') || $mdMedia('xs')) && $scope.customFullscreen;
        function dialog(blob, ev) {
            $mdDialog.show({
                controller: 'previewController',
                controllerAs: 'lc',
                templateUrl: routerHelper.buildUrl('/chat/previewdialog/'),
                parent: angular.element(document.body),
                targetEvent: ev,
                clickOutsideToClose: true,
                resolve: {
                    doc: function () { return chatBus.preview(blob, 0) }
                }
                //fullscreen: useFullScreen
            });
        }


    };

})();


//'use strict';
(function () {
    angular.module('app.chat').controller('previewController', previewController);
    previewController.$inject = ['$mdDialog', 'doc', '$rootScope', '$sce'];
    function previewController($mdDialog, doc, $rootScope, $sce) {
        var lc = this;
        //lc.users = users;
        lc.close = close;
        console.log(doc);
        function close() {
            $mdDialog.hide();
        }

        lc.view = 'chat-' + doc.viewName + '.html';
        lc.items = doc.content;
        if (doc.viewName === 'Text') {
            lc.items[0] = $sce.trustAsResourceUrl(lc.items[0]);
        }
        $rootScope.$on('$stateChangeStart', function () {
            $mdDialog.hide();
        });
    }
})();


