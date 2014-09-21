mBox.controller('BoxQuizzesCtrl',
		['$scope', '$rootScope', '$timeout', 'sBox',  'sNewUpdates', 'sUserDetails',
        function ($scope, $rootScope, $timeout, sBox, sNewUpdates, sUserDetails) {
            var jsResources = window.JsResources;

            
            var consts = {
                view: {
                    thumb: 'thumb',
                    list: 'list'
                },
                itemsLimit: 21
            };

            $scope.qOptions =  {
                currentView: consts.view.thumb,
                itemsLimit: consts.itemsLimit,
                manageTab: false,
                starsLength: 5,
                starsWidth: 69,
            };
           // $scope.quizzes = [];
            sBox.quizes({ id: $scope.boxId, pageNumber: 0 }).then(function (response) {
                var data = response.success ? response.payload : [];
                $scope.quizzes = _.map(data, function (quiz) {
                    quiz.isNew = sNewUpdates.isNew($scope.boxId, 'quizzes', quiz.id);
                    return quiz;
                });
                //_.forEach($scope.quizzes, function (quiz) {
                //    quiz.isNew = sNewUpdates.isNew($scope.boxId, 'quizzes', quiz.id);
                //});

                $scope.quizzes.sort(sort);
                //$scope.filteredItems = $filter('filter')($scope.items, filterItems);
                $scope.options.loader = false;

            });
                      
           

           // $scope.quizzes.sort(sort);

            //$timeout(function () {
            //    $scope.options.loader = false;
            //}, 1000);

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

                $rootScope.$broadcast('initQuiz', { boxId: $scope.boxId, boxName: $scope.info.name });
                $timeout(function () {
                    $rootScope.options.quizOpen = true;
                });
            };

            $scope.removeQuiz = function (quiz) {
                cd.confirm2(jsResources.SureYouWantToDelete + ' ' + quiz.name + "?").then(function () {

                    var data = {
                        id: item.id,
                    }

                    Quiz.delete(data).then(remove);

                });

                function remove(response) {
                    if (!(response.Success || response.success)) {
                        alert('error deleting ' + quiz.name); //translate
                        return;
                    }
                    var index = $scope.quizzes.indexOf(item);

                    if (index > -1) {
                        $scope.quizzes.splice(index, 1);
                    }

                    if (!quiz.publish) {
                        $rootScope.$broadcast('closeQuizCreate', quiz.id);
                    }

                    sBoxData.removeQuiz(quiz);
                }
            };

            $scope.selectQuiz = function () {
                if (item.type === 'Quiz' && !item.publish) {
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
                quiz = _.find($scope.quizzes, function (quiz) {
                    return quiz.id === data.quizId;
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

            });

            $scope.$on('QuizDeleted', function (e, data) {
                if (data.boxId !== $scope.boxId) {
                    return;
                }

                var quiz, index;
                quiz = _.find($scope.quizzes, function (quiz) {
                    return quiz.id === data.quizId;
                }),
                index = $scope.quizzes.indexOf(quiz);

                if (!quiz) {
                    return;
                }

                $scope.quizzes.splice(index, 1);
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

