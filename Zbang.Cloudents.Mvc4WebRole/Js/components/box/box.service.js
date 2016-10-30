var app;
(function (app) {
    "use strict";
    var BoxService = (function () {
        function BoxService(ajaxService2) {
            this.ajaxService2 = ajaxService2;
        }
        BoxService.prototype.getBox = function (boxid) {
            return this.ajaxService2.get("/box/data/", { id: boxid }, "boxData");
        };
        ;
        BoxService.prototype.getFeed = function (boxid, top, skip) {
            return this.ajaxService2.get('/qna/', { id: boxid, top: top, skip: skip });
        };
        ;
        BoxService.prototype.getReplies = function (boxid, commentId, replyId) {
            return this.ajaxService2.get('/qna/replies/', { boxid: boxid, id: commentId, replyId: replyId });
        };
        BoxService.prototype.leaderBoard = function (boxid) {
            return this.ajaxService2.get('/box/leaderboard/', { id: boxid });
        };
        BoxService.prototype.getRecommended = function (boxid) {
            return this.ajaxService2.get('/box/recommended/', { id: boxid });
        };
        ;
        BoxService.prototype.items = function (boxId, tabId, page) {
            return this.ajaxService2.get("/box/items/", { id: boxId, tabId: tabId, page: page }, "boxItems");
        };
        ;
        BoxService.prototype.getTabs = function (boxid) {
            return this.ajaxService2.get('/box/tabs/', { id: boxid });
        };
        ;
        BoxService.prototype.addItemToTab = function (boxId, tabId, itemId) {
            return this.ajaxService2.post("/box/additemtotab/", {
                boxId: boxId,
                tabId: tabId,
                itemId: itemId
            }, "boxItems");
        };
        BoxService.prototype.createTab = function (name, boxId) {
            return this.ajaxService2.post('/box/createtab/', {
                name: name,
                boxId: boxId
            });
        };
        BoxService.prototype.renameTab = function (tabId, name, boxId) {
            return this.ajaxService2.post('/box/renametab/', {
                tabId: tabId,
                name: name,
                boxId: boxId
            });
        };
        BoxService.prototype.deleteTab = function (tabId, boxId) {
            return this.ajaxService2.post("/box/deletetab/", {
                tabId: tabId,
                boxId: boxId
            }, "boxItems");
        };
        BoxService.prototype.deleteItem = function (itemId) {
            return this.ajaxService2.post("/item/delete/", {
                itemId: itemId
            }, "boxItems");
        };
        ;
        BoxService.prototype.filterItem = function (term, boxId, page) {
            return this.ajaxService2.get("/search/iteminbox/", {
                term: term,
                boxId: boxId,
                page: page
            });
        };
        ;
        BoxService.prototype.getMembers = function (boxid) {
            return this.ajaxService2.get('/box/members/', { boxId: boxid });
        };
        ;
        BoxService.prototype.getQuizzes = function (boxid) {
            return this.ajaxService2.get('/box/quizes/', { id: boxid });
        };
        ;
        BoxService.prototype.postComment = function (content, boxId, files, anonymously) {
            return this.ajaxService2.post("/qna/addcomment/", { content: content, boxId: boxId, files: files, anonymously: anonymously });
        };
        BoxService.prototype.deleteComment = function (commentId, boxId) {
            return this.ajaxService2.post('/qna/deletecomment/', {
                questionId: commentId,
                boxId: boxId
            });
        };
        BoxService.prototype.postReply = function (content, boxId, commentId, files) {
            return this.ajaxService2.post('/qna/addreply/', {
                content: content,
                boxId: boxId,
                commentId: commentId,
                files: files
            });
        };
        BoxService.prototype.deleteReply = function (postId, boxId) {
            return this.ajaxService2.post('/qna/deletereply/', {
                answerId: postId,
                boxId: boxId
            });
        };
        BoxService.prototype.likeComment = function (postId, boxId) {
            return this.ajaxService2.post('/qna/likecomment/', {
                commentId: postId,
                boxId: boxId
            });
        };
        BoxService.prototype.likeReply = function (replyId, boxId) {
            return this.ajaxService2.post('/qna/likereply/', {
                replyId: replyId, boxId: boxId
            });
        };
        BoxService.prototype.commentLikes = function (postId, boxId) {
            return this.ajaxService2.get('/qna/commentlikes/', {
                id: postId,
                boxId: boxId
            });
        };
        BoxService.prototype.replyLikes = function (replyId, boxId) {
            return this.ajaxService2.get('/qna/replylikes/', {
                id: replyId,
                boxId: boxId
            });
        };
        BoxService.prototype.follow = function (boxId) {
            return this.ajaxService2.post("/share/subscribetobox/", {
                boxId: boxId
            }, "boxData");
        };
        BoxService.prototype.notification = function (boxId) {
            return this.ajaxService2.get('/box/getnotification/', {
                boxId: boxId
            });
        };
        BoxService.prototype.unfollow = function (boxId) {
            return this.ajaxService2.post("/box/delete/", {
                id: boxId
            }, "boxData");
        };
        BoxService.prototype.updateBox = function (boxId, name, course, professor, privacy, notification) {
            return this.ajaxService2.post("/box/updateinfo/", {
                id: boxId,
                name: name,
                courseCode: course,
                professor: professor,
                boxPrivacy: privacy,
                notification: notification
            }, "boxData");
        };
        BoxService.prototype.feedLikes = function (boxId) {
            return this.ajaxService2.get("/qna/Likes/", {
                id: boxId
            });
        };
        BoxService.$inject = ["ajaxService2"];
        return BoxService;
    }());
    angular.module("app.box").service("boxService", BoxService);
})(app || (app = {}));
