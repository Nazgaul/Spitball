var app;
(function (app) {
    "use strict";
    var unreadCount = 0;
    var ChatBus = (function () {
        function ChatBus(ajaxService, userDetailsFactory) {
            var _this = this;
            this.ajaxService = ajaxService;
            this.userDetailsFactory = userDetailsFactory;
            this.setUnread = function (count) {
                _this.ajaxService.deleteCacheCategory("accountDetail");
                unreadCount = count;
            };
            this.getUnread = function () {
                return unreadCount;
            };
            var response = userDetailsFactory.get();
            this.setUnread(response.unread);
        }
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
            var factory = function (ajaxService2, userDetailsFactory) {
                return new ChatBus(ajaxService2, userDetailsFactory);
            };
            factory['$inject'] = ["ajaxService2", "userDetailsFactory"];
            return factory;
        };
        return ChatBus;
    }());
    angular
        .module("app.chat")
        .factory("chatBus", ChatBus.factory());
})(app || (app = {}));
