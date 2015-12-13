﻿(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', 'boxData', '$stateParams', '$scope', '$state', 'user', '$rootScope'];

    function box(boxService, boxData, $stateParams, $scope, $state, user, $rootScope) {

        if ($state.current.name === 'box') {
            $state.go('box.feed', $stateParams, { location: "replace" });
        }
        var b = this, boxId = $stateParams.boxId;
        b.data = boxData;
        b.showLeaderboard =  b.isAcademic = b.data.boxType === 'academic';
        b.needFollow = boxData.userType === 'invite' || boxData.userType === 'none';
        b.owner = boxData.userType === 'owner';
        b.follow = follow;
        b.updateBox = updateBox;
        b.inviteToBox = inviteToBox;

        b.isActiveState = isActiveState;

        b.toggleSettings = toggleSettings;

        //stuff for child elements
        b.canDelete = canDelete;

        $scope.$on("close_invite", function () {
            b.inviteOpen = false;
        });

        function isActiveState(state) {
            return state === $state.current.name;
        }
        function canDelete(userId) {
            if (user.isAdmin || user.id === userId) {
                return true;
            }
            return false;

        }
        function toggleSettings() {
            $rootScope.$broadcast('close-collapse');
            b.settingsOpen = true;

            b.settings = b.settings || {};
            //boxType
            //privacySetting

            b.settings.name = b.data.name;
            b.settings.needFollow = b.needFollow;
            if (b.isAcademic) {
                b.settings.courseId = b.data.courseId;
                b.settings.professorName = b.data.professorName;
            } else if (b.owner) {
                b.settings.privacy = b.data.privacySetting;
            }

            if (!b.settings.notificationSettings) {
                boxService.notification(boxId).then(function (response) {
                    b.settings.notificationSettings = response;
                });
            }
        }

        function updateBox(updateBoxForm) {
            if (b.settings.needFollow) {
                boxService.unfollow(boxId).then(function () {
                    $state.go('dashboard');
                });
                return;
            }
            var needToSave = false;
            angular.forEach(updateBoxForm, function (value, key) {
                if (key[0] == '$') return;
                if (!needToSave) {
                    needToSave = !value.$pristine;
                }
            });
            if (needToSave) {

                b.data.name = b.settings.name;
                if (b.isAcademic) {
                    b.data.courseId = b.settings.courseId;
                    b.data.professorName = b.settings.professorName;
                }

                boxService.updateBox(boxId, b.data.name, b.settings.courseId, b.settings.professorName, b.settings.privacy, b.settings.notificationSettings).then(function (response) {
                    b.settingsOpen = false;
                    $stateParams.boxName = response.queryString;
                    $state.go('box.feed', $stateParams, { location: "replace" });

                });

            }
        }

        function inviteToBox() {
            $rootScope.$broadcast('close-collapse');
            b.inviteOpen = true;
            $scope.$broadcast('open_invite');
        }

        $scope.$on('close-collapse', function () {
            b.inviteOpen = false;
            b.settingsOpen = false;
        });

        $scope.$on('hide-leader-board', function () {
            b.showLeaderboard = false;
        });
        function follow() {
            boxService.follow(boxId);
            b.needFollow = false;
        }

    }
})();


