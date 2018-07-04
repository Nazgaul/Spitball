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
            pricesList:[10,20,40,80]
        }
    },
    watch:{
        price(val){
            let splitLength=val.toString().split('.');
            if(splitLength.length===2&&splitLength[1].length>=3) {
                this.price = parseFloat(val).toFixed(2)
            }
        }
    },
    methods: {
        ...mapMutations({updateLoading:"UPDATE_LOADING"}),
        ...mapActions(['updateUserBalance','updateToasterParams']),
        submitQuestion() {
            if(this.accountUser && this.accountUser.balance < this.price){
                this.errorMessage = "You don't have enough balance in your account"
                return;
            }
            var self = this;
            if (this.submitForm()) {
                this.updateLoading(true);
                console.log('start loading');
                questionService.postQuestion(this.subject.id, this.textAreaValue, this.selectedPrice || this.price, this.files)
                    .then(function () {
                        // debugger;
                        let val= self.selectedPrice || self.price;
                            self.updateUserBalance(-val);
                            self.$router.push({path: '/ask', query: {q: ''}});
                                self.updateToasterParams({
                                    toasterText: 'Question posted, the best brains are working on it right now',
                                    showToaster: true,
                                });
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
            console.log(this.files)
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
            let val=this.selectedPrice||this.price||0;
            this.selectedPrice?this.price=null:"";
            return this.accountUser.balance-val
        },
        validForm() {
            return this.subject && this.textAreaValue.length && (this.selectedPrice || this.price >= 10);
        },
    },
    created() {
        var self = this;
        questionService.getSubjects().then(function (response) {
            self.subjectList = response.data
        })
    }
}