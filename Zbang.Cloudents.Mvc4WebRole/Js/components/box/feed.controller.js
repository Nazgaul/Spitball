﻿(function () {
    angular.module('app.box.feed').controller('FeedController', feed);
    feed.$inject = ['boxService', '$stateParams', '$timeout', 'externalUploadProvider', 'itemThumbnailService',
        'user', 'userUpdatesService', '$mdDialog', '$scope', '$rootScope',
        'resManager', 'CacheFactory', '$q', 'routerHelper'];

    function feed(boxService, $stateParams, $timeout, externalUploadProvider,
        itemThumbnailService, user, userUpdatesService,
        $mdDialog, $scope, $rootScope, resManager, cacheFactory, $q, routerHelper) {
        var self = this, boxId = parseInt($stateParams.boxId, 10), page = 0;

        self.add = {
            files: [],
            disabled: false,
            googleDisabled: true,
            dropboxDisabled: true
        };

        self.add.createReply = createReply;

        self.add.createComment = createComment;


        externalUploadProvider.googleDriveInit().then(function () {
            self.add.googleDisabled = false;
        });
        externalUploadProvider.dropboxInit().then(function () {
            self.add.dropboxDisabled = false;
        });

        self.add.google = google;
        self.add.dropbox = dropbox;


        self.deleteComment = deleteComment;

        self.deleteReply = deleteReply;
        self.postItemTemplate = postItemTemplate;
        self.myPagingFunction = myPagingFunction;
        self.likeComment = likeComment;
        self.likeReply = likeReply;
        self.likeReplyDialog = likeReplyDialog;
        self.likeCommentDialog = likeCommentDialog;

        boxService.getFeed(boxId, page).then(function (response) {
            self.data = response;
            assignData();

        });
        function likeCommentDialog(comment, ev) {
            if (!comment.likesCount) {
                return;
            }
            showLikes(function () {
                return boxService.commentLikes(comment.id, boxId);
            }, ev);
        }
        function likeReplyDialog(reply, ev) {
            if (!reply.likesCount) {
                return;
            }
            showLikes(function() {
                return boxService.replyLikes(reply.id, boxId);
            }, ev);
        }

        function showLikes(func,ev) {
            $mdDialog.show({
                controller: 'likesController',
                controllerAs: 'lc',
                templateUrl: routerHelper.buildUrl('/box/likesdialog/'),
                parent: angular.element(document.body),
                resolve: {
                    users: func 
                },
                targetEvent: ev,
                clickOutsideToClose: true,
                //fullscreen: true
            });
        }

        function likeComment(comment) {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            $scope.$emit('follow-box');
            boxService.likeComment(comment.id, boxId).then(function (response) {
                response ? comment.likesCount++ : comment.likesCount--;
            });
        }
        function likeReply(reply) {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            $scope.$emit('follow-box');
            boxService.likeReply(reply.id, boxId).then(function (response) {
                response ? reply.likesCount++ : reply.likesCount--;
            });
        }

        function myPagingFunction() {
            if (!user.id) {
                var defer = $q.defer();
                defer.resolve();
                return defer.promise;
            }
            page++;
            return boxService.getFeed(boxId, page).then(function (response) {
                if (!response.length) {
                    return;
                }
                self.data = self.data.concat(response);
                assignData();
            });
        }

        function assignData() {
            for (var i = 0; i < self.data.length; i++) {
                var files = self.data[i].files;
                for (var j = 0; j < files.length; j++) {
                    var item = files[j];
                    if (item.done) {
                        continue;
                    }
                    if (item.type) {
                        var retVal = itemThumbnailService.assignValue(item.source, 100, 141);
                        item.thumbnail = retVal.thumbnail;
                        item.nameExtension = item.name.replace(/\.[^/.]+$/, "");
                        //item.icon = retVal.icon;
                    } else {
                        item.publish = true;
                    }
                    item.done = true;
                }
                for (var k = 0; k < self.data[i].replies.length; k++) {
                    self.data[i].replies[k].files = itemThumbnailService.assignValues(self.data[i].replies[k].files, 100, 141);
                }
            }
            userUpdatesService.boxUpdates(boxId, function (updates) {
                for (var jj = 0; jj < updates.length; jj++) {
                    var update = updates[jj];
                    attachNew(update);

                }
            });
        }

        function postItemTemplate(elmenet) {
            if (elmenet.type) {
                return 'item-template.html';
            }
            return 'quiz-template.html';
        }
        function attachNew(update) {
            if (update.itemId) {
                for (var i = 0; i < self.data.length; i++) {
                    var questionForItem = self.data[i];
                    var exists = questionForItem.files.find(function (e) {
                        return e.id === update.itemId;
                    });
                    if (exists) {
                        questionForItem.isNew = true;
                        return;
                    }
                    for (var k = 0; k < questionForItem.answers.length; k++) {
                        exists = questionForItem.answers[k].files.find(function (e) {
                            return e.id === update.itemId;
                        });
                        if (exists) {
                            questionForItem.isNew = true;
                            return;
                        }
                    }
                    //console.log(update.itemId);

                }
            }
            if (update.questionId) {
                var question = self.data.find(function (e) {
                    return e.id === update.questionId;
                });
                if (!question) {
                    // console.log('something wrong');
                }
                if (update.answerId) {
                    var answer = question.answers.find(function (e) {
                        return e.id === update.answerId;
                    });
                    if (!answer) {
                        // console.log('something wrong');
                    }
                    answer.isNew = true;
                }
                question.isNew = true;


            }
        }
        function deleteComment(ev, post) {

            //boxType //userType
            var confirm = $mdDialog.confirm()
                  .title(resManager.get('deletePost'))
                  .targetEvent(ev)
                  .ok(resManager.get('dialogOk'))
                  .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = self.data.indexOf(post);
                self.data.splice(index, 1);
                boxService.deleteComment(post.id);
            });
        }
        function deleteReply(ev, post, reply) {

            //boxType //userType
            var confirm = $mdDialog.confirm()
                  .title(resManager.get('deleteReply'))
                  //.textContent('All of the banks have agreed to forgive you your debts.')
                  .targetEvent(ev)
                  .ok(resManager.get('dialogOk'))
                  .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = post.answers.indexOf(reply);
                post.answers.splice(index, 1);
                boxService.deleteReply(reply.id);
            });
        }
        function createReply(comment) {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            var filesId = self.add.files.filter(function (e) {
                return e.postId == comment.id;
            }).map(function (c) {
                return c.system.id;
            });



            self.add.disabled = true;
            boxService.postReply(self.add.newReplyText, boxId, comment.id, filesId).then(function (response) {
                var newComment = {
                    content: extractUrls(self.add.newReplyText.replace(/[&<>]/g, replaceTag)),
                    creationTime: new Date(),
                    id: response,
                    url: user.url,

                    userId: user.id,
                    userImage: user.image,
                    userName: user.name,
                    files: self.add.files.map(pushItem)
                };
                comment.replies.push(newComment);
                self.add.newReplyText = '';
                self.add.files = [];
                comment.focusReply = false;
                //comment.showFrom = false;
                $scope.$emit('follow-box');
            }).finally(function () {
                self.add.disabled = false;
            });

        }

        function pushItem(c) {
            var temp = c.system;
            //temp = itemThumbnailService.assignValues(c, 100, 125);
            var retVal = itemThumbnailService.assignValue(temp.source, 100, 141);
            temp.thumbnail = retVal.thumbnail;
            temp.icon = retVal.icon;
            temp.numOfViews = undefined;
            temp.type = 'item';
            return temp;
        }

        function createComment() {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            self.add.disabled = true;
            var files = self.add.files.filter(function (e) {
                return e.postId == null;
            });
            var filesId = files.map(function (c) {
                return c.system.id;
            });
            boxService.postComment(self.add.newText, boxId, filesId, self.add.anonymous).then(function (response) {
                self.add.newText = self.add.newText || '';
                var newComment = {
                    content: extractUrls(self.add.newText.replace(/[&<>]/g, replaceTag)),
                    creationTime: new Date(),
                    id: response.commentId,
                    url: response.userUrl,
                    userId: response.userId,
                    userImage: response.userImage,
                    userName: response.userName,
                    files: files.map(pushItem)
                };
                self.data.unshift(newComment);
                self.add.newText = '';
                self.add.files = [];
                self.add.anonymous = false;
                $scope.$emit('follow-box');
                /*answers: []
content: "asdasdasd"
creationTime: "2015-11-04T13:11:32.6519547Z"
files: []
id: "b05758b9-7e1d-4b3b-af7f-a54600fa5c9c"
url: "/user/1/ram-y/"
userId: 1
userImage: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/c6f9a62f-0289-4e7f-a07a-ff7500945ee4.jpg"
userName: "ram y"*/

            }).finally(function () {
                self.add.disabled = false;
            });
        }
        function google(post) {
            postId = post;
            externalUploadProvider.google(boxId).then(externalUploadComplete);
        }
        function dropbox(post) {
            postId = post;
            externalUploadProvider.dropBox(boxId).then(externalUploadComplete);
        }
        function externalUploadComplete(response) {
            var link = response.item;
            self.add.files.push({
                complete: true,
                name: link.name,
                system: link,
                postId: postId,
                remove: function () {
                    removeItem(link);
                }
            });
        }

        var postId;
        self.add.upload = uploadFile;
        self.openReply = openReply;

        function openReply(post) {
            $scope.b.closeCollapse();
            if (post.showReplies) {
                post.showReplies = false;
                return;
            }
            angular.forEach(self.data, function (elem) {
                elem.showReplies = false;
            });

            self.add.newReplyText = '';
            self.add.files = [];
            if (!post.repliesCount) {
                post.focusReply = true;
                expandReply();
                return;
            }
            if (post.repliesCount > 1 && post.repliesCount !== post.replies.length) {
                boxService.getReplies(boxId, post.id).then(function (response) {
                    response.reverse().pop();
                    post.replies = response.concat(post.replies);
                    expandReply();
                });
            } else {
                expandReply();
            }

            function expandReply() {
                post.showReplies = true;
            }

        }
        function uploadFile(post) {
            postId = post;
        }

        function removeFile(file, uploader) {
            //var file = this;
            if (file.status === plupload.UPLOADING) {
                uploader.stop();
            }
            uploader.removeFile(file);

            if (uploader.total.queued > 0) {
                $timeout(function () {
                    uploader.start();
                });
            }
            removeItem(file);

        }
        function removeItem(item) {
            if (item.system) {
                boxService.deleteItem(item.system.id, boxId);
            }
            var index = self.add.files.indexOf(item);
            self.add.files.splice(index, 1);
        }


        self.add.fileUpload = {
            url: '/upload/file/',
            options: {
                chunk_size: '3mb'
            },
            callbacks: {
                filesAdded: function (uploader, files) {
                    $scope.$emit('follow-box');
                    for (var i = 0; i < files.length; i++) {
                        var file = files[i];
                        file.sizeFormated = plupload.formatSize(file.size);
                        file.complete = false;
                        file.postId = postId;
                        file.remove = function () { removeFile(file, uploader); }
                        self.add.files.push(file);
                    }
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                beforeUpload: function (up, file) {
                    up.settings.multipart_params = {
                        fileName: file.name,
                        fileSize: file.size,
                        boxId: boxId,
                        comment: true
                    };
                },
                fileUploaded: function (uploader, file, response) {
                    file.complete = true;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        file.system = obj.payload.item;
                        cacheFactory.clearAll();
                    }
                }
                //error: function (uploader, error) {
                //    u.alert = error.message;
                //}
            }
        }

        var tagsToReplace = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;'
        };

        function replaceTag(tag) {
            return tagsToReplace[tag] || tag;
        }

        function extractUrls(d) {
            if (!d) {
                return;
            }
            var urlex = /\b((?:https?:\/\/|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}\/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))/i;

            var array = d.match(urlex) || [];
            var matches = [];
            for (var k = 0; k < array.length; k++) {
                if (matches.indexOf(array[k]) < 0)
                    matches.push(array[k]);
            }
            if (!matches.length) {
                return d;
            }
            for (var i = 0; i < matches.length; i++) {
                var url = matches[i];
                if (!url) {
                    continue;
                }
                if (url.indexOf('http') !== 0) {
                    url = 'http://' + url;
                }
                d = d.replace(matches[i], "<a target=\"_blank\" href=\"" + url + "\">" + matches[i] + "</a>");
            }

            return d;
        }
    }
})();



