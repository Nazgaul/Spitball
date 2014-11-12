mBox.controller('BoxMembersCtrl',
        ['$scope', '$filter', 'sModal', 'sBox', '$timeout', '$analytics', 'resManager', 'sShare', 'sUserDetails','sFacebook',
        function ($scope, $filter, sModal, sBox, $timeout, $analytics, resManager, sShare, sUserDetails, sFacebook) {
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

                member.reinvited = false;
                member.reinvitedItem = false;
                member.action = true;
                sBox.removeUser({ boxId: $scope.info.boxId, userId: member.id }).then(function () { //uid

                });

                if (member.userStatus === 'Subscribe') {
                    remove(true);
                    member.removed = true;
                    return;
                }

                if (member.userStatus === 'Invite') {
                    $timeout(remove, 3000);
                    $timeout(function () { member.uninvited = true; }, 10);
                    member.uninvitedItem = true;
                }

                function remove() {
                    $analytics.eventTrack('Box Members', {
                        category: 'Remove User',
                        label: 'User removed a member or someone invited'
                    });

                    var index = $scope.members.indexOf(member);
                    $scope.members.splice(index, 1);
                }


            };

            $scope.reinviteUser = function (member) {
                member.action = true;
                
                if (isNaN(member.email)) {
                    emailInvite();
                } else {
                    facebookInvite();
                }

                function facebookInvite() {       
                    sShare.facebookInvite.box({ id: member.email, boxId: $scope.boxId }).then(openFbModal, function () {
                        member.action = false;
                    });

                    function openFbModal(response) {
                        $scope.params.facebookInvite = true;

                        sFacebook.send({
                            path: response.url,
                            to: member.email
                        }).then(function () {
                            $analytics.eventTrack('Box Members', {
                                category: 'Facebook Reinvite'
                            });
                            $timeout(function () { member.reinvited = true; }, 50);              
                           member.reinvitedItem = true;

                        }).finally(function () {
                            $scope.params.facebookInvite = false;
                            member.action = false;
                        });
                    }
                }


                function emailInvite() {
                    sShare.invite.box({ boxId: $scope.boxId, recepients: [member.email] }).then(function () {
                        member.action = false;
                    });

                    $analytics.eventTrack('Box Members', {
                        category: 'Reinvite'
                    });
                }

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