"use strict";
mBox.controller('SideBarCtrl',
    ['$scope', 'sBox',
        function ($scope, sBox) {

            sBox.sideBar({ id: $scope.boxId }).then(function (response) {
                var data = response.success ? response.payload : {};
                
                $scope.recommendedCourses = data.recommendBoxes;
                parseLeaderboard(data.leaderBoard);

            });

            function parseLeaderboard(leaderBoard) {
                if (!leaderBoard || leaderBoard.length === 0) {
                    return;
                }                
                $scope.leaderBoard = {
                    first: leaderBoard[0],
                    second: leaderBoard[1],
                    third: leaderBoard[2],
                    fourth: leaderBoard[3],
                    fifth: leaderBoard[4]
                };
            }

        }]);
