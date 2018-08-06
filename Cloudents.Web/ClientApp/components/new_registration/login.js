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
            // codeSent: false,
            googleApi: false,
            confirmed: false,
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
            stepNumber: 'start',
            userEmail: this.$store.getters.getEmail || '',
            recaptcha: '',
            emailSent: false,
            agreeTerms: false,
            stepsEnum: {
                "verifyPhone": "verifyPhone",
                "enterPhone": "enterPhone",
                "emailConfirmed": "emailConfirmed",
                "congrats": "congrats"

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
        ...mapGetters(['fromPath']),
        ...mapGetters({getShowToaster: 'getShowToaster', getToasterText: 'getToasterText'}),
    },
    methods: {
        ...mapMutations({updateLoading: "UPDATE_LOADING"}),
        ...mapActions({updateToasterParams: 'updateToasterParams'}),

        //do not change step, only here
        changeStepNumber(step) {
            if (this.stepsEnum.hasOwnProperty(step)) {
                this.stepNumber = this.stepsEnum[step];
            }
            console.log('step!!!222', this.stepNumber)
        },

        googleLogIn() {
            var self = this;
            var authInstance = gapi.auth2.getAuthInstance();
            this.updateLoading(true);
            authInstance.signIn().then(function (googleUser) {
                var idToken = googleUser.getAuthResponse().id_token;
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
                console.error(error, 'errrr')
                self.updateLoading(false);
            });
        },

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
                    let step = resp.data;
                    self.changeStepNumber(step);
                }, function (error) {
                    self.updateLoading(false);
                    self.errorMessage.phone = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                })
        },
        emailSend() {
            let self = this;
            this.updateLoading(true);
            registrationService.emailRegistration(this.userEmail, this.recaptcha)
                .then(function (resp) {
                    // TODO step
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
            let url = self.fromPath || defaultSubmitRoute;
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
        registrationService.getLocalCode().then(({data}) => {
            this.phone.countryCode = data.code;
        });
        //check if new user param exists in email url
        this.isNewUser = this.$route.query['newUser'] !== undefined;

     },

}