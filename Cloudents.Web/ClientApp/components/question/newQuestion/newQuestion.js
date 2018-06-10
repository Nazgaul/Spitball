import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionService from '../../../services/questionService';
import disableForm from "../../mixins/submitDisableMixin"
import {mapGetters, mapMutations} from 'vuex'

export default {
    mixins: [disableForm],
    components: {extendedTextArea},
    data() {
        return {
            subjectList: [],
            subject: '',
            textAreaValue: '',
            price: null,
            selectedPrice:null,
            files: [],
            errorMessage:''
        }
    },
    methods: {
        ...mapMutations(["UPDATE_LOADING"]),
        submitQuestion() {
            if(this.accountUser && this.accountUser.balance < this.price){
                this.errorMessage = "You don't have enough balance in your account"
                return;
            }
            var self = this;
            if (this.submitForm()) {
                this.UPDATE_LOADING(true);
                questionService.postQuestion(this.subject.id, this.textAreaValue, this.selectedPrice || this.price, this.files)
                    .then(function () {
                            self.$router.push({path: '/ask', query: {q: ''}});
                        },
                        function (error) {
                            self.UPDATE_LOADING(false)
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
        },
        selectOtherAmount(){
            this.selectedPrice = null;
            var selected = this.$el.querySelector('.automatic-amount:checked');
            if(selected){
                selected.checked=false;
            }
        }
    },
    computed: {
        ...mapGetters(['accountUser']),
        validForm() {
            return this.subject && this.textAreaValue.length && (this.selectedPrice || this.price >= 0.5);
        },
    },
    created() {
        var self = this;
        questionService.getSubjects().then(function (response) {
            self.subjectList = response.data
        })
    }
}