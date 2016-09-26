module app {
    "use strict";

    export interface IBoxService {
        getBox(boxid: number): angular.IPromise<any>;
        getFeed(boxid: number, top: number, skip: number): angular.IPromise<any>;
        getReplies(boxid: number, commentId: Guid, replyId: Guid): angular.IPromise<any>;
        leaderBoard(boxid: number): angular.IPromise<any>;
        getRecommended(boxid: number): angular.IPromise<any>;
        updateBox(boxId: number, name: string, course: string, professor: string, privacy, notification): angular.IPromise<any>;
        unfollow(boxId: number): angular.IPromise<any>;
        notification(boxId: number): angular.IPromise<any>;
        follow(boxId: number): angular.IPromise<any>;
        replyLikes(replyId, boxId: number): angular.IPromise<any>;
        commentLikes(postId: Guid, boxId: number): angular.IPromise<any>;
        likeReply(replyId: Guid, boxId: number): angular.IPromise<any>;
        likeComment(postId: Guid, boxId: number): angular.IPromise<any>;
        deleteReply(postId: Guid, boxId: number): angular.IPromise<any>;
        postReply(content: string, boxId: number, commentId: Guid, files): angular.IPromise<any>;
        deleteComment(commentId: Guid, boxId: number): angular.IPromise<any>;
        postComment(content: string, boxId: number, files: Array<number>, anonymously: boolean): angular.IPromise<any>;
        getQuizzes(boxid: number): angular.IPromise<any>;
        getMembers(boxid: number): angular.IPromise<any>;
        filterItem(term: string, boxId: number, page: number): angular.IPromise<any>;
        deleteItem(itemId: number): angular.IPromise<any>;
        deleteTab(tabId: Guid, boxId: number): angular.IPromise<any>;
        renameTab(tabId: Guid, name: string, boxId: number): angular.IPromise<any>;
        getRecommended(boxid: number): angular.IPromise<any>;
        items(boxId: number, tabId: Guid, page: number): angular.IPromise<any>;
        getTabs(boxid: number): angular.IPromise<any>;
        addItemToTab(boxId: number, tabId: Guid, itemId: number): angular.IPromise<any>;
        createTab(name: string, boxId: number): angular.IPromise<any>;
    }
    class BoxService implements IBoxService {
        static $inject = ["ajaxService2"];
        constructor(private ajaxService2: IAjaxService2) {

        }
        getBox(boxid: number) {
            return this.ajaxService2.get('/box/data/', { id: boxid });
        };
        getFeed(boxid, top, skip) {
            return this.ajaxService2.get('/qna/', { id: boxid, top: top, skip: skip });
        };
        getReplies(boxid, commentId, replyId) {
            return this.ajaxService2.get('/qna/replies/', { boxid: boxid, id: commentId, replyId: replyId });
        }
        leaderBoard(boxid: number) {
            return this.ajaxService2.get('/box/leaderboard/', { id: boxid });
        }
        getRecommended(boxid: number) {
            return this.ajaxService2.get('/box/recommended/', { id: boxid });
        };
        items(boxId: number, tabId:Guid, page:number) {
            return this.ajaxService2.get('/box/items/', { id: boxId, tabId: tabId, page: page });
        };
        getTabs(boxid: number) {
            return this.ajaxService2.get('/box/tabs/', { id: boxid });
        };
        addItemToTab(boxId: number, tabId: Guid, itemId: number) {
            return this.ajaxService2.post('/box/additemtotab/', {
                boxId: boxId,
                tabId: tabId,
                itemId: itemId
            });
        }
        createTab(name:string, boxId: number) {
            return this.ajaxService2.post('/box/createtab/', {
                name: name,
                boxId: boxId
            });
        }
        renameTab(tabId: Guid, name: string, boxId: number) {
            return this.ajaxService2.post('/box/renametab/', {
                tabId: tabId,
                name: name,
                boxId: boxId
            });
        }
        deleteTab(tabId: Guid, boxId: number) {
            return this.ajaxService2.post('/box/deletetab/', {
                tabId: tabId,
                boxId: boxId
            });
        }
        deleteItem(itemId: number) {
            return this.ajaxService2.post('/item/delete/', {
                itemId: itemId
            });
        };
        filterItem(term: string, boxId: number, page: number) {
            return this.ajaxService2.get('/search/iteminbox/', {
                term: term,
                boxId: boxId,
                page: page
            });
        };
        getMembers(boxid: number) {
            return this.ajaxService2.get('/box/members/', { boxId: boxid });
        };
        getQuizzes(boxid: number) {
            return this.ajaxService2.get('/box/quizes/', { id: boxid });
        };
        postComment(content: string, boxId: number, files: Array<number>, anonymously: boolean) {
            return this.ajaxService2.post('/qna/addcomment/', { content: content, boxId: boxId, files: files, anonymously: anonymously });
        }
        deleteComment(commentId: Guid, boxId: number) {
            return this.ajaxService2.post('/qna/deletecomment/', {
                questionId: commentId,
                boxId: boxId
            });
        }
        postReply(content: string, boxId: number, commentId: Guid, files: Array<number>) {
            return this.ajaxService2.post('/qna/addreply/', {
                content: content,
                boxId: boxId,
                commentId: commentId,
                files: files
            });
        }
        deleteReply(postId: Guid, boxId: number) {
            return this.ajaxService2.post('/qna/deletereply/', {
                answerId: postId,
                boxId: boxId
            });
        }
        likeComment(postId: Guid, boxId: number) {
            return this.ajaxService2.post('/qna/likecomment/', {
                commentId: postId,
                boxId: boxId
            });
        }
        likeReply(replyId: Guid, boxId: number) {
            return this.ajaxService2.post('/qna/likereply/', {
                replyId: replyId, boxId: boxId
            });
        }
        commentLikes(postId: Guid, boxId: number) {
            return this.ajaxService2.get('/qna/commentlikes/', {
                id: postId,
                boxId: boxId
            });
        }
        replyLikes(replyId: Guid, boxId: number) {
            return this.ajaxService2.get('/qna/replylikes/', {
                id: replyId,
                boxId: boxId
            });
        }
        follow(boxId: number) {
            return this.ajaxService2.post('/share/subscribetobox/', {
                boxId: boxId
            });

        }
        notification(boxId: number) {
            return this.ajaxService2.get('/box/getnotification/', {
                boxId: boxId
            });
        }
        unfollow(boxId: number) {
            return this.ajaxService2.post('/box/delete/', {
                id: boxId
            });

        }
        updateBox(boxId: number, name: string, course: string, professor: string, privacy, notification) {
            return this.ajaxService2.post('/box/updateinfo/', {
                id: boxId,
                name: name,
                courseCode: course,
                professor: professor,
                boxPrivacy: privacy,
                notification: notification
            });
        }
    }
    angular.module("app.box").service("boxService", BoxService);
}

