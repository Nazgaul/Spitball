<template>
    <form class="resetPassword" @submit.prevent="change">
        <p>{{$t('loginRegister_resetpass_title')}}</p>
        <sb-input
                :errorMessage="errorMessages.Password"
                :hint="passHint"
                :class="[hintClass,'widther']"
                v-model="password"
				:placeholder="$t('loginRegister_resetpass_input')"
				:bottomError="true"
				:autofocus="true" 
				name="pass" type="password"/>

		<sb-input 
            :errorMessage="errorMessages.ConfirmPassword"
            v-model="confirmPassword"
            class="mt-4 widther"
            :placeholder="$t('loginRegister_resetpass_input_confirm')"  
            :bottomError="true" 
            type="password" name="pass"
            :autofocus="false"/>

        <v-btn  
            type="submit"
            :loading="loading"
            large rounded 
            class="white--text btn-login">
                <span>{{$t('loginRegister_resetpass_btn')}}</span>
        </v-btn>
    </form>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import SbInput from "../../../question/helpers/sbInput/sbInput.vue";
import scoreMixin from '../../global/dialogInjection/globalDialogs/auth/scoreMixin';

export default {
    name: 'resetPassword',
    mixins: [scoreMixin],
    components:{
        SbInput
    },
	data() {
		return {
			password: '',
			confirmPassword: '',
            errorMessages: {
                Password: '',
                ConfirmPassword: '',
            }
		}
    },
    computed: {
        ...mapGetters(['getGlobalLoading']),
        loading(){
            return this.getGlobalLoading
        },
    },
    methods: {
        ...mapActions(['changePassword']),
        change(){
            let paramsObj = {
                id: this.$route.query['Id'] ? this.$route.query['Id'] : '',
                code: this.$route.query['code'] ? this.$route.query['code'] : '',
                password: this.password,
                confirmPassword: this.confirmPassword
            }
            let self = this
            this.changePassword(paramsObj).catch((res) => {
                Object.keys(res.response.data).map(e => {
                    console.log(res.response.data[e]);
                    self.errorMessages[e] = res.response.data[e][0]
                })
            })
        },
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';
    .resetPassword {
        .responsive-property(width, 400px, null, auto);
        @media (max-width: @screen-xs) {
            display: flex;
            flex-direction: column;
            align-items: center;
            margin: 0 auto;
        }
        text-align: center;
        p {
        .responsive-property(font-size, 28px, null, 22px);
        .responsive-property(letter-spacing, -0.51px, null, -0.4px);
        line-height: 1.54;
        color: @color-login-text-title;
        .responsive-property(margin-bottom, 62px, null, 32px);

        }
        .input-wrapper {
            input {
            .login-inputs-style();
            }
        }
        button{
        margin: 30px 0 0;
        @media (max-width: @screen-xs) {
            margin: 50px 0 0;
        }
        .responsive-property(width, 100%, null, 72%);
        font-size: 16px;
        font-weight: 600;
        letter-spacing: -0.42px;
        text-align: center;
        text-transform: none !important;
        }
    }
</style>

