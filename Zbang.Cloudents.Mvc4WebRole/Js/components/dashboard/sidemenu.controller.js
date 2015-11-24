(function() {
    angular.module('app.dashboard').controller('SideMenu', dashboard);
    dashboard.$inject = ['dashboardService', 'userDetails', '$rootScope'];

    function dashboard(dashboardService, userDetails, $rootScope) {
        var d = this;
        d.courses = [];
        d.privateBoxes = [];


        $rootScope.$on('universityChange', function () {
            getBoxes();
        });

        userDetails.get().then(function (response) {
            d.userUrl = response.url;
            if (response.university.id) {
                getBoxes();
            }
        });

        function getBoxes() {
            dashboardService.getBoxes(0).then(function (response2) {
                d.courses = $.grep(response2, function (b) {
                    return b.boxType === 'academic';
                });
                d.privateBoxes = $.grep(response2, function (b) {
                    return b.boxType === 'box';
                });
            });
        }
    }
})();