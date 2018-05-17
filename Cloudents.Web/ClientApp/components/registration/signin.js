import stepTemplate from './steps/stepTemplate.vue';
import VueRecaptcha from 'vue-recaptcha';
import registrationService from '../../services/registrationService';

export default {
    components: {stepTemplate, VueRecaptcha},
    data() {
        return {
            userEmail: '',
            password: '',
            keepLogedIn: false,
            recaptcha: '',
        }
    },
    methods: {
        submit() {
            self = this;
            registrationService.signIn(this.userEmail, this.password, this.recaptcha)
                .then(function () {
                    this.$router.push({path: '/note', query: {q: ''}});
                }, function (reason) {
                    debugger;
                    console.error(reason);
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