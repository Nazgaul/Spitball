define(['app'], function (app) {
    app.controller('DashboardCtrl',
        ['$scope', '$modal','$document',
         '$window','Dashboard', 'Box',
         'User', 'NewUpdates',

         function ($scope, $modal, $document,$window, Dashboard, Box, User, NewUpdates) {
             $scope.title = 'Dashboard';
             $scope.academicBoxes = [];
             $scope.groupBoxes = [];

             maxVisible();

             $scope.options = {
                 maxAcademicVisible: $scope.rows * $scope.cols - 1,//-1 is browse button             
                 addBoxPerScroll : 3
             };
             $scope.partials = {
                 friends: '/Dashboard/FriendsPartial',
                 createBox: '/Dashboard/PrivateBoxPartial'
             };

             $scope.myCourses = JsResources.CoursesFollow;

             

             Dashboard.boxList().then(function (data) {
                 $scope.wall = data.payload.wall;
                 $scope.friends = data.payload.friends;

                 mapBoxes(data.payload.boxes);

                 $scope.contentLoaded = true;

                 $scope.$broadcast('update-scroll');

                 document.getElementById('mLoading').style.display = 'none';
                 document.getElementById('dashboard').style.display = 'block';
                 document.getElementById('dashboard').style.opacity = 1;

                 calculateGroupsVisible();




             });
             //tempfix

             $scope.removeBox = function (box) {
                 cd.confirm2($scope.removeConfirm(box)).then(function () {
                     Box.remove({ id: box.id }).then(function () {
                         var index;
                         if (box.boxType === 'academic') {
                             index = $scope.academicBoxes.indexOf(box);
                             $scope.academicBoxes.splice(index, 1);
                         } else {
                             index = $scope.groupBoxes.indexOf(box);
                             $scope.groupBoxes.splice(index, 1);
                         }

                     });
                 });
             };

             $scope.openCreateBox = function () {
                 var modalInstance = $modal.open({
                     windowClass: "privateBox",
                     templateUrl: $scope.partials.createBox,
                     controller: 'CreateBoxCtrl',
                     backdrop: 'static'
                 });

                 modalInstance.result.then(function (box) {
                     cd.pubsub.publish('nav', box.url);
                     //location.path();
                 }, function () {
                     //dismiss
                 });
             };

             $scope.openShowFriends = function () {
                 User.friends().then(function (data) {
                     var modalInstance = $modal.open({
                         windowClass: "boxSettings",
                         templateUrl: $scope.partials.friends,
                         controller: 'ShowFriendsCtrl',
                         backdrop: 'static',
                         resolve: {
                             friends: function () {
                                 return data.payload.my;
                             }
                         }
                     });

                     modalInstance.result.then(function (box) {
                     }, function () {
                         //dismiss
                     });
                 });
             };


             $scope.removeConfirm = function (box) {
                 if (box.userType === 'none') {
                     return;
                 }

                 if (box.userType === 'owner') {
                     return JsResources.SureYouWantTo + ' ' + JsResources.ToDeleteBox;
                 }

                 if (box.membersCount <= 2 && box.commentCount < 2 && !box.itemCount) {
                     return 'You have created an empty course, if you unfollow this course it will be deleted. Do you want to delete the course?';
                 }

                 return JsResources.SureYouWantTo + ' ' + JsResources.ToLeaveGroup;
             };

             $scope.getTitle = function (box) {
                 if (box.userType === 'none') {
                     return '';
                 }

                 if ((box.userType === 'owner') || (box.membersCount <= 2 && box.commentCount < 2 && !box.itemCount)) {
                     return JsResources.Delete;
                 }

                 return JsResources.LeaveGroup;
             };

             $scope.showRemove = function (box) {
                 if (box.userType === 'owner' || box.userType === 'subscribe') {
                     return true;
                 }

                 return false;
             };

             $scope.addBoxes = function () {
                 var academicRemain = $scope.academicBoxes.length - $scope.options.maxAcademicVisible,
                    groupRemain;

                 if (academicRemain >= $scope.options.addBoxPerScroll) {
                     $scope.options.maxAcademicVisible += $scope.options.addBoxPerScroll;

                     console.log($scope.options.maxAcademicVisible, $scope.options.maxGroupVisible);///


                     return;
                 }

                 if (academicRemain < $scope.options.addBoxPerScroll) {
                     $scope.options.maxAcademicVisible += academicRemain;
                     groupRemain = $scope.options.addBoxPerScroll - academicRemain;
                     $scope.options.maxGroupVisible += groupRemain;

                     console.log($scope.options.maxAcademicVisible, $scope.options.maxGroupVisible);///
                     return;
                 }
                 groupRemain = $scope.groupBoxes.length - $scope.options.maxGroupVisible;
                 if (groupRemain > 0) {
                     $scope.options.maxGroupVisible += groupRemain;
                 }
                 console.log($scope.options.maxAcademicVisible, $scope.options.maxGroupVisible);///


             };

             function mapBoxes(boxes) {
                 var academic = [], group = [];
                 for (var i = 0, l = boxes.length; i < l; i++) {
                     boxes[i].boxPicture = boxes[i].boxPicture || '/images/emptyState/my_default3.png';
                     if (boxes[i].boxType === 'academic') {
                         academic.push(boxes[i]);
                     } else {
                         group.push(boxes[i]);
                     }
                     //show box updates                
                     boxes[i].numOfUpdates = NewUpdates.getBoxUpdates(boxes[i].id);
                 }



                 $scope.academicBoxes = academic;
                 $scope.groupBoxes = group;

             }
             //document.body.scrollHeight > document.body.clientHeight;
             function maxVisible() {
                 var height = document.body.clientHeight - 130,//130 is top bar and title
                     width = document.body.clientWidth - 324, //sidebar + margins
                     boxWidth = 238, boxHeight = 120; //include margins

                 $scope.cols = Math.floor(width / boxWidth);
                 $scope.rows = Math.ceil(height / boxHeight);                                   
             }

             function calculateGroupsVisible() {
                 if ($scope.academicBoxes.length > $scope.options.maxAcademicVisible) {
                     $scope.options.maxGroupVisible = 0;
                     return;
                 }

                 $scope.options.maxGroupVisible = $scope.options.maxAcademicVisible - $scope.academicBoxes.length;
             }

             $window.onresize = maxVisible;
         }
    ]);
});