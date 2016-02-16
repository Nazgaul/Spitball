(function () {
    angular.module('app.box').service('boxService', box);
    box.$inject = ['ajaxService'];

    function box(ajaxservice) {
        var d = this;
        d.getBox = function (boxid) {
            return ajaxservice.get('/box/data/', { id: boxid });
        };
        d.getFeed = function (boxid, page) {
            return ajaxservice.get('/qna/', { id: boxid, page: page });
        };
        d.getReplies = function(boxid, commentId) {
            return ajaxservice.get('/qna/replies/', { boxid: boxid, id: commentId});
        }
        d.leaderBoard = function (boxid) {
            return ajaxservice.get('/box/leaderboard/', { id: boxid });
        }
        d.getRecommended = function (boxid) {
            return ajaxservice.get('/box/recommended/', { id: boxid });
        };
        d.items = function (boxId, tabId, page) {
            return ajaxservice.get('/box/items/', { id: boxId, tabId: tabId, page: page });
        };
        d.getTabs = function (boxid) {
            return ajaxservice.get('/box/tabs/', { id: boxid });
        };
        d.addItemToTab = function (boxId, tabId, itemId) {
            return ajaxservice.post('/box/additemtotab/', {
                boxId: boxId,
                tabId: tabId,
                itemId: itemId
            });
        }
        d.createTab = function (name, boxId) {
            return ajaxservice.post('/box/createtab/', {
                name: name,
                boxId: boxId
            });
        }
        d.renameTab = function (tabId, name, boxId) {
            return ajaxservice.post('/box/renametab/', {
                tabId: tabId,
                name: name,
                boxId: boxId
            });
        }
        d.deleteTab = function (tabId, boxId) {
            return ajaxservice.post('/box/deletetab/', {
                tabId: tabId,
                boxId: boxId
            });
        }
        d.deleteItem = function (itemId) {
            return ajaxservice.post('/item/delete/', {
                itemId: itemId
            });
        };
        d.filterItem = function (term, boxId, page) {
            return ajaxservice.get('/search/iteminbox/', {
                term: term,
                boxId: boxId,
                page: page
            });
        };
        d.getMembers = function (boxid) {
            return ajaxservice.get('/box/members/', { boxId: boxid });
        };
        d.getQuizzes = function (boxid) {
            return ajaxservice.get('/box/quizes/', { id: boxid });
        };


        d.postComment = function (content, boxId, files, anonymously) {
            return ajaxservice.post('/qna/addcomment/', { content: content, boxId: boxId, files: files, anonymously: anonymously });
        }
        d.deleteComment = function (commentId) {
            return ajaxservice.post('/qna/deletecomment/', {
                questionId: commentId
            });
        }
        d.postReply = function (content, boxId, commentId, files) {
            return ajaxservice.post('/qna/addreply/', {
                content: content,
                boxId: boxId,
                commentId: commentId,
                files: files
            });
        }
        d.deleteReply = function (postId) {
            return ajaxservice.post('/qna/deletereply/', {
                answerId: postId
            });
        }
        d.likeComment = function (postId, boxId) {
            return ajaxservice.post('/qna/likecomment/', {
                commentId: postId,
                boxId: boxId
            });
        }
        d.likeReply = function (replyId, boxId) {
            return ajaxservice.post('/qna/likereply/', {
                replyId: replyId, boxId: boxId
            });
        }
        d.commentLikes = function (postId, boxId) {
            return ajaxservice.get('/qna/commentlikes/', {
                id: postId,
                boxId: boxId
            });
        }
        d.replyLikes = function (replyId, boxId) {
            return ajaxservice.get('/qna/replylikes/', {
                id: replyId,
                boxId: boxId
            });
        }
        d.follow = function (boxId) {
            return ajaxservice.post('/share/subscribetobox/', {
                boxId: boxId
            });

        }
        d.notification = function (boxId) {
            return ajaxservice.get('/box/getnotification/', {
                boxId: boxId
            });
        }
        d.unfollow = function (boxId) {
            return ajaxservice.post('/box/delete/', {
                id: boxId
            });

        }
        d.updateBox = function (boxId, name, course, professor, privacy, notification) {
            return ajaxservice.post('/box/updateinfo/', {
                id: boxId,
                name: name,
                courseCode: course,
                professor: professor,
                boxPrivacy: privacy,
                notification: notification
            });
        }

    }
})();