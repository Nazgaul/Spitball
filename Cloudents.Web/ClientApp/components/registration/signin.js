import stepTemplate from './steps/stepTemplate.vue';
import VueRecaptcha from 'vue-recaptcha';
import registrationService from '../../services/registrationService';
import SbInput from "../question/helpers/sbInput/sbInput.vue";
import pageTemplate from "./registration.vue"
import disableForm from '../mixins/submitDisableMixin'

import {mapGetters, mapMutations, mapActions} from 'vuex'

const defaultSubmitRoute = {path: '/ask', query: {q: ''}};

export default {
    mixins: [disableForm],
    components: {stepTemplate, VueRecaptcha, SbInput, pageTemplate},
    data() {
        return {
            userEmail: '',
            //rememberMe: false,
            submitted: false,
            recaptcha: '',
            errorMessage: {
                code: '',
                email: ''
            },
            codeSent: false,
            confirmationCode: ''
        }
    },
    computed: {
        ...mapGetters(['fromPath'])
    },
    methods: {
        ...mapMutations({updateLoading: "UPDATE_LOADING"}),
        ...mapActions({updateToasterParams: 'updateToasterParams'}),
        submit() {
            this.updateLoading(true);
            self = this;
            registrationService.signIn(this.userEmail, this.recaptcha)
                .then(({ step }) => {
                    //TODO: NewSignIn step result
                    self.updateLoading(false);
                    self.codeSent = true;
                }, function (reason) {
                    self.$refs.recaptcha.reset();
                    self.updateLoading(false);
                    self.errorMessage.email = reason.response.data ? Object.values(reason.response.data)[0][0] : reason.message;
                });
        },
        resendSms() {
            registrationService.resendCode()
                .then(success => {
                        this.updateToasterParams({
                            toasterText: 'A verification code was sent to your phone',
                            showToaster: true,
                        });
                    },
                    error => {
                        console.error(error, 'sign in resend error')
                    })
        },
        verifyCode() {
            var self = this;
            if (this.submitForm()) {
                self.updateLoading(false);
                registrationService.smsCodeVerification(this.confirmationCode)
                    .then(function () {
                        self.updateLoading(false);
                        let url = self.fromPath || defaultSubmitRoute;
                        window.isAuth = true;
                        self.$router.push({...url});
                    }, function (reason) {
                        self.updateLoading(false);
                        self.submitForm(false)
                        self.errorMessage.code = "Invalid code";
                    });
            }
        },
        onVerify(response) {
            this.recaptcha = response;
        },
        onExpired() {
            this.recaptcha = "";
        }
    }
}