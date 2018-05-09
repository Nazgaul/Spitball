import stepTemplate from '../stepTemplate.vue'
import codesJson from './CountryCallingCodes'
﻿import registrationService from '../../../../services/registrationService'

export default {
    components: {stepTemplate},
    data() {
        return {
            countryCodesList: codesJson,
            codeSent: false,
            confirmationCode: '',
            phone: {
                phoneNum: '',
                countryCode: ''
            }
        }
    },
    methods: {
        updateEmail() {
            this.$emit('updateEmail');
        },
        sendCode() {
            registrationService.smsRegistration(this.phone.countryCode + '' + this.phone.phoneNum)
                .then(function () {
                    this.codeSent = true;
                });
        },
        next() {
            this.$emit('next');

        }
    },
}