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
        ...mapActions({updateToasterParams: 'updateToasterParams'
        }),

        next() {
            self = this;
            if (this.submitForm()) {
                this.updateLoading(true);
                registrationService.emailRegistration(this.userEmail, this.recaptcha)
                    .then(function () {
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
                console.log('INST:',authInstance);
                authInstance.signIn().then(function (googleUser) {
                    var idToken = googleUser.getAuthResponse().id_token;
                    console.log('Token:', idToken);
                    registrationService.googleRegistration(idToken)
                        .then(function () {
                            self.updateLoading(false);
                            console.log('got here')
                            self.$emit('next');
                        }, function (reason) {
                            self.updateLoading(false);
                            self.submitForm(false);
                            console.error(reason, 'res');
                        });
                }, function (error) {
                    console.log(error,'errrr')
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
                        console.error('resent error',error)
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
    }
};


