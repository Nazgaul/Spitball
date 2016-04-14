(function () {
    angular.module('app.user').service('userUpdatesService', userUpdates);
    userUpdates.$inject = ['ajaxService', '$q', 'userDetailsFactory', '$rootScope', '$window', '$stateParams'];

    function userUpdates(ajaxservice, $q, userDetails, $rootScope, $window, $stateParams) {
        var self = this;
        self.data = [];

        var deferred = $q.defer();

        userDetails.init().then(function () {
            if (userDetails.get().university.id) {
                getUpdates();
            }
        });
        self.updatesNum = updatesNum;
        self.boxUpdates = boxUpdates;
        self.deleteUpdates = deleteUpdates;

        self.allUpdates = {};

        $rootScope.$on('universityChange', function () {
            if (!Object.keys(self.allUpdates).length) {
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
                self.data = response2;
                for (var i = 0; i < response2.length; i++) {
                    var currBox = typeof (self.allUpdates[response2[i].boxId]) == "undefined" ? {} : self.allUpdates[response2[i].boxId];

                    if (response2[i].questionId) {
                        currBox[response2[i].questionId] = true;
                    }
                    if (response2[i].answerId) {
                        currBox[response2[i].answerId] = true;
                    }
                    self.allUpdates[response2[i].boxId] = currBox;
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
            updatesNum(boxId, function (length) {
                if (!length) {
                    return;
                }
                deleteFromServer(boxId);
                var tempArr = [];
                for (var i = 0; i < self.data.length; i++) {
                    var temp = self.data[i];
                    if (temp.boxId !== boxId) {
                        tempArr.push(temp);
                    }
                }
                self.data = tempArr;
            });
        }



        function updatesNum(boxid, callBack) {
            boxUpdates(boxid, function (x) {
                var v = self.allUpdates[boxid] ? Object.keys(self.allUpdates[boxid]).length : 0;
                callBack(v);
            });
        }

        function boxUpdates(boxid, callBack) {
            var promise = deferred.promise;

            promise.then(function () {
                var v = self.allUpdates[boxid];
                callBack(v);
            });
        }
    }

})();