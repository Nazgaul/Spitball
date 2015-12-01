(function () {
    angular.module('app.dashboard').controller('SideMenu', dashboard);
    dashboard.$inject = ['dashboardService', 'userDetailsFactory', '$rootScope'];

    function dashboard(dashboardService, userDetails, $rootScope) {
        var d = this;
        d.courses = [];
        d.privateBoxes = [];


        $rootScope.$on('universityChange', function () {
            getBoxes();
        });
        userDetails.init().then(function () {
            d.userUrl = userDetails.get().url;

            if (userDetails.get().university.id) {
                getBoxes();
            }
        });

        function getBoxes() {
            dashboardService.getBoxes(0).then(function (response2) {
                for (var i = 0; i < response2.length; i++) {
                    var b = response2[i];
                    if (b.boxType === 'academic') {
                        d.courses.push(b);
                    } else {
                        d.privateBoxes.push(b);
                    }
                }
                //d.courses = $.grep(response2, function (b) {
                //    return b.boxType === 'academic';
                //});
                //d.privateBoxes = $.grep(response2, function (b) {
                //    return b.boxType === 'box';
                //});
            });
        }

        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
            if (fromState.parent === 'box') {
                if (fromParams.boxtype === 'box') {
                    var box = d.privateBoxes.filter(function(i) {
                        return i.id == fromParams.boxId;
                    }) || {};
                    box[0].updates = 0;
                } else {
                    var abox = d.courses.filter(function (i) {
                        return i.id == fromParams.boxId;
                    }) || {};
                    abox[0].updates = 0;
                }

            }
        });
    }
})();