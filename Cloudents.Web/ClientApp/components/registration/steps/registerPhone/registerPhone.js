import stepTemplate from '../stepTemplate.vue'
import codesJson from './CountryCallingCodes'

﻿import registrationService from '../../../../services/registrationService'

export default {
    components: {stepTemplate},
    data() {
        return {
            countryCodesList: codesJson.sort((a, b) => a.name.localeCompare(b.name)),
            codeSent: false,
            confirmationCode: '',
            phone: {
                phoneNum: '',
                countryCode: ''
            }
        }
    },
    methods: {
        sendCode() {
            var self = this
            registrationService.smsRegistration(this.phone.countryCode + '' + this.phone.phoneNum)
                .then(function () {
                    self.codeSent = true;
                });
        },
        next() {
            var self = this
            registrationService.smsCodeVerification(this.confirmationCode)
                .then(function () {
                    self.$emit('next');
                });

        }
    },
}