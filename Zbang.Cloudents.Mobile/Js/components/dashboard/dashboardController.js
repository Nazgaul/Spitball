angular.module('dashboard', ['ajax']).
    controller('DashboardController',
    ['dashboardService', 'user', function (dashboardService) {
        var dashboard = this;


        var page = 0,
            isFetching,
            endResult;



        dashboard.welcomeClosed = true;

        dashboard.closeWelcome = function () {
            dashboard.welcomeClosed = true;
        };


        //#region box list
        dashboard.getBoxList = function (isAppend) {
            if (isFetching || endResult) {
                return;
            }
            isFetching = true;

            dashboardService.getBoxList(page).then(function (boxes) {
                page++;       

                if (!boxes.length) {
                    endResult = true;
                    return;
                } else {
                    dashboard.welcomeClosed = true;
                }

                
                if (!isAppend) {
                    dashboard.boxes = boxes;
                    return;
                }

                dashboard.boxes = dashboard.boxes.concat(boxes);

            }).finally(function () {
                isFetching = false;
            });
        };

        dashboard.getBoxList();

        //#endregion

        //#region recommended boxes

        dashboardService.getRecommendedBoxesList().then(function (recommendedBoxes) {
            dashboard.recommendedBoxes = recommendedBoxes;
        });

        dashboard.test = function (e) {
            console.log(e);
        };
    }]
);  