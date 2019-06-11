import { mapActions, mapGetters } from 'vuex';
import tutorService from "../../services/tutorService";
import { LanguageService } from "../../services/language/languageService";
import { validationRules } from "../../services/utilities/formValidationRules";
import analyticsService from '../../services/analytics.service';
import universityService from "../../services/universityService.js";
import debounce from "lodash/debounce";

export default {
    data() {
        return {
            suggests: [],
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
        ...mapGetters(['accountUser']),
        isAuthUser(){
            return !!this.accountUser;
        }
    },
    methods: {
        ...mapActions(['updateRequestDialog', 'updateToasterParams']),
        search: debounce(function(ev){
            let term = ev.target.value.trim();
            if(!!term){
                universityService.getCourse({term, page:0}).then(data=>{
                    this.suggests = data;
                })
            }
        },300),
        sendRequest() {
            let self = this;
            if(self.$refs.tutorRequestForm.validate()) {
                if(this.isAuthUser){
                    self.btnRequestLoading = true;
                    let serverObj = {
                        course: self.tutorCourse,
                        text: self.tutorRequestText,
                    };
                    let analyticsObject = {
                        userId: self.accountUser.id,
                        course: self.tutorCourse
                    }
                    analyticsService.sb_unitedEvent('Action Box', 'Request_T', `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
                    tutorService.requestTutor(serverObj)
                                .then(() => {
                                          self.tutorRequestDialogClose();
                                          self.updateToasterParams({
                                            toasterText: LanguageService.getValueByKey("tutorRequest_request_received"),
                                            showToaster: true,
                                          })
                                      },
                                      () => {
                                        self.updateToasterParams({
                                            toasterText: "We having trouble connection you to the room",
                                            showToaster: true,
                                            toasterType: 'error-toaster'
                                          })
                                      }).finally(() => {
                        self.btnRequestLoading = false;
                    });
                }else{
                    self.btnRequestLoading = true;
                    let serverObj = {
                        text: self.tutorRequestText,
                        name: self.guestName,
                        mail: self.guestMail,
                        phone: self.guestPhone,
                        course: self.tutorCourse,
                        university: self.guestUniversity
                    };
                    
                    tutorService.requestTutorAnonymous(serverObj)
                                .then(() => {
                                          self.tutorRequestDialogClose();
                                          self.updateToasterParams({
                                            toasterText: LanguageService.getValueByKey("tutorRequest_request_received"),
                                            showToaster: true,
                                          })
                                      },
                                      () => {
                                        self.updateToasterParams({
                                            toasterText: "We having trouble connection you to the room",
                                            showToaster: true,
                                            toasterType: 'error-toaster'
                                        })
                                      }).finally(() => {
                        self.btnRequestLoading = false;
                    });
                }
            }
        },
        tutorRequestDialogClose() {
            this.updateRequestDialog(false);
        },
    }
};