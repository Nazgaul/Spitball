(function() {
    angular.module('app.dashboard').controller('SideMenu', dashboard);
    dashboard.$inject = ['dashboardService', 'accountService'];

    function dashboard(dashboardService, accountService) {
        var d = this;
        d.courses = [];
        d.privateBoxes = [];
        dashboardService.getBoxes(0).then(function(response) {
            d.courses = $.grep(response, function(b) {
                return b.boxType === 'academic';
            });
            d.privateBoxes = $.grep(response, function(b) {
                return b.boxType === 'box';
            });
        });
        accountService.getDetails().then(function (response) {
            d.userUrl = response.url;
        });
    }
})();