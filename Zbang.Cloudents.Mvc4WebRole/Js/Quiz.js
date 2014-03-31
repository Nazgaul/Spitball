(function ($, cd, dataContext, pubsub, ZboxResources, analytics, Modernizr) {
    "use strict";

    if (window.scriptLoaded.isLoaded('quiz')) {
        return;
    }
    var eById = document.getElementById.bind(document),
        quizSideBar = eById('quizSideBar'),
        quizQuestionList = eById('quizQuestionList'),
        quizAddQuestion = eById('quizAddQuestion'),
        quizPreview = eById('quizPreview'),
        saveBtn = eById('saveQuiz');

    var consts = {
        quizValid: 3,
        quizEmpty: 0
    }
    function Quiz(data) {
        var that = this;
        that.name = data.name
        that.questions = data.questions;
        that.errors = data.errors;
    }

    function Question(data) {
        var that = this;
        that.text = data.text || '';
        that.answers = data.answers;
        that.correctAnswer = data.correctAnswer;
    }

    function Answer(data) {
        var that = this;
        that.index = data.index;
        that.text = data.text;
    }
    pubsub.subscribe('initQuiz', function () {

    });

    function showQuiz() {
        //show the quiz div

    }

    function validateQuiz(quiz) {
        if (!quiz.name) {
            return false;
        }
        if (!quiz.questions.length) {
            return false;
        }

        if (quiz.errors) {
            return false;
        }

        return true;
    }

    function validateQuestion(question) {
        var count = consts.quizValid;//3 is valid
        if (!question.text.length) {
            count--;
        }
        if (question.answers.length < 2)  {
            count--;
        }
        if (question.correctAnswer === -1) {
            count--;
        }
        return count;
    }
    function parseQuiz() {
        var questions = quizQuestionList.querySelectorAll('.questionHolder'),
            questionsArr = [], errors = 0, quizName;

        quizName = quizSideBar.querySelector('.quizName').value;


        for (var i = 0, l = questions.length; i < l; i++) {
            var question = parseQuestion(questions[i]);
            if (question.obj) {
                questionsArr.push(question.obj);
            } else if (question.elm) {
                errors++;
            }
        }
        return new Quiz({ name: quizName, questions: questionsArr, errors: errors });

        function parseQuestion(question) {
            var answersArr = [],
                correctAnswer = -1,
                answers = question.querySelectorAll('.quizAnswer'),
                questionText;
            //check question text 
            questionText = question.querySelector('.questionText').value;

            //get question answers
            var aIndex = 1;
            for (var i = 0, l = answers.length; i < l; i++) {
                var answer = answers[i].querySelector('.questionAnswer');
                if (answer.value.length > 0) {
                    answersArr.push(new Answer({ index: aIndex, text: answer.value }));
                    if (answer.nextElementSibling.checked) {
                        correctAnswer = aIndex;
                    }
                    aIndex++;

                }
            }

            var questionObj = new Question({ text: questionText, answers: answersArr, correctAnswer: correctAnswer }),
                validCount = validateQuestion(questionObj);
            if (validCount === consts.quizValid) {
                return { obj: questionObj };
            } else if (validCount < consts.quizValid && validCount > consts.quizEmpty) {
                return { elm: question };//fix
            }

            return {};//fix
        }
    }

    function registerEvents() {
        $(quizQuestionList).on('keyup', '.questionAnswer', function (e) {
            var nextAnswer = this.parentElement.nextElementSibling,
                checkbox = this.nextElementSibling;

            if (nextAnswer && this.value.length) {   //enable next input
                nextAnswer.firstElementChild.disabled = false;
            }

            //enable/disable next radio button
            if ($(this.parentElement).index() < 2) {
                return;
            }

            checkbox.disabled = this.value.length === 0;

        });
        $(quizQuestionList).on('change', '.questionHolder', function (e) {
            if (e.target.type !== 'checkbox') {
                return;
            }
            var state = e.target.checked;
            $(this).find('input').prop('checked', false);
            e.target.checked = state;
        });

        $(quizQuestionList).on('click', '.quizRemoveQuestion', function () {
            $(this).parent().remove();
        });

        $(quizAddQuestion).click(function () {
            quizQuestionList.insertAdjacentHTML('beforeend', eById('quizQuestionTemplate').innerHTML);
        });

        $(saveQuiz).click(function () {

            var quiz = parseQuiz();

            if (validateQuiz(quiz)) {
                console.log('saved');
            } else {
                console.log('not saved');
            }

            console.log(quiz);


        });

        $(quizPreview).click(function () {
            var quiz = parseQuiz(),
                question,answer,
                previewObj = {}, questionObj = {};

            previewObj.name = quiz.name || 'Quiz name here';

            var questionsHTML = '';
            for (var i = 0, l = quiz.questions.length; i < l; i++) {
                question = quiz.questions[i];

                var answersHTML='';
                for (var j = 0, jL = question.answers.length; j < jL; j++) {
                    answer = question.answers[j];
                    answer.correct = '';
                    if (answer.index === question.correctAnswer) {
                        answer.correct = 'correct';
                    }
                    answersHTML += cd.attachTemplateToData('quizAnswerPreviewTemplate', answer);
                }

                questionObj.index = (i+1);
                questionObj.text = question.text;
                questionObj.answers = answersHTML;

                questionsHTML += cd.attachTemplateToData('quizQuestionPreviewTemplate', questionObj);
            }

            previewObj.questions = questionsHTML;

            var previewHTML = cd.attachTemplateToData('quizPreviewTemplate', previewObj);
            $('body').append(previewHTML);   

            
        });
    }
    registerEvents();

})(jQuery, window.cd, window.cd.data, cd.pubsub, window.ZboxResources, window.cd.analytics, Modernizr);