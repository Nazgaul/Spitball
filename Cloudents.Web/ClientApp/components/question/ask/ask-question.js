import questionTextArea from "./../helpers/question-text-area/questionTextArea.vue";
import questionService from '../../../services/questionService';

export default {
    components: {questionTextArea},
    data() {
        return {
            subjectList: [],
            subject: '',
            textAreaValue: '',
            price: 0.5
        }
    },
    methods: {
        ask() {
            var self = this;
            questionService.postQuestion(this.subject.id, this.textAreaValue, this.price)
                .then(function () {
                    self.$router.push({path: '/note', query: {q: ''}});
                });
        },
    },
    computed: {
        validForm() {
            return this.subject && this.textAreaValue.length && this.price >= 0.5;
        }
    },
    created() {
        var self = this;
        questionService.getSubjects().then(function (response) {
            self.subjectList = response.data
        })
    }
}