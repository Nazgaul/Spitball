module app {
    "use strict";
    export interface IChatBus {
        setUnread(count: number): void;
        getUnread(): number;
        messages(q: string, page: number): angular.IPromise<Array<any>>;
        chat(id: string, userIds: Array<number>, dateTime: Date, top: number): angular.IPromise<Array<any>>;
        preview(blob: string, i: number): angular.IPromise<Array<any>>;
        read(id: string): angular.IPromise<any>;
    }

    class ChatBus implements IChatBus {
        static $inject = ["ajaxService2"];
        private unreadCount = 0;
        constructor(private ajaxService: IAjaxService2) {

        }

        setUnread(count: number): void {
            this.unreadCount = count;
        };
        getUnread = (): number => {
            return this.unreadCount;
        };

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
    }
    angular
        .module("app.chat")
        .service("chatBus", ChatBus);
}

