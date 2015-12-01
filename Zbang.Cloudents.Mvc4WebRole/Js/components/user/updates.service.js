(function () {
    angular.module('app.user').service('userUpdatesService', userUpdates);
    userUpdates.$inject = ['ajaxService', '$q', 'userDetailsFactory', '$rootScope'];

    function userUpdates(ajaxservice, $q, userDetails, $rootScope) {
        var self = this;
        self.data = [];

        var deferred = $q.defer();

        userDetails.init().then(function() {
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
        function getUpdates() {
            ajaxservice.get('/user/updates/').then(function (response2) {
                self.data = response2;
                deferred.resolve();//(self.data);
            });
        }

        function deleteUpdates(boxId) {
            ajaxservice.post('/box/DeleteUpdates', new {
                boxId: boxId
            });
        }

        

        function updates(boxid, callBack) {
            boxUpdates(boxid, function(x) {
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