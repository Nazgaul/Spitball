angular.module('box', ['ajax']).
    controller('BoxController',
    ['boxService', function (boxService) {
        var box = this;  

        box.setCurrentTab = function (tab) {
            box.currentTab = tab;

            switch (tab) {
                case 'feed':
                    getFeed();
                    break;
                case 'items':
                    getItems();
                    break;
            }
        };

        box.goBack = function () {
            boxService.goBack();
        };

        box.setCurrentTab('feed');   
    }]
);