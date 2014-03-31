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
        that.index = data.index;
        that.text = data.text || '';
        that.answers = data.answers;
        that.correctAnswer = data.correctAnswer;
        that.element = data.element;
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

        if (quiz.errors.length) {
            return false;
        }

        return true;
    }

    function validateQuestion(question) {
        var count = consts.quizValid;//3 is valid
        if (!question.text.length) {
            count--;
        }
        if (question.answers.length < 2) {
            count--;
        }
        if (question.correctAnswer === -1) {
            count--;
        }
        return count;
    }
    function parseQuiz() {
        var questions = quizQuestionList.querySelectorAll('.questionHolder'),
            questionsArr = [], errors = [], quizName;

        quizName = quizSideBar.querySelector('.quizName').value;


        for (var i = 0, l = questions.length; i < l; i++) {
            var question = parseQuestion(questions[i]);
            if (question.obj) {
                questionsArr.push(question.obj);
            } else if (question.elm) {
                errors.push(question.elm)
            }
        }
        return new Quiz({ name: quizName, questions: questionsArr,errors: errors });

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
                    aIndex++;
                    if (answer.nextElementSibling.checked) {
                        correctAnswer = i;
                    }
                }
            }

            var questionObj = new Question({ text: questionText, answers: answersArr, correctAnswer: correctAnswer, element: question }),
                validCount = validateQuestion(questionObj);
            if (validCount === consts.quizValid) {
                return {obj: questionObj};
            } else if (validCount < consts.quizValid && validCount > consts.quizEmpty) {
                return { elm: question };
            }

            return {};
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
                previewObj = {};

            //previewObj.
            //for (var i = 0; i < length; i++) {

            //}
        });
    }
    registerEvents();

})(jQuery, window.cd, window.cd.data, cd.pubsub, window.ZboxResources, window.cd.analytics, Modernizr);