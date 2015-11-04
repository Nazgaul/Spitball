(function () {
    angular.module('app.box.feed').controller('FeedController', feed);
    feed.$inject = ['boxService', '$stateParams', '$timeout'];

    function feed(boxService, $stateParams, $timeout) {
        var self = this;
        boxService.getFeed($stateParams.boxId).then(function (response) {
            self.data = response;

            for (var i = 0; i < self.data.length; i++) {
                for (var j = 0; j < self.data[i].files.length; j++) {
                    self.data[i].files[j].thumbnail = buildThumbnailUrl(self.data[i].files[j].source);
                }
                for (var k = 0; k < self.data[i].answers.length; k++) {
                    for (var l = 0; l < self.data[i].answers[k].files.length; l++) {
                        self.data[i].answers[k].files[l].thumbnail = buildThumbnailUrl(self.data[i].answers[k].files[l].source);
                    }
                }
            }
        });

        function buildThumbnailUrl(name) {
            return 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(name) + '.jpg?width=100&height=125&mode=crops&scale=both';
        }

        self.disabled = false;
        self.createComment = function () {
            //console.log(self.newText)

            self.disabled = true;

            //content,boxId, files, anonymously
            boxService.postComment(self.newText, $stateParams.boxId).then(function (response) {
                self.data.unshift({
                    content: self.newText,
                    creationTime: new Date(),
                    id: response.commentId,
                    url: response.userUrl,
                    userId: response.userId,
                    userImage: response.userImage,
                    userName: response.userName
                });
                self.newText = '';
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
                self.disabled = false;
            });
        }

        self.fileUpload = {
            url: '/upload/file/',
            options: {
                chunk_size: '3mb'
            },
            callbacks: {
                filesAdded: function (uploader, files) {

                    for (var i = 0; i < files.length; i++) {
                        files[i].sizeFormated = plupload.formatSize(files[i].size);
                        files[i].complete = false;
                        u.files.push(files[i]);
                    }
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },

                beforeUpload: function (up, file) {
                    up.settings.multipart_params = {
                        fileName: file.name,
                        fileSize: file.size,
                        boxId: boxid,
                        question: false,
                        isComment: false

                    };


                    //var data = {};
                    //if (file.newQuestion) {
                    //    data.newQuestion = true;
                    //}
                    //if (file.questionId) {
                    //    data.questionId = file.questionId;
                    //}

                    //$rootScope.$broadcast('BeforeUpload', data);

                },
                //uploadProgress: function (uploader, file) {
                //    console.log(file);
                //},
                fileUploaded: function (uploader, file, response) {
                    file.complete = true;
                    // $scope.loading = false;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        $rootScope.$broadcast('item_upload', obj.payload);
                    }
                },
                error: function (uploader, error) {
                    u.alert = error.message;
                }
            }
        }
    }
})();

