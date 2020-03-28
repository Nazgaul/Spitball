<template>
    <div class="loginDetails text-center">
        <div class="loginTitle mb-6" v-t="'loginRegister_setemail_title'"></div>

        <v-text-field 
            v-model="currentEmail"
            class="widther input-fields" 
            color="#304FFE"
            outlined
            height="44" 
            dense
            :label="$t('loginRegister_setemailpass_input_email')"
            :error-messages="errors.email"
            :rules="[rules.required, rules.email]"
            placeholder=" "
            type="email"
        >
        </v-text-field>

        <v-text-field 
            v-model="password"
            :class="[hintClass,'widther','input-fields','mb-3']"
            color="#304FFE"
            outlined
            height="44"
            dense
            :label="$t('loginRegister_setemailpass_input_pass')"
            :error-messages="errors.password"
            :rules="[rules.required, rules.minimumCharsPass]"
            placeholder=" "
            type="password"
            :hint="passHint"
        >
        </v-text-field>

    </div>
</template>

<script>
import { validationRules } from '../../../../../../../services/utilities/formValidationRules2';

export default {
    props: {
        email: {
            type: String,
            default: '',
            required: true,
        },
		errors: {
			type: Object
		}
    },
    data() {
        return {
            password: '',
            rules: {
                required: (value) => validationRules.required(value),
                minimumCharsPass: (value) => validationRules.minimumChars(value, 8),
                email: value => validationRules.email(value)
            },
            passScoreObj: {
                0: { name: this.$t("login_password_indication_weak"), className: "bad" },
                1: { name: this.$t("login_password_indication_weak"), className: "bad" },
                2: { name: this.$t("login_password_indication_strong"), className: "good" },
                3: { name: this.$t("login_password_indication_strong"), className: "good" },
                4: { name: this.$t("login_password_indication_strongest"), className: "best" }
            },
            score: {
				default: 0,
				required: false
			},
        }
    },
    computed: {
        currentEmail: {
            get() {
                return this.email
            },
            set(email) {
                this.$emit('updateEmail', email)
            }
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
    methods:{
        changeScore() {
            this.score = global.zxcvbn(this.password).score;
        }
    },
    created() {
        this.$loadScript("https://unpkg.com/zxcvbn@4.4.2/dist/zxcvbn.js");
    }
}
</script>

<style lang="less">
@import '../../../../../../../styles/mixin.less';
@import '../../../../../../../styles/colors.less';
.loginDetails {

    .loginTitle {
        .responsive-property(font-size, 28px, null, 22px);
        color: @color-login-text-title;
    }
}
</style>