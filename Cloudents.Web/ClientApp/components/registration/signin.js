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
        ...mapMutations(["UPDATE_LOADING"]),
        submit() {
            this.UPDATE_LOADING(true);
            self = this;
            registrationService.signIn(this.userEmail, this.recaptcha, this.rememberMe)
                .then(function () {
                    self.UPDATE_LOADING(false);
                    self.codeSent = true;
                }, function (reason) {
                    self.UPDATE_LOADING(false);
                    self.errorMessage.email = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                });
        },
        verifyCode() {
            registrationService.smsCodeVerification(this.confirmationCode)
                .then(function () {
                    self.UPDATE_LOADING(false);
                    let url = self.fromPath || defaultSubmitRoute;
                    window.isAuth = true;
                    self.$router.push({...url});
                }, function (reason) {
                    self.UPDATE_LOADING(false);
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