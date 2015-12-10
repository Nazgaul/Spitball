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
                    url: '/{boxtype:box|course}/{universityType}/{boxId}/{boxName}/',
                    controller: 'BoxController as b',
                    //containerClass: 'boxState',
                    resolve: {
                        boxData: [
                            'boxService', '$stateParams', function (boxService, $stateParams) {
                                return boxService.getBox($stateParams.boxId);

                            }
                        ]
                    },
                    //onEnter: ['$location', '$state', '$stateParams', function ($location, $state, $stateParams) {
                        //$state.go('box.feed');
                    //}],
                },
                templateUrl: '/box/indexpartial/'
            },
              
            {
                state: 'box.feed',
                config: {
                    url: 'feed/',
                    controller: 'FeedController as f',
                    parent: 'box'
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