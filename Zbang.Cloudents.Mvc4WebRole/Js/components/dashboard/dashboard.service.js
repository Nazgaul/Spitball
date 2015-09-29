(function () {
    angular.module('app.dashboard').service('dashboardService', dashboard);
    dashboard.$inject = ['$q', 'ajaxService', 'userUpdatesService'];

    function dashboard($q, ajaxservice, userUpdatesService) {
        var d = this;
        var serverCall = false;
        var defer = $q.defer();

        d.getBoxes = function (page) {
            if (d.boxes) {
                defer.resolve(d.boxes);
                return defer.promise;
            }

            if (!serverCall) {
                serverCall = true;
                ajaxservice.get('/dashboard/boxlist/', {page : page}, 1800000).then(function (response) {
                    serverCall = false;
                    d.boxes = response;
                    for (var i = 0; i < d.boxes.length; i++) {
                        (function (box) {
                            userUpdatesService.getUpdates(box.id, function (val) {
                                box.updates = val;
                            });
                        })(d.boxes[i]);
                    }
                    defer.resolve(d.boxes);
                });
            }
            return defer.promise;
        }

        d.getUniversityMeta = function () {
            return ajaxservice.get('/dashboard/sidebar/', null, 1800000);
        }


        d.createPrivateBox = function (boxName) {
            return ajaxservice.post('/dashboard/Create', { boxName: boxName });
        }
    }
})();