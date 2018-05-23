import questionTextArea from "./../helpers/question-text-area/questionTextArea.vue";
import questionService from '../../../services/questionService';

export default {
    components: {questionTextArea},
    data() {
        return {
            subjectList: [],
            subject: '',
            textAreaValue: '',
            price: 0.5,
            files: []
        }
    },
    methods: {
        submitQuestion() {
            var self = this;
            questionService.postQuestion(this.subject.id, this.textAreaValue, this.price, this.files)
                .then(function () {
                    self.$router.push({path: '/ask', query: {q: ''}});
                });
        },
        addFile(filename){
            this.files.push(filename);
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