import stepTemplate from '../steps/stepTemplate.vue'
import codesJson from './CountryCallingCodes';
import submitDisable from '../../mixins/submitDisableMixin'
import {mapMutations, mapActions, mapGetters} from 'vuex'
import registration from '../registration.vue';

﻿import registrationService from '../../../services/registrationService'
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import {REGISTRATION_STEPS} from "../../../store/constants";
import { debug } from 'util';

const defaultSubmitRoute = {path: '/ask', query: {q: ''}};

export default {
    mixins: [submitDisable],
    components: {stepTemplate, SbInput},
    props: {
        default: false,
    },
    data() {
        return {
            countryCodesList: codesJson.sort((a, b) => a.name.localeCompare(b.name)),
            codeSent: false,
            confirmed: false,
            confirmationCode: '',
            phone: {
                phoneNum: '',
                countryCode: ''
            },
            errorMessage: {
                phone: '',
                code: ''
            },
            isNewUser: false,
            showDialog: false,
            toasterTimeout: 5000,

        }
    },
    watch: {
        getShowToaster: function (val) {
            if (val) {

                var self = this;
                setTimeout(function () {
                    self.updateToasterParams({
                        showToaster: false
                    })
                }, this.toasterTimeout)
            }
        },
        // confirmed(){
        //     console.log('code', this.props.code)
        //     return  this.props.code;
        // }
    },
    computed: {
        ...mapGetters(['fromPath']),
        ...mapGetters({getShowToaster: 'getShowToaster', getToasterText: 'getToasterText'}),
        // codeWatch(){
        //     return this.code
        // }

    },
    methods: {
        ...mapMutations({updateLoading: "UPDATE_LOADING"}),
        ...mapActions({updateToasterParams: 'updateToasterParams'}),

        $_back() {
            let url = this.fromPath || {path: '/ask', query: {q: ''}};
            this.$router.push({...url});
        },
        showDialogFunc() {
            this.showDialog = true
        },
        hideDialog() {
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
                            toasterText: 'A verification code was sent to your phone',
                            showToaster: true,
                        });
                    }, function (error) {
                        self.submitForm(false);
                        self.updateLoading(false);
                        debugger;
                        self.errorMessage.phone= error.response.data ? Object.values(error.response.data)[0][0] : error.message;
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
                        //got to congratulations route
                        console.log('code in verify ', self.props);
                        if(self.isNewUser){
                            self.$router.push({path: '/congrats'});
                            return;
                        }else if(self.codeSent === true){
                            let url = self.fromPath || defaultSubmitRoute;
                            window.isAuth = true;
                            self.$router.push({...url});
                            return
                        }
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
    created() {
        registrationService.getLocalCode().then(({data}) => {
            this.phone.countryCode = data.code;
        });
        this.code = this.$route.params.code;
        this.isNewUser = this.$route.query['newUser'] !== undefined;
        if (this.code !== '' && this.code === 'enterPhone') {
            this.codeSent = false
        } else if (this.code === 'verifyPhone') {
            this.codeSent = true
        }

    }

}