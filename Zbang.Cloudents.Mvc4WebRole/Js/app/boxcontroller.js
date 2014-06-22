//angular.module('mBox', []).
//    controller('BoxController', ['$scope', '$route', '$modal', '$q', 'Box', 'NewUpdates', function ($scope, $route, $modal, $q, Box, NewUpdates) {
//        $scope.boxId = cd.getParameterFromUrl(2);
//        $scope.uniName = cd.getParameterFromUrl(1);
//        $scope.boxName = cd.getParameterFromUrl(3);
//        var infoPromise = Box.info({ boxUid: $scope.boxId }), //uid
//            itemsPromise = Box.items({ boxUid: $scope.boxId, pageNumber: 0, uniName: $scope.uniName, boxName: $scope.boxName }), //uid
//            qnaPromise = Box.QnA.list({ boxId: $scope.boxId, uniName: $scope.uniName, boxName: $scope.boxName }),
//            all = $q.all([infoPromise, itemsPromise, qnaPromise]);

//        all.then(function (data) {
//            var info = data[0].success ? data[0].payload : {},
//                items = data[1].success ? data[1].payload : {},
//                qna = data[2].success ? data[2].payload : {};

//            $scope.info = {
//                name: info.name,
//                comments: info.comments,
//                courseId: info.courseId,
//                itemsLength: info.items,
//                membersLength: info.members,
//                members: info.subscribers,
//                ownerName: info.ownerName,
//                ownerId: info.ownerUid, //uid
//                privacy: info.privacySetting,
//                tabs: info.tabs,
//                userType: info.userType,
//                uniCountry: info.uniCountry
//            };

//            $scope.info.currentTab = null;

//            $scope.items = 'items';
//            //commentsCount: 0
//            //date: "2014-06-05T14:54:29Z"
//            //id: 2966
//            //numOfViews: 0
//            //owner: "guy golan"
//            //ownerId: 18372
//            //publish: false
//            //rate: 0
//            //type: "Quiz"
//            //userUrl: "/user/1/ram"

//            $scope.qna = 'qna'
//            //answers: [{id:233b4bdc-3fb3-4be7-8ec5-a2f400c5063e,…}]
//            //content: "asdsadas"
//            //creationTime: "2014-03-20T09:54:27Z"

//            //files: []
//            //id: "59d599bb-34f5-4692-ac84-a2f400c43b74"
//            //url: "/user/1/ram"
//            //userImage: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg"
//            //userName: "guy golan"
//            //userUid: 18372

//            document.getElementById('mLoading').style.display = 'none';
//            document.getElementById('box').style.display = 'block';
//            document.getElementById('box').style.opacity = 1;

//        });
//        $scope.addQuiz = function () {
//            $scope.params.quizOpen = true;
//            $scope.$emit('initQuiz');
//        };

//        $scope.$on('addedItem', function (event, item) {

//        });

//        $scope.openUploadPopup = function () {

//        };

//        $scope.openBoxManage = function (tab) {

//        };

//        $scope.createTab = function () {

//        };

//        $scope.shareFacebook = function () {
//        };

//        $scope.shareEmail = function () {
//        };

//        $scope.selectTab = function (tab) {
//        };

//    }]).
//    controller('QnAController', ['Box', function (Box) {
//        $scope.questions = $scope.qna

//        //question 

//        //content: "asdsadas"
//        //creationTime: "2014-03-20T09:54:27Z"
//        //files: []
//        //id: "59d599bb-34f5-4692-ac84-a2f400c43b74"
//        //url: "/user/1/ram"
//        //userImage: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg"
//        //userName: "guy golan"
//        //userUid: 18372

//        //answer 

//        //answer: false
//        //content: "asdasd"
//        //creationTime: "2014-03-20T09:57:20Z"
//        //files: []
//        //iRate: false
//        //id: "233b4bdc-3fb3-4be7-8ec5-a2f400c5063e"
//        //questionId: "59d599bb-34f5-4692-ac84-a2f400c43b74"
//        //rating: 0
//        //url: "/user/1/ram"
//        //userId: 18372
//        //userImage: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg"
//        //userName: "guy golan"
//        $scope.states = {
//            emptyState: 0,
//            question: 1,
//            answers: 2
//        }

//        $scope.addAnswer = function (question) {

//        };
//        $scope.deleteAnswer = function (question, answer) {

//        };


//        $scope.addFiles = function (question, answer) {

//        }

//        $scope.viewItem = function (link) {

//        }

//        $scope.deleteItem = function () {
//        };

//        $scope.downloadItem = function () {
//        };

//    }]).
//    controller('UploadController', ['Box', function (Box) {
//        $scope.saveLink = function () {
//        };

//        $scope.saveDropbox = function () {
//        };

//        $scope.saveGoogleDrive = function () {
//        };

//        $scope.uploader = null//upload;
//    }]).
//    controller('ManageController', ['Box', function (Box) {
//        //Settings

//        $scope.formData = {};

//        $scope.save = function () {
//        };

//        $scope.cancel = function () {
//        };

//        $scope.delete = function () {
//        };
//    }]);