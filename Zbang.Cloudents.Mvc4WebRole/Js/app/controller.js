var dashboard = angular.module('mDashboard', ['apiService','ngModal','Filters']);

dashboard.config(function (ngModalDefaultsProvider) {
    return ngModalDefaultsProvider.set({
        closeButtonHtml: "<i class='fa fa-times'></i>"
    });
});

dashboard.controller('DashboardController', ['$scope', 'Dashboard','Box', function ($scope, Dashboard,Box) {
    $scope.academicBoxes = [];
    $scope.groupBoxes = [];

    var boxList = Dashboard.boxList({
        success: function (data) {
            $scope.wall = data.payload.wall;
            $scope.friends = data.payload.friends;

            mapBoxes(data.payload.boxes);
        

        $scope.contentLoaded = true;
        document.getElementById('dashboard').style.display = 'block';
        document.getElementById('mLoading').style.display = 'none';
        }
    });

    //var friends = Dashboard.friends.(function (data) {
    //    console.log(friends);
    //    console.log(data);
    //})
    $scope.title = 'CoursesFollow';
    //$scope.title = ZboxResources.CoursesFollow;
    

    $scope.getPartialHTML = function (box) {
        return document.getElementById('dashBoxTemplate').innerHTML;
    };

    $scope.removeBox = function (box) {
        Box.create.query(function () { });

    };

    
    $scope.creatingBox = false;
    //$scope.createBox = function () {                            
    //        analytics.trackEvent('Dashboard', 'Create privte box', 'Number of clicks on "create" after writing down the box name');
    //        $scope.creatingBox = true;
    //        Box.create({
    //            data: data
    //        });
    //        dataContext.createBox({
    //            data: data,
    //            success: function (result) {
    //                var box = new Box(result);
    //                cd.resetForm($('#createBoxDialog'));
    //                $privateBoxDialog.hide();
    //                $('#BoxName').val('');
    //                isSubmit = false;
    //                cd.pubsub.publish('nav', box.boxUrl/* + '?r=dashboard'*/);
    //            },
    //            error: function (msg) {
    //                isSubmit = false;
    //                cd.notification(msg[0].Value[0]);
    //            }
    //        });
    //    }
    //};

    function mapBoxes(boxes) {
        var academic = [], group = [];
        for (var i = 0, l = boxes.length; i < l; i++) {
            if (boxes[i].boxType === 'academic') {
                academic.push(boxes[i]);
            } else {
                group.push(boxes[i]);
            }
        }

        $scope.academicBoxes = academic;
        $scope.groupBoxes = group;

    };

}]);

dashboard.controller('DashboardAsideController',['$scope','User', function ($scope,User) {
    var dashWall = document.getElementById('dash_Wall'),
        dashWallTextItem = dashWall.getAttribute('data-itemText'),
        dashWallTextComment = dashWall.getAttribute('data-commenttext');

   var activityAction = function (activity) {
       switch (activityAction.action) {
           case 'item': return dashWallTextItem;
           case 'question': return dashWallTextComment;
           case 'answer': return dashWallTextComment;
           default:
               return '';

       };
   }
   
   $scope.showAllDialogShown = false;
   $scope.toggleShowFriends = function () {
       User.friends({
           success: function (data) {
               $scope.users = data.payload.my;
               $scope.showAllDialogShown = !$scope.showAllDialogShown;

           }
       });
       
   };

   $scope.sendUserMessage = function (user) {
       $scope.showAllDialogShown = false;
       var friend = [{ id: user.uid, name: user.name, userImage: user.image }];
       cd.pubsub.publish('messageFromPopup', { id: ''.id, data: friend });

   }

}]);

 