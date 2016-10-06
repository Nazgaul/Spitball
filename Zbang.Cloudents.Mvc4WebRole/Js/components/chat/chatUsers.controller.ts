module app {
    "use strict";
    var page = 0;


    class ChatUsers {
        static $inject = ["chatBus", "userDetailsFactory", "$timeout",
            "$rootScope", "$scope", "notificationService", "resManager"];
        focusSearch = false;
        users = [];
        term: string;
        constructor(private chatBus: IChatBus,
            private userDetailsFactory: IUserDetailsFactory,
            private $timeout: angular.ITimeoutService,
            private $rootScope: angular.IRootScopeService,
            private $scope: angular.IScope,
            private notificationService /*TODO*/,
            private resManager: IResManager) {
            this.search();

            $scope.$on("hub-status", (e, args) => {
                var user = this.users.find(f => (f.id === args.userId));
                if (!user) {
                    return;
                }
                user.lastSeen = new Date().toISOString();
                user.online = args.online;
                $scope.$applyAsync();
            });
            $scope.$on("refresh-boxes", () => {
                this.search().then(() => {
                    $scope.$applyAsync();
                });
            });
            $scope.$on("remove-box", () => {
                this.search().then(() => {
                    $scope.$applyAsync();
                });
            });
            $scope.$on("hub-chat",
                (e, args) => {
                    //im not in chat at all
                    var self = this;
                    if (!self.users.length) {
                        this.search();
                        notificationService.send(resManager.get('toasterChatMessage'),
                            args.message,
                            null,
                            onNotificationClick);
                        //soundsService.handlers.chat();
                        self.updateUnread();
                        $scope.$applyAsync();
                        return;
                    }

                    //got conversation with that user
                    var user = self.getConversationPartner(args.chatRoom);

                    if (user) {
                        notificationService.send(user.name, args.message, user.image, onNotificationClick);
                        //soundsService.handlers.chat();
                        user.unread++;
                        self.updateUnread();
                        $scope.$applyAsync();
                        return;
                    }
                    //got no conversation with that user
                    user = self.users.find(f => (f.id === args.user));
                    if (!user) {
                        notificationService.send(resManager.get('toasterChatMessage'),
                            args.message,
                            null,
                            onNotificationClick);
                        //soundsService.handlers.chat();

                        //need to refresh data to find it.
                        self.search();
                        return;
                    }
                    notificationService.send(user.name, args.message, user.image);
                    //soundsService.handlers.chat();
                    user.unread++;
                    user.conversation = args.id;
                    self.updateUnread();
                    $scope.$applyAsync();



                    function onNotificationClick() {
                        const partner = self.getConversationPartner(args.chatRoom);
                        self.chat(partner);

                    }

                });
        }
        private getConversationPartner(chatRoomId) {
            return this.users.find(f => (f.conversation === chatRoomId));
        }
        search(term?: string, loadNextPage?: boolean): angular.IPromise<any> {
            if (!loadNextPage) {
                page = 0;
            }
            if (!term) {
                this.term = '';
            }
            return this.chatBus.messages(term, page).then(response => {
                if (loadNextPage) {
                    this.users = this.makeUniqueAndRemoveMySelf(this.users.concat(response));

                } else {
                    page = 0;
                    this.users = this.makeUniqueAndRemoveMySelf(response);
                }
                // TODO: hack to fix
                if (!Modernizr.cssscrollbar) {
                    this.$scope["c"].updateScrollbar2("update");
                }
                this.updateUnread();
            });
        }
        expandSearch() {
            this.$rootScope.$broadcast("expandChat");
            this.focusSearch = true;
        }
        

        private makeUniqueAndRemoveMySelf(array) {
            const flags = [];
            const output = [];
            const l = array.length;
            let i: number;
            for (i = 0; i < l; i++) {
                if (array[i].id === this.userDetailsFactory.get().id) continue;
                if (flags[array[i].id]) continue;
                flags[array[i].id] = true;
                output.push(array[i]);
            }
            return output;
        }
        private updateUnread() {
            if (this.users) {
                var x = 0;
                for (let i = 0; i < this.users.length; i++) {
                    x += this.users[i].unread || 0;
                }
                this.$timeout(() => { //give it some delay
                    this.chatBus.setUnread(x);
                });
            } //else {
            //c.unread = ++c.unread;
            //chatBus.setUnread(++c.unread);
            //}

        }
        usersPaging = ()=> {
            page++;
            this.search(this.term, true);
        }

        chat(user) {

            if (user.unread) {
                user.unread = 0;
                this.chatBus.read(user.conversation);
                this.updateUnread();
            }
            this.$rootScope.$broadcast("expandChat");
            this.$scope.$emit("go-chat", user);
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
        }
    }


    angular.module("app.chat").controller("chatUsers", ChatUsers);
}