import stepTemplate from './steps/stepTemplate.vue';
import VueRecaptcha from 'vue-recaptcha';
import registrationService from '../../services/registrationService';
import SbInput from "../question/helpers/sbInput/sbInput.vue";
import {mapGetters, mapMutations} from 'vuex'

const defaultSubmitRoute = {path: '/ask', query: {q: ''}};

export default {
    components: {stepTemplate, VueRecaptcha, SbInput},
    data() {
        return {
            userEmail: '',
            rememberMe: false,
            submitted: false,
            recaptcha: '',
            errorMessage:{
                code:'',
                email:''
            },
            codeSent: false,
            confirmationCode: ''
        }
    },
    computed: {
        ...mapGetters(['fromPath'])
    },
    methods: {
        ...mapMutations({updateLoading:"UPDATE_LOADING"}),
        submit() {
            this.updateLoading(true);
            self = this;
            registrationService.signIn(this.userEmail, this.recaptcha, this.rememberMe)
                .then(function () {
                    self.updateLoading(false);
                    self.codeSent = true;
                }, function (reason) {
                    self.updateLoading(false);
                    self.errorMessage.email = reason.response.data ? Object.values(reason.response.data)[0][0] : reason.message;
                });
        },
        verifyCode() {
            registrationService.smsCodeVerification(this.confirmationCode)
                .then(function () {
                    self.updateLoading(false);
                    let url = self.fromPath || defaultSubmitRoute;
                    window.isAuth = true;
                    self.$router.push({...url});
                }, function (reason) {
                    self.updateLoading(false);
                    self.errorMessage.code = reason.response.data;
                });
        },
        onVerify(response) {
            this.recaptcha = response;
        },
        onExpired() {
            this.recaptcha = "";
        }
    }
}