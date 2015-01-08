﻿angular.module('box', ['ajax', 'feed', 'boxItems']).
    controller('BoxController',
    ['$scope', '$stateParams', 'boxService', function ($scope, $stateParams, boxService) {
        var box = this;

        var boxData;

        boxService.getData($stateParams.boxId).then(function (data) {
            boxData = data;
            box.id =
            box.name = data.name;
            box.professorName = data.professorName;
            box.courseId = data.courseId;

            boxService.doneLoad();
        });

        box.setCurrentTab = function (tab) {
            window.scrollTo(0, 0);
            box.currentTab = tab;
        };

        $scope.$on('uploadStart', function () {
            box.uploading = true;
            box.setCurrentTab(null);
        });


        $scope.$on('uploadComplete', function () {
            box.uploading = false;
            box.setCurrentTab('feed');

        });

        box.setCurrentTab('feed');
    }]
);