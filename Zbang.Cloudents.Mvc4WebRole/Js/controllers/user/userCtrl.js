var mUser = angular.module('mUser', []);
mUser.controller('UserCtrl',
        ['$scope',
        function ($scope) {
            cd.pubsub.publish('initUser');
            cd.pubsub.publish('user');
            //todo proper return;
        }
        ]);
