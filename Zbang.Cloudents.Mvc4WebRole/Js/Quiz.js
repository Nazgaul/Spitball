(function ($, cd, dataContext, pubsub, ZboxResources, analytics, Modernizr) {
    "use strict";

    if (window.scriptLoaded.isLoaded('quiz')) {
        return;
    }
    var eById = document.getElementById.bind(document),
        quizSideBar = eById('quizSideBar'),
        quizName = quizSideBar.getElementsByClassName('quizName')[0],
        quizQuestionList = eById('quizQuestionList'),
        quizAddQuestion = eById('quizAddQuestion'),
        quizPreview = eById('quizPreview'),
        mainDiv = eById('main'),
        saveBtn = eById('saveQuiz');

    var consts = {
        initQuestionsLength: 3
    }

    var quizId, boxId, boxName;

    function Quiz(data) {
        var that = this;
        that.name = data.name || '';
        that.questions = data.questions;
        that.errors = data.errors;
    }

    function Question(data) {
        var that = this;
        that.text = data.text || '';
        that.answers = data.answers;
        that.correctAnswer = (data.correctAnswer >= 0 || data.correctAnswer <= 3) ? data.correctAnswer : -1;
    }

    function Answer(data) {
        var that = this;
        that.text = data.text || '';
    }
    pubsub.subscribe('initQuiz', function (data) {
        boxId = data.boxId;
        boxName = data.boxName;

        initQuiz();
        showQuiz();
        registerEvents();

    });

    function initQuiz() {
        var calls = [];
        dataContext.quizCreate({
            data: { boxId: boxId },
            success: function (data) {
                quizSideBar.setAttribute('data-id', data);
                quizId = data;
            }
        });

        for (var i = 0; i < consts.initQuestionsLength; i++) {
            appendQuestion();

        }
    }

    function showQuiz() {
        //show the quiz div
        mainDiv.classList.remove('noQuiz');
        quizName.focus();
    }

    function validateQuiz(quiz) {
        if (!quiz.name) {
            return false;
        }
        if (!quiz.questions.length) {
            return false;
        }

        if (quizSideBar.querySelector('.error')) {
            return false;
        }

        return true;
    }


    function parseQuiz() {
        var questions = quizQuestionList.querySelectorAll('.questionHolder'),
            questionsArr = [], errors = 0, quizName;

        var quizNameElement = quizSideBar.querySelector('.quizName');
        quizName = quizNameElement.value;

        if (!quizName.length) {
            quizNameElement.classList.add('error');
        }



        for (var i = 0, l = questions.length; i < l; i++) {
            var question = parseQuestion(questions[i]);
            if (question) {
                questionsArr.push(question);
            }

        }

        return new Quiz({ name: quizName, questions: questionsArr, errors: errors });

        function parseQuestion(question) {

            //check question text 
            var questionText = question.querySelector('.questionText').value;

            //get question answers
            var answersArr = [], correctAnswer,
                answers = question.querySelectorAll('.quizAnswer');
            for (var i = 0, l = answers.length; i < l; i++) {
                var answer = answers[i].querySelector('.questionAnswer');
                if (answer.value.length > 0) {
                    answersArr.push(new Answer({ text: answer.value }));
                    if (answer.nextElementSibling.checked) {
                        correctAnswer = i;
                    }
                }
            }

            var questionObj = new Question({ text: questionText, answers: answersArr, correctAnswer: correctAnswer });

            if (!validateQuestion()) {
                return false;
            }

            return questionObj;

            function validateQuestion() {
                var isValid = true;


                //if (questionObj.text.length === 0 && questionObj.correctAnswer === -1 && questionObj.answers.length < 2) {
                //    return false;
                //}

                if (!questionObj.text.length) {
                    //question.querySelector('.emptyQuestion').classList.add('error');
                    isValid = false;
                }

                if (questionObj.correctAnswer === -1) {
                    //question.querySelector('.correctAnswerText').classList.add('error');
                    isValid = false;
                }

                var answers = question.getElementsByClassName('questionAnswer');

                if (questionObj.answers.length === 0) {

                    if (!answers[0].value.length) {
                        //answers[0].classList.add('error');
                    }
                    if (!answers[1].value.length) {
                        //answers[1].classList.add('error');
                    }
                    isValid = false;
                }

                if (questionObj.answers.length === 1) {
                    if (!answers[0].value.length) {
                        //answers[0].classList.add('error');
                    }
                    else if (!answers[1].value.length) {
                        //answers[1].classList.add('error');
                    }
                    isValid = false;
                }

                return isValid;

            }
        }


    }

    function registerEvents() {
        $(quizQuestionList).on('click', '.quizRemoveQuestion', removeQuestion)
                           .on('focusout', '.questionText', function () { saveQuestion(this); })
                           .on('change', '.correctAnswer', saveAnswer)
                           .on('keyup', '.questionAnswer', toggleAnswerRadioBtn)
                           .on('focusout', '.questionAnswer', saveAnswer)
                           .on('click', '.questionAnswer[readonly="readonly"]', addAnswer);

        $(quizAddQuestion).click(appendQuestion);

        $(quizName).focusout(updateQuiz);

        $(window).on('beforeunload', function () {
            return 'Quiz changes might be lost';
        });


        //$(saveQuiz).click(saveQuiz);          
        $(quizPreview).click(previewQuiz);


    }

    //#region Quiz

    function updateQuiz() {
        var quizId = quizSideBar.getAttribute('data-id');

        dataContext.quizUpdate({
            data: { id: quizId, text: quizName.value },
            error: function () { }
        });
    }

    function saveQuiz() {
        //var quiz = parseQuiz();

        //if (validateQuiz(quiz)) {
        //    console.log('saved');
        //} else {
        //    console.log('not saved');
        //}

        //console.log(quiz);
    }

    function previewQuiz() {
        var quiz = parseQuiz(),
              question, answer,
              previewObj = {}, questionObj = {};

        previewObj.name = quiz.name || 'Quiz name here';

        var questionsHTML = '';
        for (var i = 0, l = quiz.questions.length; i < l; i++) {
            question = quiz.questions[i];

            var answersHTML = '';
            for (var j = 0, jL = question.answers.length; j < jL; j++) {
                answer = question.answers[j];
                answer.correct = '';
                if (j === question.correctAnswer) {
                    answer.correct = 'class="correct"';
                }
                answersHTML += cd.attachTemplateToData('quizAnswerPreviewTemplate', answer);
            }

            questionObj.text = question.text;
            questionObj.answers = answersHTML;

            questionsHTML += cd.attachTemplateToData('quizQuestionPreviewTemplate', questionObj);
        }

        previewObj.questions = questionsHTML;
        previewObj.numOfQuestions = quiz.questions.length;

        var previewHTML = cd.attachTemplateToData('quizPreviewTemplate', previewObj);
        $('body').append(previewHTML);
        mainDiv.classList.add('previewQuiz');
        $('#preview').find('.closeDialog').one('click', function () {
            mainDiv.classList.remove('previewQuiz');
        });
    }



    //#endregion Quiz

    //#region Question
    function appendQuestion() {
        var indexObj = { index: quizQuestionList.children.length + 1 },
        html = cd.attachTemplateToData('quizQuestionTemplate', indexObj);
        quizQuestionList.insertAdjacentHTML('beforeend', html);
    }
    function saveQuestion(question, callback) {
        var questionHolder = $(question).parents('.questionHolder')[0],
            questionId = questionHolder.getAttribute('data-id'),
            questionText = question.value;

        if (questionId) {
            updateQuestion(questionId, questionText);
            return;
        }

        quizAddQuestion.disabled = true;
        dataContext.quizQCreate({
            data: { quizId: quizId, text: questionText },
            success: function(data){
                questionHolder.setAttribute('data-id', data);
                if (callback) {
                    callback();
                }
            }, 
            error: function () { },
            always: function () {
                quizAddQuestion.disabled = false;
            }
        });
    }

    function removeQuestion(e) {
        e.preventDefault();
        var question = this.parentElement.parentElement,
            questionId = question.getAttribute('data-id');

        if (questionId) {
            dataContext.quizQDelete({
                data: { id: questionId },
                error: function () { }
            });
        }

        quizQuestionList.removeChild(question);

        var questions = quizQuestionList.querySelectorAll('.questionText'),
            initString = "Question #";
        for (var i = 0, l = questions.length; i < l; i++) {
            question = questions[i];

            if (!Modernizr.input.placeholder) {
                if (question.value === question.placeholder) {
                    question.placeholder = initString + (i + 1);
                }
                continue;
            }
            if (!question.value) {
                question.placeholder = initString + (i + 1);
            }

        }
    }

    function updateQuestion(id,text) {            
        dataContext.quizQUpdate({
            data: { id: id, text: text }
        });
    }

    //#endregion Question

    //#region Answer
    function toggleAnswerRadioBtn(e) {
        var answersList = e.delegateTarget.querySelector('.quizAnswersList'),
                quizAnswer = this.parentElement;

        if (!this.value.length) {
            removeAnswer(quizAnswer.getAttribute('data-id'));
        }

        //if it's the first / second child element
        if (answersList.firstElementChild === quizAnswer || answersList.firstElementChild.nextElementSibling === quizAnswer) {
            return;
        }

        var radioBtn = this.nextElementSibling;
        if (this.value.length > 0) {
            radioBtn.disabled = false;
        } else {
            radioBtn.disabled = true;
            radioBtn.checked = false;

        }
    }

    function addAnswer(e) {
        e.preventDefault();

        var answerInput = this,
            answersList = answerInput.parentElement.parentElement,
            answersLength = answersList.children.length,
            answer = answerInput.parentElement,
            indexObj = { index: answersLength, topIndex: $(answersList.parentElement.parentElement).index() + 1 },
            html = cd.attachTemplateToData('quizAnswerTemplate', indexObj);

        answer.insertAdjacentHTML('beforebegin', html);

        $(answerInput.parentElement.previousElementSibling.firstElementChild).focus();
    }

    function saveAnswer() {
        var answerInput, isCorrect, answerText,
            answer = this.parentElement,
            question = $(answer).parents('.questionHolder')[0],
            questionId = question.getAttribute('data-id'),
            answerId = answer.getAttribute('data-id');

        if (this.type === 'textarea') { //check if user focusout the answer or clicked the radio button
            answerInput = this;
            isCorrect = answerInput.nextElementSibling.checked;
        } else {
            answerInput = this.previousElementSibling;
            isCorrect = this.checked;
        }
        answerText = answerInput.value;

        if (!answerText) {
            return;
        }

        if (!questionId) {
            saveQuestion(question.querySelector('.questionText'),
                function () {                    
                    save();
                });
            return;
        }

        if (!answerId) {
            save();
            return;
        }

        update();

        function save() {
            dataContext.quizACreate({
                data: { questionId: questionId, text: answerText, correctAnswer: isCorrect },
                success: function (data) {
                    answer.setAttribute('data-id', data);
                },
                error: function () { }
            });
        }
        function update() {
            dataContext.quizAUpdate({
                data: { id: answerId, text: answerText , correctAnswer: isCorrect }
            });
        }
    }

    function removeAnswer(answerId) {
        if (!answerId) {
            return;
        }

        dataContext.quizADelete({
            data: { id: answerId },
            error: function () { }
        });
    }
    //#endregion Answer
})(jQuery, window.cd, window.cd.data, cd.pubsub, window.ZboxResources, window.cd.analytics, Modernizr);