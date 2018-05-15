import stepTemplate from '../stepTemplate.vue'

﻿import registrationService from '../../../../services/registrationService'
import VueRecaptcha from 'vue-recaptcha';

var auth2;

export default {
    components: {stepTemplate, VueRecaptcha},
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
            registrationService.emailRegistration(this.userEmail, this.recaptcha)
                .then(function () {
                    debugger;
                    self.emailSent = true
                }, function (reason) {
                });
        },
        googleLogIn() {
            var self = this;

            var authInstance = gapi.auth2.getAuthInstance();

            authInstance.signIn().then(function (googleUser) {
                debugger;
                var idToken = googleUser.getAuthResponse().id_token;
                registrationService.googleRegistration(idToken)
                    .then(function () {
                        self.$emit('next');
                    }, function (reason) {
                        debugger;
                        // rejection
                    });
            });
        },
        onVerify(response) {
            this.recaptcha = response;
            debugger;
            console.log('Verify: ' + response)
        },
    },
    beforeCreate() {
        gapi.load('auth2', function () {
            auth2 = gapi.auth2.init({
                client_id: '341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com',
            });
        });
    }
}


