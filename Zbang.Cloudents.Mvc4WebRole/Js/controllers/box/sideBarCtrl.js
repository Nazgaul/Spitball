"use strict";
mBox.controller('SideBarCtrl',
    ['$scope', 'sBox',
        function ($scope, sBox) {

            sBox.sideBar({ id: $scope.boxId }).then(function (response) {
                var data = response.success ? response.payload : {};
              
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
                    first: leaderBoard[0] == null ? null : {
                        id: leaderBoard[0].id,
                        name: leaderBoard[0].name.split(" ")[0],
                        score: leaderBoard[0].score,
                        image: leaderBoard[0].image
                    },
                    second: leaderBoard[1] == null ? null : {
                        id: leaderBoard[1].id,
                        name: leaderBoard[1].name.split(" ")[0],
                        score: leaderBoard[1].score,
                        image: leaderBoard[1].image
                    },
                    third: leaderBoard[2] == null ? null : {
                        id: leaderBoard[2].id,
                        name: leaderBoard[2].name.split(" ")[0],
                        score: leaderBoard[2].score,
                        image: leaderBoard[2].image
                    },
                    fourth: leaderBoard[3],
                    fifth: leaderBoard[4]
                };
            }

        }]);
