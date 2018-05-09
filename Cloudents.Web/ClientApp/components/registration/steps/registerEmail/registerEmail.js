import {mapGetters, mapActions} from 'vuex'
import stepTemplate from '../stepTemplate.vue'
﻿import registrationService from '../../../../services/registrationService'
var auth2;

export default {
    components: {stepTemplate},
    data() {
        return {
            userEmail: this.$store.getters.getEmail || '',
            emailSent: false
        }
    },
    computed: {
        ...mapGetters(['getEmail']),
    },
    methods: {
        ...mapActions(['updateEmail']),
        next() {
            this.updateEmail(this.userEmail);
            this.emailSent = true;
        },
        googleLogIn() {
            var self = this;
            auth2.grantOfflineAccess().then(function (authResult) {
                if (authResult['code']) {
                    registrationService.googleRegistration({token: authResult['code']})
                        .then(function () {
                            self.$emit('next');
                        });
                } else {
                    //TODO: handle the error
                }
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


