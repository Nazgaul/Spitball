(function () {
    angular.module('app.box.feed').controller('FeedController', feed);
    feed.$inject = ['boxService', '$stateParams', '$timeout', 'externalUploadProvider', 'itemThumbnailService',
        'user', 'userUpdatesService', '$mdDialog', '$scope'];

    function feed(boxService, $stateParams, $timeout, externalUploadProvider, itemThumbnailService, user, userUpdatesService, $mdDialog, $scope) {
        var self = this, boxId = parseInt($stateParams.boxId, 10), page = 0, ajaxFinish = true;

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

        boxService.getFeed(boxId, page).then(function (response) {
            self.data = response;
            assignData();

        });
        function myPagingFunction() {
            if (ajaxFinish) {
                ajaxFinish = false;
                page++;
                boxService.getFeed(boxId, page).then(function (response) {
                    if (!response.length) {
                        return;
                    }
                    self.data = self.data.concat(response);
                    assignData();
                    ajaxFinish = true;
                });
            }
            //var page = self.viewData.length / 25;
            //self.viewData = self.viewData.concat(self.data.slice(page * 25, (page * 25) + 25));
        }

        function assignData() {
            for (var i = 0; i < self.data.length; i++) {
                var files = self.data[i].files;
                for (var j = 0; j < files.length; j++) {
                    var item = files[j];
                    if (item.type) {
                        var retVal = itemThumbnailService.assignValue(item.source, 100, 125);
                        item.thumbnail = retVal.thumbnail;
                        item.icon = retVal.icon;
                    } else {
                        item.publish = true;
                    }
                }
                //self.data[i].files = itemThumbnailService.assignValues(self.data[i].files, 100, 125);
                for (var k = 0; k < self.data[i].answers.length; k++) {
                    self.data[i].answers[k].files = itemThumbnailService.assignValues(self.data[i].answers[k].files, 100, 125);
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
                  .title('Would you like to delete this post?')
                  //.textContent('All of the banks have agreed to forgive you your debts.')
                  .targetEvent(ev)
                  .ok('Ok')
                  .cancel('Cancel');

            $mdDialog.show(confirm).then(function () {
                var index = self.data.indexOf(post);
                self.data.splice(index, 1);
                boxService.deleteComment(post.id);
            });
        }
        function deleteReply(ev, post, reply) {

            //boxType //userType
            var confirm = $mdDialog.confirm()
                  .title('Would you like to delete this reply?')
                  //.textContent('All of the banks have agreed to forgive you your debts.')
                  .targetEvent(ev)
                  .ok('Ok')
                  .cancel('Cancel');

            $mdDialog.show(confirm).then(function () {
                var index = post.answers.indexOf(reply);
                post.answers.splice(index, 1);
                boxService.deleteReply(reply.id);
            });
        }
        function createReply(comment) {

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
                    id: response.commentId,
                    url: response.userUrl,

                    userId: user.id,
                    userImage: user.image,
                    userName: user.name,
                    files: self.add.files.map(pushItem)
                };
                comment.answers.push(newComment);
                self.add.newReplyText = '';
                self.add.files = [];
                comment.showFrom = false;
                $scope.$emit('follow-box');
            }).finally(function () {
                self.add.disabled = false;
            });

        }

        function pushItem(c) {
            var temp = c.system;
            //temp = itemThumbnailService.assignValues(c, 100, 125);
            var retVal = itemThumbnailService.assignValue(temp.source, 100, 125);
            temp.thumbnail = retVal.thumbnail;
            temp.icon = retVal.icon;
            temp.numOfViews = undefined;
            temp.type = 'item';
            return temp;
        }

        function createComment() {

            var files = self.add.files.filter(function (e) {
                return e.postId == null;
            });
            var filesId = files.map(function (c) {
                return c.system.id;
            });

            self.add.disabled = true;

            //content,boxId, files, anonymously
            boxService.postComment(self.add.newText, boxId, filesId, self.add.anonymous).then(function (response) {
                var newComment = {
                    content:  extractUrls(self.add.newText.replace(/[&<>]/g, replaceTag)),
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
                postId: postId
            });
        }

        var postId;
        self.add.upload = uploadFile;
        self.openReply = openReply;

        function openReply(post) {
            angular.forEach(self.data, function (elem) {
                elem.showFrom = false;
            });
            post.showFrom = true;
            self.add.newReplyText = '';
            self.add.files = [];
        }
        function uploadFile(post) {
            postId = post;
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
                        files[i].sizeFormated = plupload.formatSize(files[i].size);
                        files[i].complete = false;
                        files[i].postId = postId;
                        self.add.files.push(files[i]);
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

