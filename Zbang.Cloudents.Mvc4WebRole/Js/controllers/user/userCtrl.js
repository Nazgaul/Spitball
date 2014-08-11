﻿var mUser = angular.module('mUser', []);
mUser.controller('UserCtrl',
    ['$scope', '$rootScope', '$routeParams', '$q', 'sUserDetails', 'sUser', 'sLibrary',
    function ($scope, $rootScope, $routeParams, $q, sUserDetails, sUser, sLibrary) {

        $scope.consts = {
            admin: {
                score: 1000000,
                membersLimit: 50
            },
            activity: {
                tabs: {
                    files: 'f',
                    questions: 'q',
                    answers: 'a'
                },
                limit: 8
            }

        }

        //#region profile
        sUser.minProfile({ userId: $routeParams.userId }).then(function (response) {
            $scope.profile = response.payload;
            $scope.profile.isSelf = $scope.profile.id === sUserDetails.getDetails().id

            $rootScope.$broadcast('viewContentLoaded');

            getData();
        });

        $scope.sendUserMessage = function () {
            //TODO: send message
        };
        //#endregion



        //#region admin
        $scope.admin = {
            visible: function () {
                return $scope.profile.score >= $scope.consts.admin.score && $scope.profile.isSelf;
            }
        };

        $scope.sendMembersMessage = function () {
        };

        //#endregion

        //#region friends
        $scope.friends = {};
        //#endregion

        //#region activity
        $scope.activity = {
            currentTab: $scope.consts.activity.tabs.files
        }

        //#endregion

        //cd.pubsub.publish('initUser');
        //cd.pubsub.publish('user');
        //todo proper return;

        function getData() {
            $scope.promises = {};

            $scope.promises.boxes = sUser.boxes({ userId: $scope.profile.id }).then(boxesResponse);


            $scope.promises.activity = sUser.activity({ userId: $scope.profile.id }).then(activityResponse);


            if ($scope.profile.isSelf) {  //need to show invites only when self user page
                $scope.promises.invites = sUser.invites().then(invitesResponse);
            }

            if (!$scope.admin.visible) {  //dont show members list for non admins
                $scope.promises.friends = sUser.friends({ userId: $scope.profile.id }).then(friendsResponse);
                return;
            }

            var membersPromise = sUser.adminFriends(),  //promise for members and departments
                departmentsPromise = sLibrary.departments();

            $scope.promises.admin = $q.all([membersPromise, departmentsPromise]);

            $scope.promises.admin.then(adminRespose);


            function invitesResponse(response) {
                $scope.invites = response.payload;
            }

            function friendsResponse(response) {
                $scope.friends.list = {
                    all: [],
                    common: []
                }
                var commonFriend;
                _.each(response.payload.user, function (userFriend) {
                    commonFriend = _.find(response.payload.my, function (myFriend) {
                        return userFriend.uid === myFriend.uid; //uid
                    });

                    commonFriend ? $scope.friends.list.common.push(commonFriend) : $scope.friends.list.all.push(userFriend);
                });
            }

            function boxesResponse(response) {
                $scope.boxes = {
                    following: [],
                    common: []
                };

                var box;
                for (var i = 0, l = response.payload.length; i < l; i++) {
                    box = response.payload[i];
                    if (box.userType === 'subscribe' || box.userType === 'owner') {
                        $scope.boxes.common.push(box);
                    }
                    else {
                        $scope.boxes.following.push(box);
                    }
                }
            }

            function activityResponse(response) {
                $scope.activity = response.payload;
            }

            function adminRespose(response) {
                $scope.admin.members = response[0].payload,
                $scope.admin.departmemnts = response[1].payload;
            }
        }
    }
    ]);