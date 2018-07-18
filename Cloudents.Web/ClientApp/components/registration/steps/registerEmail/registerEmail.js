import stepTemplate from '../stepTemplate.vue';

import registrationService from '../../../../services/registrationService';
import VueRecaptcha from 'vue-recaptcha';
import disableForm from '../../../mixins/submitDisableMixin'
import SbInput from '../../../question/helpers/sbInput/sbInput.vue';
import {mapMutations, mapActions} from 'vuex'

var auth2;

export default {
    components: {stepTemplate, VueRecaptcha, SbInput},
    mixins: [disableForm],
    data() {
        return {
            userEmail: this.$store.getters.getEmail || '',
            recaptcha: '',
            emailSent: false,
            agreeTerms: false,
            errorMessage: ''
        }
    },
    methods: {
        ...mapMutations({updateLoading: "UPDATE_LOADING"}),
        ...mapActions({
            updateToasterParams: 'updateToasterParams'
        }),
        next() {
            self = this;
            if (this.submitForm()) {
                this.updateLoading(true);
                registrationService.emailRegistration(this.userEmail, this.recaptcha)
                    .then(function (resp) {
                        // TODO step
                        let step = resp.data.step;
                        console.log('step', resp);
                        if (step === "emailConfirmed") {
                            self.emailSent = true;
                        } else {
                            self.$router.push({name: 'phoneVerify', params: {code: `${step}`}});
                        }

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
                            console.log('google', step);
                            self.$router.push({name: 'phoneVerify', params: {code: `${step}`}});
                            // }
                            self.updateLoading(false);
                            // self.$emit('next');
                        }, function (error) {
                            //TODO: duplicate callback
                            self.updateLoading(false);
                            self.submitForm(false);
                            self.errorMessage = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                            console.error(error);
                        });
                }, function (error) {
                    console.error(reason);
                    self.updateLoading(false);
                });
            }
        },
        resend() {
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
        onVerify(response) {
            this.recaptcha = response;
        },
        onExpired() {
            this.recaptcha = "";
        }
    },
    beforeCreate() {
        gapi.load('auth2', function () {
            auth2 = gapi.auth2.init({
                client_id: '341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com',
            });
        });
    },
    created() {
        console.log(this.$route.params.code);
        if (this.$route.params.code && this.$route.params.code === 'emailConfirmed') {
            this.emailSent = true;
        } else if (this.$route.params.code && this.$route.params.code === 'verifyPhone') {
            this.$router.push({name: 'phoneVerify', params: {code: `${step}`}});
        }

    }
};


