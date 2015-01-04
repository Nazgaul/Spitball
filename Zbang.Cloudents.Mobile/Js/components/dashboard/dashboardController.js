angular.module('dashboard', ['ajax']).
    controller('DashboardController',
    ['dashboardService', 'user', function (dashboardService, user) {
        var dashboard = this;

        console.log(user);
    }]
);