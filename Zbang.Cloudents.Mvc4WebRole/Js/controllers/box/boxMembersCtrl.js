mBox.controller('BoxMembersCtrl',
        ['$scope', '$filter', 'sModal', 'sBox', '$timeout', '$analytics', 'resManager', 'sShare', 'sUserDetails', 
        function ($scope, $filter, sModal, sBox, $timeout, $analytics, resManager, sShare, sUserDetails) {
            "use strict";
            //Members

            $scope.params = {
                userId: sUserDetails.getDetails().id
            };

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

                //member.reinvited = false;
                //member.reinvitedItem = false;
                member.action = true;
                sBox.removeUser({ boxId: $scope.boxId, userId: member.id }).finally(function () {
                    member.action = false;
                });
                remove();
                //if (member.userStatus === 'subscribe') {
                //    remove(true);
                //    member.removed = true;
                //    return;
                //}

                //if (member.userStatus === 'invite') {
                //    $timeout(remove, 3000);
                //    $timeout(function () { member.uninvited = true; });
                //    member.uninvitedItem = true;
                //}

                function remove() {
                    $analytics.eventTrack('Box Members', {
                        category: 'Remove User',
                        label: 'User removed a member or someone invited'
                    });

                    var index = members.indexOf(member);
                    members.splice(index, 1);
                    $scope.members = members;
                    $scope.params.membersLength = $scope.members.length;
                }


            };

            $scope.reinviteUser = function (member) {
                member.action = true;
                member.reinvitedItem = true;
                $timeout(function () {
                    member.reinvited = true;
                });
                $timeout(function () {
                    member.reinvited = false;
                    member.reinvitedItem = false;
                }, 1500);

                sShare.invite.box({ boxId: $scope.boxId, recepients: [member.id] }).finally(function () {
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
                switch (status) {
                    case 'Subscribe':
                        return resManager.get('ActiveMember');//add resource
                    case 'Owner':
                        return resManager.get('Owner');
                    case 'Invite':
                        return resManager.get('Pending'); //add resource
                }
            };

        }]
    );