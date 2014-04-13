(function ($, cd, dataContext, pubsub, ZboxResources, analytics) {
    "use strict";

    if (window.scriptLoaded.isLoaded('qvm')) {
        return;
    }

    var eById = document.getElementById.bind(document);

    cd.loadModel('quiz', 'QuizContext', QuizViewModel);

    function QuizViewModel() {

        function UserComment(data) {
            var that = this;
            that.id = data.id;
            that.questionId = data.questionId;
            that.text = data.text;
            that.date = data.date;
            that.userId = data.userId;
            that.userName = data.userName;
            that.userPicture = data.userPicture;
        }
        function AnswerPerQuestion(data) {
            var that = this;
            that.questionId = data.q;
            that.answerId = data.a;
        }
        var quiz, quizTQuestion,
            quizUserScore, quizUserRight,
            quizUserWrong, quizRetake,
            quizTimerToggle, quizTimer,
            quizTimerResult, quizShare,
            quizFS, quizMsg, quizCL;

        var quizId = cd.getParameterFromUrl(4), startTime, stopWatch, firstTime = true;

        cd.pubsub.subscribe('quiz', function (data) {
            if (!firstTime) {
                return;
            }
            firstTime = false;

            initQuiz();

            pubsub.publish('quiz_load');
        });

        function assignElements() {
            quiz = eById('quiz'), quizTQuestion = eById('quizTQuestion'),
            quizUserScore = eById('quizUserScore'), quizUserRight = eById('quizUserRight'),
            quizUserWrong = eById('quizUserWrong'), quizRetake = eById('quizRetake'),
            quizTimerToggle = eById('quizTimerToggle'), quizTimer = eById('quizTimer'),
            quizCL = eById('quiz_CL'), quizMsg = eById('quiz_msg'),quizFS = eById('quiz_FS'),
            quizTimerResult = eById('quizTimerResult'), quizShare = eById('quizShare');
        }

        function initQuiz() {
            assignElements();
            checkAnswerSheet();

            if (!stopWatch) {
                stopWatch = new Stopwatch(quizTimer);
            }

            quizCL.value = cd.location();

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
                getComments();
                showScore(timeTaken);

                function fillAnswers() {
                    var answerSheet = {}, qa;
                    for (var i = 0, l = userResult.questions.length; i < l; i++) {
                        qa = userResult.questions[i];
                        answerSheet[qa.questionId] = qa.answerId;
                    }

                    var questions = quizTQuestion.children,
                        question, questionId;

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


            $(quizCheckAnswers).click(function () {

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
                    success: function () {
                        getComments();
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

            $(quizTQuestion).on('change', 'input', function () {
                quizCheckAnswers.disabled = false;

                if (stopWatch.isRunning) {
                    return;
                }

                toggleTimer(true, false);
            });

            $(quizCL).click(function (e) {
                e.preventDefault();
                this.select();
            });

            $(quizMsg).click(function () {
                cd.pubsub.publish('message');
            });

            $(quizFS).click(function () {
                var itemName = quiz.querySelector('.itemNameText').textContent,
                    uniName = cd.getParameterFromUrl(1),
                    boxName = cd.getParameterFromUrl(3)
                cd.shareFb(cd.location(), //url
                  itemName, //title
                  uniName ? boxName+ ' - ' + uniName : boxName, //caption
                  JsResources.IShared + ' ' + itemName + ' ' +  JsResources.OnCloudents +
                  '<center>&#160;</center><center></center>' + JsResources.CloudentsJoin,
                  null //picture
                  );
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
                    question.classList.add('userCorrect');
                    continue;
                }

                question.classList.add('userWrong');
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

        function getComments() {

            $('.quizComments').empty();

            dataContext.quizGetDiscussion({
                data: { quizId: quizId },
                success: function (data) {
                    data = data || {};
                    var map = data.map(function (comment) {
                        return new UserComment(comment);
                    });
                    populateComments(map);

                    cd.updateTimeActions(quizTQuestion);
                    registerDiscussionEvents();
                    quiz.classList.add('checkQuiz');

                }
            });

            function populateComments(comments) {

                var comment, commentsObj = {};
                for (var i = 0, l = comments.length ; i < l; i++) {
                    comment = comments[i];

                    if (!commentsObj[comment.questionId]) {
                        commentsObj[comment.questionId] = [];
                    }
                    commentsObj[comment.questionId].push(comment);
                }

                var questions = quizTQuestion.children,
                    question, questionId, comment;

                for (var i = 0, l = questions.length; i < l; i++) {
                    question = questions[i];
                    questionId = question.getAttribute('data-id');
                    if (commentsObj[questionId]) {
                        appendComments(question, commentsObj[questionId]);
                    }

                    question.getElementsByClassName('quizUserCommentImg')[0].src = cd.userDetail().img;
                }


            }

            function registerDiscussionEvents() {
                $('.qNumOfCmnts').click(function () {
                    $('.commentWpr').removeClass('show');
                    $(this).parents('.commentWpr').addClass('show');
                });

                $(quizTQuestion).on('keyup', '.cTextArea', function () {
                    this.nextElementSibling.disabled = this.value.length === 0;
                });

                $('.askBtn').click(function () {

                    var text = this.previousElementSibling.value,
                        question = $(this).parents('li')[0],
                        questionId = question.getAttribute('data-id');
                    if (text.length > 0) {
                        dataContext.quizCreateDiscussion({
                            data: { questionId: questionId, text: text }
                        });
                    }

                    var comment = {
                        id: '',
                        userId: cd.userDetail().nId,
                        userPicture: cd.userDetail().img,
                        userName: cd.userDetail().name,
                        text: text,
                        date: new Date()
                    }

                    var html = cd.attachTemplateToData('quizCommentTemplate', comment);
                    var commentsElement = question.getElementsByClassName('quizComments')[0];
                    commentsElement.insertAdjacentHTML('beforeend', html);
                    cd.parseTimeString(commentsElement.lastElementChild.querySelector('.createTime'));
                });

                $(quizTQuestion).on('click', '.closeDialog', function () {
                    $(this).parents('.commentWpr').removeClass('show');
                });


            }

            function appendComments(question, comments) {
                question.getElementsByClassName('qNumOfCmnts')[0].textContent = comments.length + ' ' + (comments.length > 1 ? ZboxResources.Comments : ZboxResources.Comment);

                var commentsHTML = '';
                for (var i = 0, l = comments.length; i < l; i++) {
                    commentsHTML += cd.attachTemplateToData('quizCommentTemplate', comments[i]);
                }

                question.getElementsByClassName('quizComments')[0].insertAdjacentHTML('beforeend', commentsHTML);
            }
        }

        function clearQuiz() {
            quiz.classList.remove('checkQuiz');
            $(quizTQuestion).find('input').removeAttr('disabled').prop('checked', false);
            $(quizTQuestion).children().removeClass('noAnswer userWrong');
            $(quizTQuestion).find('.userCorrect').removeClass('userCorrect');
            stopWatch.reset();
            stopWatch = null;
            firstTime = true;
        }



        //#region rate

        //function registerRateEvents() {

        //}

        //#endregion

    }

})(jQuery, cd, cd.data, cd.pubsub, ZboxResources, cd.analytics);