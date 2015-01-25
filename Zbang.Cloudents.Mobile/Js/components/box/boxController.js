angular.module('box', ['ajax', 'feed', 'boxItems']).
    controller('BoxController',
    ['$scope', '$stateParams', 'boxService', '$location', function ($scope, $stateParams, boxService, $location) {
        var box = this;

        var boxData,
            lastTab,
            firstTime = true;

        boxService.getData($stateParams.boxId).then(function (data) {
            boxData = data;
            box.name = data.name;
            box.professorName = data.professorName;
            box.courseId = data.courseId;

            boxService.doneLoad();
        });

        box.setCurrentTab = function (tab) {
            window.scrollTo(0, 0);
            box.currentTab = tab;
            if (tab) { //dont want to change hash if there is no tab selected for upload
                $location.hash(tab);
                if (firstTime) {
                    $location.replace();
                    firstTime = false;
                }
            }

        };

        $scope.$on('uploadStart', function () {
            follow();
            box.uploading = true;
            lastTab = box.currentTab;
            box.setCurrentTab(null);
        });


        $scope.$on('uploadComplete', function () {
            box.uploading = false;
            box.setCurrentTab(lastTab);
        });

        box.setCurrentTab($location.hash() || 'feed');


        $scope.$on('followBox', follow);


        function follow() {
            boxData.userType = 'owner';
        }
    }]
);