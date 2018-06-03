import stepTemplate from '../stepTemplate.vue'
import codesJson from './CountryCallingCodes'
import submitDisable from '../../../mixins/submitDisableMixin'

﻿import registrationService from '../../../../services/registrationService'

export default {
    mixins:[submitDisable],
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
            var self = this;
            this.submitForm();
            registrationService.smsCodeVerification(this.confirmationCode)
                .then(function () {
                    self.$emit('next');
                });

        }
    },
}