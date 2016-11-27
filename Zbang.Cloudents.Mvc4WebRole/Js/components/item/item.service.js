var app;
(function (app) {
    'use strict';
    var ItemService = (function () {
        function ItemService(ajaxService2) {
            this.ajaxService2 = ajaxService2;
        }
        ItemService.prototype.getDetails = function (boxId, itemId, firstTime) {
            return this.ajaxService2.get('/item/load/', { boxId: boxId, itemId: itemId, firstTime: firstTime });
        };
        ;
        ItemService.prototype.getPreview = function (blobName, index, itemId, boxId) {
            return this.ajaxService2.get('/item/preview/', {
                blobName: blobName,
                index: index,
                id: itemId,
                boxId: boxId
            });
        };
        ;
        ItemService.prototype.addLink = function (url, boxid, tabid, question, name) {
            return this.ajaxService2.post('/upload/link', {
                url: url,
                boxid: boxid,
                tabid: tabid,
                question: question,
                name: name
            });
        };
        ItemService.prototype.addFromDropBox = function (boxId, tabid, url, name, question) {
            return this.ajaxService2.post('/upload/dropbox/', {
                boxId: boxId,
                tabId: tabid,
                url: url,
                name: name,
                question: question
            });
        };
        ItemService.prototype.renameItem = function (name, id) {
            return this.ajaxService2.post('/item/rename', {
                id: id,
                newName: name
            })
                .then(function (response) { return ({ name: response["name"], url: response["url"] }); });
        };
        ItemService.prototype.getComments = function (boxId, itemId) {
            return this.ajaxService2.get('/item/comment', {
                boxId: boxId,
                itemId: itemId
            }, 'itemComment');
        };
        ItemService.prototype.addComment = function (comment, boxId, id) {
            return this.ajaxService2.post('/item/addcomment', {
                itemId: id,
                boxId: boxId,
                comment: comment
            }, ['itemComment', "boxData"]);
        };
        ItemService.prototype.replycomment = function (reply, itemId, boxid, commentId) {
            return this.ajaxService2.post('/item/replycomment', {
                comment: reply,
                itemid: itemId,
                commentid: commentId,
                boxid: boxid
            }, ['itemComment', "boxData"]);
        };
        ItemService.prototype.deleteReply = function (replyId, itemId) {
            return this.ajaxService2.post('/item/deletecommentreply', {
                ReplyId: replyId,
                itemid: itemId
            }, 'itemComment');
        };
        ItemService.prototype.deleteComment = function (commentId, itemId) {
            return this.ajaxService2.post('/item/deletecomment', {
                CommentId: commentId,
                itemid: itemId
            }, 'itemComment');
        };
        ItemService.prototype.like = function (id, boxId) {
            return this.ajaxService2.post('/item/like', {
                itemId: id,
                boxId: boxId
            }, "boxData");
        };
        ;
        ItemService.prototype.followbox = function () {
            this.ajaxService2.deleteCacheCategory("boxData");
        };
        ItemService.prototype.flag = function (badItem, other, itemId) {
            return this.ajaxService2.post('/item/flagrequest/', {
                badItem: badItem,
                other: other,
                itemId: itemId
            });
        };
        ItemService.$inject = ['ajaxService2'];
        return ItemService;
    }());
    angular.module('app.item').service('itemService', ItemService);
})(app || (app = {}));
