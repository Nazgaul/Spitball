import { mapGetters, mapMutations, mapActions } from 'vuex';
import questionThread from "./questionThread.vue";
import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionCard from "./../helpers/new-question-card/new-question-card.vue";
import answerCard from "./../helpers/question-card/question-card.vue";
import sbDialog from '../../wrappers/sb-dialog/sb-dialog.vue'
import loginToAnswer from '../../question/helpers/loginToAnswer/login-answer.vue';
import questionService from "../../../services/questionService";

import disableForm from "../../mixins/submitDisableMixin.js";
import { sendEventList } from '../../../services/signalR/signalREventSender';
import analyticsService from '../../../services/analytics.service';
import searchService from '../../../services/searchService';
import { LanguageService } from "../../../services/language/languageService";
import feedStore from '../../../store/feedStore';
import Question from "../../../store/question";
import storeService from "../../../services/store/storeService";

export default {
    mixins: [disableForm],
    components: {questionThread, questionCard, answerCard, extendedTextArea, sbDialog, loginToAnswer},
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
        ]),
        ...mapMutations({updateLoading: "UPDATE_LOADING", updateSearchLoading:'UPDATE_SEARCH_LOADING'}),
        ...mapGetters(["getQuestion"]),
        resetSearch(){
            this.updateSearchLoading(true);
            this.$router.push({path:"/feed"});
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

                questionService.answerQuestion(self.id, self.textAreaValue)
                    .then(function (resp) {                       
                        analyticsService.sb_unitedEvent("Submit_answer", "Homwork help");
                        self.textAreaValue = "";
                        self.updateLoading(false);
                        //self.getData(true);//TODO: remove this line when doing the client side data rendering (make sure to handle delete as well)
                    }, (error) => {
                        console.log(error);
                        // self.errorHasAnswer = error.response.data["Text"] ? error.response.data["Text"][0] : '';
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
        ...mapGetters(["accountUser", "chatAccount", "getCorrectAnswer", "isDeletedAnswer", "loginDialogState", "isCardOwner"]),
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
            let val = !this.cardOwner && (!this.accountUser || this.userNotAnswered);
            this.showForm = (val && !this.questionData.answers.length);
            return val;
        },
    },
    created() {               
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