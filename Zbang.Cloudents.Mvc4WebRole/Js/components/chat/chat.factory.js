var app;
(function (app) {
    "use strict";
    var ChatBus = (function () {
        function ChatBus(ajaxService) {
            this.ajaxService = ajaxService;
            this.unreadCount = 0;
        }
        ChatBus.prototype.setUnread = function (count) {
            this.unreadCount = count;
        };
        ;
        ChatBus.prototype.getUnread = function () {
            return this.unreadCount;
        };
        ;
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
        ChatBus.$inject = ["ajaxService2"];
        return ChatBus;
    }());
    angular
        .module("app.chat")
        .service("chatBus", ChatBus);
})(app || (app = {}));
