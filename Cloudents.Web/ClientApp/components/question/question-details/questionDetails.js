import { mapGetters, mapActions } from 'vuex';
import questionThread from "./questionThread.vue";
import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionCard from "./../helpers/new-question-card/new-question-card.vue";
import answerCard from "./../helpers/question-card/question-card.vue";
import sbDialog from '../../wrappers/sb-dialog/sb-dialog.vue'
import questionService from "../../../services/questionService";
import disableForm from "../../mixins/submitDisableMixin.js";
import analyticsService from '../../../services/analytics.service';
import { LanguageService } from "../../../services/language/languageService";

export default {
    mixins: [disableForm],
    components: {questionThread, questionCard, answerCard, extendedTextArea, sbDialog},
    props: {
        id: {Number}, // got it from route
        questionId: {Number}
    },
    data() {
        return {
            textAreaValue: "",
            errorHasAnswer: '',
            errorDuplicatedAnswer:'',
            answerFiles: [],
            errorLength:{},
            cardList: [],
            showForm: false,
            showDialogSuggestQuestion: false,
            showDialogLogin: false,
            build: null,
            cahceQuestion: {},
            submitLoader: false,
            hasData: false
        };
    },
    beforeRouteLeave(to, from, next) {
        this.resetQuestion();
        next();
    },
    methods: {
        ...mapActions([
            "resetQuestion",
            "removeDeletedAnswer",
            "updateToasterParams",
            'setQuestion'
        ]),
        ...mapGetters(["getQuestion"]),
        resetSearch(){
            this.$router.push({name: "feed"});
        },
        submitAnswer() {
            
            if (!this.textAreaValue || this.textAreaValue.trim().length < 15) {
                this.errorLength= {
                    errorText: LanguageService.getValueByKey("questionDetails_error_minChar"),
                    errorClass: true
                };
                return;
            }
            if (!this.textAreaValue || this.textAreaValue.trim().length > 540) {
                this.errorLength= {
                    errorText: LanguageService.getValueByKey("questionDetails_error_maxChar"),
                    errorClass: true
                };
                return;
            }
            var self = this;
            if(this.hasDuplicatiedAnswer(self.textAreaValue, self.questionData.answers)) {
                console.log("duplicated answer detected");
                this.errorDuplicatedAnswer = LanguageService.getValueByKey("questionDetails_error_duplicated");
                return;
            }else{
                this.errorDuplicatedAnswer = '';
            }
            if (self.submitForm()) {
                this.removeDeletedAnswer();
                self.textAreaValue = self.textAreaValue.trim();
                this.submitLoader = true;
                questionService.answerQuestion(self.id, self.textAreaValue)
                    .then(function () {                       
                        analyticsService.sb_unitedEvent("Submit_answer", "Homwork help");
                        self.textAreaValue = "";
                    
                        //self.getData(true);//TODO: remove this line when doing the client side data rendering (make sure to handle delete as well)
                    }, (error) => {
                        console.log(error);
                        // self.errorHasAnswer = error.response.data["Text"] ? error.response.data["Text"][0] : '';
                        self.submitForm(false);
                      
                    }).finally(()=>{
                        this.submitLoader = false;
                    });
            }
        },
        hasDuplicatiedAnswer(currentText, answers){  
            let duplicated = answers.filter(answer=>{
                return answer.text.indexOf(currentText) > -1;
            });
            return duplicated.length > 0;
        },

        addFile(filenames) {
            this.answerFiles = this.answerFiles.concat(filenames);
            //this.answerFiles.push(...filename.split(','));
        },
        removeFile(index) {
            this.answerFiles.splice(index, 1);
        },
        getData(skipViewerUpdate) {
            let updateViewer = skipViewerUpdate ? false : true;
            //enable submit btn
            this.$data.submitted = false;
            this.setQuestion(this.id).then(()=>{
                if (updateViewer) {
                    this.cahceQuestion = {...this.questionData};
                }
            }).finally(() => {
                this.hasData = true
            })
        },
        showAnswerField() {            
            if (this.accountUser) {
                this.showForm = true;
            }
            else {
                this.dialogType = '';
                this.$openDialog('login')
            }
        },
        goToAnswer(hash) {
            this.$vuetify.goTo(hash)
            // let elem = this.$refs.answers;
            // elem.scrollIntoView({ behavior: 'smooth', block: 'center' })
        }
    },
    watch: {
        textAreaValue(){
            this.errorLength = {};
        },
        hasData(val) {
            let hash = this.$route.hash
            if(hash && val) {
                this.goToAnswer(hash);
            }
        },
        //watch route(url query) update, and het question data from server
        '$route': 'getData'
    },
    computed: {
        ...mapGetters(["accountUser", "chatAccount", "getCorrectAnswer", "isCardOwner"]),
        questionData(){
            return this.getQuestion();
        },
        errorTextArea(){
                return this.errorLength;
        },
        cardOwner(){
            return this.isCardOwner;
        },
        userAnswered() {
            if(this.accountUser) {
                return this.questionData.answers.length && this.questionData.answers.filter(i => i.user.id === this.accountUser.id).length;
            }
            return null;
        },
    },
    created() {               
        this.getData();
    },
}