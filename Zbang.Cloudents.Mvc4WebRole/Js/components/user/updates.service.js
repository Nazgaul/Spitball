(function () {
    angular.module('app.user').service('userUpdatesService', userUpdates);
    userUpdates.$inject = ['ajaxService', '$q'];

    function userUpdates(ajaxservice, $q) {
        var self = this;
        self.data = [];

        var deferred = $q.defer();
        ajaxservice.get('/user/updates/').then(function (response) {
            self.data = response;
            deferred.resolve();//(self.data);
        });

        self.getUpdates = function (boxid, callBack) {
            var promise = deferred.promise;

            promise.then(function () {
                var v = self.data.filter(function (i) {
                    return i.boxId === boxid;
                }).length;
                callBack(v);
            });


        }
    }

})();