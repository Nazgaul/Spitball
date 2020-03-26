import { validationRules } from '../../services/utilities/formValidationRules';

import registrationService from '../../services/registrationService2';

import analyticsService from '../../services/analytics.service.js';

import * as routeNames from '../../routes/routeNames';

export default {

    data() {
        return {
            googleLoading: false,
            routeNames,
            labels: {
                fname: this.$t('loginRegister_setemailpass_first'),
                lname: this.$t('loginRegister_setemailpass_last'),
                email: this.$t('loginRegister_setemailpass_input_email'),
                genderMale: this.$t('loginRegister_setemailpass_male'),
                genderFemale: this.$t('loginRegister_setemailpass_female'),
                password: this.$t('loginRegister_setemailpass_input_pass'),
            },
            score: {
                default: 0,
                required: false
            },
            passScoreObj: {
                0: { name: this.$t("login_password_indication_weak"), className: "bad" },
                1: { name: this.$t("login_password_indication_weak"), className: "bad" },
                2: { name: this.$t("login_password_indication_strong"), className: "good" },
                3: { name: this.$t("login_password_indication_strong"), className: "good" },
                4: { name: this.$t("login_password_indication_strongest"), className: "best" }
            },
            rules: {
                required: (value) => validationRules.required(value),
                minimumChars: (value) => validationRules.minimumChars(value, 2),
                minimumCharsPass: (value) => validationRules.minimumChars(value, 8),
                email: value => validationRules.email(value)
            },
        };
    },


    computed: {
        btnLoading() {
            return this.$store.getters.getGlobalLoading
        },
        errorMessages(){
            return this.$store.getters.getErrorMessages
        },
        passHint() {
            if (this.password.length > 0) {
                this.changeScore()
                return `${this.passScoreObj[this.score].name}`;
            }
            return null
        },
        hintClass() {
            if (this.passHint) {
                return this.passScoreObj[this.score].className;
            }
            return null
        }
    },

    methods: {
        gmailRegister() {
            this.googleLoading = true;
            let self = this
            registrationService.googleRegistration()
                .then(({data}) => {
                    if (!data.isSignedIn) {
                        analyticsService.sb_unitedEvent('Registration', 'Start Google')
                        self.component = 'setPhone2'
                    } else {
                        analyticsService.sb_unitedEvent('Login', 'Start Google')
                        self.$store.commit('setRegisterDialog', false)
                        self.$store.commit('setLoginDialog', false)
                        self.$store.dispatch('updateLoginStatus', true)
                    }
                }).catch(error => {
                    self.$store.commit('setErrorMessages', { 
                        gmail: error.response.data["Google"] ? error.response.data["Google"][0] : '' 
                    });
                    self.$appInsights.trackException({exception: new Error(error)})
                    self.$refs.recaptcha.reset()
                })
        },
        changeScore() {
            this.score = global.zxcvbn(this.password).score;
        }
    },

    mounted() {
        let self = this;
        this.$nextTick(function () {
            this.$loadScript("https://apis.google.com/js/client:platform.js").then(()=>{
                self.$store.dispatch('gapiLoad');
            }).catch(ex => {
                self.$appInsights.trackException({exception: new Error(ex)});
            })
        });
    }

}