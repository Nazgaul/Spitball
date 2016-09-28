var app;
(function (app) {
    "use strict";
    var page = 0;
    var ChatUsers = (function () {
        function ChatUsers(chatBus, userDetailsFactory, $timeout, $rootScope, $scope, notificationService, resManager) {
            var _this = this;
            this.chatBus = chatBus;
            this.userDetailsFactory = userDetailsFactory;
            this.$timeout = $timeout;
            this.$rootScope = $rootScope;
            this.$scope = $scope;
            this.notificationService = notificationService;
            this.resManager = resManager;
            this.focusSearch = false;
            this.users = [];
            this.search();
            $scope.$on("hub-status", function (e, args) {
                var user = _this.users.find(function (f) { return (f.id === args.userId); });
                if (!user) {
                    return;
                }
                user.lastSeen = new Date().toISOString();
                user.online = args.online;
                $scope.$applyAsync();
            });
            $scope.$on("refresh-boxes", function () {
                _this.search().then(function () {
                    $scope.$applyAsync();
                });
            });
            $scope.$on("hub-chat", function (e, args) {
                var self = _this;
                if (!self.users.length) {
                    _this.search();
                    notificationService.send(resManager.get('toasterChatMessage'), args.message, null, onNotificationClick);
                    self.updateUnread();
                    $scope.$applyAsync();
                    return;
                }
                var user = self.getConversationPartner(args.chatRoom);
                if (user) {
                    notificationService.send(user.name, args.message, user.image, onNotificationClick);
                    user.unread++;
                    self.updateUnread();
                    $scope.$applyAsync();
                    return;
                }
                user = self.users.find(function (f) { return (f.id === args.user); });
                if (!user) {
                    notificationService.send(resManager.get('toasterChatMessage'), args.message, null, onNotificationClick);
                    self.search();
                    return;
                }
                notificationService.send(user.name, args.message, user.image);
                user.unread++;
                user.conversation = args.id;
                self.updateUnread();
                $scope.$applyAsync();
                function onNotificationClick() {
                    var partner = self.getConversationPartner(args.chatRoom);
                    self.chat(partner);
                }
            });
        }
        ChatUsers.prototype.getConversationPartner = function (chatRoomId) {
            return this.users.find(function (f) { return (f.conversation === chatRoomId); });
        };
        ChatUsers.prototype.search = function (term, loadNextPage) {
            var _this = this;
            if (!loadNextPage) {
                page = 0;
            }
            if (!term) {
                this.term = '';
            }
            return this.chatBus.messages(term, page).then(function (response) {
                if (loadNextPage) {
                    _this.users = _this.makeUniqueAndRemoveMySelf(_this.users.concat(response));
                }
                else {
                    page = 0;
                    _this.users = _this.makeUniqueAndRemoveMySelf(response);
                }
                _this.updateUnread();
            });
        };
        ChatUsers.prototype.expandSearch = function () {
            this.$rootScope.$broadcast("expandChat");
            this.focusSearch = true;
        };
        ChatUsers.prototype.makeUniqueAndRemoveMySelf = function (array) {
            var flags = [];
            var output = [];
            var l = array.length;
            var i;
            for (i = 0; i < l; i++) {
                if (array[i].id === this.userDetailsFactory.get().id)
                    continue;
                if (flags[array[i].id])
                    continue;
                flags[array[i].id] = true;
                output.push(array[i]);
            }
            return output;
        };
        ChatUsers.prototype.updateUnread = function () {
            var _this = this;
            if (this.users) {
                var x = 0;
                for (var i = 0; i < this.users.length; i++) {
                    x += this.users[i].unread || 0;
                }
                this.$timeout(function () {
                    _this.chatBus.setUnread(x);
                });
            }
        };
        ChatUsers.prototype.usersPaging = function () {
            page++;
            this.search(this.term, true);
        };
        ChatUsers.prototype.chat = function (user) {
            if (user.unread) {
                this.chatBus.read(user.conversation);
                this.updateUnread();
            }
            this.$rootScope.$broadcast("expandChat");
            this.$scope.$emit("go-chat", user);
        };
        ChatUsers.$inject = ["chatBus", "userDetailsFactory", "$timeout",
            "$rootScope", "$scope", "notificationService", "resManager"];
        return ChatUsers;
    }());
    angular.module("app.chat").controller("chatUsers", ChatUsers);
})(app || (app = {}));
//# sourceMappingURL=chatUsers.controller.js.map