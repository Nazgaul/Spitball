import { validationRules } from '../../services/utilities/formValidationRules';
import * as routeNames from '../../routes/routeNames'

export default {

    data() {
        return {
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
                let passScoreObj = this.getPassScoreObj;
                this.changeScore()
                return `${passScoreObj[this.score].name}`;
            }
            return null
        },
        hintClass() {
            let passScoreObj = this.getPassScoreObj;
            if (this.passHint) {
                return passScoreObj[this.score].className;
            }
            return null
        }
    },

    methods: {
        changeScore() {
            this.score = global.zxcvbn(this.password).score;
        }
    }

}