﻿(function () {
    angular.module('app.dashboard').service('dashboardService', dashboard);
    dashboard.$inject = ['$q', 'ajaxService', 'userUpdatesService'];

    function dashboard($q, ajaxservice, userUpdatesService) {
        var d = this;
        var serverCall = false;
        var defer = $q.defer();

        d.getBoxes = function () {
            if (d.boxes) {
                defer.resolve(d.boxes);
                return defer.promise;
            }

            if (!serverCall) {
                serverCall = true;
                ajaxservice.get('/dashboard/boxlist/').then(function (response) {
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
            return ajaxservice.get('/dashboard/university/', null, 1800000);
        }


        d.createPrivateBox = function (boxName) {
            return ajaxservice.post('/dashboard/create/', { boxName: boxName });
        }

        d.leaderboard = function() {
            return ajaxservice.get('/dashboard/leaderboard/');
        }

        d.recommended = recommended;

        function recommended() {
            return ajaxservice.get('/dashboard/recommendedcourses/');
        }
    }
})();