//define('manageCtrl',['app'], function (app) {
mBox.controller('SettingsCtrl',
    ['$scope', '$modal', '$modalInstance', '$location', 'sUserDetails', 'sBox', 'data',

     function ($scope, $modal, $modalInstance, $location, UserDetails, Box, data) {
         //Settings
         var states = {
             settings: 1,
             members: 2
         };
         $scope.info = data.info;
         $scope.info.members = data.members;
         $scope.info.boxId = data.boxId;
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
                             user: member
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
             var index = $scope.info.members.indexOf(member);
             $scope.info.members.splice(index, 1);
             Box.removeUser({ boxUid: $scope.info.boxId, userId: member.uid }).then(function () { //uid

             });
         };

         $scope.reinviteUser = function (member) {
             Box.invite({ Recepients: [member.uid], boxUid: $scope.info.boxId }).then(function () { //uid

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