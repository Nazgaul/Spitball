
angular.module('InviteEmail', []).
    controller('InviteEmailCtrl',
    ['$scope','$routeParams', 'sShare', 'sFocus', '$timeout','$analytics','sNotify',
         function ($scope, $routeParams, sShare, sFocus, $timeout, $analytics,sNotify) {
             "use strict";
             var emailRegExp = new RegExp(/^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$/),
                 sendFunc, sendObj = {};
             
             if ($scope.box && $scope.box.id) { 
                 sendFunc = sShare.invite.box; //invite to box if box.id exists we're in box page
                 sendObj =  {
                     boxId: $scope.box.id
                 };
             } else {
                 sendFunc = sShare.invite.cloudents; //invite to cloudents
             }

             $scope.formData = {
                 emailList: []
             };
             $scope.params = {
                 invFormSent: false,
                 invalidEmails: 0,
                 validEmails: 0
             };

             $scope.focusInput = function () { //fix for clicking white space
                 sFocus('emailInput');
             };


             $scope.submit = function () {
                 $analytics.eventTrack('Invite by email', {
                     category: 'Invite',
                     label:'Social popup user invited by email'
                 });

                 addEmail(true);

                 for (var i = 0, l = $scope.formData.emailList.length; i < l; i++) {
                     if ($scope.formData.emailList[i].invalid) {
                         return;
                     }
                 }

                 var emails = _.map($scope.formData.emailList, function (email) {
                     return email.address;
                 });


                 sendObj.recepients = emails;

                 sendFunc(sendObj).then(function () {
                     $scope.params.invFormSent = true; //animation
                     $timeout(function () { $scope.params.invFormSent = false; }, 2000);//animation

                     $scope.formData.emailList = [];
                     $scope.params.invalidEmails = $scope.params.validEmails = 0;
                     sendObj.recepients = null;
                 }, function () {
                     sendObj.recepients = null;
                 });
             };

             $scope.remove = function (email) {
                 var index = $scope.formData.emailList.indexOf(email);
                 $scope.formData.emailList.splice(index, 1);

                 if (email.invalid) {
                     $scope.params.invalidEmails--;
                 } else {
                     $scope.params.validEmails--;
                 }


                 sFocus('emailInput');
                 $scope.$broadcast('itemChange');
                 $analytics.eventTrack('Invite by email', {
                     category: 'Remove',
                     label: 'User removed an email'
                 });
             };

             $scope.edit = function (email) {
                 if (!email.invalid) {
                     return;
                 }

                 $scope.formData.emailInput = email.address;
                 $scope.remove(email);
                 sFocus('emailInput');
                 $scope.$broadcast('itemChange');
                 $analytics.eventTrack('Invite by email', {
                     category: 'Edit',
                     label: 'User edit an email'
                 });
             };


             $scope.keydownListener = function (e) {
                 if (e.keyCode === 9 || e.keyCode === 13 || e.keyCode === 186 || e.keyCode === 188) { // , ; TAB  
                     e.preventDefault();
                     return;
                 }
                 if (e.keyCode === 8 && !$scope.formData.emailInput) { //if backspace and value is empty delete last email
                     $scope.remove($scope.formData.emailList.slice(-1)[0]);
                     e.preventDefault();
                     return;
                 }

                 if (e.keyCode === 9 && !$scope.formData.emailInput) {
                     e.preventDefault();
                 }
             };


             $scope.keyupListener = function (e) {
                 if (e.keyCode === 188 || e.keyCode === 186 || e.keyCode === 9 || e.keyCode === 32) { // , ; TAB 
                     addEmail();
                 }
             };


             function addEmail(isAutoAdd) {
                 if (!$scope.formData.emailInput) {
                     return;
                 }

                 var email = {
                     address: $scope.formData.emailInput
                 },

                     index = $scope.formData.emailList.indexOf(email);
                 if (index > -1) {
                     if (isAutoAdd) {
                         return;
                     }
                     sNotify.alert('Contact already exists'); //translate
                     return;
                 }

                 if (!emailRegExp.test($scope.formData.emailInput)) {
                     email.invalid = true;
                     $scope.params.invalidEmails++;
                 } else {
                     $scope.params.validEmails++;
                 }

                 $scope.formData.emailInput = null;

                 $scope.formData.emailList.push(email);
                 $scope.$broadcast('itemChange');
             }
         }]).directive('inputResizer',
    ['$timeout',
        function ($timeout) {
            "use strict";
            return {
                restrict: 'A',
                scope: {
                    googleBtn: '='
                },
                link: function (scope, elem) {
                    var $input = elem.find('.inviteInput'),
                        $list = elem.find('.emailList'),
                        $wrapper = elem.find('.emailUser');

                    scope.$on('itemChange', function () {
                        $timeout(setWidth, 0);
                    });


                    //TODO: maybe use this on share popup
                    $input.on('keyup', function () {
                        if ($input[0].scrollWidth > $input[0].clientWidth) {
                            $input.width($wrapper.width());
                        }
                    });


                    function setWidth() {
                        var $emails = $list.find('.emailItem'), $email,
                            width = 0;

                        if (!$emails.length) {
                            return;
                        }
                        var lastOffsetTop = $emails.last().offset().top;

                        for (var i = $emails.length - 1; i >= 0 ; i--) {
                            $email = angular.element($emails[i]);

                            if ($email.offset().top === lastOffsetTop) {
                                width += $email.outerWidth(true);
                                lastOffsetTop = $email.offset().top;
                            }


                        }

                        var calcWidth = $wrapper.width() - width - 3;
                        if (calcWidth < 150) {
                            $input.width(195);
                            return;
                        }

                        $input.width(calcWidth);
                    }
                }
            };
        }
    ]);
