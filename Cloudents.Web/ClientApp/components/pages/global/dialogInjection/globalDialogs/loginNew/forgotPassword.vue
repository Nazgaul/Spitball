<template>
    <v-form class="forgotPass" @submit.prevent="resetPass">
        <div class="top">
            <p>{{$t('loginRegister_forgot_title')}}</p>
            <span>{{$t('loginRegister_forgot_subtitle')}}</span>
        </div>

        <v-text-field
            v-model="firstName"
            class="input-fields"
            color="#304FFE"
            outlined
            height="44"
            dense
            :label="labels['fname']"
            :rules="[rules.required, rules.minimumChars]"
            :error-messages="firstNameError"
            placeholder=" "
            autocomplete="nope"
            type="text"
        >
        </v-text-field>

        <v-btn  
            :loading="isEmailLoading"
            type="submit"
            large rounded 
            class="white--text btn-login">
                <span>{{$t('loginRegister_forgot_btn')}}</span>
        </v-btn>

        <router-link class="bottom" :to="{name: 'setPassword'}">{{$t('loginRegister_forgot_remember')}}</router-link>
    </v-form>
</template>

<script>
import { mapActions, mapGetters,mapMutations } from 'vuex';

export default {
    computed: {
        ...mapGetters(['getEmail1','getErrorMessages','getGlobalLoading']),
        isEmailLoading(){
            return this.getGlobalLoading
        },
        errorMessages(){
            return this.getErrorMessages
        },
        email:{
            get(){
               return this.getEmail1
            },
            set(val){
                this.updateEmail(val)
            },
        }
    },
    methods: {
        ...mapActions(['updateEmail', 'resetPassword']),
        ...mapMutations(['setErrorMessages']),
        resetPass(){
            this.resetPassword()
        }
    },
    watch: {
        email: function(){
            this.setErrorMessages({})
        }
    },
    mounted() {
        this.setErrorMessages({})
    }
}
</script>

<style lang="less">
@import '../../../../../../styles/mixin.less';
@import '../../../../../../styles/colors.less';
    .forgotPass{
        text-align: center;
        @media (max-width: @screen-xs) {
            display: flex;
            flex-direction: column;
            align-items: center;
        }
        .top{
                display: flex;
                flex-direction: column;
                align-items: center;
                margin-bottom: 30px;
            p{
                .responsive-property(font-size, 28px, null, 22px);
                .responsive-property(letter-spacing, -0.51px, null, -0.4px);
                margin: 0;
                line-height: 1.54;
                color: @color-login-text-title;
            }
            span {
                cursor: initial;
                .responsive-property(font-size, 16px, null, 14px);
                .responsive-property(letter-spacing, -0.42px, null, -0.37px);
                padding-top: 8px;
                color: @color-login-text-subtitle;
            }
        }
        .input-wrapper {
            input {
                .login-inputs-style();
                padding-left: 54px !important;
            }
            i {
                position: absolute;
                left: 12px;
                top: 10px;
                font-size: 31px;
            }
        }
        button{
            margin: 66px 0 48px;
            @media (max-width: @screen-xs) {
                margin: 45px 0 48px;
            }
            .responsive-property(width, 100%, null, 72%);
            font-size: 16px;
            font-weight: 600;
            letter-spacing: -0.42px;
            text-align: center;
            text-transform: none !important;
        }
        .bottom {
            cursor: pointer;
            font-size: 14px;
		    letter-spacing: -0.37px;
		    color: @global-blue;
        }
    }
</style>

