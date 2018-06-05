import stepTemplate from './steps/stepTemplate.vue';
import VueRecaptcha from 'vue-recaptcha';
import registrationService from '../../services/registrationService';
import SbInput from "../question/helpers/sbInput/sbInput.vue";

export default {
    components: {stepTemplate, VueRecaptcha, SbInput},
    data() {
        return {
            userEmail: '',
            password: '',
            keepLogedIn: false,
            submitted:false,
            recaptcha: '',
            errorMessage:''
        }
    },
    computed:{
        disableSubmit: function () {
            return this.submitted || !this.userEmail || !this.password || !this.recaptcha;
        }
    },
    methods: {
        submit() {
            self = this;
                registrationService.signIn(this.userEmail, this.password, this.recaptcha)
                    .then(function () {
                        self.$router.push({path: '/note', query: {q: ''}});
                    }, function (reason) {
                        debugger;
                        self.errorMessage = reason.response.data;
                    });
        },
        onVerify(response) {
            this.recaptcha = response;
        },
        onExpired() {
            this.recaptcha = "";
        }
    },
}