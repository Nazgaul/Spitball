import stepTemplate from './helpers/stepTemplate.vue'
import codesJson from './helpers/CountryCallingCodes';
import {mapMutations, mapActions, mapGetters} from 'vuex'
import VueRecaptcha from 'vue-recaptcha';

﻿import registrationService from '../../services/registrationService'
import analyticsService from '../../services/analytics.service';
import SbInput from "../question/helpers/sbInput/sbInput.vue";

const defaultSubmitRoute = {path: '/ask'};
const initialPointsNum = 100;
var auth2;

export default {
    components: {stepTemplate, SbInput, VueRecaptcha},
    props: {
        default: false,
    },
    data() {
        return {
            siteKey: '6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1',
            gaCategory: '',
            marketingData: {

            },
            loader: null,
            loading: false,
            countryCodesList: codesJson.sort((a, b) => a.name.localeCompare(b.name)),
            toUrl: '',
            progressSteps: 5,
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
            stepsEnum: {
                "startstep": 1,
                "emailconfirmed": 2,
                "enterphone": 3,
                "verifyphone": 4,
                "congrats": 5,
                "loginstep": 6,
                "expiredstep": 7
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
        ...mapGetters({
            getShowToaster: 'getShowToaster',
            getToasterText: 'getToasterText',
            lastActiveRoute: 'lastActiveRoute',
            campaignName: 'getCampaignName',
            campaignData: 'getCampaignData'
        }),
    },
    methods: {
        ...mapMutations({updateLoading: "UPDATE_LOADING"}),
        ...mapActions({updateToasterParams: 'updateToasterParams', updateCampaign: 'updateCampaign'}),

        //do not change step, only from here
        changeStepNumber(param) {
            let step = param.toLowerCase();
            console.log(step)
            if (this.stepsEnum.hasOwnProperty(step)) {
                this.stepNumber = this.stepsEnum[step];
            }
            console.log(this.stepNumber)
        },
        goToLogin() {
            this.changeStepNumber('loginStep');
        },
        showRegistration() {
            this.changeStepNumber('startStep');
        },
        submit() {
            let self = this;
            self.loading = true;
            registrationService.signIn(this.userEmail, this.recaptcha)
                .then((response) => {
                    self.loading = false;
                    analyticsService.sb_unitedEvent('Login', 'Start');
                    let step = response.data.step;
                    self.changeStepNumber(step)
                }, function (reason) {
                    self.$refs.recaptcha.reset();
                    self.loading = false;
                    self.errorMessage.email = reason.response.data ? Object.values(reason.response.data)[0][0] : reason.message;
                });
        },

        googleLogIn() {
            var self = this;
            self.updateLoading(true);
            let authInstance = gapi.auth2.getAuthInstance();
            authInstance.signIn().then(function (googleUser) {
                let idToken = googleUser.getAuthResponse().id_token;
                registrationService.googleRegistration(idToken)
                    .then(function (resp) {
                        self.updateLoading(false);
                        let newUser = resp.data.isNew;
                        console.log(newUser);
                        if (newUser) {
                            analyticsService.sb_unitedEvent('Registration', 'Start Google');
                        } else {
                            analyticsService.sb_unitedEvent('Login', 'Start Google');
                        }
                        let step = resp.data.step;
                        self.changeStepNumber(step);

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
            this.$router.push({path: `${url.path }`});
        },
        showDialogFunc() {
            this.showDialog = true
        },
        hideDialog() {
            this.showDialog = false
        },
        //sms code
        sendCode() {
            let self = this;
            self.loading = true;
            registrationService.smsRegistration(this.phone.countryCode + '' + this.phone.phoneNum)
                .then(function (resp) {
                    self.errorMessage.code = '';
                    self.updateToasterParams({
                        toasterText: 'A verification code was sent to your phone',
                        showToaster: true,
                    });
                    self.loading = false;
                    analyticsService.sb_unitedEvent('Registration', 'Phone Submitted');
                    self.changeStepNumber('verifyPhone');
                }, function (error) {
                    self.loading = false;
                    self.errorMessage.phone = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                })
        },
        resendSms() {
            let self = this;
            self.updateLoading(true);
            analyticsService.sb_unitedEvent('Registration', 'Resend SMS');
            registrationService.resendCode()
                .then((success) => {
                        self.updateLoading(false);
                        self.updateToasterParams({
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
            self.loading = true;
            registrationService.emailRegistration(this.userEmail, this.recaptcha)
                .then(function (resp) {
                    let step = resp.data.step;
                    self.changeStepNumber(step);
                    analyticsService.sb_unitedEvent('Registration', 'Start');
                    self.loading = false;
                }, function (error) {
                    self.recaptcha = "";
                    self.$refs.recaptcha.reset();
                    self.loading = false;
                    self.errorMessage = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                });
        },
        resendEmail() {
            var self = this;
            self.updateLoading(true);
            analyticsService.sb_unitedEvent('Registration', 'Resend Email');
            registrationService.emailResend()
                .then(response => {
                        self.updateLoading(false);
                        self.updateToasterParams({
                            toasterText: 'Email sent',
                            showToaster: true,
                        })
                    },
                    error => {
                        self.updateLoading(false);
                        console.error('resent error', error)
                    })
        },
        smsCodeVerify() {
            let self = this;
            self.loading = true;
            registrationService.smsCodeVerification(this.confirmationCode)
                .then(function () {
                    //got to congratulations route if new user
                    if (self.isNewUser) {
                        self.changeStepNumber('congrats');
                        analyticsService.sb_unitedEvent('Registration', 'Phone Verified');
                        self.loading = false;

                    } else {
                        self.loading = false;
                        analyticsService.sb_unitedEvent('Login', 'Phone Verified');
                        let url = self.lastActiveRoute || defaultSubmitRoute;
                        window.isAuth = true;
                        self.$router.push({path: `${url.path }`});

                    }
                }, function (error) {
                    self.loading = false;
                    self.errorMessage.code = "Invalid code";
                });
        },
        finishRegistration() {
            this.loading = true;
            analyticsService.sb_unitedEvent('Registration', 'Congrats');
            let url = this.toUrl || defaultSubmitRoute;
            window.isAuth = true;
            this.loading = false;
            this.$router.push({path: `${url.path }`});
        }
    },
    mounted() {
        this.$nextTick(function () {
            gapi.load('auth2', function () {
                auth2 = gapi.auth2.init({
                    client_id: '341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com',
                })
            })
        })
    },
    created() {
        // if(!this.campaignName || this.campaignName === '' ){
        //     this.updateCampaign('noCampaign');
        // }
        //check if returnUrl exists
        if (!!this.$route.query.returnUrl) {
            this.toUrl = {path: `${this.$route.query.returnUrl}`, query: {q: ''}};
        }
        if (this.$route.query && this.$route.query.step) {
            let step = this.$route.query.step;
            this.changeStepNumber(step);
        } else if (this.$route.fullPath === '/signin') {
            this.changeStepNumber('loginStep')
        }

        registrationService.getLocalCode().then(({data}) => {
            this.phone.countryCode = data.code;
        });
        //check if new user param exists in email url
        this.isNewUser = this.$route.query['newUser'] !== undefined;
        if (this.isNewUser && this.stepNumber === 3) {
            analyticsService.sb_unitedEvent('Registration', 'Email Verified');

        }

    },
}