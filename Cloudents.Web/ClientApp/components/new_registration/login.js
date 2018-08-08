import stepTemplate from './helpers/stepTemplate.vue'
import codesJson from './helpers/CountryCallingCodes';
import {mapMutations, mapActions, mapGetters} from 'vuex'
import VueRecaptcha from 'vue-recaptcha';

﻿import registrationService from '../../services/registrationService'
import SbInput from "../question/helpers/sbInput/sbInput.vue";

const defaultSubmitRoute = {path: '/ask', query: {q: ''}};
const initialPointsNum = 100;
var auth2;

export default {
    components: {stepTemplate, SbInput, VueRecaptcha},
    props: {
        default: false,
    },
    data() {
        return {
            countryCodesList: codesJson.sort((a, b) => a.name.localeCompare(b.name)),
            progressSteps: 0,
            confirmationCode: '',
            initialPointsNum,
            phone: {
                phoneNum: '',
                countryCode: ''
            },
            errorMessage: {
                phone: '',
                code: ''
            },
            isNewUser: false,
            showDialog: false,
            toasterTimeout: 5000,
            stepNumber: 1,
            userEmail: this.$store.getters.getEmail || '',
            recaptcha: '',
            agreeTerms: false,
            stepsEnum: {
                "startStep": 1,
                "emailConfirmed": 2,
                "enterPhone": 3,
                "verifyPhone": 4,
                "congrats": 5,
                "loginStep": 6
            }
        }
    },
    watch: {
        getShowToaster: function (val) {
            if (val) {
                var self = this;
                setTimeout(function () {
                    self.updateToasterParams({
                        showToaster: false
                    })
                }, this.toasterTimeout)
            }
        },

    },
    computed: {
        ...mapGetters({getShowToaster: 'getShowToaster', getToasterText: 'getToasterText', fromPath: 'fromPath'}),
    },
    methods: {
        ...mapMutations({updateLoading: "UPDATE_LOADING"}),
        ...mapActions({updateToasterParams: 'updateToasterParams'}),

        //do not change step, only from here
        changeStepNumber(step) {
            if (this.stepsEnum.hasOwnProperty(step)) {
                this.stepNumber = this.stepsEnum[step];
            }
            console.log('step name-::', step, 'step number-::', this.stepNumber);
        },
        goToLogin(){
            this.changeStepNumber('loginStep');
        },
        submit() {
            this.updateLoading(true);
            self = this;
            registrationService.signIn(this.userEmail, this.recaptcha)
                .then((response) => {
                    self.updateLoading(false);
                    let step = response.data.step;
                    console.log('signIn step func', step)
                    this.changeStepNumber(step)
                }, function (reason) {
                    self.$refs.recaptcha.reset();
                    self.updateLoading(false);
                    self.errorMessage.email = reason.response.data ? Object.values(reason.response.data)[0][0] : reason.message;
                });
        },

        googleLogIn() {
            var self = this;
            let authInstance = gapi.auth2.getAuthInstance();
            this.updateLoading(true);
            authInstance.signIn().then(function (googleUser) {
                let idToken = googleUser.getAuthResponse().id_token;
                registrationService.googleRegistration(idToken)
                    .then(function (resp) {
                        let step = resp.data.step;
                        self.changeStepNumber(step);
                        self.updateLoading(false);
                    }, function (error) {
                        self.updateLoading(false);
                        self.errorMessage = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                        console.error(error);
                    });
            }, function (error) {
                self.updateLoading(false);
            });
        },
        // captcha events methods
        onVerify(response) {
            this.recaptcha = response;
        },
        onExpired() {
            this.recaptcha = "";
        },

        $_back() {
            let url = this.fromPath || {path: '/ask', query: {q: ''}};
            this.$router.push({...url});
        },
        showDialogFunc() {
            this.showDialog = true
        },
        hideDialog() {
            this.showDialog = false
        },
        //sms code
        sendCode() {
            this.updateLoading(true);
            let self = this;
            registrationService.smsRegistration(this.phone.countryCode + '' + this.phone.phoneNum)
                .then(function (resp) {
                    self.updateLoading(false);
                    self.errorMessage.code = '';
                    self.updateToasterParams({
                        toasterText: 'A verification code was sent to your phone',
                        showToaster: true,
                    });
                    self.changeStepNumber('verifyPhone');
                }, function (error) {
                    self.updateLoading(false);
                    self.errorMessage.phone = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                })
        },
        resendSms() {
            registrationService.resendCode()
                .then((success) => {
                        this.updateToasterParams({
                            toasterText: 'A verification code was sent to your phone',
                            showToaster: true,
                        });
                    },
                    error => {
                        self.errorMessage = error.text;
                        console.error(error, 'sign in resend error')
                    })
        },
        emailSend() {
            let self = this;
            this.updateLoading(true);
            registrationService.emailRegistration(this.userEmail, this.recaptcha)
                .then(function (resp) {
                    let step = resp.data.step;
                    self.changeStepNumber(step);
                    self.updateLoading(false);
                }, function (error) {
                    self.updateLoading(false);
                    self.recaptcha = "";
                    self.$refs.recaptcha.reset();
                    self.errorMessage = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                });
        },
        resendEmail() {
            registrationService.emailResend()
                .then(response => {
                        this.updateToasterParams({
                            toasterText: 'Email sent',
                            showToaster: true,
                        })
                    },
                    error => {
                        console.error('resent error', error)
                    })
        },
        smsCodeVerify() {
            let self = this;
            this.updateLoading(true);
            registrationService.smsCodeVerification(this.confirmationCode)
                .then(function () {
                    self.updateLoading(false);
                    //got to congratulations route if new user
                    if (self.isNewUser) {
                        self.changeStepNumber('congrats')
                    } else {
                        let url = self.fromPath || defaultSubmitRoute;
                        window.isAuth = true;
                        self.$router.push({...url});
                        return
                    }
                }, function (error) {
                    self.updateLoading(false);
                    self.errorMessage.code = "Invalid code";
                });
        },
        finishRegistration() {
            let url = this.fromPath || defaultSubmitRoute;
            window.isAuth = true;
            this.$router.push({...url});
        }
    },
    mounted() {
        // TODO try to fix and use without timeout
        setTimeout(function () {
            gapi.load('auth2', function () {
                auth2 = gapi.auth2.init({
                    client_id: '341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com',
                })
            })
        }, 500);
    },
    created() {
        if (this.$route.fullPath === '/login') {
            this.changeStepNumber('loginStep');
        } else if (this.$route.query && this.$route.query.step) {
            let step = this.$route.query.step;
            this.changeStepNumber(step);
        } else {
            let step = 'startStep';
            this.changeStepNumber(step);
        }
        registrationService.getLocalCode().then(({data}) => {
            this.phone.countryCode = data.code;
        });
        //check if new user param exists in email url
        this.isNewUser = this.$route.query['newUser'] !== undefined;
        //define number of steps to complete
        if (this.isNewUser) {
            this.progressSteps = 6; // will show congrats page
        }
        else {
            this.progressSteps = 5;
        }


    },

}