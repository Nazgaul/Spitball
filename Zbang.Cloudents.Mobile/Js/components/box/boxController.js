angular.module('box', ['ajax','feed','boxItems']).
    controller('BoxController',
    ['$stateParams', 'boxService', function ($stateParams, boxService) {
        var box = this;        
        
        var boxData;
        
        boxService.getData($stateParams.boxId).then(function (data) {
            boxData = data;
            box.name = data.name;
            box.professorName = data.professorName;
            box.courseId = data.courseId;
        });

        box.setCurrentTab = function (tab) {
            box.currentTab = tab;
        };

        box.goBack = function () {
            boxService.goBack();
        };

        box.setCurrentTab('feed');
    }]
);