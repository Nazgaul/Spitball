mBox.controller('BoxMembersCtrl',
        ['$scope', '$filter', 'sModal', 'sBox', '$timeout',
        function ($scope, $filter, sModal, sBox, $timeout) {
            "use strict";
            //Members

            $scope.params = {};

            var members;;

            sBox.members({ boxId: $scope.boxId }).then(function (boxMembers) {                
                members = boxMembers;

                $scope.members = $filter('orderByFilter')(members, { field: 'name', input: '' });
                $scope.params.membersLength = members.length;
                $scope.options.loader = false;
            });



            $scope.sendUserMessage = function (member) {
                sModal.open('shareEmail', {
                    data: {
                        singleMessage: true,
                        users: [member]
                    }                    
                });

                $analytics.eventTrack('Box Members', {
                    category: 'Send Message',
                    label: 'User sent a message to another member'
                });
            };

            $scope.removeUser = function (member) {

                member.reinvited = false;
                member.reinvitedItem = false;
                member.action = true;
                sBox.removeUser({ boxId: $scope.info.boxId, userId: member.id }).then(function () { //uid

                });

                if (member.sUserStatus === 'Subscribe') {                    
                    remove(true);
                    member.removed = true;
                    return;
                }

                if (member.sUserStatus === 'Invite') {
                    $timeout(remove, 3000);
                    $timeout(function () { member.uninvited = true; }, 10);
                    member.uninvitedItem = true;
                }

                function remove(isMember) {
                    $analytics.eventTrack('Box Members', {
                        category: 'Remove User',
                        label: 'User removed a member or someone invited'
                    });

                    var index = $scope.members.indexOf(member);
                    $scope.members.splice(index, 1);
                }


            };

            $scope.reinviteUser = function (member) {
                member.reinvitedItem = true;
                member.action = true;
                $timeout(function () { member.reinvited = true; }, 10);
                member.action = false;
                sBox.invite({ Recepients: [member.id], boxUid: $scope.info.boxId }).then(function () { //uid
                    member.action = false;
                });
                $analytics.eventTrack('Box Members', {
                    category: 'Reinvite'
                });
            };

            $scope.searchMembers = function () {
                if (!$scope.params.search) {
                    $scope.members = $filter('orderByFilter')(members, { field: 'name', input: '' });

                    return;
                }

                $scope.members = $filter('orderByFilter')(members, { field: 'name', input: $scope.params.search });
            };

            $scope.userStatus = function (status) {
                var jsResources = window.JsResources;
                switch (status) {
                    case 'Subscribe':
                        return jsResources.ActiveMember;//add resource
                    case 'Owner':
                        return jsResources.Owner;
                    case 'Invite':
                        return jsResources.Pending; //add resource
                }
            };

        }]
    );