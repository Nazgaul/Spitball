"use strict";
var app;
(function (app) {
    "use strict";
    var chunkSize = 50;
    var Conversation = (function () {
        function Conversation($scope, chatBus, userDetailsFactory, $timeout, itemThumbnailService, realtimeFactory, $uiViewScroll, routerHelper, $mdDialog) {
            var _this = this;
            this.$scope = $scope;
            this.chatBus = chatBus;
            this.userDetailsFactory = userDetailsFactory;
            this.$timeout = $timeout;
            this.itemThumbnailService = itemThumbnailService;
            this.realtimeFactory = realtimeFactory;
            this.$uiViewScroll = $uiViewScroll;
            this.routerHelper = routerHelper;
            this.$mdDialog = $mdDialog;
            this.lastPage = false;
            this.messages = [];
            this.upload = {
                url: "/upload/chatfile/",
                options: {
                    chunk_size: "3mb"
                },
                callbacks: {
                    filesAdded: function (uploader) {
                        _this.$timeout(function () {
                            uploader.start();
                        }, 1);
                    },
                    beforeUpload: function (up, file) {
                        up.settings.multipart_params = {
                            fileName: file.name,
                            fileSize: file.size,
                            users: [_this.userChat.id]
                        };
                    },
                    fileUploaded: function (uploader, file, response) {
                        var obj = JSON.parse(response.response);
                        if (obj.success) {
                            _this.messages.push({
                                time: new Date().toISOString(),
                                partner: false,
                                blob: obj.payload,
                                thumb: _this.itemThumbnailService.getChat(obj.payload)
                            });
                            _this.realtimeFactory.sendMsg(_this.userChat.id, null, _this.userChat.conversation, obj.payload);
                        }
                    }
                }
            };
            $scope.$on("go-conversation", function (e, args) {
                _this.conversation(args);
            });
            $scope.$on("preview-ready", function (e, args) {
                var message = _this.messages.find(function (f) { return (f.blob === args); });
                if (message) {
                    message.thumb += '&1=1';
                }
            });
            $scope.$on("hub-chat", function (e, args) {
                //if its me
                if (args.userId !== userDetailsFactory.get().id) {
                    //can be from different platform
                    if (!_this.userChat) {
                        return;
                    }
                    if (!_this.userChat.conversation) {
                        _this.userChat.conversation = args.chatRoom;
                    }
                    var messages = _this.messages.filter(function (message) { return (message.text === args.message); });
                    var attachments = _this.messages.filter(function (message) { return (message.blob === args.blob); });
                    // if there's no message with same text or there's one - but older than a minute ago
                    //(meaning we are connected to the chatroom in other place and sent new message there): 
                    if (!args.blob && (!messages.length || messages[messages.length - 1].time < new Date(+new Date() - 60000).toISOString())) {
                        _this.messages.push({
                            text: args.message,
                            time: new Date().toISOString(),
                            partner: false
                        });
                    }
                    if (args.blob && !attachments.length) {
                        _this.messages.push({
                            blob: args.blob,
                            time: new Date().toISOString(),
                            thumb: itemThumbnailService.getChat(args.blob),
                            partner: false
                        });
                    }
                    // TODO: hack - do better
                    _this.$timeout(function () {
                        var unread = _this.chatBus.getUnread();
                        _this.chatBus.setUnread(--unread);
                    });
                    $scope.$apply();
                    _this.scrollToBotton();
                    return;
                }
                // im in the same chat
                if (_this.userChat && _this.userChat.conversation === args.chatRoom) {
                    if (args.blob) {
                        args.thumb = itemThumbnailService.getChat(args.blob);
                    }
                    _this.messages.push({
                        text: args.message,
                        time: new Date().toISOString(),
                        partner: true,
                        blob: args.blob,
                        thumb: args.thumb
                    });
                    // TODO: hack - do better
                    _this.$timeout(function () {
                        var unread = _this.chatBus.getUnread();
                        _this.chatBus.setUnread(--unread);
                    });
                    $scope.$apply();
                    _this.chatBus.read(_this.userChat.conversation);
                    _this.scrollToBotton();
                    return;
                }
            });
        }
        Conversation.prototype.conversation = function (userChat) {
            var _this = this;
            this.userChat = userChat;
            this.chatBus.chat(this.userChat.conversation, [this.userChat.id, this.userDetailsFactory.get().id], null, chunkSize).then(function (response) {
                _this.messages = _this.handleChatMessages(response);
                _this.scrollToBotton();
                if (response.length < chunkSize) {
                    _this.lastPage = true;
                }
            });
        };
        Conversation.prototype.send = function () {
            if (!this.newText) {
                return;
            }
            this.messages.push({
                text: this.newText,
                time: new Date().toISOString(),
                partner: false
            });
            this.realtimeFactory.sendMsg(this.userChat.id, this.newText, this.userChat.conversation);
            this.newText = "";
        };
        Conversation.prototype.handleChatMessages = function (response) {
            response.reverse();
            for (var i = 0; i < response.length; i++) {
                response[i].partner = response[i].userId !== this.userDetailsFactory.get().id;
                if (response[i].blob) {
                    response[i].thumb = this.itemThumbnailService.getChat(response[i].blob);
                }
            }
            return response;
        };
        Conversation.prototype.scrollToBotton = function () {
            var _this = this;
            this.$timeout(function () {
                _this.$scope.$broadcast("chat-scroll");
            });
        };
        Conversation.prototype.loadMoreMessages = function () {
            var _this = this;
            var firstMessage = this.messages[0];
            if (!firstMessage.id) {
                return;
            }
            this.chatBus.chat(this.userChat.conversation, [this.userChat.id, this.userDetailsFactory.get().id], this.messages[0].time, chunkSize).then(function (response) {
                _this.messages = _this.handleChatMessages(response).concat(_this.messages);
                _this.$timeout(function () {
                    _this.$uiViewScroll(angular.element("#chatMessage_" + firstMessage.id));
                });
                if (response.length < chunkSize) {
                    _this.lastPage = true;
                }
            });
        };
        // dialog
        Conversation.prototype.dialog = function (blob, ev) {
            var _this = this;
            this.$scope.$broadcast("disablePaging");
            this.$mdDialog.show({
                controller: "previewController",
                controllerAs: "lc",
                templateUrl: this.routerHelper.buildUrl("/chat/previewdialog/"),
                parent: angular.element(document.body),
                targetEvent: ev,
                clickOutsideToClose: true,
                resolve: {
                    doc: function () { return _this.chatBus.preview(blob, 0); },
                    blob: function () { return blob; }
                },
                fullscreen: true
            }).finally(function () {
                _this.$scope.$broadcast("enablePaging");
            });
        };
        return Conversation;
    }());
    Conversation.$inject = ["$scope", "chatBus", "userDetailsFactory",
        "$timeout", "itemThumbnailService", "realtimeFactory",
        "$uiViewScroll", "routerHelper", "$mdDialog"];
    angular.module("app.chat").controller("conversation", Conversation);
})(app || (app = {}));
//# sourceMappingURL=conversation.controller.js.map