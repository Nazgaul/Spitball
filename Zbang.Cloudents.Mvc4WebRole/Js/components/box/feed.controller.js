(function () {
    angular.module('app.box.feed').controller('FeedController', feed);
    feed.$inject = ['boxService', '$stateParams', '$timeout', 'externalUploadProvider', 'itemThumbnailService', 'user'];

    function feed(boxService, $stateParams, $timeout, externalUploadProvider, itemThumbnailService, user) {
        var self = this, boxId = $stateParams.boxId;
        boxService.getFeed(boxId).then(function (response) {
            self.data = response;

            for (var i = 0; i < self.data.length; i++) {
                self.data[i].files = itemThumbnailService.assignValues(self.data[i].files, 100, 125);
                for (var k = 0; k < self.data[i].answers.length; k++) {
                    self.data[i].answers[k].files = itemThumbnailService.assignValues(self.data[i].answers[k].files, 100, 125);
                }
            }
        });


        self.add = {
            files: [],
            disabled: false,
            googleDisabled: true,
            dropboxDisabled: true
        };

        self.add.createReply = function (comment) {


            var filesId = self.add.files.map(function (c) {
                return c.system.id;
            });

            self.add.disabled = true;
            boxService.postReply(self.add.newText, boxId, comment.id, filesId).then(function (response) {
                comment.answers.unshift({
                    content: self.newText,
                    creationTime: new Date(),
                    id: response.commentId,
                    url: response.userUrl,

                    userId: user.id,
                    userImage: user.image,
                    userName: user.name,
                    files: self.add.files.map(function (c) {
                        var temp = c.system;
                        //temp = itemThumbnailService.assignValues(c, 100, 125);
                        var retVal = itemThumbnailService.assignValue(c.source, 100, 125);
                        temp.thumbnail = retVal.thumbnail;
                        temp.icon = retVal.icon;
                        return temp;
                    })
                });
                self.add.newText = '';
                self.add.files = [];
            }).finally(function () {
                self.add.disabled = false;
            });

        }

        self.add.createComment = function () {

            var filesId = self.add.files.map(function (c) {
                return c.system.id;
            });

            self.add.disabled = true;

            //content,boxId, files, anonymously
            boxService.postComment(self.add.newText, boxId, filesId, self.add.anonymous).then(function (response) {
                self.data.unshift({
                    content: self.newText,
                    creationTime: new Date(),
                    id: response.commentId,
                    url: response.userUrl,
                    userId: response.userId,
                    userImage: response.userImage,
                    userName: response.userName,
                    files: self.add.files.map(function (c) {
                        var temp = c.system;
                        var retVal = itemThumbnailService.assignValue(c.source, 100, 125);
                        temp.thumbnail = retVal.thumbnail;
                        temp.icon = retVal.icon;
                        return temp;
                    })
                });
                self.add.newText = '';
                self.add.files = [];
                self.add.anonymous = false;
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


        //TODO: upon collapse
        externalUploadProvider.googleDriveInit().then(function () {
            self.add.googleDisabled = false;
        });
        externalUploadProvider.dropboxInit().then(function () {
            self.add.dropboxDisabled = false;
        });

        self.add.google = function () {
            externalUploadProvider.google(boxId).then(externalUploadComplete);
        }
        self.add.dropbox = function () {
            externalUploadProvider.dropBox(boxId).then(externalUploadComplete);
        }

        function externalUploadComplete(response) {
            for (var i = 0; i < response.length; i++) {
                self.add.files.push({
                    complete: true,
                    name: response[i].name,
                    system: response[i]
                });
            }
        }

        self.add.fileUpload = {
            url: '/upload/file/',
            options: {
                chunk_size: '3mb'
            },
            callbacks: {
                filesAdded: function (uploader, files) {

                    for (var i = 0; i < files.length; i++) {
                        files[i].sizeFormated = plupload.formatSize(files[i].size);
                        files[i].complete = false;
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
                        //isComment: false

                    };
                },
                fileUploaded: function (uploader, file, response) {

                    /*boxId: 100346
fileDto: {id: 392401, name: "google.png", likes: 0, ownerId: 1, owner: "ram y", numOfViews: 0,…}
commentsCount: 0
date: "2015-11-05T08:19:37.883568Z"
downloadUrl: "/d/100346/392401/"
id: 392401
likes: 0
name: "google.png"
numOfDownloads: 0
numOfViews: 0
owner: "ram y"
ownerId: 1
source: "fb4bd221-360e-4d69-b70c-1334619d1ad3.png"
sponsored: false
url: "/item/noa-university/100346/nice-picture/392401/google.png/"
userUrl: "/user/1/ram-y/"*/
                    file.complete = true;
                    // $scope.loading = false;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        file.system = obj.payload.fileDto;
                        // $rootScope.$broadcast('item_upload', obj.payload);
                    }
                }
                //error: function (uploader, error) {
                //    u.alert = error.message;
                //}
            }
        }
    }
})();

