import stepTemplate from '../stepTemplate.vue'
import codesJson from './CountryCallingCodes'
import submitDisable from '../../../mixins/submitDisableMixin'
import {mapMutations} from 'vuex'

﻿import registrationService from '../../../../services/registrationService'
import SbInput from "../../../question/helpers/sbInput/sbInput.vue";

export default {
    mixins: [submitDisable],
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
            errorMessage: {
                phone: '',
                code: ''
            }
        }
    },
    methods: {
        ...mapMutations(["UPDATE_LOADING"]),
        sendCode() {
            this.UPDATE_LOADING(true);
            var self = this
            registrationService.smsRegistration(this.phone.countryCode + '' + this.phone.phoneNum)
                .then(function () {
                    self.UPDATE_LOADING(false);
                    self.codeSent = true;
                    self.errorMessage.code = '';
                }, function (error) {
                    self.submitForm(false);
                    self.UPDATE_LOADING(false);
                    self.errorMessage.phone = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                });
        },
        next() {
            var self = this;
            if (this.submitForm()) {
                this.UPDATE_LOADING(true);
                registrationService.smsCodeVerification(this.confirmationCode)
                    .then(function () {
                        self.UPDATE_LOADING(false);
                        self.$emit('next');
                    }, function (error) {
                        self.submitForm(false);
                        self.UPDATE_LOADING(false);
                        self.errorMessage.code = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                    });
            }
        }
    },
    created(){
        debugger;
        registrationService.getLocalCode().then(({data})=>{
            this.phone.countryCode=data.code;
        })
    }
}