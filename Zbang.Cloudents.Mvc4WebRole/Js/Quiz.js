(function ($, cd, dataContext, pubsub, ZboxResources, analytics, Modernizr) {
    "use strict";

    if (window.scriptLoaded.isLoaded('quiz')) {
        return;
    }
    var eById = document.getElementById.bind(document),
        quizSideBar = eById('quizSideBar'),
        quizQuestionList = eById('quizQuestionList'),
        quizAddQuestion = eById('quizAddQuestion'),
        saveBtn = eById('saveQuiz');

    var consts = {
        emptyQuestion : 1000,
        validQuestion: 1001,
        errorQuestion : 1002,
    }

    function Quiz(data) {
        var that = this;
        that.name = data.name
        that.questions = data.questions;
    }

    function Question(data) {
        var that = this;
        that.text = data.questionText;
        that.answers = data.answers;
        that.correctAnswer = data.correctAnswer;
        that.element = data.element;

    }
    pubsub.subscribe('initQuiz', function () {

    });

    function showQuiz() {
        //show the quiz div

    }



    function registerEvents() {
        $(quizQuestionList).on('keyup', '.questionHolder', function (e) {
            var question = this;

            if (e.target.classList.contains('questionAnswer')) {
                enableNextOption(e.target);
                return;
            }


                
            function enableNextOption(answer) {
                var nextAnswer = answer.parentElement.nextElementSibling;

                if (nextAnswer && answer.value.length) {
                    nextAnswer.firstElementChild.disabled = nextAnswer.lastElementChild.disabled = false;
                }
            }


        });



        $(quizQuestionList).on('click', '.quizRemoveQuestion', function () {
            $(this).parent().remove();
        });

        $(quizAddQuestion).click(function () {
            quizQuestionList.insertAdjacentHTML('beforeend', eById('quizQuestionTemplate').innerHTML);
        });

        $(saveQuiz).click(function () {

            var quiz = parseQuiz(),
                quizQuestions = quiz.questions,
                errorElements = [];

            if (!quiz.name) {
                errorElements.push(quizSideBar.querySelector('.quizName'));
            }

            for (var i = 0, l = quizQuestions; i < l; i++) {
                var validCount = quizQuestions[i] + (answersArr.length > 1) + answerSelected;

            }

            function parseQuiz() {
                var questions = quizQuestionList.querySelectorAll('.questionHolder'),
                    questionsArr = [], quizName;

                quizName = quizSideBar.querySelector('.quizName').value;
            
                
                for (var i = 0, l = questions.length; i < l; i++) {

                    questionsArr.push(parseQuestion(questions[i]));

                    //switch (checkValidQuestion(questions[i])){
                    //    //case consts.validQuestion:
                    //    //    questions[i].style.border = '3px solid green';
                    //    //    break;
                    //    case consts.errorQuestion:
                    //        errorElements.push(questions[i]);
                    //        break;
                    //    //case consts.emptyQuestion:
                    //    //    questions[i].style.border = '3px solid blue';
                    //    //    break;
                    //    default:
                    //        console.log('hacking not allowed');
                    //}                        
                    //valid
                    //var validCount = isQuestionText + (answersArr.length > 1) + answerSelected;
                    //switch (validCount) {
                    //    case 0:
                    //        return consts.emptyQuestion;
                    //    case 3:
                    //        return consts.validQuestion;
                    //        break;
                    //    default:
                    //        return consts.errorQuestion;
                    //}

                }
                return new Quiz({name : quizName, questions: questionsArr});                
                
                function parseQuestion(question) {
                    var answersArr = [],
                        correctAnswer,
                        answers = question.querySelectorAll('.quizAnswer'),
                        questionText;
                    //check question text 
                    questionText = question.querySelector('.questionText').value;

                    //get question answers
                    for (var i = 0, l = answers.length; i < l; i++) {
                        var answer = answers[i].querySelector('.questionAnswer');
                        if (answer.value.length > 0) {
                            answersArr.push(answer.value);
                            if (answer.nextElementSibling.checked) {
                                correctAnswer = i;
                            }                                
                        }
                    }

                    return new Question({ text: questionText, answers: answersArr,correctAnswer : correctAnswer,element: question });
                }
            }

        });
    }
    registerEvents();

})(jQuery, window.cd, window.cd.data, cd.pubsub, window.ZboxResources, window.cd.analytics, Modernizr);