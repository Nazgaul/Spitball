import { mapActions, mapGetters, mapMutations } from 'vuex';
import UserAvatar from '../helpers/UserAvatar/UserAvatar.vue';
import tutorService from "../../services/tutorService";
import { LanguageService } from "../../services/language/languageService";
import { validationRules } from "../../services/utilities/formValidationRules";
import analyticsService from '../../services/analytics.service';

export default {
    components: {
        UserAvatar,
        FileUpload
    },
    data() {
        return {
            tutorCourse: '',
            tutorRequestText: '',
            btnRequestLoading: false,
            validRequestTutorForm: false,
            guestName: '',
            guestMail: '',
            guestPhone: '',
            rules: {
                required: (value) => validationRules.required(value),
                maximumChars: (value) => validationRules.maximumChars(value, 255)
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
        ...mapGetters(['accountUser', 'getSelectedClasses', 'getRequestTutorDialog']),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        userImageUrl() {
            if(this.isAuthUser && this.accountUser.image.length > 1) {
                return `${this.accountUser.image}`;
            }
            return '';
        },
        userName() {
            if (this.isAuthUser) {
            return this.accountUser.name
            }
            else {
                return 'JD';
            }
        },
        isAuthUser(){
            return !!this.accountUser;
        }
    },
    methods: {
        ...mapActions(['updateRequestDialog', 'updateToasterParams']),
        ...mapMutations(['UPDATE_LOADING']),
        sendRequest() {
            console.log(this.guestMail)
            // let self = this;
            // if(self.$refs.tutorRequestForm.validate()) {
            //     if(this.isAuthUser){
            //         self.btnRequestLoading = true;
            //         let serverObj = {
            //             course: self.tutorCourse,
            //             text: self.tutorRequestText,
            //         };
            //         let analyticsObject = {
            //             userId: self.accountUser.id,
            //             course: self.tutorCourse
            //         }
            //         analyticsService.sb_unitedEvent('Action Box', 'Request_T', `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
            //         tutorService.requestTutor(serverObj)
            //                     .then(() => {
            //                               self.tutorRequestDialogClose();
            //                               self.updateToasterParams({
            //                                 toasterText: LanguageService.getValueByKey("tutorRequest_request_received"),
            //                                 showToaster: true,
            //                               })
            //                           },
            //                           () => {
            //                           }).finally(() => {
            //             self.btnRequestLoading = false;
            //         });
            //     }else{
            //         self.btnRequestLoading = true;
            //         let serverObj = {
            //             text: self.tutorRequestText,
            //             name: self.guestName,
            //             mail: self.guestMail,
            //             phone: self.guestPhone
            //         };
                    
            //         tutorService.requestTutorAnonymous(serverObj)
            //                     .then(() => {
            //                               self.tutorRequestDialogClose();
            //                               self.updateToasterParams({
            //                                 toasterText: LanguageService.getValueByKey("tutorRequest_request_received"),
            //                                 showToaster: true,
            //                               })
            //                           },
            //                           () => {
            //                           }).finally(() => {
            //             self.btnRequestLoading = false;
            //         });
            //     }
            // }
        },

        tutorRequestDialogClose() {
            this.updateRequestDialog(false);
        },
    }
};