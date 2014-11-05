
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
        sort: {
            asc: 0,
            des: 1
        }        
    });
mUser.controller('UserCtrl',
    ['$scope', '$rootScope', '$timeout', '$routeParams', '$q', '$filter', '$location', 'sModal', 'debounce', 'sUserDetails', 'sUser', 'sShare', 'sBox', 'sLibrary', 'userConstants',
    function ($scope, $rootScope, $timeout, $routeParams, $q, $filter, $location, sModal, debounce, sUserDetails, sUser, sShare, sBox, sLibrary, userConstants) {
        "use strict";

        $scope.params = {
            sortDirection: userConstants.sort.des
        };

        //#region profile
        $scope.profile = {};

        sUser.minProfile({ userId: $routeParams.userId }).then(function (response) {
            $scope.profile = response;
            $scope.profile.isSelf = $scope.profile.id === sUserDetails.getDetails().id;

            getData();
            $timeout(function () {
                $rootScope.$broadcast('viewContentLoaded');
            });
        });

        //sendUserMessage - is on main.js as well
        $scope.sendUserMessage2 = function () {

            sModal.open('shareEmail', { 
                data: {
                    users: [$scope.profile],
                    singleMessage: true
                }
            });

            //TODO analytics

        };
        //#endregion


        $scope.inviteCloudents = function () {
            sModal.open('cloudentsInvite');

            //TODO analytics
        };


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

            //TODO analytics
        };

        var lastQuery;
        $scope.filterMembers = debounce(function () {
            if ($scope.admin.members.search === lastQuery) {
                return;
            }
            lastQuery = $scope.admin.members.search;
            $scope.admin.members.limit = userConstants.admin.membersLimit;
            $scope.admin.members.list = $filter('orderByFilter')($scope.admin.members.fullList, 'name');

        }, 150);

        $scope.sortByDeptartment = function () {
            var reverse;
            if ($scope.params.sortDirection === userConstants.sort.des) {
                $scope.params.sortDirection = userConstants.sort.asc;
                reverse = false;
            } else if ($scope.params.sortDirection === userConstants.sort.asc) {
                $scope.params.sortDirection = userConstants.sort.des;
                reverse = true;
            }

            var array = $filter('orderBy')($scope.admin.members.list, ['department', 'name'], reverse);

            $scope.admin.members.list = array;

            //TODO analytics
        };

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

            sModal.open('shareEmail', { data: sendData });
            //TODO analytics
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

            //TODO analytics
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
                //TODO analytics

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
                //TODO analytics

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
                //TODO analytics

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
                //TODO analytics

                $scope.invites.showAll = false;
                $scope.invites.limit = userConstants.invites.list.init;
            },
            reInvite: function (invite) {
                if (invite.inviteType === 'inviteToCloudents') {
                    sShare.invite.cloudents({ recepients: [invite.userid] }).then(function () { });
                    return;
                }

                //TODO analytics

                sShare.invite.box({ boxId: invite.boxid, recepients: [invite.userid] }).then(function () { });

                invite.submitted = true;
            }
        }
        //#endregion 

        function getData() {
            $scope.promises = {};

            $scope.promises.boxes = sUser.boxes({ userId: $scope.profile.id }).then(boxesResponse).finally(function () {
                $scope.boxes.loading = false;
            });


            $scope.promises.activity = sUser.activity({ userId: $scope.profile.id }).then(activityResponse).finally(function () {
                $scope.activity.loading = false;
            });


            if ($scope.profile.isSelf) {  //need to show invites only when self user page
                $scope.promises.invites = sUser.invites().then(invitesResponse).finally(function () {
                    $scope.invites.loading = false;
                });
            }

            if (!$scope.admin.visible()) {  //dont show members list for non admins
                $scope.promises.friends = sUser.friends({ userId: $scope.profile.id }).then(friendsResponse).finally(function () {
                    $scope.friends.loading = false;
                });
                return;
            }

            var membersPromise = sUser.adminFriends(),  //promise for members and departments
                departmentsPromise = sLibrary.departments();

            $scope.promises.admin = $q.all([membersPromise, departmentsPromise]);

            $scope.promises.admin.then(adminRespose).finally(function () {
                $scope.admin.members.loading = false;
            });


            function invitesResponse(response) {
                $scope.invites.list = response;                
            }

            function friendsResponse(response) {
                var commonFriend;
                _.each(response.user, function (userFriend) {
                    commonFriend = _.find(response.my, function (myFriend) {
                        return userFriend.id === myFriend.id;
                    });

                    commonFriend ? $scope.friends.common.list.push(commonFriend) : $scope.friends.all.list.push(userFriend);
                });
            }

            function boxesResponse(response) {
                var box;
                for (var i = 0, l = response.length; i < l; i++) {
                    box = response[i];
                    if (box.userType === 'subscribe' || box.userType === 'owner') {
                        $scope.boxes.common.list.push(box);
                    }
                    else {
                        $scope.boxes.following.list.push(box);
                    }
                }
            }

            function activityResponse(response) {
                $scope.activity.items.list = response.items;
                $scope.activity.questions.list = response.questions;
                $scope.activity.answers.list = response.answers;
            }

            function adminRespose(response) {
                $scope.admin.members.fullList = $scope.admin.members.list = $filter('orderByFilter')(response[0], { field: 'name', input: '' });
                $scope.admin.members.departmemnts = response[1];
            }

        }
    }
    ]);