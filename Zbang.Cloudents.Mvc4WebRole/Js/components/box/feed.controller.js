'use strict';
(function () {
    angular.module('app.box.feed').controller('FeedController', feed);
    feed.$inject = ['boxService', '$stateParams', '$timeout', 'externalUploadProvider', 'itemThumbnailService',
        'user', 'userUpdatesService', '$mdDialog', '$scope', '$rootScope',
        'resManager', 'CacheFactory', '$q', 'routerHelper', '$window'];

    function feed(boxService, $stateParams, $timeout, externalUploadProvider,
        itemThumbnailService, user, userUpdatesService,
        $mdDialog, $scope, $rootScope, resManager, cacheFactory, $q, routerHelper, $window) {
        var self = this, boxId = parseInt($stateParams.boxId, 10), top = ($window.innerHeight <= 600) ? 10 : 15;

        self.add = {
            files: [],
            disabled: false
        };
        self.add.createReply = createReply;
        self.add.createComment = createComment;

        self.add.google = google;
        self.add.dropbox = dropbox;
        self.add.init = initThirdParties;



        self.deleteComment = deleteComment;

        self.deleteReply = deleteReply;
        self.postItemTemplate = postItemTemplate;
        self.myPagingFunction = myPagingFunction;
        self.likeComment = likeComment;
        self.likeReply = likeReply;
        self.likeReplyDialog = likeReplyDialog;
        self.likeCommentDialog = likeCommentDialog;
        var feedUpdates = {};

        function appendUpdates(postsList) {
            if (!feedUpdates) {
                return postsList;
            }
            for (var i = 0; i < postsList.length; i++) {
                var currentPost = postsList[i];
                if (feedUpdates && feedUpdates[currentPost.id]) {
                    currentPost.isNew = true;
                }
                if (typeof (currentPost) !== 'undefined') {
                    for (var j = 0; j < currentPost.replies.length; j++) {
                        var currentreply = currentPost.replies[j];
                        if (feedUpdates[currentreply.id]) {
                            currentreply.isNew = true;
                        }
                    }
                }
            }
            return postsList;
            //for (currentPost in postsList) {
            //    if (feedUpdates[postsList[currentPost].id]) {
            //        postsList[currentPost].isNew = true;
            //    }
            //    for (currentreply in postsList[currentPost].replies) {
            //        if (feedUpdates[postsList[currentPost].replies[currentreply].id]) {
            //            postsList[currentPost].replies[currentreply].isNew = true;
            //        }
            //    }
            //}
        }
        

        boxService.getFeed(boxId, top, 0).then(function (response) {
            //self.data = response;
            var x = assignData(response);
            //appendUpdates(self.data);

            if (!user.id) {
                self.data = x;
            }
            else {
                userUpdatesService.boxUpdates(boxId, function (updates) {
                    feedUpdates = updates;
                    self.data = appendUpdates(x);

                });
            }

        });
        //userUpdatesService.boxUpdates(boxId, function (updates) {
        //    feedUpdates = updates;
        //    appendUpdates(self.data);
        //});
        function likeCommentDialog(comment, ev) {
            if (!comment.likesCount) {
                return;
            }
            showLikes(function () {
                return boxService.commentLikes(comment.id, boxId);
            }, ev);
        }
        function likeReplyDialog(reply2, ev) {
            if (!reply2.likesCount) {
                return;
            }
            showLikes(function () {
                return boxService.replyLikes(reply2.id, boxId);
            }, ev);
        }

        function showLikes(func, ev) {
            $mdDialog.show({
                controller: 'likesController',
                controllerAs: 'lc',
                templateUrl: routerHelper.buildUrl('/box/likesdialog/'),
                parent: angular.element(document.body),
                resolve: {
                    users: func
                },
                targetEvent: ev,
                clickOutsideToClose: true
                //fullscreen: true
            });
        }

        function likeComment(comment) {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            $scope.$emit('follow-box');
            self.add.disabled = true;
            boxService.likeComment(comment.id, boxId).then(function (response) {
                if (response) {
                    comment.likesCount++;
                    comment.isLiked = true;
                } else {
                    comment.likesCount--;
                    comment.isLiked = false;
                }

            }).finally(function () {
                self.add.disabled = false;
            });
        }
        function likeReply(reply2) {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            self.add.disabled = true;
            $scope.$emit('follow-box');
            boxService.likeReply(reply2.id, boxId).then(function (response) {
                if (response) {
                    reply2.likesCount++;
                    reply2.isLiked = true;
                } else {
                    reply2.likesCount--;
                    reply2.isLiked = false;
                }

            }).finally(function () {
                self.add.disabled = false;
            });
        }

        function myPagingFunction() {
            //timestamp = currentTimestamp;
            return boxService.getFeed(boxId, top, self.data.length).then(function (response) {
                if (!response.length) {
                    return;
                }
                var x = self.data;
                // self.data = self.data.concat(response);
                self.data = x.concat(assignData(response));
            });
        }

        function initThirdParties() {
            externalUploadProvider.googleDriveInit().then(function () {
                self.add.googleDisabled = false;
            });
            externalUploadProvider.dropboxInit().then(function () {
                self.add.dropboxDisabled = false;
            });
        }

        function assignData(data) {
            for (var i = 0; i < data.length; i++) {
                var currentPost = data[i];
               // currentPost.creationTime = $filter('date')(currentPost.creationTime, 'medium');
                var files = currentPost.files;
                for (var j = 0; j < files.length; j++) {
                    var item = files[j];
                    if (item.done) {
                        continue;
                    }
                    if (item.type !== 'quiz') {
                        var retVal = itemThumbnailService.assignValue(item.source, 100, 141);
                        item.thumbnail = retVal.thumbnail;
                        item.nameExtension = item.name.replace(/\.[^/.]+$/, "");
                        //item.icon = retVal.icon;
                    } else {
                        item.publish = true;
                    }
                    item.done = true;
                }
                currentPost.replies = currentPost.replies.reverse();
                for (var k = 0; currentPost.replies && k < currentPost.replies.length; k++) {
                    var currentReply = currentPost.replies[k];
                    //currentReply.creationTime = $filter('date')(currentReply.creationTime, 'medium');
                    angular.forEach(currentReply.files, buildItem);
                }
            }
            appendUpdates(data);
            return data;

            function buildItem(elem) {
                var retVal2 = itemThumbnailService.assignValue(elem.source, 100, 141);
                elem.thumbnail = retVal2.thumbnail;
                elem.nameExtension = elem.name.replace(/\.[^/.]+$/, "");
            }
        }

        function postItemTemplate(elmenet) {
            if (elmenet.type === 'quiz') {
                return 'quiz-template.html';
            }
            return 'item-template.html';

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
                boxService.deleteComment(post.id,boxId);
            });
        }
        function deleteReply(ev, post, reply2) {

            //boxType //userType
            var confirm = $mdDialog.confirm()
                  .title(resManager.get('deleteReply'))
                  .targetEvent(ev)
                  .ok(resManager.get('dialogOk'))
                  .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = post.replies.indexOf(reply2);
                post.replies.splice(index, 1);
                boxService.deleteReply(reply2.id,boxId);
            });
        }
        function createReply(comment) {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            var filesId = self.add.files.filter(function (e) {
                return e.postId === comment.id;
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
                self.data = assignData(self.data);
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
                    files: files.map(pushItem),
                    replies: []
                };
                self.data.unshift(newComment);
                self.data = assignData(self.data);
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
        var postId;
        function google(post) {
            postId = post;
            externalUploadProvider.google(boxId, null, true).then(externalUploadComplete);
        }
        function dropbox(post) {
            postId = post;
            externalUploadProvider.dropBox(boxId, true).then(externalUploadComplete);
        }
        function externalUploadComplete(response2) {
            if (angular.isArray(response2)) {
                for (var j = 0; j < response2.length; j++) {
                    pushItem2(response2[j]);
                }
                return;
            }
            pushItem2(response2);
            function pushItem2(response) {
                if (!response) {
                    return;
                }
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
        }


        self.add.upload = uploadFile;
        self.openReply = openReply;
        self.reply = reply;

        function reply(post) {
            openReply(post);
            post.focusReply = true;
        }

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
            if (post.repliesCount > 4 && post.repliesCount !== post.replies.length) {
                boxService.getReplies(boxId, post.id, post.replies[0].id).then(function (response) {
                    //response.reverse();//.pop();
                    //response.pop();
                    //response.pop();
                    //response.pop();
                    post.replies = response.concat(post.replies);
                    assignData([post]);
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
                boxService.deleteItem(item.system.id);
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
                        file.remove = function () {
                            removeFile(file, uploader);
                            self.add.disabled = false;
                        }
                        self.add.files.push(file);
                    }
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                beforeUpload: function (up, file) {
                    self.add.disabled = true;
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
                },
                uploadComplete: function () {
                    self.add.disabled = false;
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



