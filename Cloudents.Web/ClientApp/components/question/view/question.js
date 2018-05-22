import questionService from '../../../services/questionService';
import miniChat from "./../../chat/private-chat/private-chat.vue";
import questionThread from "./questionThread.vue";

export default {
    components: {miniChat, questionThread},
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
    },
    created() {
        var self = this;
        questionService.getQuestion(this.id)
            .then(function (response) {
                self.questionData = response.data

            });
    }
}