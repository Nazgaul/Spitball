import questionTextArea from "./../helpers/questionTextArea.vue";
import questionService from '../../../services/questionService';

export default {
    components: {questionTextArea},
    data() {
        return {
            subjectList: [],
            subject: '',
            questionText: '',
            price: 0.5
        }
    },
    methods: {
        ask() {
            var self = this;
            questionService.postQuestion(this.subject.id, this.questionText, this.price)
                .then(function () {
                    self.$router.push({path: '/note', query: {q: ''}});
                });
        },
    },
    computed: {
        validForm() {
            return (this.subject && this.questionText.length && this.price >= 0.5) ? true : false
        }
    },
    created() {
        var self = this;
        questionService.getSubjects().then(function (response) {
            self.subjectList = response.data
        })
    }
}