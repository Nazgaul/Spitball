angular.module('dashboard', ['ajax']).
    controller('DashboardController',
    ['dashboardService', 'userDetails', function (dashboardService, userDetails) {
        var dashboard = this;


        var page = 0,
            isFetching,
            endResult;


        dashboardService.doneLoad();       

        if (!userDetails.isFirstTimeDashboard()) {
            dashboard.welcomeClosed = true;
        } else {
            dashboardService.disableFirstTime();
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
            dashboard.loading = true;
            dashboardService.getBoxList(page).then(function (boxes) {
                page++;

                if (!boxes.length) {
                    getRecommended();
                    endResult = true;
                    return;
                }

                if (boxes.length < 20) {
                    getRecommended();
                }



                if (!isAppend) {
                    dashboard.boxes = boxes;                    
                    return;
                }

                dashboard.boxes = dashboard.boxes.concat(boxes);

               

            }).finally(function () {
                isFetching = false;
                dashboard.loading = false;
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