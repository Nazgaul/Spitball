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
                    templateUrl: function () {
                        return routerHelper.buildUrl('/box/indexpartial/');
                    },
                    controller: 'BoxController as b'
                  

                }
            }, {
                state: 'box.feed',
                config: {
                    templateUrl: function () {
                        return routerHelper.buildUrl('/box/feedpartial/');
                    },
                    url: '#feed',
                    controller: 'FeedController as f'
                }
            }, {
                state: 'box.items',
                config: {
                    templateUrl: function () {
                        return routerHelper.buildUrl('/box/itemspartial/');
                    },
                    url: '#items',
                    controller: 'ItemsController as i',

                }
            }, {
                state: 'box.quiz',
                config: {
                    templateUrl: function () {
                        return routerHelper.buildUrl('/box/quizpartial/');
                    },
                    url: '#quizzes',
                    controller: 'QuizzesController as q'
                }
            },
                {
                    state: 'box.members', config: {
                        templateUrl: function () {
                            return routerHelper.buildUrl('/box/memberspartial/');
                        },
                        url: '#members',
                        controller: 'MembersController as m'
                    }
                }


            ];
        }
    }
})();