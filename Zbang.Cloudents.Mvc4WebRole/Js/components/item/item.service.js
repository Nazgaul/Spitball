﻿'use strict';
(function () {
    angular.module('app.item').service('itemService', library);
    library.$inject = ['ajaxService'];

    function library(ajaxservice) {
        var d = this;

        d.getDetails = function (boxId, itemId, firstTime) {
            return ajaxservice.get('/item/load/', { boxId: boxId, itemId: itemId, firstTime: firstTime });
        };
        //d.content = function(boxId, itemId) {
        //    return ajaxservice.get('/item/content/', { boxId: boxId, itemId: itemId })
        //}
        d.getPreview = function (blobName, index, itemId, boxId) {
            return ajaxservice.get('/item/preview/', {
                blobName: blobName,
                index: index,
                id: itemId,
                boxId: boxId
            });
        };


        d.addLink = function (url, boxid, tabid, question, name) {
            return ajaxservice.post('/upload/link', {
                url: url,
                boxid: boxid,
                tabid: tabid,
                question: question,
                name: name
            });
        }
        d.addFromDropBox = function (boxId, tabid, url, name, question) {
            return ajaxservice.post('/upload/dropbox/', {
                boxId: boxId,
                tabId: tabid,
                url: url,
                name: name,
                question: question
            });
        }

        d.renameItem = function (name, id) {
            return ajaxservice.post('/item/rename', {
                id: id,
                newName: name
            }).then(function (response) {
                return { name: response.name, url: response.url }
            });
        }

        d.getComments = function (boxId, itemId) {
            return ajaxservice.get('/item/comment', {
                boxId: boxId,
                itemId: itemId
            })
        }

        d.addComment = function (comment, boxId, id) {
            return ajaxservice.post('/item/addcomment', {
                itemId: id,
                boxId: boxId,
                comment: comment
            });
        }

        d.replycomment = function (reply, itemId, boxid, commentId) {
            return ajaxservice.post('/item/replycomment', {
                comment: reply,
                itemid: itemId,
                commentid: commentId,
                boxid: boxid
            });
        }

        d.deleteReply = function (replyId) {
            return ajaxservice.post('/item/deletecommentreply', {
                ReplyId: replyId
            });
        }

        d.deleteComment = function (commentId) {
            return ajaxservice.post('/item/deletecomment', {
                CommentId: commentId
            });
        }

        //no need to wipe cache
        d.like = function (id, boxId) {
            return ajaxservice.post('/item/like', {
                itemId: id,
                boxId: boxId
            });
        };



        d.flag = function (badItem, other, itemId) {
            return ajaxservice.post('/item/flagrequest/', {
                badItem: badItem,
                other: other,
                itemId: itemId
            });
        }

    }
})();