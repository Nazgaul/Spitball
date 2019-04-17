import { getAllQuesitons, acceptAnswer } from './markQuestionService'
import { deleteQuestion } from '../delete/deleteQuestionService'
import { deleteAnswer } from '../../../answer/answerComponents/delete/deleteAnswerService'

export default {
    data() {
        return {
            questions: [],
            page: 0,
            scrollLock: false,
            loading: true
        };
    },
    methods: {
        deleteQuestionByID(question) {
            let id = question.id;
            let numberArr = [];
            numberArr.push(id);
            deleteQuestion(numberArr)
                .then(() => {
                        this.$toaster.success(`Questions were deleted: ${id}`);
                        let questionIndex = this.questions.indexOf(question);
                        this.questions.splice(questionIndex, 1);
                    },
                    (error) => {
                        this.$toaster.error('Something went wrong');
                        console.log('component delete error', error);
                    }
                );
        },
        deleteAnswerByID(question, answer) {
            let answerId = answer.id;
            let idsArray = [];
            idsArray.push(answerId);
            deleteAnswer(idsArray)
                .then(() => {
                        this.$toaster.success(`Answer were deleted: ${answerId}`);
                        let answerIndex = question.answers.indexOf(answer);
                        question.answers.splice(answerIndex, 1);
                    },
                    (error) => {
                        this.$toaster.error('Something went wrong');
                        console.log('component delete error', error);
                    }
                );
        },


        openQuestion(url) {
            window.open(url, "_blank");
        },
        acceptQuestion(question, answer) {
            acceptAnswer(question.toServer(answer.id)).then(() => {
                //alert(`SUCCESS: question id: ${question.id} accepted answer id: ${answer.id}`)
                //remove the question from the list
                this.$toaster.success(`Question id: ${question.id} accepted answer id: ${answer.id}`);
                let questionIndex = this.questions.indexOf(question);
                this.questions.splice(questionIndex, 1);
            }, () => {
                this.$toaster.error(`ERROR FAILED TO ACCEPT question id: ${question.id} answer id: ${answer.id}`);
            });
        },
        advancePage() {
            this.page++;
        },
        getQuestions() {
            getAllQuesitons(this.page).then((questionsResponse) => {
                //this.questions = this.questions.concat(questionsResponse);
                questionsResponse.forEach(question => {
                    this.questions.push(question);
                });
                this.loading = false;
                if (questionsResponse && questionsResponse.length > 0) {
                    this.advancePage();
                }

                this.scrollLock = false;
                console.log('page', this.page);
            });
        },
        handleScroll(event) {
            let offset = 2000;
            if (event.target.scrollHeight - offset < event.target.scrollTop) {
                if (!this.scrollLock) {
                    this.scrollLock = true;
                    this.getQuestions();
                }
            }
        }
    }
    ,
    created() {
        this.getQuestions();
    }
    ,
    beforeMount() {
        this.$nextTick(function () {
            let containerElm = document.getElementById('question-wrapper-scroll');
            containerElm.addEventListener('scroll', this.handleScroll);
        });
    }
    ,
    beforeDestroy() {
        let containerElm = document.getElementById('question-wrapper-scroll');
        if (!containerElm) return;
        containerElm.removeEventListener('scroll', this.handleScroll);
    }
}