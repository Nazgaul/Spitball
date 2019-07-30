import { mapActions, mapGetters } from 'vuex';
import tutorService from "../../services/tutorService";
import { LanguageService } from "../../services/language/languageService";
import { validationRules } from "../../services/utilities/formValidationRules";
import analyticsService from '../../services/analytics.service';
import universityService from "../../services/universityService.js";
import debounce from "lodash/debounce";
import VueRecaptcha from 'vue-recaptcha';


export default {
    components:{VueRecaptcha},
    data() {
        return {
            siteKey: '6LfyBqwUAAAAAM-inDEzhgI2Cjf2OKH0IZbWPbQA',
            recaptcha: '',
            isProfile: false,
            suggestsUniversities: [],
            suggestsCourses: [],
            tutorCourse: '',
            tutorRequestText: '',
            btnRequestLoading: false,
            validRequestTutorForm: false,
            guestName: '',
            guestMail: '',
            guestPhone: '',
            guestUniversity: '',
            rules: {
                required: (value) => validationRules.required(value),
                maximumChars: (value) => validationRules.maximumChars(value, 255),
                email: (value) => validationRules.email(value),
            },
            btnSubmitPlaceholder: LanguageService.getValueByKey("tutorRequest_btn_submit"),
            btnClosePlaceholder: LanguageService.getValueByKey("tutorRequest_btn_close"),
            titlePlaceholder: LanguageService.getValueByKey("tutorRequest_title"),
            coursePlaceholder: LanguageService.getValueByKey("tutorRequest_select_course_placeholder"),
            topicPlaceholder: LanguageService.getValueByKey("tutorRequest_topic_placeholder"),
            guestNamePlaceHolder : LanguageService.getValueByKey("tutorRequest_name"),
            guestEmailPlaceHolder : LanguageService.getValueByKey("tutorRequest_email"),
            guestPhoneNumberPlaceHolder : LanguageService.getValueByKey("tutorRequest_phoneNumber"),
            universityPlaceHolder : LanguageService.getValueByKey("tutorRequest_university"),
        };
    },
    computed: {
        ...mapGetters(['accountUser','getCurrTutor', 'getTutorRequestAnalyticsOpenedFrom']),
        isAuthUser(){
            return !!this.accountUser;
        },
        dialogTitle(){
            if(!this.getCurrTutor) return LanguageService.getValueByKey('tutorRequest_title')
            let currTutor = this.getCurrTutor;
            let message = this.isProfile || currTutor.name ? LanguageService.getValueByKey('tutorRequest_title_tutor_list'): LanguageService.getValueByKey('tutorRequest_title');
            let name = currTutor.name ? currTutor.name : '';
            return `${message} ${name}`;
        }
    },
    methods: {
        ...mapActions(['updateRequestDialog', 'updateToasterParams']),
        searchCourses: debounce(function(ev){
            let term = ev.target.value.trim();
            if(!term) {
                this.tutorCourse = ''
                return 
            }
            if(!!term){
                universityService.getCourse({term, page:0}).then(data=>{
                    this.suggestsCourses = data;
                })
            }
        },300),
        searchUniversities: debounce(function(ev){
            let term = ev.target.value.trim();
            if(!!term){
                universityService.getUni({term, page:0}).then(data=>{
                    this.suggestsUniversities = data;
                })
            }
        },300),
        sendRequest() {
            let self = this;
            let tutorId = null
            if(this.getCurrTutor) {
                tutorId = this.getCurrTutor.userId || this.getCurrTutor.id
            }
            if(self.$refs.tutorRequestForm.validate()) {
                self.btnRequestLoading = true;
                let serverObj = {
                    captcha: (self.recaptcha)? self.recaptcha : null,
                    text: (self.tutorRequestText)? self.tutorRequestText : null,
                    name: (self.guestName)? self.guestName : null,
                    email: (self.guestMail)? self.guestMail : null,
                    phone: (self.guestPhone)? self.guestPhone : null,
                    course: (self.tutorCourse)? self.tutorCourse : null,
                    university: (self.guestUniversity.id)? self.guestUniversity.id : null,
                    tutorId: tutorId
                };
                self.sendAnalyticEvent(false);
                tutorService.requestTutor(serverObj)
                            .then(() => {
                                        self.tutorRequestDialogClose();
                                        self.updateToasterParams({
                                        toasterText: LanguageService.getValueByKey("tutorRequest_request_received"),
                                        showToaster: true,
                                      })
                                    }).catch((err)=>{
                                        self.updateToasterParams({
                                          toasterText: !!err.response.data.error ? err.response.data.error[0] : LanguageService.getValueByKey("tutorRequest_request_error"),
                                          showToaster: true,
                                          toasterType: 'error-toaster'
                                        })
                                  }).finally(() => {
                                      self.btnRequestLoading = false
                                      self.$refs['recaptcha'].reset();
                                    });
            }
        },
        tutorRequestDialogClose() {
            this.updateRequestDialog(false);
        },
        onVerify(response) {
            this.recaptcha = response;
            this.sendRequest()
            this.$refs['recaptcha'].reset();
        },
        onExpired() {
            this.recaptcha = "";
            this.$refs['recaptcha'].reset();
        },
        submit(guest){
            if(guest){
                this.$refs['recaptcha'].execute()
            }else{
                this.sendRequest();
            }
        },
        sendAnalyticEvent(beforeSubmit){
            let analyticsObject = {
                userId: this.isAuthUser ? this.accountUser.id : 'GUEST',
                course: this.tutorCourse,
                fromDialogPath: this.getTutorRequestAnalyticsOpenedFrom.path,
                fromDialogComponent: this.getTutorRequestAnalyticsOpenedFrom.component
            }
            if(beforeSubmit){
                analyticsService.sb_unitedEvent('Request Tutor Dialog Opened', `${analyticsObject.fromDialogPath}-${analyticsObject.fromDialogComponent}`, `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
            }else{
                analyticsService.sb_unitedEvent('Request Tutor Submit', `${analyticsObject.fromDialogPath}-${analyticsObject.fromDialogComponent}`, `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
            }
        }
    },

    created() {
        this.sendAnalyticEvent(true);
        this.isProfile = this.$route.name === 'profile'? true : false;
        let captchaLangCode = global.lang === 'he' ? 'iw' : 'en';
        this.$loadScript(`https://www.google.com/recaptcha/api.js?onload=vueRecaptchaApiLoaded&render=explicit&hl=${captchaLangCode}`);
    },
};