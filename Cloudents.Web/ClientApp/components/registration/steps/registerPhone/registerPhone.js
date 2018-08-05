import stepTemplate from '../stepTemplate.vue'
import codesJson from './CountryCallingCodes'
import submitDisable from '../../../mixins/submitDisableMixin'
import {mapMutations, mapActions} from 'vuex'

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
        ...mapMutations({updateLoading:"UPDATE_LOADING"}),
        ...mapActions({
            updateToasterParams: 'updateToasterParams'
        }),
        sendCode() {
            this.updateLoading(true);
            var self = this;
            if (this.submitForm()) {
                registrationService.smsRegistration(this.phone.countryCode + '' + this.phone.phoneNum)
                    .then(function () {
                        self.updateLoading(false);
                        self.codeSent = true;
                        self.submitForm(false);
                        self.errorMessage.code = '';
                        this.updateToasterParams({
                            toasterText: 'Code sent',
                            showToaster: true,
                        });
                    }, function (error) {
                        self.submitForm(false);
                        self.updateLoading(false);
                         debugger;

                        self.errorMessage.phone = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                    });
            }
        },
        next() {
            var self = this;
            if (this.submitForm()) {
                this.updateLoading(true);
                registrationService.smsCodeVerification(this.confirmationCode)
                    .then(function () {
                        self.updateLoading(false);
                        self.$emit('next');
                    }, function (error) {
                        // debugger;
                        self.submitForm(false);
                        self.updateLoading(false);
                        self.errorMessage.code = "Invalid code";
                    });
            }
        }
    },
    created(){
        //debugger;
        registrationService.getLocalCode().then(({data})=>{
            this.phone.countryCode=data.code;
        })
    }
}