(function ($, cd, dataContext, pubsub, JsResources, analytics) {
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
            quizFS, quizMsg, quizCL,
            ratePopupTimeout, ratedQuizzes,
            $rateContainer, $rateBubble,
            $rateBtn, $ratePopup,
            initialRate,currentRate,
            $rateBubble, choosedRate = false,
            $bubbleText,

            quizId = cd.getParameterFromUrl(4), stopWatch;

        cd.pubsub.subscribe('quiz', function () {

            initQuiz();
            //loadRate();
            pubsub.publish('quiz_load');
        });

        function assignElements() {
            quiz = eById('quiz'), quizTQuestion = eById('quizTQuestion'),
            quizUserScore = eById('quizUserScore'), quizUserRight = eById('quizUserRight'),
            quizUserWrong = eById('quizUserWrong'), quizRetake = eById('quizRetake'),
            quizTimerToggle = eById('quizTimerToggle'), quizTimer = eById('quizTimer'),
            quizCL = eById('quiz_CL'), quizMsg = eById('quiz_msg'), quizFS = eById('quiz_FS'),
            quizTimerResult = eById('quizTimerResult'), quizShare = eById('quizShare');
            $rateContainer = $('#quizRateContainer'), $rateBubble = $('#quizRateBubble'), $rateBtn = $('#quizRateBtn'),
            $ratePopup = $('#quizRatePopup'), $rateBubble = $('#quizRateBubble'), $bubbleText = $rateBubble.find('.bubbleText');
        }

        function initQuiz() {
            assignElements();

            if (!stopWatch) {
                stopWatch = new Stopwatch(quizTimer);
            }

            var savedData = cd.localStorageWrapper.getItem(quizId);
            if (savedData) {
                fillUnregisterSheet(savedData);
            } else {
                checkAnswerSheet();
            }




            quizCL.value = cd.location();

            registerEvents();
        //    setRateStorage();
            function fillUnregisterSheet(savedData) {

                cd.localStorageWrapper.removeItem(quizId);

                var answerSheet = JSON.parse(savedData),
                    $quizTQuestion = $(quizTQuestion);

                quizCheckAnswers.disabled = false;

                for (var i = 0, l = answerSheet.Answers.length; i < l; i++) {
                    $quizTQuestion.find('[data-id="' + answerSheet.Answers[i].answerId + '"]')[0].checked = true;
                }

                stopWatch.startTime = new Date(answerSheet.StartTime);
                stopWatch.endTime = new Date(answerSheet.EndTime);
                stopWatch.renderTimeDiff();

                saveAnswers();
            }

            function checkAnswerSheet(sentSheet) {
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
                        $(quizTQuestion).find('[data-id="' + qa.answerId + '"]')[0].checked = true;
                    }      
                }

            }

            function setRateStorage() {
                $.data($rateContainer[0], 'fetchrate', true);

                var $rated = $('.rated');
                if ($rated.length) {
                    $rated.toggleClass('rated').text(5 - $rated.index());
                }

                if (cd.register()) {
                    if (!ratedQuizzes) {
                        ratedQuizzes = JSON.parse(cd.localStorageWrapper.getItem('ratedQuizzes'));
                        if (!ratedQuizzes) {
                            ratedQuizzes = {};
                        }
                        if (!ratedQuizzes[cd.userDetail().nId]) {
                            ratedQuizzes[cd.userDetail().nId] = [];
                        }
                    }
                }
            }

        }

        function loadRate() {
            if (ratedQuizzes && ratedQuizzes[cd.userDetail().nId] && ratedQuizzes[cd.userDetail().nId].indexOf(quizId) > -1) {
                    return;                
            }

            ratePopupTimeout = setTimeout(function () {
                $ratePopup.removeClass('changedItem').addClass('show');

                if (!cd.register()) {
                    $ratePopup.one('click', '.star', function () {
                        cd.pubsub.publish('register', { action: true });
                    });
                    return;
                }

                $ratePopup.one('click', '.star', function () {
                    $ratePopup.addClass('rated');

                    var $this = $(this),
                        startWidth = $('.stars .full').width(),
                        itemRate = getQuizRate(),
                        currentRate = 5 - $this.index(),
                        fakeRate = calculateFakeRate(startWidth, currentRate);

                    self.rate(fakeRate);
                    setQuizRate(currentRate);
                    if (ratedQuizzes[cd.userDetail().nId].indexOf(quizId) === -1) {
                        ratedQuizzes[cd.userDetail().nId].push(quizId);
                        cd.localStorageWrapper.setItem('ratedQuizzes', JSON.stringify(ratedQuizzes));
                    }
                    setTimeout(function () {
                        $ratePopup.removeClass('show').removeClass('rated');
                    }, 3000);

                });
                $ratePopup.one('click', '.closeDialog', function (e) {
                    if (ratedQuizzes[cd.userDetail().nId].indexOf(quizId) === -1) {
                        ratedQuizzes[cd.userDetail().nId].push(quizId);
                        cd.localStorageWrapper.setItem('ratedQuizzes', JSON.stringify(ratedQuizzes));
                    }
                    $ratePopup.addClass('changedItem').remove('show');

                });

            }, 3000);//3 seocnds
        }

        function registerEvents() {

            $(quizCheckAnswers).off('click').click(saveAnswers);

            $(quizRetake).off('click').click(function () {
                clearQuiz();

                toggleTimer(false, true);
            });

            $(quizTimerToggle).off('click').click(function () {
                toggleTimer(!stopWatch.isRunning, false);

            });

            $(quizTQuestion).off('click').on('change', 'input', function (e) {
                quizCheckAnswers.disabled = false;

                if (stopWatch.isRunning) {
                    return;
                }

                toggleTimer(true, false);
            });

            $(quizCL).off('click').click(function (e) {
                e.preventDefault();
                this.select();
            });

            $(quizMsg).off('click').click(function () {
                cd.pubsub.publish('message');
            });

            $(quizFS).off('click').click(function () {
                var itemName = quiz.querySelector('.itemNameText').textContent,
                    uniName = cd.getParameterFromUrl(1),
                    boxName = cd.getParameterFromUrl(3);

                if (uniName === 'my') {
                    uniName = null;
                }
                cd.shareFb(cd.location(), //url
                  itemName, //title
                  uniName ? boxName + ' - ' + uniName : boxName, //caption
                  JsResources.FbShareQuiz.format(itemName),
                  '/Images/cloudents-share-Quiz.png' //picture
                  );
            });

            pubsub.subscribe('quizclear', function () {
                clearQuiz();
            });

            //rateEvents();

            var rateMenuOpen = false;
            function rateEvents() {
              

                $rateContainer.click(function (e) {
                    trackEvent('Rate item menu opend', 'User opened the rate menu');
                    e.stopPropagation();

                    if (!cd.register()) {
                        cd.pubsub.publish('register', { action: true });
                        return;
                    }

                    if (rateMenuOpen) {
                        return;
                    }

                    rateMenuOpen = true;

                    if ($.data($rateContainer[0], 'fetchrate')) {
                        getQuizRate();
                    }

                    $rateBtn.toggleClass('clicked');
                    $bubbleText.text($bubbleText.attr('data-step1'));
                });

                $rateContainer.on('click', '.star', function (e) {
                    e.stopPropagation();
                    var startWidth = $('.stars .full').width();

                    clearTimeout(ratePopupTimeout);

                    if ($ratePopup.is(':visible')) {
                        setTimeout(function () {
                            $ratePopup.removeClass('show').addClass('rated2');
                        }, 500);
                    } else {
                        $ratePopup.removeClass('show');
                    }


                    if (choosedRate) {
                        return;
                    }

                    var $this = $(this),
                        $rated = $rateBubble.find('.rated'),
                        index = $this.index(),
                        currentRate = 5 - index;

                    if ($this.index() === $rated.index()) {
                        return; // don't do anything if user selects rating which is already selected
                    }

                    choosedRate = true;

                    setItemQuiz(currentRate);

                    self.rate(calculateFakeRate(startWidth, currentRate));

                    trackEvent('Rate', 'User rated a quiz with ' + currentRate + ' stars');

                    toggleStarClass($rated, currentRate);



                    initialRate = currentRate;
                    startWidth = self.rate();

                    setTimeout(function () {
                        $bubbleText.text($bubbleText.attr('data-step2'));
                        $rateBubble.addClass('closing');
                    }, 1000);
                    setTimeout(function () {
                        $rateBtn.removeClass('clicked');
                        $rateBubble.removeClass('closing');
                        rateMenuOpen = choosedRate = false;
                    }, 2000);
                });

                $('body').click(function () {
                    if (choosedRate) {
                        return;
                    }
                    $rateBtn.removeClass('clicked');
                    $rateBubble.removeClass('closing');
                    rateMenuOpen = choosedRate = false;
                });
            }


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
                answer.parentElement.classList.add('wrong');
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
                //$('[data-discussion]').off('click').click(function () {
                //    var that = this,
                //        $wrapper = $(that).parents('.commentWpr'),
                //        $comments = $wrapper.find('.quizComments'),
                //        isVisible = $comments.is(':visible');

                //    //$('.quizComments').slideUp({
                //    //   // duration: 500,
                //    //    complete: function () {
                //    //        $('.commentWpr').removeClass('show');
                //    //    }
                //    //});

                //    if (isVisible) {
                //        $('.quizComments').hide();
                //        $('.commentWpr').removeClass('show');
                //        return;
                //    }

                //    $('.quizComments').hide();
                //    $('.commentWpr').removeClass('show');


                //    $wrapper.removeClass('show');


                //    $comments.show();
                //    $wrapper.addClass('show');
                //});

                $(quizTQuestion).off('input').on('input', '.cTextArea', function (e) {
                    this.nextElementSibling.disabled = this.value.length === 0;
                }).off('focus').on('focus', '.cTextArea', function (e) {
                    this.nextElementSibling.style.display = 'block';
                }).off('blur').on('blur', '.cTextArea', function (e) {
                    this.nextElementSibling.style.display = 'none';
                });

                $('.askBtn').off('click').click(function () {
                    var that = this,
                        text = that.previousElementSibling.value,
                        question = $(that).parents('li')[0],
                        questionId = question.getAttribute('data-id'),
                        commentsElement = question.getElementsByClassName('quizComments')[0];


                    text = text.trim();

                    if (!text.length) {
                        cd.notification('cannot save empty comment');
                        return;
                    }

                    that.disabled = true;
                    dataContext.quizCreateDiscussion({
                        data: { questionId: questionId, text: text },
                        success: function (data) {
                            that.previousElementSibling.value = '';
                            $(that.previousElementSibling).height('');
                            commentsElement.lastElementChild.setAttribute('data-id', data);
                        },
                        error: function () {
                            that.disabeld = false;
                        }
                    });

                    var comment = {
                        id: '',
                        userId: cd.userDetail().nId,
                        userPicture: cd.userDetail().img,
                        userName: cd.userDetail().name,
                        text: text,
                        date: new Date()
                    },
                    html = cd.attachTemplateToData('quizCommentTemplate', comment);

                    commentsElement.insertAdjacentHTML('beforeend', html);

                    cd.parseTimeString(commentsElement.lastElementChild.querySelector('.createTime'));

                    var commentsLength = commentsElement.children.length;
                    setCommentsLength(question, commentsLength);

                });

                $(quizTQuestion).off('click').on('click', '.closeDialog', function () {

                    var comment = this.parentElement,
                        $comment = $(comment),
                        commentId = comment.getAttribute('data-id'),
                        commentsLength = comment.parentElement.children.length,
                        question = $comment.parents('li')[0];

                    dataContext.quizDeleteDiscussion({
                        data: { id: commentId }
                    });

                    $comment.remove();
                    setCommentsLength(question, commentsLength - 1);

                });


            }

            function appendComments(question, comments) {
                setCommentsLength(question, comments.length);

                var commentsHTML = '';
                for (var i = 0, l = comments.length; i < l; i++) {
                    commentsHTML += cd.attachTemplateToData('quizCommentTemplate', comments[i]);
                }


                var quizComments = question.getElementsByClassName('quizComments')[0];
                quizComments.insertAdjacentHTML('beforeend', commentsHTML);
                $(quizComments).find('[data-uid]').not('[data-uid="' + cd.userDetail().nId + '"]').each(function () {
                    $(this).parent().find('button').remove();
                });
            }
        }

        function clearQuiz() {
            quiz.classList.remove('checkQuiz');
            $('.commentWpr').removeClass('show');
            $(quizTQuestion).find('input').removeAttr('disabled').prop('checked', false);
            //$('.quizComments').hide();
            $(quizTQuestion).find('.userCorrect').removeClass('userCorrect');
            $(quizTQuestion).find('.noAnswer').removeClass('noAnswer');
            $(quizTQuestion).find('.wrong').removeClass('wrong');
            $(quizTQuestion).find('.userWrong').removeClass('userWrong');

            if (stopWatch) {
                stopWatch.reset();
                stopWatch = null;
            }            
        }

        function saveAnswers() {
            toggleTimer(false, false);

            var answerSheet = checkAnswers(),
                sendData = {
                    StartTime: stopWatch.startTime, 
                    EndTime: stopWatch.endTime,
                    QuizId: quizId,
                    Answers: answerSheet
                };




            if (!cd.register()) {
                cd.localStorageWrapper.setItem(quizId, JSON.stringify(sendData));
                cd.pubsub.publish('register', { action: true });
                return;

            }

            showScore();

            dataContext.quizSaveQuest({
                data: sendData,
                success: function () {
                    getComments();
                },
                error: function () { }
            });
        }

        function setCommentsLength(question, length) {
            var title = question.getElementsByClassName('qNumOfCmnts')[0];
            if (!length) {
                title.textContent = JsResources.AddComment;
                return;
            }
            title.textContent = length + ' ' + (length > 1 ? JsResources.Comments : JsResources.Comment);
        }


        //#region rate

        function getQuizRate() {
            dataContext.getQuizRate({
                data: { quizId: quizId},
                success: function (data) {
                    var rate = data.Rate >= 0 ? data.Rate : 0;
                    initialRate = rate;
                    $.data($rateContainer[0], 'fetchrate', false);
                    var $rated = $rateBubble.find('.rated');
                    toggleStarClass($rated, rate);
                }
            });
        }
        function setQuizRate(rate) {
            dataContext.rateQuiz({
                data: {
                    quizId: quizId,
                    rate: rate
                }
            });
        }
        //function registerRateEvents() {

        //}

        //#endregion

        function trackEvent(action, label) {
            analytics.trackEvent('Quiz', action, label);
        }

    }

})(jQuery, cd, cd.data, cd.pubsub, JsResources, cd.analytics);