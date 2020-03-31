<template>
    <div class="loginDetails text-center">
        <!-- <div class="loginTitle mb-6" v-t="'loginRegister_setemail_title'"></div> -->

        <v-text-field 
            v-model="currentEmail"
            type="email"
            :label="$t('loginRegister_setemailpass_input_email')"
            :error-messages="errors.email"
            :rules="[rules.required, rules.email]"
            color="#304FFE"
            height="44" 
            dense
            outlined
            placeholder=" "
        >
        </v-text-field>

        <v-text-field 
            v-model="password"
            type="password"
            :label="$t('loginRegister_setemailpass_input_pass')"
            :error-messages="errors.password"
            :rules="[rules.required, rules.minimumCharsPass]"
            color="#304FFE"
            height="44"
            dense
            outlined
            placeholder=" "
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
            }
        }
    },
    watch: {
        password() {
            if(this.errors.password) {
                this.errors.email = ''
                this.errors.password = ''
            }
        },
        email() {
            if(this.errors.email) {
                this.errors.email = ''
                this.errors.password = ''
            }
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
        }
    }
}
</script>

<style lang="less">
// @import '../../../../../../../styles/mixin.less';
// @import '../../../../../../../styles/colors.less';
// .loginDetails {
//     .loginTitle {
//         .responsive-property(font-size, 28px, null, 22px);
//         color: @color-login-text-title;
//     }
// }
</style>