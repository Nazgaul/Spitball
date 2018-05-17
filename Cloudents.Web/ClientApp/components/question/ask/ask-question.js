import questionTextArea from "./../helpers/questionTextArea.vue";
import questionService from '../../../services/questionService';

export default {
    components: {questionTextArea},
    data() {
        return {
            subjectList: [],
            category: '',
            questionText: '',
            price: 0.5
        }
    },
    methods: {
        ask() {
            console.log(this.price, this.questionText, this.category)
        }
    },
    computed: {
        validForm() {
            return (this.category && this.questionText.length && this.price >= 0.5) ? true : false
        }
    },
    created() {
        var self = this;
        questionService.getSubjects().then(function (response) {
            self.subjectList = response.data
        })
    }
}