angular.module('mBox').
    controller('BoxController', ['$route', '$q', 'Box', 'NewUpdates', function ($route, $q, Box, NewUpdates) {
        $scope.boxId = cd.getParameterFromUrl(2);

        var infoPromise = $q.all(Box.info({ boxId: boxId })),
            itemsPromise = $q.all(Box.items({ boxId: boxId })),
            qnaPromise = $q.all(Box.QnA.list({ boxId: boxId }));

        Box.items().then(function (data) {
            data = data.payload || {};
            $scope.boxName = ''; //name
            $scope.boxType = '';
            $scope.comments = 0;
            $scope.courseId = 0;            
            $scope.ownerName = '';
            $scope.ownerId = 0;
            $scope.privacySetting = '';
            $scope.members = 29;
            $scope.subscribers = null;
            $scope.tabs = null;
            $scope.uniCountry = 'IL';
            $scope.userType = 'subscribe';
            /*boxType: "academic"
            comments: 0
            courseId: "10234"
            items: 22
            members: 29
            name: "בימי שואה ופקודה"
            ownerName: "The Open University - האוניברסיטה הפתוחה"
            ownerUid: 920
            privacySetting: "anyoneWithUrl"
            subscribers:
            tabs: []
            uniCountry: "IL"
            userType: "subscribe"*/

            $scope.items = 'items';
            //commentsCount: 0
            //date: "2014-06-05T14:54:29Z"
            //id: 2966
            //numOfViews: 0
            //owner: "guy golan"
            //ownerId: 18372
            //publish: false
            //rate: 0
            //type: "Quiz"
            //userUrl: "/user/1/ram"

            $scope.qna = 'qna'
            //answers: [{id:233b4bdc-3fb3-4be7-8ec5-a2f400c5063e,…}]
            //content: "asdsadas"
            //creationTime: "2014-03-20T09:54:27Z"
            
            //files: []
            //id: "59d599bb-34f5-4692-ac84-a2f400c43b74"
            //url: "/user/1/ram"
            //userImage: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg"
            //userName: "guy golan"
            //userUid: 18372

            $scope.addQuiz = function () {
            };

            $scope.openUploadPopup = function () {
            };

            $scope.openBoxManage = function (tab) {
            };

            $scope.createTab = function () {
            };
            
            $scope.shareFacebook = function () {
            };

            $scope.shareEmail = function () {
            };

            $scope.selectTab = function (tab) {
            };
        });
    }]).
    controller('QnAController', ['Box', function (Box) {
        $scope.questions = $scope.qna

        //question 

        //content: "asdsadas"
        //creationTime: "2014-03-20T09:54:27Z"
        //files: []
        //id: "59d599bb-34f5-4692-ac84-a2f400c43b74"
        //url: "/user/1/ram"
        //userImage: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg"
        //userName: "guy golan"
        //userUid: 18372

        //answer 

        //answer: false
        //content: "asdasd"
        //creationTime: "2014-03-20T09:57:20Z"
        //files: []
        //iRate: false
        //id: "233b4bdc-3fb3-4be7-8ec5-a2f400c5063e"
        //questionId: "59d599bb-34f5-4692-ac84-a2f400c43b74"
        //rating: 0
        //url: "/user/1/ram"
        //userId: 18372
        //userImage: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg"
        //userName: "guy golan"
        $scope.states = {
            emptyState: 0,
            question : 1,
            answers : 2
        }

        $scope.addAnswer = function (question) {

        };
        $scope.deleteAnswer = function (question, answer) {

        };


        $scope.addFiles = function (question,answer) {

        }

        $scope.viewItem = function (link) {

        }

        $scope.deleteItem = function () {
        };

        $scope.downloadItem = function () {
        };
        
    }]).
    controller('UploadController', ['Box', function (Box) {
        $scope.saveLink = function () {
        };

        $scope.saveDropbox = function () {
        };

        $scope.saveGoogleDrive = function () {
        };

        $scope.uploader = null//upload;
    }]).
    controller('ManageController', ['Box', function (Box) {
        //Settings

        $scope.formData = {};

        $scope.save = function () {
        };

        $scope.cancel = function () {
        };

        $scope.delete = function () {
        };
    }]);