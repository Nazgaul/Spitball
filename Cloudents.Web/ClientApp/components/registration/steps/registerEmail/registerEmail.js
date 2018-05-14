import stepTemplate from '../stepTemplate.vue'

﻿import registrationService from '../../../../services/registrationService'

var auth2;

export default {
    components: {stepTemplate},
    data() {
        return {
            userEmail: this.$store.getters.getEmail || '',
            emailSent: false,
            errorMessages: ''
        }
    },
    methods: {
        next() {
            self = this;
            registrationService.emailRegistration(this.userEmail)
                .then(function () {
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
        }
    },
    beforeCreate() {
        gapi.load('auth2', function () {
            auth2 = gapi.auth2.init({
                client_id: '341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com',
            });
        });
    }
}


