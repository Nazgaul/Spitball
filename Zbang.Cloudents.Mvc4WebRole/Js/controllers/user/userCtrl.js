var mUser = angular.module('mUser', [])
    .constant('userConstants', {
        activity: {
            items: {
                init: 8,
                more: 8,
                starsWidth: 67,
                stars: 5
            }, questions: {
                init: 3,
                more: 3
            }, answers: {
                init: 3,
                more: 3
            }, tabs: {
                items: 'upTab1',
                questions: 'upTab2',
                answers: 'upTab3'
            }
        },
        boxes: {
            common: {
                init: 3
            }, following: {
                init: 6
            }
        },
        friends: {
            common: {
                init: 7
            }, all: {
                init: 6
            }
        },
        invites: {
            list: {
                init: 6
            }
        },
        admin: {
            score: 1000000,
            membersLimit: 50
        },
        partials: {
            message: '/Share/MessagePartial/',
        }
    });
mUser.controller('UserCtrl',
    ['$scope', '$rootScope', '$timeout', '$routeParams', '$q', '$filter', '$location', '$modal', 'debounce', 'sUserDetails', 'sUser', 'sShare', 'sBox', 'sLibrary', 'userConstants',
    function ($scope, $rootScope, $timeout, $routeParams, $q, $filter, $location, $modal, debounce, sUserDetails, sUser, sShare, sBox, sLibrary, userConstants) {


        //#region profile
        $scope.profile = {};

        sUser.minProfile({ userId: $routeParams.userId }).then(function (response) {
            $scope.profile = response.payload;
            $scope.profile.isSelf = $scope.profile.id === sUserDetails.getDetails().id;

            getData();
            //$timeout(function () {
            $rootScope.$broadcast('viewContentLoaded');
            //});
        });

        $scope.sendUserMessage = function () {

            var modalInstance = $modal.open({
                templateUrl: userConstants.partials.message,
                controller: 'ShareCtrl',
                backdrop: 'static',
                resolve: {
                    data: function () {
                        return {
                            users: [$scope.profile],
                            singleMessage: true
                        }
                    }
                }
            });

            modalInstance.result.then(function () {
            }, function () {
                //dismiss
            });

        };
        //#endregion



        //#region admin
        $scope.admin = {
            visible: function () {
                return $scope.profile.score >= userConstants.admin.score && $scope.profile.isSelf;
            },
            members: {
                limit: userConstants.admin.membersLimit,
                selectAll: false,
                loading: true,
                selected: 0
            }
        };

        $scope.addMembersLimit = function () {
            $scope.admin.members.limit += userConstants.admin.membersLimit;
        };

        $scope.toggleSelectMember = function (member) {
            if (member.isChecked) {
                $scope.admin.members.selected++;
                return;
            }

            $scope.admin.members.selected--;

        };
        $scope.toggleSelectAll = function () {
            $scope.admin.members.selectAll = !$scope.admin.members.selectAll;

            _.forEach($scope.admin.members.fullList, function (member) {
                member.isChecked = $scope.admin.members.selectAll;
            });

            if ($scope.admin.members.selectAll) {
                $scope.admin.members.selected = $scope.admin.members.fullList.length;
            } else {
                $scope.admin.members.selected = 0;
            }
        };

        var lastQuery;
        $scope.filterMembers = debounce(function () {
            if ($scope.admin.members.search === lastQuery) {
                return;
            }
            lastQuery = $scope.admin.members.search;
            $scope.admin.members.limit = userConstants.admin.membersLimit;
            $scope.admin.members.list = $filter('orderByFilter')($scope.admin.members.fullList, { field: 'name', input: $scope.admin.members.search });

        }, 150);

        $scope.sendMembersMessage = function () {
            var sendData = {
                users: _.filter($scope.admin.members.fullList, function (member) {
                    return member.isChecked;
                })
            }
            if (sendData.users.length === 0) {
                return;
            }
            if (sendData.users.length === 1) {
                sendData.singleMessage = true;
            } else {
                sendData.groupMessage = true;
            }

            var modalInstance = $modal.open({
                templateUrl: userConstants.partials.message,
                controller: 'ShareCtrl',
                backdrop: 'static',
                resolve: {
                    data: function () {
                        return sendData;
                    }
                }
            });

            modalInstance.result.then(function () {
            }, function () {
                //dismiss
            });

        };

        //#endregion

        //#region activity
        $scope.activity = {
            loading: true,
            currentTab: userConstants.activity.tabs.items,
            items: {
                limit: userConstants.activity.items.init,
                list: []

            },
            questions: {
                limit: userConstants.activity.questions.init,
                list: []

            },
            answers: {
                limit: userConstants.activity.answers.init,
                list: []
            }
        }

        $scope.itemRating = function (rating) {
            return userConstants.activity.items.starsWidth / userConstants.activity.items.stars * rating;
        };
        $scope.addMoreActivity = function (type) {
            $scope.activity[type].limit += userConstants.activity[type].init;
        };

        $scope.setActivityTab = function (type) {
            $scope.activity.items.limit = userConstants.activity.items.init;
            $scope.activity.questions.limit = userConstants.activity.questions.init;
            $scope.activity.answers.limit = userConstants.activity.answers.init;

            $scope.activity.currentTab = type;
        }

        //#endregion

        //#region boxes
        $scope.boxes = {
            showAll: false,
            loading: true,
            common: {
                init: userConstants.boxes.common.init,
                limit: userConstants.boxes.common.init,
                list: []
            },
            following: {
                init: userConstants.boxes.following.init,
                limit: userConstants.boxes.following.init,
                list: []
            },
            toggleShowAll: function () {
                if (!$scope.boxes.showAll) {
                    $scope.boxes.showAll = true;
                    $scope.boxes.common.limit = $scope.boxes.common.list.length;
                    $scope.boxes.following.limit = $scope.boxes.following.list.length;
                    return;
                }
                $scope.boxes.showAll = false;
                $scope.boxes.common.limit = userConstants.boxes.common.init;
                $scope.boxes.following.limit = userConstants.boxes.following.init;
            },
            followBox: function (box) {
                sBox.follow({ boxId: box.id }).then(function () {
                    $location.path(box.url);
                });
            }
        }
        //#endregion

        //#region friends
        $scope.friends = {
            showAll: false,
            loading: true,
            all: {
                init: userConstants.friends.all.init,
                limit: userConstants.friends.all.init,
                list: []
            },
            common: {
                init: userConstants.friends.common.init,
                limit: userConstants.friends.common.init,
                list: []
            },
            toggleShowAll: function () {
                if (!$scope.friends.showAll) {
                    $scope.friends.showAll = true;
                    $scope.friends.common.limit = $scope.friends.common.list.length;
                    $scope.friends.all.limit = $scope.friends.all.list.length;
                    return;
                }
                $scope.friends.showAll = false;
                $scope.friends.common.limit = userConstants.friends.common.init;
                $scope.friends.all.limit = userConstants.friends.all.init;
            }
        };
        //#endregion



        //#region invites
        $scope.invites = {
            showAll: false,
            init: userConstants.invites.list.init,
            limit: userConstants.invites.list.init,
            list: [],
            loading: true,
            toggleShowAll: function () {
                if (!$scope.invites.showAll) {
                    $scope.invites.showAll = true;
                    $scope.invites.limit = $scope.invites.list.length;
                    return;
                }
                $scope.invites.showAll = false;
                $scope.invites.limit = userConstants.invites.list.init;
            },
            reInvite: function (invite) {
                if (invite.inviteType === 'inviteToCloudents') {
                    sShare.invite.cloudents({ recepients: [invite.userid] }).then(function () { });
                    return;
                }

                sShare.invite.box({ boxUid: invite.boxid, recepients: [invite.userid] }).then(function () { }); //uid

                invite.submitted = true;
            }
        }
        //#endregion 

        function getData() {
            $scope.promises = {};

            $scope.promises.boxes = sUser.boxes({ userId: $scope.profile.id }).then(boxesResponse);


            $scope.promises.activity = sUser.activity({ userId: $scope.profile.id }).then(activityResponse);


            if ($scope.profile.isSelf) {  //need to show invites only when self user page
                $scope.promises.invites = sUser.invites().then(invitesResponse);
            }

            if (!$scope.admin.visible()) {  //dont show members list for non admins
                $scope.promises.friends = sUser.friends({ userId: $scope.profile.id }).then(friendsResponse);
                return;
            }

            var membersPromise = sUser.adminFriends(),  //promise for members and departments
                departmentsPromise = sLibrary.departments();

            $scope.promises.admin = $q.all([membersPromise, departmentsPromise]);

            $scope.promises.admin.then(adminRespose);


            function invitesResponse(response) {
                $scope.invites.list = response.payload;
                $scope.invites.loading = false;
            }

            function friendsResponse(response) {
                var commonFriend;
                _.each(response.payload.user, function (userFriend) {
                    commonFriend = _.find(response.payload.my, function (myFriend) {
                        return userFriend.uid === myFriend.uid; //uid
                    });

                    commonFriend ? $scope.friends.common.list.push(commonFriend) : $scope.friends.all.list.push(userFriend);
                });

                $scope.friends.loading = false;                

            }

            function boxesResponse(response) {
                var box;
                for (var i = 0, l = response.payload.length; i < l; i++) {
                    box = response.payload[i];
                    if (box.userType === 'subscribe' || box.userType === 'owner') {
                        $scope.boxes.common.list.push(box);
                    }
                    else {
                        $scope.boxes.following.list.push(box);
                    }
                }
                $scope.boxes.loading = false;                
            }

            function activityResponse(response) {                
                    $scope.activity.items.list = response.payload.items;
                    $scope.activity.questions.list = response.payload.questions;
                    $scope.activity.answers.list = response.payload.answers;

                    $scope.activity.loading = false;                
            }

            function adminRespose(response) {
                $scope.admin.members.fullList = $scope.admin.members.list = $filter('orderByFilter')(response[0].payload, { field: 'name', input: '' });
                $scope.admin.members.departmemnts = response[1].payload;
                $scope.admin.members.loading = false;
            }
        }
    }
    ]);