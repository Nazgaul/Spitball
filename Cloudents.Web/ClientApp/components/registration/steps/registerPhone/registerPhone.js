import stepTemplate from '../stepTemplate.vue'
import codesJson from './CountryCallingCodes'
import submitDisable from '../../../mixins/submitDisableMixin'

﻿import registrationService from '../../../../services/registrationService'
import SbInput from "../../../question/helpers/sbInput/sbInput.vue";

export default {
    mixins:[submitDisable],
    components: {stepTemplate, SbInput},
    data() {
        return {
            countryCodesList: codesJson.sort((a, b) => a.name.localeCompare(b.name)),
            codeSent: false,
            confirmationCode: '',
            phone: {
                phoneNum: '',
                countryCode: ''
            },
            errorMessage:{
                phone:'',
                code:''
            }
        }
    },
    methods: {
        sendCode() {
            var self = this
            registrationService.smsRegistration(this.phone.countryCode + '' + this.phone.phoneNum)
                .then(function () {
                    self.codeSent = true;
                    self.errorMessage.code = '';
                }, function (error) {
                    self.submitForm(false);
                    debugger;
                    self.errorMessage.phone= error.response.data ? error.response.data["0"].description : error.message
                });
        },
        next() {
            var self = this;
            this.submitForm();
            registrationService.smsCodeVerification(this.confirmationCode)
                .then(function () {
                    self.$emit('next');
                }, function (error) {
                    self.submitForm(false);
                    debugger;
                    self.errorMessage.code= error.response.data ? error.response.data["0"].description : error.message
                });

        }
    },
}