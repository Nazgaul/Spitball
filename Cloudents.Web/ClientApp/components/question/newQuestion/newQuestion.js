import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionService from '../../../services/questionService';
import disableForm from "../../mixins/submitDisableMixin"
import {mapGetters, mapMutations, mapActions} from 'vuex'
import { LanguageService } from "../../../services/language/languageService";

export default {
    mixins: [disableForm],
    components: {extendedTextArea},

    data() {
        return {
            subjectList: [],
            errorTextArea: {},
            subject: '',
            textAreaValue: '',
            price: null,
            selectedPrice: null,
            files: [],
            errorMessage: '',
            errorMessageSubject: '',
            errorSelectPrice: '',
            pricesList: [10, 20, 40, 80],
            actionType:"question",
            selectedColor: {}
        }
    },
    watch: {
        price(val) {
            let splitLength = val.toString().split('.');
            if (splitLength.length === 2 && splitLength[1].length >= 3) {
                this.price = parseFloat(val).toFixed(2)
            }
        }
    },
    methods: {
        ...mapMutations({updateLoading: "UPDATE_LOADING"}),
        ...mapActions(['updateUserBalance', 'updateToasterParams']),
        submitQuestion() {
            let readyToSend = true;
            //error handling stuff ( redo with newer version to validate with in build validators
            if (!this.selectedPrice && this.price < 1 && !this.selectedPrice || this.price > 100) {
                readyToSend = false
            }
            //if
            if (this.currentSum < 0) {
               readyToSend = false
            }
            if (!this.selectedPrice) {
                this.errorSelectPrice = LanguageService.getValueByKey("question_newQuestion_error_minSum")
            }
            if (this.textAreaValue.length < 15) {
                this.errorTextArea = {
                    errorText: LanguageService.getValueByKey("question_newQuestion_error_minChars"),
                    errorClass: true
                };
                readyToSend = false
            }
            if (!this.subject) {
                this.errorMessageSubject = LanguageService.getValueByKey("question_newQuestion_error_pickSubject"),
                readyToSend = false
            }
            if (!readyToSend) {
                return
            }
            var self = this;
            if (this.submitForm()) {
                this.updateLoading(true);
                // this.textAreaValue = this.textAreaValue.trim();
                questionService.postQuestion(this.subject.id, this.textAreaValue, this.selectedPrice || this.price, this.files, this.selectedColor.name || 'default' )
                    .then(function () {
                            self.$ga.event("Submit_question", "Homework help");
                            let val = self.selectedPrice || self.price;
                            self.updateUserBalance(-val);
                            self.$router.push({path: '/ask', query: {term: ''}});
                            self.updateToasterParams({
                                toasterText: LanguageService.getValueByKey("question_newQuestion_toasterPostedText"),
                                showToaster: true,
                            });
                        },
                        function (error) {
                            self.updateLoading(false);
                            console.error(error);
                            self.updateToasterParams({
                                toasterText: `${error.response.data[""][0]}` || '',
                                showToaster: true,
                            });
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
        selectOtherAmount() {
            this.selectedPrice = null;
            var selected = this.$el.querySelector('.automatic-amount:checked');
            if (selected) {
                selected.checked = false;
            }
        }
    },
    computed: {
        ...mapGetters(['accountUser']),
        currentSum() {
            let val = this.selectedPrice || this.price || 0;
            this.selectedPrice ? this.price = null : "";
            return this.accountUser.balance - val;

        },
        validForm() {
          return  this.subject && this.textAreaValue.length > 15 && (this.selectedPrice || this.price >= 10 && this.selectedPrice || this.price <=100);
        },
    },
    created() {
        var self = this;
        questionService.getSubjects().then(function (response) {
            self.subjectList = response.data
        })
        this.$on('colorSelected', (activeColor)=>{
             this.selectedColor.name = activeColor.name;
        });

    }
}