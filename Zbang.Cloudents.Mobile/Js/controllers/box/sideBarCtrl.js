mBox.controller('SideBarCtrl',
    ['$scope', 'sBox',
        function ($scope, sBox) {
            "use strict";
            sBox.sideBar({ id: $scope.boxId }).then(function (data) {                             
                parseRecommendedCourses(data.recommendBoxes);
                parseLeaderboard(data.leaderBoard);

            });

            function parseRecommendedCourses(recommendedCourses) {
                if (!recommendedCourses || recommendedCourses.length === 0) {
                    return;
                }
                $scope.recommendedCourses = recommendedCourses;
            }

            function parseLeaderboard(leaderBoard) {
                if (!leaderBoard || leaderBoard.length === 0) {
                    return;
                }

                $scope.leaderBoard = {
                    first: leaderBoard[0],
                    second: leaderBoard[1],
                    secondExist: !_.isUndefined(leaderBoard[1]),
                    third: leaderBoard[2],
                    thirdExist: !_.isUndefined(leaderBoard[2]),
                    fourth: leaderBoard[3],
                    fifth: leaderBoard[4]
                };
            }

        }]);
