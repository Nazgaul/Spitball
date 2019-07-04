<template>
    <div class="forgotPass">
        <div class="top">
            <p v-language:inner="'loginRegister_forgot_title'"/>
            <span v-language:inner="'loginRegister_forgot_subtitle'"/>
        </div>

        <sb-input 
                v-model="email"
                :errorMessage="errorMessages.email"
				placeholder="loginRegister_setpass_input_email"
				icon="sbf-email" 
				:bottomError="true"
				:autofocus="true" 
				name="email" type="email"/>
        <v-btn  :loading="isEmailLoading"
                :disabled="!email"
                @click="resetPass"
                color="#304FFE" large round 
                class="white--text">
                <span v-language:inner="'loginRegister_forgot_btn'"></span>
                </v-btn>


        <span class="bottom" @click="goLogin" v-language:inner="'loginRegister_forgot_remember'"/>
    </div>
</template>

<script>
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import { mapActions, mapGetters } from 'vuex';

export default {
    name: 'forgotPass',
    components:{
        SbInput
    },
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
        ...mapActions(['updateStep','updateEmail','resetPassword']),
        goLogin(){
            this.updateStep('setPassword')
        },
        resetPass(){
            this.resetPassword()
        }
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';

    .forgotPass{
        text-align: center;
        .top{
        display: flex;
        flex-direction: column;
        align-items: center;
        margin-bottom: 30px;
            p{
            margin: 0;
            font-size: 28px;
            line-height: 1.54;
            letter-spacing: -0.51px;
            color: #434c5f;
            }
            span{
            cursor: initial;
            padding-top: 8px;
            font-size: 16px;
            letter-spacing: -0.42px;
            color: #888b8e;
            }
        }
        .input-wrapper {
            input {
            border: none;
            border-radius: 4px;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.26);
            border: solid 1px rgba(55, 81, 255, 0.29);
            background-color: #ffffff;
            padding: 10px !important;
            padding-left: 54px !important;
            }
            i {
            position: absolute;
            left: 16px;
            top: 17px;
            font-size: 18px;
            }
        }
        button{
            margin: 66px 0 48px;
            .responsive-property(width, 100%, null, 90%);
            font-size: 16px;
            font-weight: 600;
            letter-spacing: -0.42px;
            text-align: center;
            text-transform: none !important;
        }
        .bottom{
            cursor: pointer;
            font-size: 14px;
		    letter-spacing: -0.37px;
		    color: #4452fc;
        }
    }
</style>

