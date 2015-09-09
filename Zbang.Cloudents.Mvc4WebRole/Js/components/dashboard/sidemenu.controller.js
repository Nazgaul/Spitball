(function() {
    angular.module('app.dashboard').controller('SideMenu', dashboard);
    dashboard.$inject = ['dashboardService', 'userDetailsService'];

    function dashboard(dashboardService, userDetailsService) {
        var d = this;
        d.courses = [];
        d.privateBoxes = [];
        dashboardService.getBoxes().then(function(response) {
            d.courses = $.grep(response, function(b) {
                return b.boxType === 'academic';
            });
            d.privateBoxes = $.grep(response, function(b) {
                return b.boxType === 'box';
            });
        });
        userDetailsService.getDetails().then(function (response) {
            d.userUrl = response.url;
        });
    }
})();