(function () {
    'use strict';
    angular.module('app.box.feed').controller('FeedController', feed);
    feed.$inject = ['boxService', '$stateParams', '$timeout', 'externalUploadProvider',
        'itemThumbnailService',
        'user', 'userUpdatesService', '$mdDialog', '$scope', '$rootScope',
        'resManager', 'routerHelper', '$filter', 'feedData',
        'updates', "$q", "$window", "likes"];

    function feed(boxService, $stateParams, $timeout, externalUploadProvider,
        itemThumbnailService, user, userUpdatesService,
        $mdDialog, $scope, $rootScope, resManager, routerHelper,
        $filter, feedData, updates, $q, $window, likes) {
        var self = this, boxId = parseInt($stateParams.boxId, 10), top = 30;

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
        // var feedUpdates = {};
        var disablePaging = false;

        if (user.id) {
            // feedUpdates = updates;
            userUpdatesService.deleteUpdates(boxId);
        }
        self.data = assignData(feedData);
        scrollToPosition();


        function appendLikes(postsList) {
            likes = likes || [];
            if (!likes.length) {
                return postsList;
            }
            for (var i = 0; i < postsList.length; i++) {
                var currentPost = postsList[i];
                if (likes.indexOf(currentPost.id) > -1) {
                    currentPost.isLiked = true;
                    //reply.isLiked
                }

                for (var j = 0; j < currentPost.replies.length; j++) {
                    var currentreply = currentPost.replies[j];
                    if (likes.indexOf(currentreply.id) > -1) {
                        currentreply.isLiked = true;
                    }
                }

            }
            return postsList;
        }
        function appendUpdates(postsList) {
            if (!updates) {
                return postsList;
            }
            for (var i = 0; i < postsList.length; i++) {
                var currentPost = postsList[i];
                if (updates && updates[currentPost.id]) {
                    currentPost.isNew = true;
                }
                if (angular.isDefined(currentPost) && currentPost !== null) {
                    for (var j = 0; j < currentPost.replies.length; j++) {
                        var currentreply = currentPost.replies[j];
                        if (updates[currentreply.id]) {
                            currentreply.isNew = true;
                        }
                    }
                }
            }
            return postsList;
        }


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

        $scope.$on('disablePaging', function () {
            disablePaging = true;
        });
        $scope.$on('enablePaging', function () {
            disablePaging = false;
        });

        function showLikes(func, ev) {
            disablePaging = true;
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
            }).finally(function () {
                disablePaging = false;
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
            if (disablePaging) {
                return $q.when();
            }
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
            if (!user.id) {
                return;
            }
            externalUploadProvider.googleDriveInit().then(function () {
                self.add.googleDisabled = false;
            });
            externalUploadProvider.dropboxInit().then(function () {
                self.add.dropboxDisabled = false;
            });
        }

        //self.limitFiles = 100;
        function assignData(data) {
            var flags = [];
            var output = [];
            for (var i = 0; i < data.length; i++) {
                var currentPost = data[i];
                if (flags[currentPost.id]) continue;
                flags[currentPost.id] = true;
               


                var files = currentPost.files;
                currentPost.fileCount = files.length;
                for (var j = 0; j < files.length; j++) {
                    var item = files[j];
                    if (item.done) {
                        continue;
                    }
                    if (item.type !== 'quiz') {
                        var retVal = itemThumbnailService.assignValue(item.source, 100, 141);
                        item.thumbnail = retVal.thumbnail;
                        item.name = item.name || '';
                        item.nameExtension = item.name.replace(/\.[^/.]+$/, "");
                        //item.icon = retVal.icon;
                    } else {
                        item.publish = true;
                    }
                    item.done = true;
                }
                currentPost.replies = $filter('orderBy')(currentPost.replies, 'creationTime', false);
                for (var k = 0; currentPost.replies && k < currentPost.replies.length; k++) {
                    var currentReply = currentPost.replies[k];
                    angular.forEach(currentReply.files, buildItem);
                }
                output.push(currentPost);
            }
            appendUpdates(output);
            appendLikes(output);
            return output;

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
            disablePaging = true;
            //boxType //userType
            var confirm = $mdDialog.confirm()
                  .title(resManager.get('deletePost'))
                  .targetEvent(ev)
                  .ok(resManager.get('dialogOk'))
                  .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = self.data.indexOf(post);
                self.data.splice(index, 1);
                boxService.deleteComment(post.id, boxId);
            }).finally(function () {
                disablePaging = false;
            });
        }
        function deleteReply(ev, post, reply2) {
            disablePaging = true;

            //boxType //userType
            var confirm = $mdDialog.confirm()
                  .title(resManager.get('deleteReply'))
                  .targetEvent(ev)
                  .ok(resManager.get('dialogOk'))
                  .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = post.replies.indexOf(reply2);
                post.replies.splice(index, 1);
                boxService.deleteReply(reply2.id, boxId);
            }).finally(function () {
                disablePaging = false;
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
                    content: self.add.newReplyText, //extractUrls(self.add.newReplyText.replace(/[&<>]/g, replaceTag)),
                    creationTime: new Date().toISOString(),
                    id: response,
                    url: user.url,

                    userId: user.id,
                    userImage: user.image,
                    userName: user.name,
                    likesCount: 0,
                    files: self.add.files.map(pushItem)
                };
                comment.replies.push(newComment);
                comment.repliesCount++;
                comment = assignData([comment]);
                self.add.newReplyText = '';
                self.add.files = [];
                comment.focusReply = false;
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
                    content: self.add.newText, //extractUrls(self.add.newText.replace(/[&<>]/g, replaceTag)),
                    creationTime: new Date().toISOString(),
                    id: response.commentId,
                    url: response.userUrl,
                    userId: response.userId,
                    userImage: response.userImage,
                    userName: response.userName,
                    files: files.map(pushItem),
                    likesCount: 0,
                    replies: []
                };
                self.data.unshift(newComment);
                self.data = assignData(self.data);
                self.add.newText = '';
                self.add.files = [];
                self.add.anonymous = false;
                $scope.$emit('follow-box');


            }).finally(function () {
                self.add.disabled = false;
            });
        }
        //var postId;
        function google(post) {
            //postId = post;
            externalUploadProvider.google(boxId, null, true).then(function (response2) {
                externalUploadComplete(response2, post);
            });
        }
        function dropbox(post) {
            //postId = post;
            externalUploadProvider.dropBox(boxId, true).then(function (response2) {
                externalUploadComplete(response2, post);
            });
        }
        function externalUploadComplete(response2, postId) {
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


        //self.add.upload = uploadFile;
        self.openReply = openReply;
        self.reply = reply;

        function reply(post) {
            openReply(post);
            post.focusReply = true;
        }

        function openReply(post) {
            $scope.b.closeCollapse();
            function collapse(elem) {
                if (elem.replies.length > 4) {
                    elem.replies = elem.replies.slice(Math.max(elem.replies.length - 4, 1));
                }
                elem.expandReplies = false;
            }
            if (post.expandReplies) {
                collapse(post);
                return;
            }
            angular.forEach(self.data, collapse);

            self.add.newReplyText = '';
            self.add.files = [];
            if (!post.repliesCount) {
                post.focusReply = true;
                expandReply();
                return;
            }
            if (post.repliesCount > 4 && post.repliesCount !== post.replies.length) {
                boxService.getReplies(boxId, post.id, post.replies[0].id).then(function (response) {
                    post.replies = response.concat(post.replies);
                    assignData([post]);
                    expandReply();
                });
            }
            else {
                expandReply();
            }

            function expandReply() {
                post.expandReplies = true;
            }

        }
        //function uploadFile(post) {
        //    postId = post;
        //}

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
            callbacks:
                 {
                     filesAdded: function (uploader, files) {
                         $scope.$emit('follow-box');
                         for (var i = 0; i < files.length; i++) {
                             var file = files[i];
                             (function (file) {
                                 file.sizeFormated = plupload.formatSize(file.size);
                                 file.complete = false;
                                 file.postId = uploader.settings.postId;
                                 file.remove = function () {
                                     removeFile(file, uploader);
                                     self.add.disabled = false;
                                 };

                                 self.add.files.push(file);

                                 var img = new mOxie.Image();
                                 img.onload = function () {
                                     this.crop(95, 105, false);
                                     file.content = this.getAsDataURL("image/jpeg", 80);
                                 };
                                 img.onembedded = function () {
                                     this.destroy();
                                 };

                                 img.onerror = function () {
                                     this.destroy();
                                 };
                                 img.load(file.getSource());
                             })(file);
                         }
                         $timeout(function () {
                             uploader.start();
                         },
                             1);
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
                             //cacheFactory.clearAll();
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
        self.add.fileUpload.options = function (postId) {
            return {
                postId: postId,
                chunk_size: '3mb'

            }
        };
        //self.add.fileUpload.options =  {
        //   // return {
        //        //someid:id,
        //        //header: {
        //        //    parentId:id
        //        //},
        //        chunk_size: '3mb'

        //    //}
        //};

        $scope.$on('update-thumbnail', function (e, args) {
            for (var i = 0; i < self.data.length; i++) {
                var element = self.data[i];
                var item = element.files.find(function (x) {
                    return x.id === args;
                });
                if (item) {
                    item.thumbnail += '&1=1';
                    break;
                }
            }
        });

        function scrollToPosition() {
            var yOffsetParam = $stateParams.pageYOffset;
            if (yOffsetParam) {
                $timeout(function () {
                    $window.scrollTo(0, yOffsetParam);
                });
            }
        }
    }
})();



