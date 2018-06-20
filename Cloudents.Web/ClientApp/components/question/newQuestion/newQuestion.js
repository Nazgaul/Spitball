import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionService from '../../../services/questionService';
import disableForm from "../../mixins/submitDisableMixin"
import {mapGetters, mapMutations,mapActions} from 'vuex'

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
            errorMessage:'',
            pricesList:[5,10,20,35]
        }
    },
    methods: {
        ...mapMutations({updateLoading:"UPDATE_LOADING"}),
        ...mapActions(['updateUserBalance']),
        submitQuestion() {
            if(this.accountUser && this.accountUser.balance < this.price){
                this.errorMessage = "You don't have enough balance in your account"
                return;
            }
            var self = this;
            if (this.submitForm()) {
                this.updateLoading(true);
                console.log('start loading');
                debugger;
                questionService.postQuestion(this.subject.id, this.textAreaValue, this.selectedPrice || this.price, this.files)
                    .then(function () {
                        debugger;
                        let val= self.selectedPrice || self.price;
                            self.updateUserBalance(-val);
                            self.$router.push({path: '/ask', query: {q: ''}});
                        },
                        function (error) {
                            self.updateLoading(false);
                            console.error(error);
                            self.submitForm(false);
                        });
            }
        },
        addFile(filename) {
            this.files.push(...filename.split(','));
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
        currentSum(){
            let val=this.price||this.selectedPrice||0;
            return this.accountUser.balance-val
        },
        validForm() {
            return this.subject && this.textAreaValue.length && (this.selectedPrice || this.price >= 5);
        },
    },
    created() {
        var self = this;
        questionService.getSubjects().then(function (response) {
            self.subjectList = response.data
        })
    }
}