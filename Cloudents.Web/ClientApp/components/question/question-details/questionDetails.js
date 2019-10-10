import questionThread from "./questionThread.vue";
import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionService from "../../../services/questionService";
import { mapGetters, mapMutations, mapActions } from 'vuex'
import questionCard from "./../helpers/new-question-card/new-question-card.vue";
import answerCard from "./../helpers/question-card/question-card.vue";
import disableForm from "../../mixins/submitDisableMixin.js"
import QuestionSuggestPopUp from "../../questionsSuggestPopUp/questionSuggestPopUp.vue";
import sbDialog from '../../wrappers/sb-dialog/sb-dialog.vue'
import loginToAnswer from '../../question/helpers/loginToAnswer/login-answer.vue'
import { sendEventList } from '../../../services/signalR/signalREventSender'
import { LanguageService } from "../../../services/language/languageService";
import analyticsService from '../../../services/analytics.service';
import searchService from '../../../services/searchService'
import newBaller from "../../helpers/newBaller/newBaller.vue";

import homeworkHelpStore from '../../../store/homeworkHelp_store';
import Question from "../../../store/question";
import storeService from "../../../services/store/storeService";

export default {
    mixins: [disableForm],
    components: {questionThread, questionCard, answerCard, extendedTextArea, QuestionSuggestPopUp, sbDialog, loginToAnswer, newBaller},
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
            //questionData: null,
            cardList: [],
            showForm: false,
            showDialogSuggestQuestion: false,
            showDialogLogin: false,
            build: null,
            isEdgeRtl : global.isEdgeRtl,
            cahceQuestion: {},
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
            "updateLoginDialogState",
            'updateUserProfileData',
            'setQuestion',
            'updateNewBallerDialogState'
        ]),
        ...mapMutations({updateLoading: "UPDATE_LOADING", updateSearchLoading:'UPDATE_SEARCH_LOADING'}),
        ...mapGetters(["getQuestion"]),
        resetSearch(){
            this.updateSearchLoading(true);
            this.$router.push({path:"/ask"});
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
            this.updateLoading(true);
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
                questionService.answerQuestion(self.id, self.textAreaValue, self.answerFiles)
                    .then(function (resp) {
                        //self.$ga.event("Submit_answer", "Homwork help");
                        analyticsService.sb_unitedEvent("Submit_answer", "Homwork help");
                        self.textAreaValue = "";
                        self.answerFiles = [];
                        self.updateLoading(false);
                        if(!resp.data.nextQuestions){
                            self.$data.submitted = false;
                            return;
                        };
                        self.cardList = resp.data.nextQuestions.map(searchService.createQuestionItem);
                        //self.getData(true);//TODO: remove this line when doing the client side data rendering (make sure to handle delete as well)
                        self.showDialogSuggestQuestion = true; // question suggest popup dialog
                    }, (error) => {
                        self.errorHasAnswer = error.response.data["Text"] ? error.response.data["Text"][0] : '';
                        self.submitForm(false);
                        self.updateLoading(true);
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
            });
        },
        openNewBaller(){
            if(this.accountUser ){
                let score = this.accountUser.score;
                let supressed = global.localStorage.getItem("sb-newBaller-suppresed");
                if(score < 150 && !supressed){
                    this.updateNewBallerDialogState(true);
                }
            }
        },
        showAnswerField() {
            if (this.accountUser) {
                this.showForm = true;
            }
            else {
                this.updateUserProfileData('profileMakeMoney');
                this.dialogType = '';
                this.updateLoginDialogState(true);
            }
        },

    },
    watch: {
        textAreaValue(){
            this.errorLength = {};
        },
        //watch route(url query) update, and het question data from server
        '$route': 'getData'
    },
    computed: {
        ...mapGetters(["accountUser", "chatAccount", "getCorrectAnswer", "isDeletedAnswer", "loginDialogState", "isCardOwner", "newBallerDialog",]),
        questionData(){
            return this.getQuestion();
        },
        errorTextArea(){
                return this.errorLength;
        },
        cardOwner(){
            return this.isCardOwner;
        },
        userNotAnswered() {
            this.isDeletedAnswer ? this.submitForm(false) : "";
            return !this.questionData.answers.length || (!this.questionData.answers.filter(i => i.user.id === this.accountUser.id).length || this.isDeletedAnswer);
        },
        enableAnswer() {
            let hasCorrectAnswer = !!this.questionData.correctAnswerId;
            let val = !this.cardOwner && (!this.accountUser || this.userNotAnswered) && !hasCorrectAnswer;
            this.showForm = (val && !this.questionData.answers.length);
            return val;
        },
    },
    beforeDestroy(){
        // storeService.unregisterModule(this.$store, 'Question', Question);
    },
    created() {
        // storeService.registerModule(this.$store, 'Question', Question);
        storeService.lazyRegisterModule(this.$store, 'homeworkHelpStore', homeworkHelpStore);
        
        this.getData();
        this.$root.$on('closePopUp', (name) => {
            if (name === 'suggestions') {
                this.showDialogSuggestQuestion = false;
            } else {
                this.updateLoginDialogState(false);
            }
        });

    },
}