angular.module('mInvite').
    controller('InviteCtrl',
        ['$scope', '$routeParams', '$timeout',
         'sShare', 'sGoogle', 'sFacebook', 'sUser',
         function ($scope, $routeParams, $timeout, sShare, sGoogle, sFacebook, sUser) {

             $scope.boxId = $routeParams.boxId,
             $scope.boxName = $routeParams.boxName;
             $scope.params = {
                 contactLimit: 35,
                 contactPage: 35,
                 inviteText: $scope.boxId ? 'Invite to ' : 'Invite to cloudents'
             };
             var states = {
                 cloudents: 0,
                 facebook: 1,
                 google: 2
             },
               currentUsers;



             if (!sGoogle.isAuthenticated()) {
                 sGoogle.initGApi().then(function () {
                     sGoogle.checkAuth(true);
                 });
             }

             if ($scope.boxId) {
                 selectState(states.facebook);
                 showPage();

                 return;
             }

             if (sFacebook.isAuthenticated()) {
                 $scope.selectState(states.facebook);
             } else {
                 $scope.selectState(states.cloudents);
             }

             showPage();


             $scope.addContacts = function () {
                 $scope.params.contactLimit += $scope.params.contactPage;
             };

             $scope.inviteContact = function (contact) {


                 if (currentState === states.google || currentState === states.cloudents) {
                     contact.invited = true;

                     sShare.invite.box({ recepients: [contact.id], boxId: $scope.boxId }).then(function (response) {
                         if (!response.success) {
                             alert('Error');
                         }
                     });

                     return;
                 }

                 if (currentState === states.facebook) {

                     $scope.params.facebookInvite = true;
                     sFacebook.send({
                         link: $scope.box.url,
                         to: contact.id
                     }).then(function () {
                         $scope.params.facebookInvite = false;

                         contact.invited = true;

                         var data = {
                             id: contact.id,
                             username: contact.username || contact.id,
                             firstName: contact.firstname,
                             middleName: contact.middlename,
                             lastName: contact.lastname,
                             sex: contact.gender
                         };
                         sShare.facebookInvite.box(data).then(function (response1) {
                             if (!response1.success) {
                                 alert('Error');
                             }
                         });
                     }, function () {
                         $scope.params.facebookInvite = false;
                     });

                 }
             };

             $scope.socialConnect = function () {
                 if (currentState === states.facebook) {
                     sFacebook.loginFacebook().then(function () {
                         $scope.selectState(states.facebook);
                     });
                     return;
                 }
                 if (currentState === states.google) {
                     sGoogle.checkAuth(false).then(function () {
                         $scope.selectState(states.google);
                     });
                     return;
                 }
             };

             function showPage() {
                 $timeout(function () {
                     $scope.$emit('viewContentLoaded');
                 });
             }

             function selectState(state) {        
                 var params = getParamsByState(currentState);
                 $scope.params.currentState = state;
                 $scope.params.contactLimit = $scope.params.contactPage;
                 $scope.params.contacts = null;
                 

             }

             function getParamsByState(state) {

                 var params;
                 switch (state) {
                     case states.cloudents:
                         params = {
                             text: 'Cloudents friends',
                             isConnected: true,
                             className: 'cloudentsContent'
                             
                         };

                         sUser.friends().then(function (response) {
                             var data = response.success ? response.payload : [],
                                cloudentsUsers = data.my;
                             currentUsers = cloudentsUsers;
                             $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                             $scope.$broadcast('update-scroll');
                         });

                         return params;

                     case states.google:
                         params = {
                             text: 'Gmail friends',
                             connectText : 'from your gmail account',
                             isConnected: sGoogle.isAuthenticated(),
                             className: 'gmailContent'
                         };

                         $scope.params.isConnected = params.isConnected;

                         if (!params.isConnected) {
                             return params;
                         }

                         sGoogle.contacts().then(function (response) {
                             currentUsers = response;
                             $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                             $scope.$broadcast('update-scroll');

                         });

                         return params;

                     case states.facebook:
                         params = {
                             text: 'Facebook friends',
                             connectText : 'from your facebook account',
                             isConnected: sFacebook.isAuthenticated(),
                             className: 'fbContent'
                         };

                         $scope.params.isConnected = params.isConnected;

                         if (!params.isConnected) {
                             return params;
                         }
                         sFacebook.contacts('id,first_name,middle_name,last_name,gender,username,picture.height(64).width(64)').then(function (response) {
                             currentUsers = response;
                             $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                             $scope.$broadcast('update-scroll');

                         });

                         return params;


                 }

             }


             function filterContacts() {
                 if (!$scope.params.contactSearch || $scope.params.contactSearch.length < 2) {
                     $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                     $scope.$broadcast('update-scroll');
                     return;
                 }

                 $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: $scope.params.contactSearch });
                 $scope.$broadcast('update-scroll');

             }

         }
        ]);