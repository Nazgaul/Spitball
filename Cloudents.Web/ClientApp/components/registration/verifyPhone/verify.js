import stepTemplate from '../steps/stepTemplate.vue'
import codesJson from './CountryCallingCodes';
import submitDisable from '../../mixins/submitDisableMixin'
import {mapMutations, mapActions, mapGetters} from 'vuex'
import registration from '../registration.vue';
﻿import registrationService from '../../../services/registrationService'
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import {REGISTRATION_STEPS} from "../../../store/constants";

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
            },
            showDialog: false,
            toasterTimeout: 5000,

        }
    },
    watch:{
        getShowToaster: function (val) {
            console.log('val', val);
            if (val) {
                var self = this;
                setTimeout(function () {
                    console.log('timeout')
                    self.updateToasterParams({
                        showToaster: false
                    })
                }, this.toasterTimeout)
            }
        }
    },
    computed:{
        ...mapGetters({getShowToaster:'getShowToaster', getToasterText:'getToasterText'}),
    },
    methods: {
        ...mapMutations({updateLoading:"UPDATE_LOADING"}),
        ...mapActions({updateToasterParams: 'updateToasterParams'}),

        $_back() {
            let url = this.fromPath || {path: '/ask', query: {q: ''}};
            this.$router.push({...url});
        },
        showDialogFunc(){
            this.showDialog = true
        },
        hideDialog(){
            this.showDialog = false
        },
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
                        self.updateToasterParams({
                            toasterText: 'Code sent',
                            showToaster: true,
                        });
                    }, function (error) {
                        self.submitForm(false);
                        self.updateLoading(false);
                        self.errorMessage.phone = "Invalid phone number";
                    });
            }
        },
        next() {
            var self = this;
            if (this.submitForm()) {
                console.log('trying to go next')
                this.updateLoading(true);
                registrationService.smsCodeVerification(this.confirmationCode)
                    .then(function () {
                        self.updateLoading(false);
                        //got to congratulations route
                        self.$router.push({path: '/congrats'});

                    }, function (error) {
                        self.submitForm(false);
                        self.updateLoading(false);
                        self.errorMessage.code = "Invalid code";
                    });
            }
        }
    },
    //WTF
    // nextStep() {
    //     if (REGISTRATION_STEPS.indexOf(this.getRegistrationStep) >= REGISTRATION_STEPS.length) {
    //         this.$router.push({path: '/ask', query: {q: ''}});
    //         return;
    //     }
    //     // this.step++;
    //     this.incrementRegistrationStep();
    //     this.step = this.getRegistrationStep;
    // },
    created(){
        registrationService.getLocalCode().then(({data})=>{
            this.phone.countryCode=data.code;
        })

    }
}