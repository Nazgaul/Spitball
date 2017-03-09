module app {
    'use strict';

    export interface IItemService {
        getDetails(boxId: number, itemId: number, firstTime: boolean): angular.IPromise<any>;
        getPreview(blobName: string, index: number, itemId: number, boxId: number): angular.IPromise<any>;
        addLink(url: string, boxid: number, tabid: Guid, question: Guid, name: string): angular.IPromise<any>;
        addGoogle(url: string, boxid: number, tabid: Guid, question: Guid, name: string): angular.IPromise<any>;
        addFromDropBox(boxId: number, tabid: Guid, url: string, name: string, question: Guid): angular.IPromise<any>;
        renameItem(name: string, id: number): angular.IPromise<any>;
        getComments(boxId: number, itemId: number): angular.IPromise<any>;
        addComment(comment: string, boxId: number, id: number): angular.IPromise<any>;
        replycomment(reply: string, itemId: number, boxid: number, commentId: Guid): angular.IPromise<any>;
        deleteReply(replyId: Guid, itemId: number): angular.IPromise<any>;
        deleteComment(commentId: Guid, itemId: number): angular.IPromise<any>;
        like(id: number, boxId: number): angular.IPromise<any>;
        flag(badItem, other, itemId: number): angular.IPromise<any>;
        followbox();
    }

    class ItemService implements IItemService {
        static $inject = ['ajaxService2'];
        constructor(private ajaxService2: IAjaxService2) {

        }

        getDetails(boxId, itemId, firstTime) {
            return this.ajaxService2.get('/item/load/', { boxId: boxId, itemId: itemId, firstTime: firstTime });
        };

        getPreview(blobName, index, itemId, boxId) {
            return this.ajaxService2.get('/item/preview/',
                {
                    blobName: blobName,
                    index: index,
                    id: itemId,
                    boxId: boxId
                });
        };


        addLink(url, boxid, tabid, question, name) {
            return this.ajaxService2.post('/upload/link',
                {
                    url: url,
                    boxid: boxid,
                    tabid: tabid,
                    question: question,
                    name: name
                });
        }
        addGoogle(url, boxid, tabid, question, name) {
            return this.ajaxService2.post('/upload/google',
                {
                    url: url,
                    boxid: boxid,
                    tabid: tabid,
                    question: question,
                    name: name
                });
        }
        addFromDropBox(boxId, tabid, url, name, question) {
            return this.ajaxService2.post('/upload/dropbox/',
                {
                    boxId: boxId,
                    tabId: tabid,
                    url: url,
                    name: name,
                    question: question
                });
        }

        renameItem(name, id) {
            return this.ajaxService2.post('/item/rename',
                {
                    id: id,
                    newName: name
                })
                .then(response => ({ name: response["name"], url: response["url"] }));
        }

        getComments(boxId, itemId) {
            return this.ajaxService2.get('/item/comment',
                {
                    boxId: boxId,
                    itemId: itemId
                },
                'itemComment');
        }

        addComment(comment, boxId, id) {
            return this.ajaxService2.post('/item/addcomment',
                {
                    itemId: id,
                    boxId: boxId,
                    comment: comment
                },['itemComment', "boxData"]);
        }

        replycomment(reply, itemId, boxid, commentId) {
            return this.ajaxService2.post('/item/replycomment',
                {
                    comment: reply,
                    itemid: itemId,
                    commentid: commentId,
                    boxid: boxid
                },['itemComment', "boxData"]);
        }

        deleteReply(replyId, itemId) {
            return this.ajaxService2.post('/item/deletecommentreply',
                {
                    ReplyId: replyId,
                    itemid: itemId
                },
                'itemComment');
        }

        deleteComment(commentId, itemId) {
            return this.ajaxService2.post('/item/deletecomment',
                {
                    CommentId: commentId,
                    itemid: itemId
                },
                'itemComment');
        }

        like(id, boxId) {
            return this.ajaxService2.post('/item/like',
                {
                    itemId: id,
                    boxId: boxId
                }, ["boxData","boxItems"]);
        };
        followbox() {
            this.ajaxService2.deleteCacheCategory("boxData");
        }
        flag(badItem, other, itemId) {
            return this.ajaxService2.post('/item/flagrequest/',
                {
                    badItem: badItem,
                    other: other,
                    itemId: itemId
                });
        }

    }
    angular.module('app.item').service('itemService', ItemService);

}