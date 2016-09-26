module app {
    'use strict';
    enum States {
        UserList = 1,
        Chat = 3
    }
    enum ConnectionStatuses {
        Connected = 1,
        Disconnected = 0
    }
    //var lastConnectionStatus = 0;
    var timeoutvalidate:angular.IPromise<void>;
    class ChatController {
        static $inject = ["$scope", "$timeout", "$stateParams"];
        state = States.UserList;
        connected = false;

        constructor(
            private $scope: angular.IScope,
            private $timeout: angular.ITimeoutService,
            private $stateParams: angular.ui.IStateParamsService) {

            $scope.$on("connection-state", (e, args) => {
                if (args.status === ConnectionStatuses.Disconnected) {
                    // firefox issue upon reload
                    timeoutvalidate = this.$timeout(() => {
                        this.connected = false;
                        $scope.$applyAsync();
                    },50);
                }
                else {
                    if (timeoutvalidate) {
                        this.$timeout.cancel(timeoutvalidate);
                    }
                    this.connected = true;
                    $scope.$applyAsync();
                }
               

            });

            if ($stateParams["conversationData"]) {
                this.state = States.Chat;
                this.$scope.$broadcast("go-chat", $stateParams["conversationData"]);

            }
           
            $scope.$on("open-chat-user", (e, args) => {
                this.state = States.Chat;
                this.$scope.$broadcast("go-chat", args);
            });
            $scope.$on("go-chat",
                (e, args) => {
                    this.state = States.Chat;
                    this.$timeout(() => {
                        this.$scope.$broadcast("go-conversation", args);
                    });
                });
        }
        //TODO: get rid of
        scrollSetting = {
            scrollbarPosition: 'outside',
            scrollInertia: 50
        };
        backFromChat() {
            this.state = States.UserList;
        }

    }

    angular.module("app.chat").controller("ChatController", ChatController);
}
//(function () {
//    angular.module('app.chat').controller('ChatController', chat);
//    chat.$inject = ['$timeout', '$scope', 'realtimeFactory',
//        'searchService', 'userDetailsFactory', 'chatBus', 'itemThumbnailService',
//        '$mdDialog', 'routerHelper', '$document',
//        'notificationService', 'resManager', '$rootScope', "$uiViewScroll", "$stateParams"];

//    function chat($timeout, $scope, realtimeFactory, searchService,
//        userDetailsFactory, chatBus, itemThumbnailService, $mdDialog, routerHelper, $document,
//        notificationService, resManager, $rootScope, $uiViewScroll, $stateParams) {
//        var c = this, chunkSize = 50, page = 0,
//            connectionStatuses = {
//                connected: 1,
//                disconnected: 0
//            };
//        c.states = {
//            messages: 1,
//            chat: 3
//        };


//        c.state = c.states.messages;
//        c.connected = true;
//        c.resetSearch = resetSearch;
//        c.expandSearch = expandSearch;
//        c.search = search;
//        c.chat = conversation;
//        c.send = send;
//        c.messages = [];
//        c.backFromChat = backFromChat;
//        //c.unread = 0;
//        c.dialog = dialog;
//        c.users = [];
//        c.usersPaging = usersPaging;
//        c.lastSearch = c.term = '';
//        c.loadMoreMessages = loadMoreMessages;
//        c.focusSearch = false;
//        c.lastPage = false;

//        search();


//        c.scrollSetting = {
//            scrollbarPosition: 'outside',
//            scrollInertia: 50
//        };








//        function conversation(user) {
//            c.userChat = user;
//            c.messages = [];
//            c.lastPage = false;
//            chatBus.chat(c.userChat.conversation,
//                [c.userChat.id, userDetailsFactory.get().id],
//                '',
//                chunkSize
//            ).then(function (response) {
//                c.messages = handleChatMessages(response);
//                scrollToBotton();

//                if (response.length < chunkSize) {
//                    c.lastPage = true;
//                }
//            });

//            if (c.userChat.unread) {
//                chatBus.read(c.userChat.conversation);
//                c.userChat.unread = 0;
//                updateUnread();
//            }

//            $rootScope.$broadcast("expandChat");
//            c.state = c.states.chat;
//        }



//        function loadMoreMessages() {
//            var firstMessage = c.messages[0];
//            if (!firstMessage.id) {
//                return;
//            }
//            chatBus.chat(c.userChat.conversation,
//                [c.userChat.id, userDetailsFactory.get().id],
//                c.messages[0].time,
//                chunkSize
//            ).then(function (response) {
//                c.messages = handleChatMessages(response).concat(c.messages);
//                $timeout(function () {
//                    $uiViewScroll(angular.element('#chatMessage_' + firstMessage.id));
//                });
//                if (response.length < chunkSize) {
//                    c.lastPage = true;
//                }
//            });
//        }


//        function send() {
//            if (!c.newText) {
//                return;
//            }
//            c.messages.push({
//                text: c.newText,
//                time: new Date().toISOString(),
//                partner: false
//            });
//            realtimeFactory.sendMsg(c.userChat.id, c.newText, c.userChat.conversation);
//            c.newText = '';
//        }
//        if ($stateParams.conversationData) {
//            conversation($stateParams.conversationData);
//        }
//        $scope.$on('open-chat-user', function (e, args) {
//            conversation(args);
//        });
//        $scope.$on('preview-ready', function (e, args) {
//            var message = c.messages.find(function (f) {
//                return f.blob === args;
//            });
//            if (message) {
//                message.thumb += '&1=1';
//            }
//        });



//        $scope.$on('hub-status', function (e, args) {
//            if (!c.users) {
//                return;
//            }
//            var user = c.users.find(function (f) {
//                return f.id === args.userId;
//            });
//            if (!user) {
//                return;
//            }
//            user.lastSeen = new Date().toISOString();
//            user.online = args.online;
//            $scope.$apply();
//        });






//        //dialog
//        //var useFullScreen = ($mdMedia('sm') || $mdMedia('xs')) && $scope.customFullscreen;
//        function dialog(blob, ev) {
//            $rootScope.$broadcast('disablePaging');
//            $mdDialog.show({
//                controller: 'previewController',
//                controllerAs: 'lc',
//                templateUrl: routerHelper.buildUrl('/chat/previewdialog/'),
//                parent: angular.element(document.body),
//                targetEvent: ev,
//                clickOutsideToClose: true,
//                resolve: {
//                    doc: function () { return chatBus.preview(blob, 0); },
//                    blob: function () { return blob; }
//                },
//                fullscreen: true
//            }).finally(function () {
//                $rootScope.$broadcast('enablePaging');
//            });
//        }


//    }

//})();



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

        lc.downloadLink = `/chat/download/?blobName=${blob}`;
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
