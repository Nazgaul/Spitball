import questionTextArea from "./../helpers/question-text-area/questionTextArea.vue";
import questionCard from "./../helpers/question-card/question-card.vue";
import questionService from '../../../services/questionService';
import miniChat from "./../../chat/private-chat/private-chat.vue";

export default {
    components: {questionTextArea, questionCard, miniChat},
    props: {id: {Number}},
    data() {
        return {
            textAreaValue: '',
            files: [],
            questionData:{}
        }
    },
    methods: {
        answer() {
            var self = this;
            questionService.answerQuestion(this.id, this.textAreaValue, this.files)
                .then(function () {
                    self.$router.push({path: '/note', query: {q: ''}});
                });
        }
    },
    computed: {
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },

        validForm() {
            return this.textAreaValue.length
        }
    },
    created() {
        var self = this;
        questionService.getQuestion(this.id)
            .then(function (response) {
                self.questionData = response.data

            });
    }
}