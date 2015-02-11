﻿mBox.controller('BoxQuizzesCtrl',
		['$scope', '$rootScope', '$timeout', '$analytics', 'sBox', 'sNewUpdates', 'sUserDetails', 'sQuiz', 'resManager', 'sLogin', 'sNotify', '$routeParams', '$location',
        function ($scope, $rootScope, $timeout, $analytics, sBox, sNewUpdates, sUserDetails, sQuiz, resManager, sLogin, sNotify, $routeParams, $location) {
            "use strict";
            var consts = {
                view: {
                    thumb: 'thumb',
                    list: 'list'
                },
                itemsLimit: 50
            };
            $scope.createUrl = $location.path() + "quizcreate/";

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
                    sLogin.registerAction();
                    return;
                }

                $analytics.eventTrack('Quizzes - Add Quiz', {
                    category: 'Box'
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
                sNotify.confirm(resManager.get('SureYouWantToDelete') + ' ' + (quiz.name || '') + "?").then(function () {

                    var data = {
                        id: quiz.id,
                    };

                    remove();

                    sQuiz.delete(data).then(null, function () {
                        sNotify.alert('error deleting ' + quiz.name); //translate
                    });
                    $analytics.eventTrack( 'Remove Quiz', {
                        category:'Box Quizzes'
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

                    if ($scope.info.feedLength) {
                        $scope.info.feedLength--;
                    }
                }
            };

            function calcQuizCount() {
                $scope.info.quizLength = _.countBy($scope.quizzes, function (num) {
                    return num.publish ? 'publish' : 'not';
                }).publish || 0;
            }

            $scope.selectQuiz = function (e, quiz) {
                if (!quiz.publish) {
                    $analytics.eventTrack('Quizzes - Edit Quiz', {
                        category: 'Box'
                    });
                    $location.path($scope.createUrl).search('quizId', quiz.id).hash(null);
                    return;
                }

                quiz.isNew = false;
                sNewUpdates.setOld($scope.boxId, 'quizzes', quiz.id);
            };

            //#region view
            $scope.changeView = function (view) {
                if ($scope.qOptions.currentView === view) {
                    return;
                }
                $scope.qOptions.itemsLimit = consts.itemsLimit;
                $scope.qOptions.lastView = $scope.qOptions.currentView;
                $scope.qOptions.currentView = view;

                $analytics.eventTrack('Quizzes - Change View', {
                    category: 'Box',
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

            $scope.$on('QuizCreateClose', function () {
                $rootScope.options.quizOpen = false;

            });

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
                    $scope.followBox(true);
                }

                $scope.quizzes.unshift(quizItem); //add quiz

                calcQuizCount();
                $scope.info.feedLength++;
            });

            $scope.$on('QuizDeleted', function (e, data) {

                $rootScope.options.quizOpen = false;

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

