<template>
    <div class="forgotPass text-center">
        <div class="top mb-6">
            <div class="mainTitle mb-4 mb-sm-2" v-t="'loginRegister_forgot_title'"></div>
            <div class="subTitle px-4 px-sm-0" v-t="'loginRegister_forgot_subtitle'"></div>
        </div>

        <v-text-field 
            v-model="currentEmail"
            class="widther input-fields" 
            type="email"
            color="#304FFE"
            outlined
            height="44" 
            dense
            :rules="[rules.required, rules.email]"
            :label="$t('loginRegister_setemailpass_input_email')"
            :error-messages="errors.email"
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
            required: true
        },
		errors: {
			type: Object
		}
	},
    data() {
        return {
            rules: {
                required: (value) => validationRules.required(value),
                email: value => validationRules.email(value)
            },
        }
    },
    computed: {
        currentEmail: {
            get() {
                return this.email
            },
            set(email) {
                if(email) {
                    this.$emit('updateEmail', email)
                }
            }
        }
    }
}
</script>

<style lang="less">
@import '../../../../../../../styles/mixin.less';
@import '../../../../../../../styles/colors.less';
    .forgotPass{
        .top{
            .mainTitle{
                .responsive-property(font-size, 28px, null, 22px);
                .responsive-property(letter-spacing, -0.51px, null, -0.4px);
                color: @color-login-text-title;
            }
            .subTitle {
                cursor: initial;
                .responsive-property(font-size, 16px, null, 14px);
                .responsive-property(letter-spacing, -0.42px, null, -0.37px);
                color: @color-login-text-subtitle;
            }
        }
    }
</style>

