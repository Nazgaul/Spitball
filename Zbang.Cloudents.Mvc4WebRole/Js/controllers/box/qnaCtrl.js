define('qnaCtrl',['app'], function (app) {
    app.controller('QnACtrl',
    ['$scope',
     'Box',

        function ($scope,Box) {
            $scope.questions = $scope.qna;

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
                question: 1,
                answers: 2
            };

            $scope.addAnswer = function (question) {

            };
            $scope.deleteAnswer = function (question, answer) {

            };


            $scope.addFiles = function (question, answer) {

            };

            $scope.viewItem = function (link) {

            };

            $scope.deleteItem = function () {
            };

            $scope.downloadItem = function () {
            };
        }
    ]);
});