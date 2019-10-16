import { mapActions, mapGetters, mapMutations } from 'vuex';
import UserAvatar from '../../helpers/UserAvatar/UserAvatar.vue';
import FileUpload from 'vue-upload-component/src'; //docs here https://lian-yue.github.io/vue-upload-component
import { LanguageService } from "../../../services/language/languageService";
import { validationRules } from "../../../services/utilities/formValidationRules";
import questionService from "../../../services/questionService";
import analyticsService from "../../../services/analytics.service";
import debounce from "lodash/debounce";
import universityService from '../../../services/universityService.js'

export default {
    components: {
        UserAvatar,
        FileUpload
    },
    data() {
        return {
            // questionCourse: '',
            questionText: '',
            courseQuestion: '',
            suggestsCourses: [],
            btnQuestionLoading: false,
            validQuestionForm: false,
            errorMessage: '',
            rules: {
                required: (value) => validationRules.required(value),
                maximumChars: (value) => validationRules.maximumChars(value, 255),
                minimumChars: (value) => validationRules.minimumChars(value, 15),
                matchCourse:() => (this.suggestsCourses.length && this.suggestsCourses.some(course=>course.text === this.courseQuestion.text)) || LanguageService.getValueByKey("tutorRequest_invalid")
            },
            coursePlaceholder: LanguageService.getValueByKey("addQuestion_class_placeholder"),
            topicPlaceholder: LanguageService.getValueByKey("addQuestion_ask_your_question_placeholder"),
            // uploadProp: {
            //     populatedThumnbailBox: {
            //         box_0: {
            //             populated: false,
            //             src: ''
            //         },
            //         box_1: {
            //             populated: false,
            //             src: ''
            //         },
            //         box_2: {
            //             populated: false,
            //             src: ''
            //         },
            //         box_3: {
            //             populated: false,
            //             src: ''
            //         }
            //     },
            //     componentUniqueId: `instance-${this._uid}`,
            //     extensions: ['jpeg', 'jpe', 'jpg', 'gif', 'png', 'webp'],
            //     uploadUrl: "/api/Question/ask",
            //     uploadedFiles: [],
            //     uploadedFileNames: [],
            //     MAX_FILES_AMOUNT: 4
            // }
        };
    },
    computed: {
        ...mapGetters(['accountUser', 'getSelectedClasses', 'newQuestionDialogSate']),
        userImageUrl() {
            if(this.accountUser.image.length > 1) {
                return `${this.accountUser.image}`;
            }
            return '';
        }
    },
    methods: {
        ...mapActions(['updateNewQuestionDialogState']),
        ...mapMutations(['UPDATE_LOADING']),
        requestAskClose() {
            this.updateNewQuestionDialogState(false);
        },
        submitQuestion() {
            let self = this;
            if(self.$refs.questionForm.validate()) {
                self.btnQuestionLoading =true;
                self.UPDATE_LOADING(true);
                let serverQuestionObj = {
                    text: self.questionText,
                    course: self.courseQuestion.text,
                    // files:self.uploadProp.uploadedFileNames
                };
                questionService.postQuestion(serverQuestionObj).then(() => {
                    analyticsService.sb_unitedEvent("Submit_question", "Homework help");
                    self.btnQuestionLoading =false;
                    //close dialog after question submitted
                    self.requestAskClose(false);
                    self.$router.push({path: '/feed'});
                    self.UPDATE_LOADING(false);
                }, (error) => {                    
                    let errorMessage = LanguageService.getValueByKey('addQuestion_error_general');
                    if (error && error.response && error.response.data && error.response.data[""] && error.response.data[""][0]) {
                        errorMessage = error.response.data[""][0];
                    }
                    self.errorMessage = errorMessage;
                    self.UPDATE_LOADING(false);
                    console.error(error);
                }).finally(()=>{
                    self.btnQuestionLoading =false;
                });
            }
        },
        tutorRequestDialogClose() {
            this.updateRequestDialog(false);
        },
        // openUploadInterface() {
        //     let uploadFileElement = document.querySelector(`#${this.uploadProp.componentUniqueId}`);
        //     uploadFileElement.click();
        // },
        searchCourses: debounce(function(ev){
            let term = this.isFromQuery ? ev : ev.target.value.trim()
            if(!term) {
                this.courseQuestion = ''
                this.suggestsCourses = []
                return 
            }
            if(!!term){
                universityService.getCourse({term, page:0}).then(data=>{
                    this.suggestsCourses = data;
                    this.suggestsCourses.forEach(course=>{                                               
                        if(course.text === this.courseQuestion){                           
                            this.courseQuestion = course
                        }
                    }) 
                })
            }
        },300),
        closeAddQuestionDialog() {
            this.updateNewQuestionDialogState(false);
        }
    },
    created() {
        if(this.$route.query && this.$route.query.Course){
            this.courseQuestion = this.$route.query.Course;
        }
    }
};