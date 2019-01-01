﻿import { getAllAnswers, aproveAnswer } from './flaggedAnswersService'
import { deleteAnswer} from '../delete/deleteAnswerService'
export default {
    data() {
        return {
            answers: [],
            loading: true
        }
    },
    methods: {
        aproveA(answer, index) {
            aproveAnswer(answer.id).then(() => {
                this.answers.splice(index, 1);
                this.$toaster.success(`Answer Aproved`);
            }, () => {
                this.$toaster.error(`Answer Aproved Failed`);
            })
        },
        declineAnswer(answer, index) {
            let id = answer.id
            deleteAnswer([id]).then(() => {
                this.answers.splice(index, 1);
                this.$toaster.success(`Answer Declined`);
            }, err => {
                this.$toaster.error(`Answer Declined Failed`);
            })
        }
    },
    created() {
        getAllAnswers().then((answersResp) => {
            this.answers = answersResp;
            this.loading = false;
        })
    }
}