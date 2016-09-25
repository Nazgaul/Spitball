'use strict';
(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', 'boxData', '$stateParams', '$scope',
        '$state', '$rootScope', 'userDetailsFactory',
         'resManager', '$timeout', 'userUpdatesService', '$window', 'ajaxService2'];
    function box(boxService, boxData, $stateParams, $scope, $state,
        $rootScope, userDetailsFactory, resManager, $timeout, userUpdatesService, $window, ajaxService) {

        if ($state.current.name === 'box') {
            $state.go('box.feed', $stateParams, { location: "replace" });
        }
        var b = this, boxId = $stateParams.boxId;
        b.data = boxData;
        b.showLeaderboard = b.isAcademic = boxData.boxType === 'academic' || boxData.boxType === 'academicClosed';
        b.needFollow = boxData.userType === 'invite' || boxData.userType === 'none';
        b.canInvite = boxData.boxType !== 'academicClosed' && b.isAcademic || (boxData.userType === 'owner' && !b.isAcademic);
        b.canShare = boxData.boxType !== 'academicClosed' && b.isAcademic && !b.needFollow;
        b.owner = boxData.userType === 'owner';
        b.follow = follow;
        b.updateBox = updateBox;
        b.inviteToBox = inviteToBox;
        b.closeCollapse = closeCollapse;
        b.user = userDetailsFactory.get();
        //b.url = b.user.url;
        //b.image = b.user.image;
        b.isActiveState = isActiveState;
        b.inviteExpand = inviteExpand;
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
            if (b.user.isAdmin || b.user.id === userId) {
                return true;
            }
            return false;

        }
        function toggleSettings() {
            if (b.needFollow) {
                return;
            }

            if (b.settingsHtml) {
                b.settingsOpen = true;
                return;
            }
            return ajaxService.getHtml('/box/boxsettings/').then(function (response) {
                b.settingsHtml = response;
                $timeout(function () {
                    $rootScope.$broadcast('close-collapse');
                    b.settingsOpen = true;

                    b.settings = b.settings || {};
                    //boxType
                    //privacySetting

                    b.settings.name = b.data.name;
                    b.settings.needFollow = b.needFollow;
                    b.settings.submitDisabled = false;
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
                });
            });

        }

        function updateBox(updateBoxForm) {

            if (b.settings.needFollow) {
                boxService.unfollow(boxId).then(function () {
                    $rootScope.$broadcast('remove-box', boxId);
                    $state.go('dashboard');
                });
                return;
            }
            var needToSave = false;
            angular.forEach(updateBoxForm, function (value, key) {
                if (key[0] === '$') return;
                if (!needToSave) {
                    needToSave = !value.$pristine;
                }
            });
            if (needToSave) {

                b.data.name = b.settings.name;
                b.data.privacySetting = b.settings.privacy;
                if (b.isAcademic) {
                    b.data.courseId = b.settings.courseId;
                    b.data.professorName = b.settings.professorName;
                }
                b.settings.submitDisabled = true;
                boxService.updateBox(boxId, b.data.name, b.settings.courseId, b.settings.professorName, b.settings.privacy, b.settings.notificationSettings).then(function (response) {
                    b.settingsOpen = false;
                    $stateParams.boxName = response.queryString;
                    $scope.app.showToaster(resManager.get('toasterBoxSettings'));
                    $state.go('box.feed', $stateParams, { location: "replace" });

                }).finally(function () {
                    b.settings.submitDisabled = false;
                });

            }
        }

        function inviteToBox() {
            if (b.inviteOpen) {
                b.inviteOpen = false;
                $rootScope.$broadcast('close-collapse');
            }
            else {
                if (!b.user.id) {
                    $rootScope.$broadcast('show-unregisterd-box');
                    return;
                }
                $rootScope.$broadcast('close-collapse');
                b.inviteOpen = true;
                $scope.$broadcast('open_invite');
            }
        }
        function inviteExpand() {
            if (b.html) {
                return;
            }
            return ajaxService.getHtml('/share/invitedialog/').then(function (response) {
                b.html = response;
                $timeout(function () {
                    $scope.$broadcast('open_invite');
                });
            });
        }

        function closeCollapse() {
            $rootScope.$broadcast('close-collapse');
        }

        $scope.$on('close-collapse', function () {
            b.inviteOpen = false;
            b.settingsOpen = false;
        });

        $scope.$on('hide-leader-board', function () {
            b.showLeaderboard = false;
        });

        $window.onbeforeunload = function () {
            if (!b.user.id) {
                return;
            }
            userUpdatesService.deleteUpdates(boxId);

        };
        function follow() {
            if (!b.user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            $scope.app.showToaster(resManager.get('toasterFollowBox'));
            boxService.follow(boxId);
            followBox();
        }

        function followBox() {
            b.needFollow = false;

            $rootScope.$broadcast('refresh-boxes');
        }

        $scope.$on('follow-box', followBox);

    }
})();


