angular.module('mInvite', ['InviteEmail']).
    controller('InviteCtrl',
        ['$scope', '$timeout', '$filter','$location',
         'sShare', 'sGoogle', 'sFacebook', 'sUser',
         function ($scope, $timeout, $filter, $location, sShare, sGoogle, sFacebook, sUser) {
             var globalParams = {
                 contactPage: 36
             },
             states = {
                 facebook: 1,
                 google: 2
             }, currentUsers, currentState;

             $scope.params = {
                 loader: false
             }


             if (!sGoogle.isAuthenticated()) {
                 sGoogle.initGApi().then(function () {
                     sGoogle.checkAuth(true);
                 });
             }

             $timeout(function () {
                 $scope.$emit('viewContentLoaded');

             });
             selectState(states.facebook);

             $scope.$on('FacebookAuth', function (e, isAuthenticated) {
                 if (isAuthenticated) {
                     $scope.params.notConnected = '';
                 }


                 selectState(states.facebook);

                 $scope.$apply(function () { $scope.params.loader = false; });
                 

             });

             $scope.addContacts = function () {
                 $scope.params.contactLimit += globalParams.contactPage;
             };

             $scope.inviteContact = function (contact) {


                 if (currentState === states.google || currentState === states.cloudents) {
                     contact.invited = true;

                     sShare.invite.cloudents({ recepients: [contact.id]}).then(function (response) {
                         if (!response.success) {
                             alert('Error');
                         }
                     });

                     return;
                 }

                 if (currentState === states.facebook) {

                     $scope.params.facebookInvite = true;
                     sFacebook.send({
                         path:'',
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
                         selectState(states.facebook);
                     });
                     return;
                 }
                 if (currentState === states.google) {
                     sGoogle.checkAuth(false).then(function () {
                         selectState(states.google);
                     });
                     return;
                 }
             };

             $scope.selectState = selectState;

             $scope.filterContacts = function () {
                 if (!$scope.params.contactSearch || $scope.params.contactSearch.length < 2) {
                     $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                     return;
                 }

                 $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: $scope.params.contactSearch });

             }

             function selectState(state) {
                 var params = getParamsByState(state);
                 $scope.params.currentState = state;
                 $scope.params.text = params.text;
                 $scope.params.connectText = params.connectText;
                 $scope.params.isConnected = params.isConnected;
                 $scope.params.notConnected = params.notConnected;
                 $scope.params.className = params.className;
                 $scope.params.contactLimit = globalParams.contactPage;
                 $scope.params.contacts = null;

                 currentState = state;

             };




             function getParamsByState(state) {

                 var params;
                 switch (state) {
                     case states.google:
                         params = {
                             text: 'Gmail friends',
                             connectText: 'from your gmail account',
                             isConnected: sGoogle.isAuthenticated(),
                             className: 'gmailContent'
                         };

                         $scope.params.loader = true;


                         if (!params.isConnected) {
                             params.notConnected = 'notConnected';
                             return params;
                         }



                         sGoogle.contacts().then(function (response) {
                             currentUsers = response;
                             $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                             $scope.params.loader = false;

                         });

                         return params;

                     case states.facebook:
                         params = {
                             text: 'Facebook friends',
                             connectText: 'from your facebook account',
                             isConnected: sFacebook.isAuthenticated(),
                             className: 'fbContent'
                         };
                         
                         $scope.params.loader = true;


                         if (!params.isConnected) {
                             params.notConnected = 'notConnected';
                             return params;
                         }


                         sFacebook.contacts('id,first_name,middle_name,last_name,gender,username,picture.height(64).width(64)').then(function (response) {
                             currentUsers = response;
                             $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                             $scope.params.loader = false;
                         });

                         return params;


                 }

             }



         }
        ]);