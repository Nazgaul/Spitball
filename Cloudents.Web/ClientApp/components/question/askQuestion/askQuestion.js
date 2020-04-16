import { mapActions, mapGetters } from 'vuex';
import { LanguageService } from "../../../services/language/languageService";
import { validationRules } from "../../../services/utilities/formValidationRules";
import questionService from "../../../services/questionService";
import analyticsService from "../../../services/analytics.service";
import debounce from "lodash/debounce";
import courseService from '../../../services/courseService.js'

export default {
    data() {
        return {
            questionCourse: '',
            questionText: '',
            suggestsCourses: [],
            btnQuestionLoading: false,
            validQuestionForm: false,
            errorMessage: '',
            rules: {
                required: (value) => validationRules.required(value),
                maximumChars: (value) => validationRules.maximumChars(value, 255),
                minimumChars: (value) => validationRules.minimumChars(value, 15),
            },
            coursePlaceholder: LanguageService.getValueByKey("addQuestion_class_placeholder"),
            topicPlaceholder: LanguageService.getValueByKey("addQuestion_ask_your_question_placeholder"),
        };
    },
    computed: {
        ...mapGetters(['accountUser', 'getSelectedClasses']),
        userImageUrl() {
            if(this.accountUser && this.accountUser.image.length > 1) {
                return `${this.accountUser.image}`;
            }
            return '';
        },
        userName() {
            if(this.accountUser && this.accountUser.name) {
                return this.accountUser.name;
            }
            return '';
        }
    },
    methods: {
        ...mapActions(['updateNewQuestionDialogState']),
        requestAskClose() {
            this.updateNewQuestionDialogState(false);
        },
        submitQuestion() {
            let self = this;
            if(self.$refs.questionForm.validate()) {
                self.btnQuestionLoading =true;
                let serverQuestionObj = {
                    text: self.questionText,
                    course: self.questionCourse.text ? self.questionCourse.text : self.questionCourse,
                };
                questionService.postQuestion(serverQuestionObj).then(() => {
                    analyticsService.sb_unitedEvent("Submit_question", "Homework help");
                    self.btnQuestionLoading =false;
                    //close dialog after question submitted
                    self.requestAskClose(false);
                    self.$router.push({path: '/'});
                }, (error) => {                    
                    let errorMessage = LanguageService.getValueByKey('addQuestion_error_general');
                    if (error && error.response && error.response.data && error.response.data[""] && error.response.data[""][0]) {
                        errorMessage = error.response.data[""][0];
                    }
                    self.errorMessage = errorMessage;
                    console.error(error);
                }).finally(()=>{
                    self.btnQuestionLoading =false;
                });
            }
        },
        tutorRequestDialogClose() {
            this.updateRequestDialog(false);
        },
        closeAddQuestionDialog() {
            this.updateNewQuestionDialogState(false);
        },
        searchCourses: debounce(function(ev){
            let term = ev.target.value.trim()
            if(!term) {
                this.questionCourse = ''
                this.suggestsCourses = []
                return 
            }
            if(!!term){
                courseService.getCourse({term, page:0}).then(data=>{
                    this.suggestsCourses = data;
                    this.suggestsCourses.forEach(course=>{                                               
                        if(course.text === this.questionCourse){                           
                            this.questionCourse = course
                        }
                    }) 
                })
            }
        },300),
    },
    created() {
        if(this.$route.query && this.$route.query.term){
            this.questionCourse = this.$route.query.term;
        }
        if(this.$route.query && this.$route.query.Course){
            this.questionCourse = this.$route.query.Course;
        }
    }
};