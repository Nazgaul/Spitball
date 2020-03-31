<template>
    <div class="emailRegister text-center">
        <v-layout wrap class="widther">
            <v-flex xs12 sm6 class="pr-sm-2">
                <v-text-field
                    v-model="firstName"
                    class="input-fields"
                    type="text"
                    color="#304FFE"
                    :label="$t('loginRegister_setemailpass_first')"
                    :rules="[rules.required, rules.minimumChars]"
                    height="44"
                    outlined
                    dense
                    placeholder=" "
                    autocomplete="nope"
                >
                </v-text-field>
            </v-flex>

            <v-flex xs12 sm6 class="pl-sm-2">
                <v-text-field
                    v-model="lastName"
                    class="input-fields"
                    type="text"
                    color="#304FFE"
                    :label="$t('loginRegister_setemailpass_last')"
                    :rules="[rules.required, rules.minimumChars]"
                    height="44"
                    outlined
                    dense
                    placeholder=" "
                    autocomplete="nope"
                >
                </v-text-field>
            </v-flex>
        </v-layout>

        <v-text-field 
            v-model="email"
            class="widther input-fields" 
            type="email"
            color="#304FFE"
            :label="$t('loginRegister_setemailpass_input_email')"
            :rules="[rules.required, rules.email]"
            :error-messages="errors.email"
            height="44" 
            outlined
            dense
            placeholder=" "
        >
        </v-text-field>

        <!-- DO NOT REMOVE GENDER RADIO BUTTONS -->
        <!-- <v-radio-group v-model="gender" row class="radioActive mt-n1" dense :mandatory="true">
            <v-radio :label="$t('loginRegister_setemailpass_male')" value="male" on-icon="sbf-radioOn" off-icon="sbf-radioOff"></v-radio>
            <v-radio :label="$t('loginRegister_setemailpass_female')" value="female" on-icon="sbf-radioOn" off-icon="sbf-radioOff"></v-radio>
        </v-radio-group> -->

        <v-text-field 
            v-model="password"
            class="widther input-fields"
            :class="hintClass"
            type="password"
            color="#304FFE"
            :rules="[rules.required, rules.minimumCharsPass]"
            :label="$t('loginRegister_setemailpass_input_pass')"
            :error-messages="errors.password"
            :hint="passHint"
            height="44"
            outlined
            dense
            placeholder=" "
        >
        </v-text-field>
    </div>
</template>

<script>
import { validationRules } from '../../../../../../../services/utilities/formValidationRules2';

export default {
    props: {
        errors: {
            type: Object
        }
    },
    data() {
        return {
            firstName:'',
            lastName:'',
            email: '',
            // gender: "male",
            password: "",
            rules: {
                required: (value) => validationRules.required(value),
                minimumChars: (value) => validationRules.minimumChars(value, 2),
                email: value => validationRules.email(value),
                minimumCharsPass: (value) => validationRules.minimumChars(value, 8)
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
        };
    },
    watch: {
        email() {
            if(this.errors.email) {
                this.errors.email = ''
            }
        },
        password() {
            if(this.errors.password) {
                this.errors.password = ''
            }
        }
    },
    computed: {
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
        changeScore() {
            this.score = global.zxcvbn(this.password).score;
        }
    },
    created() {
        this.$loadScript("https://unpkg.com/zxcvbn@4.4.2/dist/zxcvbn.js");
    }
};
</script>

<style lang="less">
// @import '../../../../../../../styles/mixin.less';
// @import '../../../../../../../styles/colors.less';

// .emailRegister {
//     .mainTitle {
//         .responsive-property(font-size, 28px, null, 22px);
//         color: @color-login-text-title;
//     }
// }
</style>