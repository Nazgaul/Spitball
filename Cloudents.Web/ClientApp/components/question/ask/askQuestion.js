import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionService from '../../../services/questionService';

export default {
    components: {extendedTextArea},
    data() {
        return {
            subjectList: [],
            subject: '',
            textAreaValue: '',
            price: 0.5,
            files: [],
        }
    },
    methods: {
        submitQuestion() {
            var self = this;
            questionService.postQuestion(this.subject.id, this.textAreaValue, this.price, this.files)
                .then(function () {
                    debugger;
                    self.$router.push({path: '/ask', query: {q: ''}});
                });
        },
        addFile(filename) {
            this.files.push(filename);
        },
        removeFile(index) {
            this.files.splice(index, 1);
        }
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