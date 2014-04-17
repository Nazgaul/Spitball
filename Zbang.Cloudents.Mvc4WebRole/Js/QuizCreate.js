(function ($, cd, dataContext, pubsub, ZboxResources, analytics, Modernizr) {
    "use strict";

    if (window.scriptLoaded.isLoaded('quiz')) {
        return;
    }
    var eById = document.getElementById.bind(document),
        quizSideBar, quizName, boxNameText, quizQuestionList,
        quizAddQuestion, quizPreview, mainDiv, closeQuiz,
        addQuiz, saveBtn, quitQuizDialog, quizClosePublish,
        quizCloseDraft, quizCloseDelete, quizWrapper,
        quizToggle, transitioning = false;

    assignDomElements();

    function assignDomElements() {
        quizSideBar = eById('quizSideBar'),
        mainDiv = eById('main');


        if (!quizSideBar) {
            return;
        }

        quizName = quizSideBar.getElementsByClassName('quizName')[0],
        boxNameText = quizSideBar.getElementsByClassName('quizLoc')[0],
        quizWrapper = quizSideBar.getElementsByClassName('quizWpr')[0],
        closeQuiz = eById('closeQuiz'),
        quizQuestionList = eById('quizQuestionList'),
        quizAddQuestion = eById('quizAddQuestion'),
        quizPreview = eById('quizPreview'),
        addQuiz = eById('addQuiz'),
        saveBtn = eById('saveQuiz'),
        quitQuizDialog = eById('quitQuizDialog'),
        quizClosePublish = eById('quizClosePublish'),
        quizCloseDraft = eById('quizCloseDraft'),
        quizCloseDelete = eById('quizCloseDelete');
        quizToggle = eById('quizToggle');

    }

    var consts = {
        initQuestionsLength: 3,
        emptyQuestion: 3,
        validQuestion: 0,
        minAnswers: 2,
        keyTab: 9
    }

    var quizId, boxId, boxName, firstTime = true;

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
        that.correctAnswer = (data.correctAnswer >= consts.validQuestion || data.correctAnswer <= consts.emptyQuestion) ? data.correctAnswer : -1;
    }

    function Answer(data) {
        var that = this;
        that.text = data.text || '';
    }
    pubsub.subscribe('initQuiz', function (data) {

        if (quizId && (quizId === data.quizId)) {
            return;
        }

        if (transitioning) {
            return;
        }


        boxId = data.boxId;
        boxName = data.boxName;
        initQuiz(data.quizId);


    });



    function initQuiz(quizId) {

        if (quizSideBar) {
            var rect = quizSideBar.getBoundingClientRect();
            if (rect.left >= 0 && rect.right <= $(window).width()) {
                clearQuiz();
                $(quizSideBar).one('oTransitionEnd msTransitionEnd transitionend', showState);
                return;
            }

            showState();
            return;
        }


        dataContext.quizHTML({
            success: function (data) {
                mainDiv.insertAdjacentHTML('beforeend', data);
                assignDomElements();


                if (!quizId) {
                    appendEmptyState();
                    showQuiz();
                    registerEvents();

                } else {//edit draft                        
                    editDraft();
                    return;
                }
            }
        });

        function showState() {
            if (!quizId) {
                appendEmptyState();
                showQuiz();
                registerEvents();
                return;
            }

            editDraft();
        }

        function editDraft() {
            dataContext.quizData({
                data: { quizId: quizId },
                success: function (data) {
                    data = data || {};
                    populateQuiz(data);
                    showQuiz();
                    registerEvents();
                }
            });
        }

        function appendEmptyState() {
            for (var i = 0; i < consts.initQuestionsLength; i++) {
                appendQuestion();
            }
        }


    }

    function showQuiz() {

        boxNameText.textContent = boxName;

        //show the quiz div
        addQuiz.disabled = true;
        quizToggle.checked = false;
        setTimeout(function () {
            mainDiv.classList.remove('noQuiz');
            transitioning = true;
            $(quizSideBar).one('oTransitionEnd msTransitionEnd transitionend', function () {
                transitioning = false;
            });
        }, 0);
        //did that to kick in the elastic script
        $('.quizWpr').find('textarea').each(function () {
            $(this).focus();
        });
        quizName.focus();
    }

    function validateQuiz() {

        //reset
        quizSideBar.classList.remove('nameReq');
        quizSideBar.classList.remove('error');

        //quiz name
        var quizNameText = quizName.value;

        if (!quizNameText) {
            quizSideBar.classList.add('nameReq');
        }

        //question

        var qState = getQuestionsState();

        if (qState.length === qState.empty) {
            $(quizQuestionList.firstElementChild).addClass('correctReq questionReq answerReq');
        }

        if (!quizNameText || qState.invalid > 0 || qState.valid === 0) {
            quizSideBar.classList.add('error');
            return false;
        }

        return true;
    }

    function validateQuestion(question, noChangeErrorState) {
        var $question = $(question), error = false;

        if (!noChangeErrorState) { //no need to change error state for preview
            $question.removeClass('correctReq questionReq answerReq');
        }

        var errorClass = '', count = 0;
        //correct answer
        var correctAnswer = question.querySelector('input:checked');
        if (!correctAnswer) {
            error = true;
            errorClass += 'correctReq ';
            count++;
        }

        //question text 
        var text = question.querySelector('.questionText').value;
        if (!text) {
            error = true;
            errorClass += 'questionReq ';
            count++;
        }

        //answers 
        var answers = $question.find('.questionAnswer'),
            validAnswers = answers.filter(function () { return this.value.length > 0; });

        answers.removeClass('emptyText');


        if (validAnswers.length < 2) {
            var answer, found = 0;
            for (var i = 0; i < 2 && found < 2; i++) {
                answer = answers[i];
                if (!answer.value) {
                    found++;
                    answer.classList.add('emptyText');
                }
            }

            error = true;
            errorClass += 'answerReq';


            count++;


            if (count === consts.emptyQuestion && validAnswers.length === 1) {
                count--;

            }

            if (count === consts.emptyQuestion && $question.index() > 0) {
                answers.removeClass('emptyText');
            }
        }




        if (count > consts.validQuestion && count < consts.emptyQuestion && !noChangeErrorState) {
            $question.addClass(errorClass);
        }


        return count;
    }

    function populateQuiz(data) {
        quizSideBar.setAttribute('data-id', data.id);
        quizId = data.id;
        quizName.value = data.name || '';

        for (var i = 0, l = data.questions.length; i < l; i++) {
            appendQuestion(null, data.questions[i]);
        }
    }

    function parseQuiz() {
        var questionsArr = [], quizNameText;

        //reset
        quizSideBar.classList.remove('nameReq');
        quizSideBar.classList.remove('error');

        quizNameText = quizSideBar.querySelector('.quizName').value;

        if (!quizNameText) {
            quizSideBar.classList.add('nameReq');
        }

        var qState = getQuestionsState();

        if (qState.length === qState.empty) {
            $(quizQuestionList.firstElementChild).addClass('correctReq questionReq answerReq');
        }



        if (!quizNameText || qState.invalid > 0 || qState.valid === 0) {
            quizSideBar.classList.add('error');
            return false;
        }

        for (var i = 0, l = qState.vQuestions.length; i < l; i++) {
            var question = qState.vQuestions[i];
            if (validateQuestion(question) === 0) {
                questionsArr.push(parseQuestion(question));

            }

        }

        return new Quiz({ name: quizNameText, questions: questionsArr });

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

            return questionObj;
        }


    }

    function getQuestionsState() {
        var questions = quizQuestionList.querySelectorAll('.questionHolder'),
            question, validQuestions = 0, emptyQuestions = 0, invalidQuestions = 0,
            vQuestions = [];

        for (var i = 0, l = questions.length; i < l; i++) {
            question = questions[i];
            switch (validateQuestion(question)) {
                case consts.validQuestion:
                    validQuestions++;
                    vQuestions.push(question);
                    break;
                case consts.emptyQuestion:
                    emptyQuestions++;
                    break;
                default:
                    invalidQuestions++;
                    break;

            }
        }

        return {
            valid: validQuestions, empty: emptyQuestions,
            invalid: invalidQuestions, length: questions.length,
            vQuestions: vQuestions
        };
    }

    function registerEvents() {
        if (!firstTime) {
            return;
        }

        firstTime = false;

        $(quizQuestionList).on('click', '.quizRemoveQuestion', removeQuestion)
                           .on('focusout', '.questionText', function () { saveQuestion(this); })
                            .on('keyup', '.questionText', checkQuestion)
                           .on('change', '.correctAnswer', saveAnswer)
                           .on('keyup', '.questionAnswer', toggleAnswerRadioBtn)
                           .on('focusout', '.questionAnswer', saveAnswer)
                           .on('click', '.questionAnswer[readonly="readonly"]', addAnswer)
                           .on('focusin', '.questionAnswer[readonly="readonly"]', addAnswer);

        $(quizAddQuestion).click(appendQuestion);

        $(quizName).focusout(function () {
            setTimeout(function () {
                saveQuiz();
            }, 100);

        });
        ////.keyup(function (e) {
        ////    var keyCode = e.keyCode || e.which;
        ////    if (keyCode === consts.keyTab) {
        ////        saveQuiz();
        ////    }
        //});

        $(saveBtn).click(publishQuiz);

        $(window).on('beforeunload', function () {
            return 'Quiz changes might be lost';
        });

        $(closeQuiz).click(showClosePopup);

        $(quizPreview).click(previewQuiz);

        pubsub.subscribe('deleteQuiz', function (id) {
            if (quizId === id) {
                clearQuiz();
            }
        });

    }

    //#region Quiz

    function saveQuiz() {
        if (!quizId) {
            dataContext.quizCreate({
                data: { boxId: boxId, name: quizName.value },
                success: function (data) {
                    quizSideBar.setAttribute('data-id', data);
                    quizId = data;
                    addItemToBox(false);
                }
            });
            return;
        }

        dataContext.quizUpdate({
            data: { id: quizId, name: quizName.value },
            success: function () {
                addItemToBox(false);
            },
            error: function () { }
        });
    }

    function publishQuiz() {
        if (!validateQuiz()) {
            return;
        }


        saveBtn.disabled = true;

        dataContext.quizPublish({
            data: { quizId: quizId, boxId: boxId, universityName: cd.getParameterFromUrl(1), boxName: boxName, quizName: quizName.value },
            success: function (data) {
                addItemToBox(true, data);
                clearQuiz();

            },
            always: function () {
                saveBtn.disabled = false;
            }
        });
    }

    function previewQuiz() {
        var quiz = parseQuiz(),
              question, answer,
              previewObj = {}, questionObj = {};

        if (!quiz.questions.length) {
            return;
        }

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
        setTimeout(function () { //fix for animation
            mainDiv.classList.add('previewQuiz');
        }, 0)


        $('#preview').find('.closePreview').one('click', function () {
            mainDiv.classList.remove('previewQuiz');
            setTimeout(function () {//fix for animation
                $('#preview').remove();
            }, 1000);

        });
    }

    function deleteQuiz() {

        cd.pubsub.publish('removeItem', quizId);

        dataContext.quizDelete({
            data: { id: quizId }
        });

        clearQuiz();

    }



    //#endregion Quiz

    //#region Question
    function appendQuestion(e, question) {
        var indexObj = { index: quizQuestionList.children.length + 1 },
        html = cd.attachTemplateToData('quizQuestionTemplate', indexObj);
        quizQuestionList.insertAdjacentHTML('beforeend', html);


        var answersList = quizQuestionList.lastElementChild.getElementsByClassName('quizAnswersList')[0];

        if (!question) {
            for (var i = 0; i < consts.minAnswers; i++) {
                var aIndexObj = { index: (i + 1), topIndex: indexObj.index },
                html = cd.attachTemplateToData('quizAnswerTemplate', aIndexObj);
                answersList.lastElementChild.insertAdjacentHTML('beforebegin', html);
            }

            $(quizWrapper).scrollTop(quizWrapper.scrollHeight - $(quizWrapper).height());
            return;
        }

        quizQuestionList.lastElementChild.setAttribute('data-id', question.id);
        $(quizQuestionList.lastElementChild).find('.questionText')[0].value = question.text;

        for (var i = 0, l = question.answers.length; i < l; i++) {
            var aIndexObj = { index: (i + 1), topIndex: indexObj.index },
            html = cd.attachTemplateToData('quizAnswerTemplate', aIndexObj);
            answersList.lastElementChild.insertAdjacentHTML('beforebegin', html);



            answersList.lastElementChild.previousElementSibling.setAttribute('data-id', question.answers[i].id);
            answersList.lastElementChild.previousElementSibling.firstElementChild.value = question.answers[i].text;
        }

        if (question.correctAnswer) {
            $(answersList).find('[data-id="' + question.correctAnswer + '"]').find('.correctAnswer')[0].checked = true;
        }

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
            success: function (data) {
                questionHolder.setAttribute('data-id', data);
                if (callback) {
                    callback(data);
                }
            },
            error: function () { },
            always: function () {
                quizAddQuestion.disabled = false;
            }
        });
    }

    function checkQuestion(e) {
        var question = this.parentElement.parentElement,
            questionId = question.getAttribute('data-id'),
            answers = question.querySelectorAll('.quizAnswer'),
            value, valueFound = false;
        for (var i = 0, l = answers.length; i < l && !valueFound; i++) {
            value = answers[i].firstElementChild.value;
            if (value) {
                valueFound = true;
            }
        }

        if (valueFound || this.value.length) {
            return;
        }

        if (questionId) {
            question.removeAttribute('data-id');
            dataContext.quizQDelete({
                data: { id: questionId },
                error: function () { }
            });
        }
    }

    function removeQuestion(e) {
        e.preventDefault();
        var question = this.parentElement.parentElement,
            questionId = question.getAttribute('data-id');

        if (questionId) {
            question.removeAttribute('data-id');

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

    function updateQuestion(id, text) {
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
            removeAnswer(quizAnswer);
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
        appendAnswer(this);
    }
    function appendAnswer(answerInput) {
        var answersList = answerInput.parentElement.parentElement,
        answersLength = answersList.children.length,
        answer = answerInput.parentElement,
        indexObj = { index: answersLength, topIndex: $(answersList.parentElement.parentElement).index() + 1 },
        html = cd.attachTemplateToData('quizAnswerTemplate', indexObj);

        answer.insertAdjacentHTML('beforebegin', html);

        answersList.lastElementChild.previousElementSibling.querySelector('input').disabled = true;

        var input = answerInput.parentElement.previousElementSibling.firstElementChild;
        $(input).focus();

        $(quizWrapper).scrollTop($(input).offset.top);
    }

    function saveAnswer(e) {
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
                function (qId) {
                    questionId = qId;
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
                data: { id: answerId, text: answerText, correctAnswer: isCorrect }
            });
        }
    }

    function removeAnswer(answer) {
        var answerId = answer.getAttribute('data-id')

        if (!answerId) {
            return;
        }

        answer.removeAttribute('data-id');

        dataContext.quizADelete({
            data: { id: answerId },
            error: function () { }
        });
    }

    //#endregion Answer

    //*#region close popup
    function showClosePopup() {
        $(quitQuizDialog).show();

        $(quizClosePublish).off('click').one('click', function () {
            $(quitQuizDialog).hide();
            publishQuiz();
        });
        $(quizCloseDraft).off('click').one('click', function () {
            $(quitQuizDialog).hide();
            clearQuiz();
        });

        $(quizCloseDelete).off('click').one('click', function () {
            $(quitQuizDialog).hide();
            deleteQuiz();
        });

        $(quitQuizDialog).off('click').one('click', '.closeDialog', function () {
            $(quitQuizDialog).hide();
        });

    }

    //*endregion

    function addItemToBox(isPublish, url) {
        var quiz = {
            id: quizId,
            boxid: boxId,
            name: quizName.value,
            publish: isPublish,
            description: isPublish || getContent(),
            rate: 0,
            ownerId: cd.userDetail().nId,
            owner: cd.userDetail().name,
            userUrl: cd.userDetail().url,
            type: 'quiz',
            url: url,
            date: new Date()
        }

        pubsub.publish('addItem', quiz);
        function getContent() {
            var result = '',
                questions = quizSideBar.querySelectorAll('.questionText');

            for (var i = 0, l = questions.length; i < l; i++) {
                result += questions[i].value;
            }

            return result;
        }
    }


    function clearQuiz() {
        mainDiv.classList.add('noQuiz');
        transitioning = true;
        $(quizSideBar).one('oTransitionEnd msTransitionEnd transitionend', function () {
            transitioning = false;
        });
        addQuiz.disabled = false;
        $(window).off('beforeunload');
        quizQuestionList.innerHTML = '';
        quizName.value = '';
        quizId = null;
        quizSideBar.removeAttribute('data-id');

    }
})(jQuery, window.cd, window.cd.data, cd.pubsub, window.ZboxResources, window.cd.analytics, Modernizr);