<template>
    <div class="resetPassword text-center">
        <div class="resetTitle mb-8" v-t="'loginRegister_reset_main_title'"></div>

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

        <v-text-field 
            v-model="confirmPassword"
            class="widther input-fields"
            type="password"
            color="#304FFE"
            :rules="[rules.required, rules.minimumCharsPass, rules.passwordMatch]"
            :label="$t('loginRegister_resetpass_input_confirm')"
            :error-messages="errors.confirmPassword"
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

import scoreMixin from '../scoreMixin'
export default {
    mixins: [scoreMixin],
    props: {
        errors: {
            type: Object
        }
    },
    data() {
        return {
            password: '',
            confirmPassword: '',
            rules: {
                required: (value) => validationRules.required(value),
                minimumCharsPass: (value) => validationRules.minimumChars(value, 8),
                passwordMatch: (value) => this.password === value || this.$t('loginRegister_password_not_match')
            }
        }
    }
}
</script>

<style lang='less'>
@import '../../../../../../../styles/mixin.less';
@import '../../../../../../../styles/colors.less';

.resetPassword {

    .resetTitle {
        .responsive-property(font-size, 20px, null, 22px);
        color: @color-login-text-title;
        font-weight: 600;
    }
}
</style>
