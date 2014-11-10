mBox.controller('BoxQuizzesCtrl',
		['$scope', '$rootScope', '$timeout', 'sBox', 'sNewUpdates', 'sUserDetails', 'sQuiz',
        function ($scope, $rootScope, $timeout, sBox, sNewUpdates, sUserDetails, sQuiz) {
            "use strict";

            var jsResources = window.JsResources;


            var consts = {
                view: {
                    thumb: 'thumb',
                    list: 'list'
                },
                itemsLimit: 50
            };

            $scope.qOptions = {
                currentView: consts.view.thumb,
                itemsLimit: consts.itemsLimit,
                manageTab: false,
                starsLength: 5,
                starsWidth: 69,
            };
            sBox.quizes({ id: $scope.boxId, pageNumber: 0 }).then(function (quizzes) {                
                $scope.quizzes = _.map(quizzes, function (quiz) {
                    sNewUpdates.isNew($scope.boxId, 'quizzes', quiz.id, function (isNew) {
                        quiz.isNew = isNew;
                    });
                    return quiz;
                });
                $scope.quizzes.sort(sort);
                $scope.options.loader = false;

            });
            //#region quiz
            $scope.addQuiz = function () {
                if (!sUserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                if ($scope.info.userType === 'invite' || $scope.info.userType === 'none') {
                    alert(jsResources.NeedToFollowBox);
                    return;
                }

                $analytics.eventTrack('Box Quizzes', {
                    category: 'Add Quiz'
                });

                $rootScope.$broadcast('initQuiz', { boxId: $scope.boxId, boxName: $scope.info.name });
                $timeout(function () {
                    $rootScope.options.quizOpen = true;
                });
            };
            $scope.deleteAllow = function (item) {
                return ($scope.info.userType === 'subscribe' || $scope.info.userType === 'owner') &&
                       ($scope.info.userType === 'owner' || item.ownerId === sUserDetails.getDetails().id || sUserDetails.getDetails().isAdmin);
            };


            $scope.removeQuiz = function (quiz) {
                cd.confirm2(jsResources.SureYouWantToDelete + ' ' + (quiz.name || '') + "?").then(function () {

                    var data = {
                        id: quiz.id,
                    }

                    sQuiz.delete(data).then(remove,function() {
                        alert('error deleting ' + quiz.name); //translate
                    });
                    $analytics.eventTrack('Box Quizzes', {
                        category: 'Remove Quiz'
                    });
                });

                function remove() {
                    var index = $scope.quizzes.indexOf(quiz);

                    if (index > -1) {
                        $scope.quizzes.splice(index, 1);
                    }

                    if (!quiz.publish) {
                        $rootScope.$broadcast('closeQuizCreate', quiz.id);
                    }
                    calcQuizCount();
                }
            };

            function calcQuizCount() {
                $scope.info.quizLength = _.countBy($scope.quizzes, function (num) {
                    return num.publish ? 'publish' : 'not';
                }).publish || 0;
            }

            $scope.selectQuiz = function (e, item) {
                if (!item.publish) {
                    $analytics.eventTrack('Box Quizzes', {
                        category: 'Edit Quiz'
                    });

                    $rootScope.$broadcast('initQuiz', { boxId: $scope.boxId, boxName: $scope.boxName, quizId: item.id });
                    $timeout(function () {
                        $rootScope.options.quizOpen = true;
                    });
                }                
            };

            //#region view
            $scope.changeView = function (view) {
                if ($scope.qOptions.currentView === view) {
                    return;
                }
                $scope.qOptions.itemsLimit = consts.itemsLimit;
                $scope.qOptions.lastView = $scope.qOptions.currentView;
                $scope.qOptions.currentView = view;

                $analytics.eventTrack('Box Quizzes', {
                    category: 'Change View',
                    label: 'User changed view to ' + view
                });
            };

            $scope.getView = function () {
                return $scope.qOptions.currentView === consts.view.thumb ? 'quizThumbView' : 'quizListView';
            };

            //function resetLastView() {
            //    if ($scope.options.lastView) {
            //        $scope.changeView($scope.options.lastView);
            //    }

            //}
            //#endregion



            $scope.$on('QuizAdded', function (e, quizItem) {
                if (quizItem.boxId !== $scope.boxId) {
                    return;
                }

                var quiz, index;
                quiz = _.find($scope.quizzes, function (x) {
                    return x.id === quizItem.id;
                }),
                index = $scope.quizzes.indexOf(quiz);


                if (quiz) {
                    if (!quizItem.publish) { //if it's not published only update the name
                        quiz.name = quizItem.name;
                        quiz.publish = quizItem.publish;
                        return;
                    }

                    $scope.quizzes.splice(index, 1);
                }

                $scope.quizzes.unshift(quizItem); //add quiz

                calcQuizCount();
                
            });

            $scope.$on('QuizDeleted', function (e, data) {
                if (data.boxId !== $scope.boxId) {
                    return;
                }

                var quiz, index;
                quiz = _.find($scope.quizzes, function (x) {
                    return x.id === data.quizId;
                }),
                index = $scope.quizzes.indexOf(quiz);

                if (!quiz) {
                    return;
                }

                $scope.quizzes.splice(index, 1);
                calcQuizCount();
                
            });

            function sort(a, b) {
                if (a.date > b.date) {
                    return -1;
                } else {
                    return 1;
                }
            }

            //#endregion
        }]);

