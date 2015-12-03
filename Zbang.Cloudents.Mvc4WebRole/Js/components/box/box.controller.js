(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', 'boxData', '$stateParams', '$location', '$scope', '$state', 'user'];

    function box(boxService, boxData, $stateParams, $location, $scope, $state, user) {

        if (!$location.hash()) {
            $state.go('box.feed', $stateParams, { location: "replace" });
        }
        var b = this, boxId = $stateParams.boxId;
        b.data = boxData;
        b.isAcademic = b.data.boxType === 'academic';

        b.needFollow = boxData.userType === 'invite' || boxData.userType === 'none';
        b.owner = boxData.userType === 'owner';
        b.follow = follow;
        b.updateBox = updateBox;
        b.inviteToBox = inviteToBox;



        //b.uploadOn = false;
        //b.uploadShow = isItemState($state.current.name);
        //b.toggleUpload = toggleUpload;
        b.toggleSettings = toggleSettings;

        //stuff for child elements
        b.canDelete = canDelete;
        //b.openMenu = openMenu;

        //var originatorEv;
        //function openMenu($mdOpenMenu, ev) {
        //    originatorEv = ev;
        //    $mdOpenMenu(ev);
        //};

        $scope.$on("close_invite", function () {
            b.inviteOpen = false;
        });
        function canDelete(userId) {
            if (user.isAdmin || user.id === userId) {
                return true;
            }
            return false;

        }
        function toggleSettings() {
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

                boxService.updateBox(boxId, b.data.name, b.settings.courseId, b.settings.professorName, b.settings.privacy, b.settings.notificationSettings);

            }
            b.settingsOpen = false;
        }

        function inviteToBox() {
            b.inviteOpen = true;
            $scope.$broadcast('open_invite');
        }
        //function toggleUpload(open) {

        //    b.uploadShow = !open;
        //    b.uploadOn = open;
        //    if (b.uploadShow) {
        //        $scope.$broadcast('open_upload');
        //    } else {
        //        $scope.$broadcast('close_upload');
        //    }
        //}
        function follow() {
            boxService.follow(boxId);
            b.needFollow = false;
        }
        //function isItemState(stateName) {
        //    return stateName === 'box.items';
        //}



        //$rootScope.$on('$stateChangeSuccess',
        //    function (event, toState) {

        //        if (isItemState(toState.name)) {
        //            b.uploadShow = true;
        //        } else {
        //            b.uploadShow = false;
        //        }

        //    });
    }
})();


