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
        self.getUpdates = updates;
        self.boxUpdates = boxUpdates;
        self.deleteUpdates = deleteUpdates;

        $rootScope.$on('universityChange', function () {
            getUpdates();
        });
        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
            if (fromState.parent === 'box') {
                deleteUpdates(fromParams.boxId);
            }
        });
        function getUpdates() {
            ajaxservice.get('/user/updates/').then(function (response2) {
                self.data = response2;
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
            updates(boxId, function (length) {
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



        function updates(boxid, callBack) {
            boxUpdates(boxid, function (x) {
                var v = x.length;
                callBack(v);
            });
            //var promise = deferred.promise;

            //promise.then(function () {
            //    var v = self.data.filter(function (i) {
            //        return i.boxId === boxid;
            //    }).length;
            //    callBack(v);
            //});
        }

        function boxUpdates(boxid, callBack) {
            var promise = deferred.promise;

            promise.then(function () {
                var v = self.data.filter(function (i) {
                    return i.boxId === boxid;
                });
                callBack(v);
            });
        }
    }

})();