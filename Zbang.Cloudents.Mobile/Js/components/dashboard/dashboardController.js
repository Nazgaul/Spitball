﻿angular.module('dashboard', ['ajax']).
    controller('DashboardController',
    ['dashboardService', 'userDetails', function (dashboardService, userDetails) {
        var dashboard = this;


        var page = 0,
            isFetching,
            endResult;


        if (!userDetails.isFirstTimeDashboard()) {
            dashboard.welcomeClosed = true;
        }

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
                    getRecommended();
                    endResult = true;
                    return;
                }



                if (!isAppend) {
                    dashboard.boxes = boxes;
                    dashboardService.doneLoad();
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

        function getRecommended() {
            dashboard.showRecommended = true;
            dashboardService.getRecommendedBoxesList().then(function (recommendedBoxes) {
                dashboard.recommendedBoxes = recommendedBoxes;                
            });
        }
        
        //#endregion


    
    }]
);