module app {
    "use strict";
    var unreadCount = 0;
    export interface IChatBus {
        setUnread(count: number): void;
        getUnread(): number;
        messages(q: string, page: number): angular.IPromise<Array<any>>;
        chat(id: string, userIds: Array<number>, dateTime: Date, top: number): angular.IPromise<Array<any>>;
        preview(blob: string, i: number): angular.IPromise<Array<any>>;
        read(id: string): angular.IPromise<any>;
        getUnreadFromServer(): angular.IPromise<any>; // can be number
    }

    class ChatBus implements IChatBus {
        // static $inject = ["ajaxService2"];

        constructor(private ajaxService: IAjaxService2
        ) {}

        setUnread = (count: number): void => {
            unreadCount = count;
        };
        getUnread = (): number => {
            return unreadCount;
        };

        getUnreadFromServer() {
            return this.ajaxService.get("/chat/unread")
                .then((response: number) => {
                    this.setUnread(response);
                });
        }

        messages(q: string, page: number): angular.IPromise<Array<any>> {
            return this.ajaxService.get("/chat/conversation", { q: q, page: page });
        }
        chat(id: string, userIds: Array<number>, dateTime: Date, top: number): angular.IPromise<Array<any>> {
            return this.ajaxService.get("/chat/messages", {
                chatRoom: id,
                userIds: userIds,
                startTime: dateTime,
                top: top
            });
        }
        preview(blob: string, i: number): angular.IPromise<Array<any>> {
            return this.ajaxService.get("/chat/Preview", {
                blobName: blob,
                index: i
            });
        }
        read(id: string): angular.IPromise<any> {
            return this.ajaxService.post("chat/markread", {
                chatRoom: id
            });
        }
        public static factory() {
            const factory = (ajaxService2: IAjaxService2) => {
                return new ChatBus(ajaxService2);
            };

            factory["$inject"] = ["ajaxService2"];
            return factory;
        }
    }
    angular
        .module("app.chat")
        .factory("chatBus", ChatBus.factory());
}

