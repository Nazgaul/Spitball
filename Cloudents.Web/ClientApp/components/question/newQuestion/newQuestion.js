import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionService from '../../../services/questionService';
import disableForm from "../../mixins/submitDisableMixin"
import {mapGetters,mapMutations,mapActions} from 'vuex'
import {LanguageService} from "../../../services/language/languageService";
import analyticsService from '../../../services/analytics.service';

export default {
    mixins: [disableForm],
    components: {
        extendedTextArea
    },

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
            actionType: "question",
            errorWaitTime: '',
            loading: false
        }
    },
    watch: {
        price(val) {
            if (!val) return;
            let splitLength = val.toString().split('.');
            if (splitLength.length === 2 && splitLength[1].length >= 3) {
                this.price = parseFloat(val).toFixed(2)
            }


        },
        textAreaValue() {
            this.errorTextArea = {};
        },
        //watch selected to clear custom price field
        selectedPrice(val) {
            if (val && val > 0) {
                return this.price = null
            }
        },
        // if question dialog state is false reset question form data to default
        newQuestionDialogSate: {
            immediate: true,
            handler(val) {
                if (!val) {
                    this.textAreaValue = '';
                    this.errorTextArea = {};
                    this.subject = '';
                    this.price = null;
                    this.selectedPrice = null;
                    this.errorMessage = '';
                    this.errorMessageSubject = '';
                    this.errorSelectPrice = '';
                    this.pricesList = [10, 20, 40, 80];
                    this.loading = false;
                    this.$root.$emit('previewClean', 'true');
                    this.files = [];
                    this.errorWaitTime = '';
                } else {
                    // get subject if questionDialog state is true(happens only if accountUser is true)
                    questionService.getSubjects().then((response) => {
                        this.subjectList = response.data
                    });
                }
            },
        },
    },
    methods: {
        ...mapMutations({
            updateLoading: "UPDATE_LOADING"
        }),
        ...mapActions(['updateUserBalance', 'updateToasterParams', 'updateNewQuestionDialogState']),

        submitQuestion() {
            let readyToSend = true;
            this.textAreaValue = this.textAreaValue.trim();
            //error handling stuff ( redo with newer version to validate with in build validators
            if (!this.selectedPrice && this.price < 1 && !this.selectedPrice || this.price > 100) {
                readyToSend = false
            }
            if (this.price > this.thirtyPercent) {
                readyToSend = false
            }
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
            self.loading = true;
            if (this.submitForm()) {
                this.updateLoading(true);
                questionService.postQuestion(this.subject.id, this.textAreaValue, this.selectedPrice || this.price, this.files)
                    .then(function (response) {
                            //self.$ga.event("Submit_question", "Homework help");
                            analyticsService.sb_unitedEvent("Submit_question", "Homework help")
                            let val = self.selectedPrice || self.price;
                            self.updateUserBalance(-val);
                            //close dialog after question submitted
                            self.requestNewQuestionDialogClose(false);
                            self.$router.push({
                                path: '/ask',
                                query: {
                                    term: ''
                                }
                            });
                            self.updateLoading(false);
                            // self.updateToasterParams({
                            //     toasterText: response.data.toasterText, // LanguageService.getValueByKey("question_newQuestion_toasterPostedText"),
                            //     showToaster: true,
                            // });
                            self.submitForm(false);
                        },
                        function (error) {
                            self.updateLoading(false);
                            console.error(error);
                            let errorMessage = 'Something went wrong, try again.';
                            if (error && error.response && error.response.data && error.response.data[""] && error.response.data[""][0]) {
                                errorMessage = error.response.data[""][0];
                            }
                            self.errorWaitTime = `${errorMessage}` || '';
                            self.loading = false;
                            self.submitForm(false);

                        });
            }
        },
        addFile(filename) {
            //if max reached replace
            if (this.files && this.files.length > 4) {
                this.files.splice(-1, 1, filename);
                //add
            } else {
                this.files = this.files.concat(filename);
            }
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
        },
        //close dialog
        requestNewQuestionDialogClose() {
            this.updateNewQuestionDialogState(false)
        },
    },
    beforeDestroy() {
        this.updateNewQuestionDialogState(false)
    },
    computed: {
        ...mapGetters(['accountUser', 'newQuestionDialogSate']),
        currentSum() {
            if (this.accountUser) {
                // Gaby - Deprecated!
                // let val = this.selectedPrice || this.price || 0;
                // this.selectedPrice ? this.price = null : "";
                // return this.accountUser.balance - val;
                return this.accountUser.balance.toFixed(2);
            }
        },
        thirtyPercent() {
            let notRounded = this.currentSum * 30 / 100;
            if (notRounded > 100) {
                notRounded = 100;
            }
            return parseFloat(notRounded.toFixed(2));
        },
        validForm() {
            return this.subject && this.textAreaValue.length > 15 && (this.selectedPrice || this.price >= 10 && this.selectedPrice || this.price <= 100);
        },
    },
    created() {
    }
}