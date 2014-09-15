mWizardBoxCreate.controller('SocialInviteCtrl',
    ['$scope', 'sUser', 'sGoogle', 'sFacebook', 'sShare',
         function ($scope, sUser, sGoogle, sFacebook, sShare) {

             var states = {
                 google: 'go',
                 cloudents: 'cl',
                 facebook: 'fb'
             },
             cloudentsUsers, currentUsers, currentState;

             if (!sGoogle.isAuthenticated()) {
                 sGoogle.initGApi().then(function () {
                     sGoogle.checkAuth(true).then(function (response) {

                     });
                 });
             }

             $scope.params = {};

             $scope.selectState = function (state) {
                 currentState = state;
                 var params = getParamsByState(currentState);
                 $scope.params.stateClass = params.className;
                 $scope.params.placeholder = params.placeholder;
             }

             $scope.filterContacts = function () {
                 if (!$scope.params.contactSearch || $scope.params.contactSearch.length < 2) {
                     return;
                 }

                 $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: $scope.params.contactSearch });

             };

             $scope.inviteContact = function (contact) {

                 contact.invited = true;

                 if (currentState === states.google || currentState === states.cloudents) {
                     sShare.invite.box({ recepients: [contact.id], boxUid: $scope.box.id }).then(function () {
                         if (!response.success) {
                             alert('Error');
                         }
                     });

                     return;
                 }

                 if (currentState === states.facebook) {
                     $scope.params.fbBlock = true;
                     sFacebook.send({
                         link: $scope.box.url,
                         to: contact.id
                     }).then(function (response) {
                         $scope.params.fbBlock = false;

                         var data = {
                             id: contact.id,
                             username: contact.username || contact.id,
                             firstName: contact.firstname,
                             middleName: contact.middlename,
                             lastName: contact.lastname,
                             sex: contact.gender
                         };
                         sShare.facebookInvite.box(data).then(function (response) {
                             if (!response.success) {
                                 alert('Error');
                             }
                         });
                     }, function () {
                         $scope.params.fbBlock = false;
                     });

                 }
             };

             $scope.facebookConnect = function () {
                 sFacebook.login().then(function (response) {
                     $scope.selectState(states.facebook);
                 });
             };

             $scope.googleConnect = function () {
                 sGoogle.checkAuth(true).then(function (response) {
                     $scope.selectState(states.google);
                 });
             };

             function getParamsByState(state) {
                 var params, isConnected;
                 switch (state) {
                     case state.cloudents:
                         params = {
                             className: 'js--cloudState',
                             placeholder: 'Search cloudents',
                             isConnected: true
                         };

                         sUser.friends().then(function (response) {
                             var data = response.success ? response.payload : [],
                                cloudentsUsers = data;

                             currentUsers = cloudentsUsers;
                             $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                         });
                         break;
                     case state.google:
                         params = {
                             className: 'js--gmState',
                             placeholder: 'Search cloudents',
                             isConnected: sGoogle.isAuthenticated()
                         };

                         $scope.params.isConnected = params.isConnected;

                         if (!params.isConnected) {
                             return;
                         }

                         sGoogle.contacts().then(function (response) {
                             currentUsers = response;
                             $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });

                         });

                         break;
                     case state.facebook:
                         params = {
                             className: 'js--fbState',
                             placeholder: 'Search cloudents',
                             isConnected: sFacebook.isAuthenticated()
                         };

                         $scope.params.isConnected = params.isConnected;

                         if (!params.isConnected) {
                             return;
                         }
                         sFacebook.contacts('id,first_name,middle_name,last_name,gender,username,picture.height(64).width(64)').then(function (response) {
                             currentUsers = response;
                             $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });

                         });


                 }
             }
         }]
);
