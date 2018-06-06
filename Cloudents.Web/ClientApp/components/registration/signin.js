import stepTemplate from './steps/stepTemplate.vue';
import VueRecaptcha from 'vue-recaptcha';
import registrationService from '../../services/registrationService';
import SbInput from "../question/helpers/sbInput/sbInput.vue";
import {mapGetters, mapMutations} from 'vuex'

const defaultSubmitRoute = {path: '/note', query: {q: ''}};

export default {
    components: {stepTemplate, VueRecaptcha, SbInput},
    data() {
        return {
            userEmail: '',
            password: '',
            rememberMe: false,
            submitted: false,
            recaptcha: '',
            errorMessage: '',
        }
    },
    computed: {
        ...mapGetters(['unAuthPath'])
    },
    methods: {
        ...mapMutations(["UPDATE_LOADING"]),
        submit() {
            this.UPDATE_LOADING(true);
            self = this;
            registrationService.signIn(this.userEmail, this.password, this.recaptcha, this.remeberMe)
                .then(function () {
                    self.UPDATE_LOADING(false);
                    let url = self.unAuthPath || defaultSubmitRoute;
                    window.isAuth = true;
                    self.$router.push({...url});
                }, function (reason) {
                    self.UPDATE_LOADING(false);
                    self.errorMessage = reason.response.data;
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