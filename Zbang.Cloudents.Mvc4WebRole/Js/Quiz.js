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
        saveBtn = eById('saveQuiz');

    var consts = {
        initQuestionsLength : 3
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
        calls.push(dataContext.quizCreate({
            data: { boxId: boxId },
            success: function (data) {
                quizSideBar.setAttribute('data-id', data);
            }
        }));
        for (var i = 0; i < consts.initQuestionsLength; i++) {
            calls.push(addQuestion());

        }
  
        $.when.apply(null, calls).done(function (a, b, c, d) {
            quizSideBar.setAttribute('data-id', a[0].payload);
            quizQuestionList.children[0].setAttribute('data-id', b[0].payload);
            quizQuestionList.children[1].setAttribute('data-id', c[0].payload);
            quizQuestionList.children[2].setAttribute('data-id', d[0].payload);            
            quizAddQuestion.disabled = false;
        })
    }

    function showQuiz() {
        //show the quiz div
        eById('main').classList.remove('noQuiz');
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
                    question.querySelector('.emptyQuestion').classList.add('error');
                    isValid = false;
                }

                if (questionObj.correctAnswer === -1) {
                    question.querySelector('.correctAnswerText').classList.add('error');
                    isValid = false;
                }

                var answers = question.getElementsByClassName('questionAnswer');

                if (questionObj.answers.length === 0) {

                    if (!answers[0].value.length) {
                        answers[0].classList.add('error');
                    }
                    if (!answers[1].value.length) {
                        answers[1].classList.add('error');
                    }
                    isValid = false;
                }

                if (questionObj.answers.length === 1) {
                    if (!answers[0].value.length) {
                        answers[0].classList.add('error');
                    }
                    else if (!answers[1].value.length) {
                        answers[1].classList.add('error');
                    }
                    isValid = false;
                }

                return isValid;

            }
        }


    }

    function registerEvents() {
        $(quizQuestionList).on('click', '.quizRemoveQuestion', removeQuestion)
                           .on('focusout', '.questionText', updateQuestion)
                           .on('change','.correctAnswer',saveAnswer)
                           .on('keyup', '.questionAnswer', toggleAnswerRadioBtn)
                           .on('focusout', '.questionAnswer', saveAnswer)
                           .on('click', '.questionAnswer[readonly="readonly"]', addAnswer);

        $(quizAddQuestion).click(addQuestion);

        $(quizName).focusout(updateQuiz);

        $(window).on('beforeunload', function () {
            return 'Quiz changes might be lost';
        });


        //$(saveQuiz).click(saveQuiz);          
        //$(quizPreview).click(function () {
        //    var quiz = parseQuiz(),
        //        question, answer,
        //        previewObj = {}, questionObj = {};

        //    previewObj.name = quiz.name || 'Quiz name here';

        //    var questionsHTML = '';
        //    for (var i = 0, l = quiz.questions.length; i < l; i++) {
        //        question = quiz.questions[i];

        //        var answersHTML = '';
        //        for (var j = 0, jL = question.answers.length; j < jL; j++) {
        //            answer = question.answers[j];
        //            answer.correct = '';
        //            if (answer.index === question.correctAnswer) {
        //                answer.correct = 'correct';
        //            }
        //            answersHTML += cd.attachTemplateToData('quizAnswerPreviewTemplate', answer);
        //        }

        //        questionObj.index = (i + 1);
        //        questionObj.text = question.text;
        //        questionObj.answers = answersHTML;

        //        questionsHTML += cd.attachTemplateToData('quizQuestionPreviewTemplate', questionObj);
        //    }

        //    previewObj.questions = questionsHTML;

        //    var previewHTML = cd.attachTemplateToData('quizPreviewTemplate', previewObj);
        //    $('body').append(previewHTML);




        //});


    }

    //#region Quiz

    function updateQuiz() {
        var quizId = quizSideBar.getAttribute('data-id');
                
        dataContext.quizUpdate({
            data: { id: quizId, text: quizName.value }
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

    }



    //#endregion Quiz

    //#region Question
    function addQuestion() {        
        var indexObj = { index: quizQuestionList.children.length + 1 },
        html = cd.attachTemplateToData('quizQuestionTemplate', indexObj);
        quizQuestionList.insertAdjacentHTML('beforeend', html);

        return dataContext.quizQCreate({
            data: { quizId: quizId},
            success: function (data) {
                quizQuestionList.lastElementChild.setAttribute('data-id', data);

            },
            error: function () { }
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
    
    function updateQuestion() {
        var question = this,
            questionId = question.getAttribute('data-id'),
            questionText = question.value;

        dataContext.quizQUpdate({
            data: { id: questionId, text: questionText }
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
        var answerInput, isCorrect;
        if (this.type === 'textarea') {
            answerInput = this;
            isCorrect= answerInput.nextElementSibling.checked;
        } else {
            answerInput = this.previousElementSibling.value;
            isCorrect = this.checked;
        }
        var answer = this.parentElement,
            answerId = answer.getAttribute('data-id'),            
            answerText = answerInput.value,
            
            questionId = $(answer).parents('.questionHolder')[0].getAttribute('data-id');

        if (!answerText) {          
            return;
        }

        if (!answerId) {
            dataContext.quizACreate({
                data: { quizId: questionId, text: answerText, correctAnswer: isCorrect},
                success: function (data) {
                    answer.setAttribute('data-id', data);
                },
                error: function () { }
            });
            return;
        }

        dataContext.quizAUpdate({
            data: { id: answerId, text: answerText }
        });
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