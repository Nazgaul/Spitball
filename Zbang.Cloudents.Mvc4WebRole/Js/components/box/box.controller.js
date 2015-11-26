(function () {
    angular.module('app.box').controller('BoxController', box);
    box.$inject = ['boxService', 'boxData', '$stateParams', '$location', '$scope', '$state', '$rootScope'];

    function box(boxService, boxData, $stateParams, $location, $scope, $state, $rootScope) {

        if (!$location.hash()) {
            $state.go('box.feed');
        }
        var b = this, boxId = $stateParams.boxId;
        b.data = boxData;



        b.needFollow = boxData.userType === 'invite' || boxData.userType === 'none';
        b.follow = follow;
        b.updateBox = updateBox;
        b.inviteToBox = inviteToBox;

        $scope.$on("close_invite", function () {
            b.inviteOpen = false;
        });

        b.uploadOn = false;
        b.uploadShow = isItemState($state.current.name);
        b.toggleUpload = toggleUpload;
        b.toggleSettings = toggleSettings;

        function toggleSettings() {
            b.settingsOpen = true;

            b.settings = {
                name: b.data.name,
                courseId: b.data.courseId,
                professorName: b.data.professorName,
                needFollow: b.needFollow
            };

            if (!b.notificationSettings) {
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
                b.data.courseId = b.settings.courseId;
                b.data.professorName = b.settings.professorName;
                console.log('need to save');
            }
            b.settingsOpen = false;
        }

        function inviteToBox() {
            b.inviteOpen = true;
            $scope.$broadcast('open_invite');
        }
        function toggleUpload(open) {

            b.uploadShow = !open;
            b.uploadOn = open;

            $scope.$broadcast('open_upload');
        }
        function follow() {
            boxService.follow(boxId);
            b.needFollow = false;
        }
        function isItemState(stateName) {
            return stateName === 'box.items';
        }

        $rootScope.$on('$stateChangeSuccess',
            function (event, toState) {

                if (isItemState(toState.name)) {
                    b.uploadShow = true;
                } else {
                    b.uploadShow = false;
                }

            });
    }
})();


