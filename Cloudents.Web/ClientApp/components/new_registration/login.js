import stepTemplate from './helpers/stepTemplate.vue'
import codesJson from './helpers/CountryCallingCodes';
import submitDisable from '../mixins/submitDisableMixin'
import {mapMutations, mapActions, mapGetters} from 'vuex'
import VueRecaptcha from 'vue-recaptcha';
﻿import registrationService from '../../services/registrationService'
import SbInput from "../question/helpers/sbInput/sbInput.vue";

const defaultSubmitRoute = {path: '/ask', query: {q: ''}};

export default {
    mixins: [submitDisable],
    components: {stepTemplate, SbInput, VueRecaptcha},
    props: {
        default: false,
    },
    data() {
        return {
            countryCodesList: codesJson.sort((a, b) => a.name.localeCompare(b.name)),
            codeSent: false,
            confirmed: false,
            confirmationCode: '',
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
            stepNumber :{
                type: Number,
                required: true,
                default: 1
            },
            userEmail: this.$store.getters.getEmail || '',
            recaptcha: '',
            emailSent: false,
            agreeTerms: false,
            // errorMessage: ''


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
        // codeWatch(){
        //     return this.code
        // }

    },
    methods: {
        ...mapMutations({updateLoading: "UPDATE_LOADING"}),
        ...mapActions({updateToasterParams: 'updateToasterParams'}),
        googleLogIn() {
            var self = this;

            var authInstance = gapi.auth2.getAuthInstance();
            if (this.submitForm()) {
                this.updateLoading(true);
                authInstance.signIn().then(function (googleUser) {
                    var idToken = googleUser.getAuthResponse().id_token;
                    registrationService.googleRegistration(idToken)
                        .then(function (resp) {
                            let step = resp.data.step;
                            self.$router.push({name: 'phoneVerify', params: {code: `${step}`}});
                            self.updateLoading(false);
                        }, function (error) {
                            self.updateLoading(false);
                            self.submitForm(false);
                            self.errorMessage = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                            console.error(error);
                        });
                }, function (error) {
                    console.error(error,'errrr')
                    self.updateLoading(false);
                });
            }
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
            var self = this;
            if (this.submitForm()) {
                registrationService.smsRegistration(this.phone.countryCode + '' + this.phone.phoneNum)
                    .then(function () {
                        self.updateLoading(false);
                        self.codeSent = true;
                        self.submitForm(false);
                        self.errorMessage.code = '';
                        self.updateToasterParams({
                            toasterText: 'A verification code was sent to your phone',
                            showToaster: true,
                        });
                    }, function (error) {
                        self.submitForm(false);
                        self.updateLoading(false);
                        debugger;
                        self.errorMessage.phone= error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                    });
            }
        },
        emailSend() {
            self = this;
            if (this.submitForm()) {
                this.updateLoading(true);
                console.log('111 email',this.userEmail, this.recaptcha)
                registrationService.emailRegistration(this.userEmail, this.recaptcha)
                    .then(function (resp) {
                        // TODO step
                        let step = resp.data.step;
                        console.log('step', resp);

                        // if (step === "emailConfirmed") {
                        //     self.emailSent = true;
                        // } else {
                        //     self.$router.push({name: 'phoneVerify', params: {code: `${step}`}});
                        // }
                        self.updateLoading(false);
                        self.emailSent = true;
                    }, function (error) {
                        self.updateLoading(false);
                        self.recaptcha = "";
                        self.$refs.recaptcha.reset();
                        self.submitForm(false);
                        self.errorMessage = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                    });
            }
        },
        // next() {
        //     var self = this;
        //     if (this.submitForm()) {
        //         this.updateLoading(true);
        //         registrationService.smsCodeVerification(this.confirmationCode)
        //             .then(function () {
        //                 self.updateLoading(false);
        //                 //got to congratulations route
        //                 console.log('code in verify ', self.props);
        //                 if(self.isNewUser){
        //                     self.$router.push({path: '/congrats'});
        //                     return;
        //                 }else if(self.codeSent === true){
        //                     let url = self.fromPath || defaultSubmitRoute;
        //                     window.isAuth = true;
        //                     self.$router.push({...url});
        //                     return
        //                 }
        //                 self.$router.push({path: '/congrats'});
        //             }, function (error) {
        //                 self.submitForm(false);
        //                 self.updateLoading(false);
        //                 self.errorMessage.code = "Invalid code";
        //             });
        //     }
        // }
    },

    created() {
        this.stepNumber = 1;
        registrationService.getLocalCode().then(({data}) => {
            this.phone.countryCode = data.code;
        });
        this.code = this.$route.params.code;
        this.isNewUser = this.$route.query['newUser'] !== undefined;
        if (this.code !== '' && this.code === 'enterPhone') {
            this.codeSent = false
        } else if (this.code === 'verifyPhone') {
            this.codeSent = true
        }

    }

}