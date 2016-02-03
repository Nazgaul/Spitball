(function () {
    angular.module('app.box').run(appRun);
    appRun.$inject = ['routerHelper'];
    function appRun(routerHelper) {
        routerHelper.configureStates(getStates());
        function getStates() {
            return [
            {
                state: 'box',
                config: {
                    url: '/{boxtype:box|course}/{universityType}/{boxId}/{boxName}/?{invId}',
                    controller: 'BoxController as b',
                    //containerClass: 'boxState',
                    resolve: {
                        boxData: [
                            'boxService', '$stateParams', function (boxService, $stateParams) {
                                return boxService.getBox($stateParams.boxId);

                            }
                        ]
                    },
                    data: { animateClass: 'class' },
                },
                templateUrl: '/box/indexpartial/'
            },
              
            {
                state: 'box.feed',
                config: {
                    url: 'feed/',
                    controller: 'FeedController as f',
                    parent: 'box',
                    //resolve: {
                    //    feedData: ['boxService', '$stateParams', function (boxService, $stateParams) {
                    //        boxService.getFeed(boxId, page).then(function (response) {
                    //            self.data = response;
                    //            assignData();

                    //        });
                    //    }]
                    //}
                },
                templateUrl: '/box/feedpartial/'
            }, {
                state: 'box.items',
                config: {
                    url: 'items/',
                    controller: 'ItemsController as i',
                    parent: 'box'
                },
                templateUrl: '/box/itemspartial/'
            }, {
                state: 'box.quiz',
                config: {
                    url: 'quizzes/',
                    controller: 'QuizzesController as q',
                    parent: 'box'
                },
                templateUrl: '/box/quizpartial/'
            },
                {
                    state: 'box.members',
                    config: {
                        url: 'members/',
                        controller: 'MembersController as m',
                        parent: 'box'
                    },
                    templateUrl: '/box/memberspartial/'
                }


            ];
        }
    }
})();