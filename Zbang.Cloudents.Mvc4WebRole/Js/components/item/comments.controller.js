﻿(function () {
    angular.module('app.item').controller('CommentsController', comments);
    comments.$inject = ['$stateParams', 'itemService', '$scope',
        'userDetailsFactory', '$mdDialog', 'resManager', '$mdSidenav', '$rootScope', '$timeout'];

    function comments($stateParams, itemService, $scope, userDetailsFactory,
        $mdDialog, resManager, $mdSidenav, $rootScope, $timeout) {
        var c = this, boxid = $stateParams.boxId, itemId = $stateParams.itemId;
        //$scope.$on('itemData', function (e, arg) {
        //    c.comments = arg.comments;
        //});
        c.comments = $scope.i.details.comments;
        c.currUser = userDetailsFactory.get();

        c.addComment = addComment;
        c.addCommentReply = addCommentReply;
        c.openReply = openReply;
        c.canDelete = canDelete;
        c.deleteReply = deleteReply;
        c.deleteComment = deleteComment;
        c.commentFocused = commentFocused;
        c.replyClicked = replyClicked;
        c.showButtons = false;
        c.commentDesabled = false;
        c.cancel = cancel;
        c.showCommentMenu = false;
        c.toggleComments = toggleComments;
        c.displayUnregBox = displayUnregBox;

        function toggleComments() {
            $mdSidenav('commentsMenu').toggle();
            $timeout(function() {
                $rootScope.$broadcast('updateScroll');
            },50);
        }

        function displayUnregBox() {
            if (!c.currUser.id) {
                $rootScope.$broadcast('show-unregisterd-box');
            }
            return !c.currUser.id;
        }


        function commentFocused() {
            c.showButtons = !displayUnregBox();
        }

        function replyClicked(comment) {
            if (!displayUnregBox()) {
                c.openReply(comment);
            }
        }

        function addComment(form) {
            c.commentDesabled = true;
            itemService.addComment(c.newCommentText, boxid, itemId).then(function (response) {
                var newComment = {
                    comment: c.newCommentText,
                    creationDate: new Date(),
                    id: response,
                    url: c.currUser.url,
                    replies:[],
                    userId: c.currUser.id,
                    userImage: c.currUser.image,
                    userName: c.currUser.name,
                };

                c.comments.unshift(newComment);
            },
                function (response) {
                    form.comment.$setValidity('server', false);
                    c.error = response;
                })
                .finally(function () {
                    c.commentDesabled = false;
                    if (!c.error) {
                        cancel(form);
                        c.showButtons = false;
                    }
                });;
        }

        function cancel(form) {
            c.newCommentText = '';
            $scope.app.resetForm(form);
        }

        function addCommentReply(comment, form) {
            c.commentDesabled = true;
            itemService.replycomment(c.newCommentReplyText, itemId, boxid, comment.id).then(function (response) {
                var newResponse = {
                    comment: c.newCommentReplyText,
                    creationDate: new Date(),
                    id: response,
                    url: c.currUser.url,

                    userId: c.currUser.id,
                    userImage: c.currUser.image,
                    userName: c.currUser.name,
                };

                comment.replies.push(newResponse);
                c.newCommentReplyText = '';
            },
            function (response) {
                form.reply.$setValidity('server', false);
                c.error = response;
            })
                .finally(function () {
                    c.commentDesabled = false;
                    if (!c.error) {
                        cancel(form);
                        comment.showFrom = false;
                    }
                });;
        }

        function openReply(comment) {
            angular.forEach(c.comments, function (elem) {
                elem.showFrom = false;
            });
            comment.showFrom = true;
            c.newCommentReplyText = '';
        }

        function canDelete(userId) {
            return userId == c.currUser.id;
        }

        function deleteComment(ev, comment) {

            var confirm = $mdDialog.confirm()
            .title(resManager.get('deletePost'))
            .targetEvent(ev)
            .ok(resManager.get('dialogOk'))
            .cancel(resManager.get('dialogCancel'));


            $mdDialog.show(confirm).then(function () {
                var index = c.comments.indexOf(comment);
                c.comments.splice(index, 1);
                itemService.deleteComment(comment.id);
            });


        }

        function deleteReply(ev, reply, comment) {

            var confirm = $mdDialog.confirm()
            .title(resManager.get('deletePost'))
            .targetEvent(ev)
            .ok(resManager.get('dialogOk'))
            .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = comment.replies.indexOf(reply);
                comment.replies.splice(index, 1);
                itemService.deleteReply(reply.id);
            });
        }
    }


})();


