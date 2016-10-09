module app {
    "use strict";
    var chunkSize = 50;
    class Conversation {
        static $inject = ["$scope", "chatBus", "userDetailsFactory",
            "$timeout", "itemThumbnailService", "realtimeFactory",
            "$uiViewScroll", "routerHelper", "$mdDialog"];
        userChat;
        lastPage = false;
        messages = [];
        newText: string;
        constructor(private $scope: angular.IScope,
            private chatBus: IChatBus,
            private userDetailsFactory: IUserDetailsFactory,
            private $timeout: angular.ITimeoutService,
            private itemThumbnailService: IItemThumbnailService, 
            private realtimeFactory: IRealtimeFactory,
            private $uiViewScroll: any,
            private routerHelper: IRouterHelper,
            private $mdDialog: angular.material.IDialogService


        ) {
            $scope.$on("go-conversation",
                (e, args) => {
                    this.conversation(args);

                });
            $scope.$on("preview-ready", (e, args) => {
                const message = this.messages.find(f => (f.blob === args));
                if (message) {
                    message.thumb += '&1=1';
                }
            });

            $scope.$on("hub-chat", (e, args) => {
                //if its me
                if (args.userId !== userDetailsFactory.get().id) {
                    //can be from different platform
                    if (!this.userChat) {
                        return;
                    }
                    if (!this.userChat.conversation) {
                        this.userChat.conversation = args.chatRoom;
                    }

                    const messages = this.messages.filter(message => (message.text === args.message));
                    const attachments = this.messages.filter(message => (message.blob === args.blob));
                    // if there's no message with same text or there's one - but older than a minute ago
                    //(meaning we are connected to the chatroom in other place and sent new message there): 
                    if (!args.blob && (!messages.length || messages[messages.length - 1].time < new Date(+new Date() - 60000).toISOString())) {
                        this.messages.push({
                            text: args.message,
                            time: new Date().toISOString(),
                            partner: false
                        });
                    }
                    if (args.blob && !attachments.length) {
                        this.messages.push({
                            blob: args.blob,
                            time: new Date().toISOString(),
                            thumb: itemThumbnailService.getChat(args.blob),
                            partner: false
                        });
                    }

                    
                    // TODO: hack - do better
                    this.$timeout(() => { //give it some delay
                        var unread = this.chatBus.getUnread();
                        this.chatBus.setUnread(--unread);
                    });
                    
                    $scope.$apply();
                    this.scrollToBotton();
                    return;
                }
                // im in the same chat
                if (this.userChat && this.userChat.conversation === args.chatRoom) {
                    if (args.blob) {
                        args.thumb = itemThumbnailService.getChat(args.blob);
                    }
                    this.messages.push({
                        text: args.message,
                        time: new Date().toISOString(),
                        partner: true,
                        blob: args.blob,
                        thumb: args.thumb
                    });
                    
                    // TODO: hack - do better
                    this.$timeout(() => { //give it some delay
                        var unread = this.chatBus.getUnread();
                        this.chatBus.setUnread(--unread);
                    });
                    $scope.$apply();
                    this.chatBus.read(this.userChat.conversation);
                    this.scrollToBotton();
                    return;
                }

            });

        }
        private conversation(userChat) {
            this.userChat = userChat;
            this.chatBus.chat(this.userChat.conversation,
                [this.userChat.id, this.userDetailsFactory.get().id],
                null,
                chunkSize
            ).then(response => {
                this.messages = this.handleChatMessages(response);
                this.scrollToBotton();

                if (response.length < chunkSize) {
                    this.lastPage = true;
                }
            });
        }

        send() {
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
        }

        private handleChatMessages(response) {
            response.reverse();
            for (let i = 0; i < response.length; i++) {
                response[i].partner = response[i].userId !== this.userDetailsFactory.get().id;
                if (response[i].blob) {
                    response[i].thumb = this.itemThumbnailService.getChat(response[i].blob);
                }
            }
            return response;
        }
        private scrollToBotton() {
            this.$timeout(() => {
                this.$scope.$broadcast("chat-scroll");
            });
        }

        loadMoreMessages() {
            var firstMessage = this.messages[0];
            if (!firstMessage.id) {
                return;
            }
            this.chatBus.chat(this.userChat.conversation,
                [this.userChat.id, this.userDetailsFactory.get().id],
                this.messages[0].time,
                chunkSize
            ).then(response => {
                this.messages = this.handleChatMessages(response).concat(this.messages);
                this.$timeout(() => {
                    this.$uiViewScroll(angular.element(`#chatMessage_${firstMessage.id}`));
                });
                if (response.length < chunkSize) {
                    this.lastPage = true;
                }
            });
        }

        // dialog
        dialog(blob, ev) {
            this.$scope.$broadcast("disablePaging");
            this.$mdDialog.show({
                controller: "previewController",
                controllerAs: "lc",
                templateUrl: this.routerHelper.buildUrl("/chat/previewdialog/"),
                parent: angular.element(document.body),
                targetEvent: ev,
                clickOutsideToClose: true,
                resolve: {
                    doc: () => this.chatBus.preview(blob, 0),
                    blob: () => blob
                },
                fullscreen: true
            }).finally(() => {
                this.$scope.$broadcast("enablePaging");
            });
        }

        upload = {
            url: "/upload/chatfile/",
            options: {
                chunk_size: "3mb"
            },
            callbacks: {
                filesAdded: (uploader) => {
                    this.$timeout(() => {
                        uploader.start();
                    }, 1);
                },
                beforeUpload: (up, file) => {
                    up.settings.multipart_params = {
                        fileName: file.name,
                        fileSize: file.size,
                        users: [this.userChat.id]
                    };
                },
                fileUploaded: (uploader, file, response) => {
                    const obj = JSON.parse(response.response);
                    if (obj.success) {
                        this.messages.push({
                            time: new Date().toISOString(),
                            partner: false,
                            blob: obj.payload,
                            thumb: this.itemThumbnailService.getChat(obj.payload)
                        });
                        this.realtimeFactory.sendMsg(this.userChat.id, null, this.userChat.conversation, obj.payload);

                    }
                }

            }
        };
    }

    angular.module("app.chat").controller("conversation", Conversation);


}