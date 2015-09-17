(function () {
    angular.module('app.dashboard').controller('Dashboard', dashboard);
    dashboard.$inject = ['dashboardService'];

    function dashboard(dashboardService) {
        var d = this;
        d.boxes = [];
        dashboardService.getBoxes().then(function (response) {
            d.boxes = response;
            //for (var i = 0; i < d.boxes.length; i++) {
            //    //var box = d.boxes[i];
               

            //    (function (box) {
            //        userUpdatesService.getUpdates(box.id, function (val) {
            //            box.updates = val;
            //        });
            //    })(d.boxes[i]);
            //}
            //console.log(d.boxes);
        });

      
    }
})();

(function () {
    angular.module('app.dashboard').controller('UniversityMeta', universityMeta);
    universityMeta.$inject = ['dashboardService'];

    function universityMeta(dashboardService) {
        var um = this;
        dashboardService.getUniversityMeta().then(function (response) {
            um.leaderBoard = response.leaderBoard;
            um.info = response.info;
        });
    }
})();




