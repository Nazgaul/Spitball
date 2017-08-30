mDashboard.controller('SocialInviteCtrl',
    ['$scope', '$filter', '$location', '$analytics', 'sUser', 'sGoogle', 'sFacebook', '$timeout',
         function ($scope, $filter, $location, $analytics, sUser, sGoogle, sFacebook, $timeout) {
             "use strict";
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
                 contactLimit: 21,
                 contactPage: 21
             };

             $scope.selectState = function (state) {
                 currentState = state;
                 var params = getParamsByState(currentState);
                 $scope.params.stateClass = params.className;
                 $scope.params.placeholder = params.placeholder;
                 $scope.params.contactLimit = $scope.params.contactPage;
                 $scope.params.currentUrl = $scope.box != null ? $scope.box.url : $location.absUrl();
                 $scope.params.contacts = null;
                 $analytics.eventTrack('Select State', {
                     category: 'Social Invite',
                     label: 'User selected to invite from ' + state
                 });
                 $scope.$broadcast('update-scroll');
             }

             $scope.$watch('box.url', function () {
                 var boxName = $scope.box.url;
                 if (boxName.indexOf('http') == -1) {
                     boxName = $location.protocol() + '://' + $location.host() + boxName;
                 }
                 $scope.params.currentUrl = boxName;
             });
             $scope.filterContacts = function () {
                 if (!$scope.params.contactSearch || $scope.params.contactSearch.length < 2) {
                     $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                     $scope.$broadcast('update-scroll');
                     return;
                 }

                 $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: $scope.params.contactSearch });
                 $scope.$broadcast('update-scroll');

             };

             if (sFacebook.isAuthenticated() || $scope.inviteCloudents) {
                 $scope.selectState(states.facebook);
             } else {
                 $scope.selectState(states.cloudents);
             }

             $scope.inviteContact = function (contact) {

                 if (currentState === states.google || currentState === states.cloudents) {
                     contact.invited = true;
                     $scope.invite(contact);
                     return;
                 }

                 if (currentState === states.facebook) {

                     $scope.params.facebookInvite = true;
                     $scope.inviteFacebook(contact).then(function () {
                         //success
                         $scope.params.facebookInvite = false;
                         contact.invited = true;
                     },
                     function () {
                         //error
                         $scope.params.facebookInvite = false;

                     });
                 }
             };

             $scope.socialConnect = function () {
                 if (currentState === states.facebook) {
                     sFacebook.loginFacebook().then(function () {
                         $analytics.eventTrack('Facebook Connect', {
                             category: 'Social Invite'
                         });
                         $scope.selectState(states.facebook);
                     });
                     return;
                 }
                 if (currentState === states.google) {
                     $analytics.eventTrack( 'Google Connect', {
                         category: 'Social Invite'
                     }); sGoogle.checkAuth(false).then(function () {
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
                             placeholder: 'Search Spitball',
                             isConnected: true
                         };

                         sUser.friends().then(function (data) {
                             currentUsers = angular.copy(data.my);
                             $timeout(function () {
                                 $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                                 $scope.$broadcast('update-scroll');

                             });
                             
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
                             currentUsers = angular.copy(response);
                             $timeout(function () {
                                 $scope.params.contacts = $filter('orderByFilter')(currentUsers, { field: 'name', input: '' });
                                 $scope.$broadcast('update-scroll');

                             });

                         });

                         return params;

                     case states.facebook:
                         params = {
                             className: 'js-fbState',
                             placeholder: 'Search facebook',
                             isConnected : true
                         };

                         $scope.params.isConnected = params.isConnected;

                         return params;


                 }
             }
         }]
);
