mBox.controller('BoxMembersCtrl',
        ['$scope', '$filter', '$modal', 'sBox','$timeout',
        function ($scope, $filter, $modal, sBox, $timeout) {
            //Members
            $scope.partials = {
                shareEmail: '/Share/MessagePartial/',
            };
            $scope.params = {};

            var members;;

            sBox.members({ boxId: $scope.boxId }).then(function (response) {
                var data = response.success ? response.payload : [];
                members = data;

                $scope.members = $filter('orderByFilter')(members, { field: 'name', input: '' });
                $scope.params.membersLength = members.length;
                $scope.options.loader = false;
            });

            

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
                                users: [member]
                            };
                        }
                    }
                });

                //modalInstance.result.then(function () {
                //}, function () {
                //    //dismiss
                //});
            };

            $scope.removeUser = function (member) {

                member.reinvited = false;
                member.reinvitedItem = false;
                member.action = true;
                Box.removeUser({ boxId: $scope.info.boxId, userId: member.id }).then(function () { //uid

                });

                if (member.sUserStatus === 'Subscribe') {
                    remove();
                    member.removed = true;
                    return;
                }

                if (member.sUserStatus === 'Invite') {
                    $timeout(remove, 3000);
                    $timeout(function () { member.uninvited = true; }, 10);
                    member.uninvitedItem = true;
                }

                function remove() {
                    var index = $scope.members.indexOf(member);
                    $scope.members.splice(index, 1);
                    $scope.info.membersLength--;
                }
            };

            $scope.reinviteUser = function (member) {
                member.reinvitedItem = true;
                member.action = true;
                $timeout(function () { member.reinvited = true; }, 10);
                member.action = false;
                Box.invite({ Recepients: [member.id], boxUid: $scope.info.boxId }).then(function () { //uid
                    member.action = false;
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
                        return 'Active Member';//add resource
                    case 'Owner':
                        return JsResources.Owner;
                    case 'Invite':
                        return 'Pending'; //add resource

                }
            };

        }]
    );
