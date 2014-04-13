(function ($, cd, dataContext, pubsub, ZboxResources, analytics) {
    "use strict";

    if (window.scriptLoaded.isLoaded('qvm')) {
        return;
    }

    var eById = document.getElementById.bind(document);

    cd.loadModel('quiz', 'QuizContext', QuizViewModel);

    function QuizViewModel() {

        function AnswerPerQuestion(data) {
            var that = this;
            that.questionId = data.q;
            that.answerId = data.a;
        }
        var quiz, quizTQuestion,
            quizUserScore, quizUserRight,
            quizUserWrong, quizRetake,
            quizTimerToggle, quizTimer,
            quizTimerResult, quizShare;

        var quizId = cd.getParameterFromUrl(4), startTime, stopWatch, firstTime = true;

        cd.pubsub.subscribe('quiz', function (data) {
            initQuiz();

            pubsub.publish('quiz_load');
        });

        function assignElements() {
            quiz = eById('quiz'), quizTQuestion = eById('quizTQuestion'),
            quizUserScore = eById('quizUserScore'), quizUserRight = eById('quizUserRight'),
            quizUserWrong = eById('quizUserWrong'), quizRetake = eById('quizRetake'),
            quizTimerToggle = eById('quizTimerToggle'), quizTimer = eById('quizTimer'),
            quizTimerResult = eById('quizTimerResult'), quizShare = eById('quizShare');
        }

        function initQuiz() {
            assignElements();
            checkAnswerSheet();

            if (!stopWatch) {
                stopWatch = new Stopwatch(quizTimer);
            }
            
                registerEvents();
            


            



            function checkAnswerSheet() {
                var userResult = JSON.parse(quiz.getAttribute('data-data'));
                if (!userResult) {
                    return;
                }
                quiz.removeAttribute('data-data');

                quiz.classList.add('checkQuiz');

                var timeTaken;
                if (userResult.timeTaken.indexOf('.') > -1) {
                    timeTaken = userResult.timeTaken.substr(0, userResult.timeTaken.lastIndexOf('.'));
                } else {
                    timeTaken = userResult.timeTaken;
                }   

                fillAnswers();
                checkAnswers();
                showScore(timeTaken);

                function fillAnswers() {
                    var answerSheet = {},qa;
                    for (var i = 0, l = userResult.questions.length; i < l; i++) {
                        qa = userResult.questions[i];
                        answerSheet[qa.questionId] = qa.answerId;
                    }

                    var questions = quizTQuestion.children,
                        question,questionId;

                    for (var i = 0, l = questions.length; i < l; i++) {
                        question = questions[i];
                        questionId = question.getAttribute('data-id');                                                
                        if (answerSheet[questionId]) {
                            $(question).find('[data-id="' + answerSheet[questionId] + '"]')[0].checked = true;
                        }
                    }
                }

            }



        }


        function registerEvents() {

            $(quizTQuestion).on('change', 'input', function () {
                if (stopWatch.isRunning) {
                    return;
                }
                toggleTimer(true, false);                
            });

            $(quizCheckAnswers).click(function () {
                
                quiz.classList.add('checkQuiz');


                var answerSheet = checkAnswers();
                showScore();
                toggleTimer(false, false);

                dataContext.quizSaveQuest({
                    data: {
                        StartTime: stopWatch.startTime,
                        EndTime: stopWatch.endTime,
                        QuizId: quizId,
                        Answers: answerSheet
                    },
                    error: function () { }
                });
            });

            $(quizRetake).click(function () {
                clearQuiz();

                toggleTimer(false, true);
            });

            $(quizTimerToggle).click(function () {
                toggleTimer(!stopWatch.isRunning, false);
            });
        }

        function toggleTimer(isStart, isReset) {
            if (!stopWatch) {
                stopWatch = new Stopwatch(quizTimer);
            }
            if (!isStart) {
                stopWatch.stop();
                if (isReset) {
                    stopWatch.reset();
                }
                quizTimerToggle.textContent = quizTimerToggle.getAttribute('data-quiz-play');
                return;
            }

            if (stopWatch.isRunning) {
                return;
            }

            stopWatch.start();
            quizTimerToggle.textContent = quizTimerToggle.getAttribute('data-quiz-pause');
        }

        function checkAnswers() {
            var question, questions = quizTQuestion.children,
                answerSheet = [];
            for (var i = 0, l = questions.length; i < l; i++) {
                question = questions[i];
                var answer = question.querySelector('input:checked');

                if (!answer) {
                    question.classList.add('noAnswer');
                    question.classList.add('userWrong');
                    question.setAttribute('data-no-answer', quizTQuestion.getAttribute('data-no-answer'));
                    continue;
                }

                answerSheet.push(new AnswerPerQuestion({ q: question.getAttribute('data-id'), a: answer.getAttribute('data-id') }));

                if (answer.parentElement.classList.contains('correct')) {
                    answer.classList.add('userCorrect');
                    continue;
                }

                answer.classList.add('userWrong');
            }

            return answerSheet;
        }

        function showScore(lastTime) {
            var totalQuestion = quizTQuestion.children.length,
                rightAnswers = quizTQuestion.getElementsByClassName('userCorrect').length

            $(quizTQuestion).find('input').prop('disabled', 'disabled');

            quizUserScore.setAttribute('data-user-score', Math.round(
               rightAnswers / totalQuestion * 100));
            quizUserRight.setAttribute('data-results', rightAnswers);
            quizUserWrong.setAttribute('data-results', totalQuestion - rightAnswers);
            quizTimerResult.textContent = lastTime || stopWatch.lastTime;
        }

        function clearQuiz() {
            quiz.classList.remove('checkQuiz');
            $(quizTQuestion).find('input').removeAttr('disabled').prop('checked', false);
            $(quizTQuestion).children().removeClass('noAnswer userWrong');
            $(quizTQuestion).find('.userCorrect').removeClass('userCorrect');
            
            stopWatch.reset();
        }

        
        //#region rate

        //function registerRateEvents() {

        //}

        //#endregion
    }

})(jQuery, cd, cd.data, cd.pubsub, ZboxResources, cd.analytics);