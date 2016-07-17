'use strict';
(function () {
    angular.module('app.user').service('userUpdatesService', userUpdates);
    userUpdates.$inject = ['ajaxService', '$q', 'userDetailsFactory', '$rootScope', '$window', '$stateParams'];

    function userUpdates(ajaxservice, $q, userDetails, $rootScope, $window, $stateParams) {
        var self = this;
        var data = [];

        var deferred = $q.defer();

        userDetails.init().then(function () {
            if (userDetails.get().university.id) {
                getUpdates();
            }
        });
        self.updatesNum = updatesNum;
        self.boxUpdates = boxUpdates;
        self.deleteUpdates = deleteUpdates;

        var allUpdates = {};

        $rootScope.$on('universityChange', function () {
            if (!Object.keys(allUpdates).length) {
                getUpdates();
            }
        });
        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
            if (fromState.parent === 'box') {
                deleteUpdates(fromParams.boxId);
            }
        });
        function getUpdates() {
            ajaxservice.get('/user/updates/').then(function (response2) {
                data = response2;
                for (var i = 0; i < response2.length; i++) {
                    var currBox = typeof (allUpdates[response2[i].boxId]) == "undefined" ? {} : allUpdates[response2[i].boxId];

                    if (response2[i].questionId) {
                        currBox[response2[i].questionId] = true;
                    }
                    if (response2[i].answerId) {
                        currBox[response2[i].answerId] = true;
                    }
                    allUpdates[response2[i].boxId] = currBox;
                }
                deferred.resolve();//(self.data);
            });
        }



        $window.onbeforeunload = function () {

            var boxId = $stateParams.boxId;
            if (boxId) {
                deleteFromServer(boxId);
            }
        };

        function deleteFromServer(boxId) {
            if (!userDetails.isAuthenticated()) {
                return;
            }
            ajaxservice.post('/box/deleteupdates/', {
                boxId: boxId
            });
        }

        function deleteUpdates(boxId) {
            boxId = parseInt(boxId, 10);
            updatesNum(boxId).then(function (length) {
                if (!length) {
                    return;
                }
                deleteFromServer(boxId);
                delete allUpdates[boxId];
            });
        }



        //function updatesNum(boxid, callBack) {
        //    boxUpdates(boxid, function () {
        //        var v = allUpdates[boxid] ? Object.keys(allUpdates[boxid]).length : 0;
        //        callBack(v);
        //    });
        //}

        //function boxUpdates(boxid, callBack) {
        //    var promise = deferred.promise;

        //    promise.then(function () {
        //        var v = allUpdates[boxid];
        //        callBack(v);
        //    });
        //}

        function updatesNum(boxid, callBack) {
            var q = $q.defer();

            boxUpdates(boxid).then(function () {
                q.resolve(allUpdates[boxid] ? Object.keys(allUpdates[boxid]).length : 0);
            });

            return q.promise;
        }

        function boxUpdates(boxid) {
            var promise = deferred.promise;

            var q = $q.defer();

            promise.then(function () {
                q.resolve(allUpdates[boxid]);
            });

            return q.promise;

        }
    }

})();