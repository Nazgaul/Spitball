mDashboard.controller('SocialInviteCtrl',
    ['$scope', '$filter', 'sUser', 'sGoogle', 'sFacebook', 'sShare',
         function ($scope, $filter, sUser, sGoogle, sFacebook, sShare) {

             var states = {
                 google: 'go',
                 cloudents: 'cl',
                 facebook: 'fb'
             },
             currentUsers, currentState;

             if (!sGoogle.isAuthenticated()) {
                 sGoogle.initGApi().then(function () {
                     sGoogle.checkAuth(true);
                 });
             }             


             $scope.params = {
                 contactLimit: 35,
                 contactPage: 35
             };

             $scope.selectState = function (state) {
                 currentState = state;
                 var params = getParamsByState(currentState);
                 $scope.params.stateClass = params.className;
                 $scope.params.placeholder = params.placeholder;
                 $scope.params.contactLimit = $scope.params.contactPage;
                 $scope.params.contacts = null;
             }

             $scope.filterContacts = function () {
                 if (!$scope.params.contactSearch || $scope.params.contactSearch.length < 2) {
                     $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                     $scope.$broadcast('update-scroll');
                     return;
                 }

                 $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: $scope.params.contactSearch });
                 $scope.$broadcast('update-scroll');

             };

             if (sFacebook.isAuthenticated()) {
                 $scope.selectState(states.facebook);
             } else {
                 $scope.selectState(states.cloudents);
             }

             $scope.inviteContact = function (contact) {


                 if (currentState === states.google || currentState === states.cloudents) {
                     contact.invited = true;

                     sShare.invite.box({ recepients: [contact.id], boxId: $scope.box.id }).then(function (response) {
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
                             boxId: $scope.box.id,  
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
                         alert('Error');
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
             $scope.addContacts = function () {
                 $scope.params.contactLimit += $scope.params.contactPage;
             };

             $scope.nextStep = function () {
                 $scope.next();
             };

             function getParamsByState(state) {
                 var params;
                 switch (state) {
                     case states.cloudents:
                         params = {
                             className: 'js-cloudState',
                             placeholder: 'Search cloudents',
                             isConnected: true
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
                             className: 'js-gmState',
                             placeholder: 'Search google',
                             isConnected: sGoogle.isAuthenticated()
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
                             className: 'js-fbState',
                             placeholder: 'Search facebook',
                             isConnected: sFacebook.isAuthenticated()
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
         }]
);
