import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionService from '../../../services/questionService';
import disableForm from "../../mixins/submitDisableMixin"
import {mapGetters} from 'vuex'

export default {
    mixins: [disableForm],
    components: {extendedTextArea},
    data() {
        return {
            subjectList: [],
            subject: '',
            textAreaValue: '',
            price: 0.5,
            files: [],
            errorMessage:''
        }
    },
    methods: {
        submitQuestion() {
            if(this.accountUser && this.accountUser.balance < this.price){
                this.errorMessage = "You don't have enough balance in your account"
                return;
            }
            var self = this;
            if (this.submitForm()) {
                questionService.postQuestion(this.subject.id, this.textAreaValue, this.price, this.files)
                    .then(function () {
                            self.$router.push({path: '/ask', query: {q: ''}});
                        },
                        function (error) {
                            console.error(error);
                            this.submitForm(false);
                        });
            }
        },
        addFile(filename) {
            this.files.push(filename);
        },
        removeFile(index) {
            this.files.splice(index, 1);
        }
    },
    computed: {
        ...mapGetters(['accountUser']),
        validForm() {
            return this.subject && this.textAreaValue.length && this.price >= 0.5;
        },
    },
    created() {
        var self = this;
        questionService.getSubjects().then(function (response) {
            self.subjectList = response.data
        })
    }
}