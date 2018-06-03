import stepTemplate from '../stepTemplate.vue';

import registrationService from '../../../../services/registrationService';
import VueRecaptcha from 'vue-recaptcha';

var auth2;

export default {
    components: {stepTemplate, VueRecaptcha},
    mixins:[disableForm],
    data() {
        return {
            userEmail: this.$store.getters.getEmail || '',
            recaptcha: '',
            emailSent: false,
            errorMessages: ''
        }
    },
    methods: {
        next() {
            self = this;
            this.submitForm();
            debugger;
            registrationService.emailRegistration(this.userEmail, this.recaptcha)
                .then(function () {
                    self.emailSent = true;
                }, function (reason) {
                    self.submitForm(false);
                    console.error(reason);
                });
        },
        googleLogIn() {
            var self = this;

            var authInstance = gapi.auth2.getAuthInstance();
            this.submitForm();
            authInstance.signIn().then(function (googleUser) {
                debugger;
                var idToken = googleUser.getAuthResponse().id_token;
                registrationService.googleRegistration(idToken)
                    .then(function () {
                        self.$emit('next');
                    }, function (reason) {
                        debugger;
                        self.submitForm(false);
                        console.error(reason);
                    });
            });
        },
        resend(){
            registrationService.emailResend()
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


