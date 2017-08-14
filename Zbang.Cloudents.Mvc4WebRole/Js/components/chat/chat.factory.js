var app;
(function (app) {
    "use strict";
    var unreadCount = 0;
    var ChatBus = (function () {
        function ChatBus(ajaxService) {
            this.ajaxService = ajaxService;
            this.setUnread = function (count) {
                unreadCount = count;
            };
            this.getUnread = function () {
                return unreadCount;
            };
        }
        ChatBus.prototype.getUnreadFromServer = function () {
            var _this = this;
            return this.ajaxService.get("/chat/unread")
                .then(function (response) {
                _this.setUnread(response);
            });
        };
        ChatBus.prototype.messages = function (q, page) {
            return this.ajaxService.get("/chat/conversation", { q: q, page: page });
        };
        ChatBus.prototype.chat = function (id, userIds, dateTime, top) {
            return this.ajaxService.get("/chat/messages", {
                chatRoom: id,
                userIds: userIds,
                startTime: dateTime,
                top: top
            });
        };
        ChatBus.prototype.preview = function (blob, i) {
            return this.ajaxService.get("/chat/Preview", {
                blobName: blob,
                index: i
            });
        };
        ChatBus.prototype.read = function (id) {
            return this.ajaxService.post("chat/markread", {
                chatRoom: id
            });
        };
        ChatBus.factory = function () {
            var factory = function (ajaxService2) {
                return new ChatBus(ajaxService2);
            };
            factory["$inject"] = ["ajaxService2"];
            return factory;
        };
        return ChatBus;
    }());
    angular
        .module("app.chat")
        .factory("chatBus", ChatBus.factory());
})(app || (app = {}));
//# sourceMappingURL=chat.factory.js.map