//define('manageCtrl',['app'], function (app) {
mBox.controller('SettingsCtrl',
    ['$scope', '$modal', '$modalInstance', '$location','$timeout', 'sUserDetails', 'sBox', 'data',

     function ($scope, $modal, $modalInstance, $location,$timeout, UserDetails, Box, data) {
         //Settings
         var states = {
             settings: 1,
             members: 2
         };
         $scope.info = data.info;
         $scope.info.allMembers = data.members;
         $scope.info.boxId = data.boxId;
         //TODO: this is a temp solution
         $scope.info.inviteUrl = '/invite/' + data.boxId + "/";
         $scope.info.user = UserDetails.getDetails();

         $scope.partials = {
             shareEmail: '/Share/MessagePartial/',
         };

         $scope.params = {
             state: data.tab === 'settings' ? states.settings : states.members,
             search: ''
         };

         $scope.formData = {
             name: $scope.info.name,
             notification: data.notification,
             boxUid: data.boxId
         };

         if ($scope.info.boxType === 'academic') {
             $scope.formData.courseCode = $scope.info.courseId,
             $scope.formData.professor = $scope.info.professor;
         }

         if ($scope.info.boxType === 'box') {
             $scope.formData.boxPrivacy = $scope.info.privacy;
         }


         $scope.save = function () {
             Box.updateInfo($scope.formData).then(function (response) {
                 $scope.formData.queryString = response.payload.queryString;
                 $modalInstance.close($scope.formData);
             });
         };

         $scope.cancel = function () {
             $modalInstance.dismiss();
         };

         $scope.delete = function () {
             $modalInstance.dismiss();
             Box.remove({ id: $scope.info.boxId }).then(function () {
                 $location.path('/dashboard/');
             });
         };

         $scope.deleteOrUnfollow = function () {

             if ($scope.info.userType === 'none') {
                 return;
             }
             if ($scope.info.userType === 'owner') {
                 return JsResources.Delete;
             }

             if ($scope.info.membersLength <= 2 && $scope.info.comments < 2 && !$scope.info.itemsLength) {
                 return JsResources.Delete;
             }

             return JsResources.LeaveGroup;
         };

         $scope.userStatus = function (status) {
             switch (status) {
                 case 'Subscribe':
                     return 'Active Member';//add resource
                 case 'Owner':
                     return JsResources.Owner;
                 case 'Invite':
                     return 'Pending'; //add resource

             }
         };

         //Members


         $scope.sendUserMessage = function (member) {
             var modalInstance = $modal.open({
                 windowClass: "invite",
                 templateUrl: $scope.partials.shareEmail,
                 controller: 'ShareCtrl',
                 backdrop: 'static',
                 resolve: {
                     data: function () {
                         return {
                             singleMessage: true,
                             users: [member]
                         };
                     }
                 }
             });

             modalInstance.result.then(function () {
             }, function () {
                 //dismiss
             });
         };

         $scope.removeUser = function (member) {

             member.reinvited = false;
             member.reinvitedItem = false;
             member.action = true;
             Box.removeUser({ boxUid: $scope.info.boxId, userId: member.uid }).then(function () { //uid

             });

             if (member.sUserStatus === 'Subscribe') {             
                 remove();
                 member.removed = true;
                 return;
             }

             if (member.sUserStatus === 'Invite') {
                 $timeout(remove, 3000);
                 $timeout(function () { member.uninvited = true; }, 10);
                 member.uninvitedItem = true;
             }

             function remove() {
                 var index = $scope.info.allMembers.indexOf(member);
                 $scope.info.allMembers.splice(index, 1);
             }
         };

         $scope.reinviteUser = function (member) {
             member.reinvitedItem = true;
             member.action = true;
             $timeout(function () { member.reinvited = true; }, 10);
             member.action = false;
             Box.invite({ Recepients: [member.uid], boxUid: $scope.info.boxId }).then(function () { //uid
                 member.action = false;
             });
         };

 
     }
    ]);
mBox.controller('MemberCtrl', ['$scope', function ($scope) {
    $scope.isExcludedByFilter = applySearchFilter();

    $scope.$watch(
       "params.search",
       function (newName, oldName) {

           if (newName === oldName) {

               return;

           }

           applySearchFilter();


       }
   );

    function applySearchFilter() {

        var filter = $scope.params.search.toLowerCase();
        var name = $scope.member.name.toLowerCase();
        var isSubstring = (name.indexOf(filter) !== -1);

        // If the filter value is not a substring of the
        // name, we have to exclude it from view.
        $scope.isExcludedByFilter = !isSubstring;
    };
}]);
//});