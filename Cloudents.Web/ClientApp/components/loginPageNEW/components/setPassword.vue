<template>
    <div class="setPassword">
		<p v-language:inner="'loginRegister_setemail_title'"/>
        <sb-input 
				v-model="email"
				placeholder="loginRegister_setpass_input_email"
				icon="sbf-email" 
				:bottomError="true"
				:autofocus="true" 
                :errorMessage="errorMessages.email"
				name="email" type="email"/>

		<sb-input 
                class="mt-4"
                icon="sbf-key"
				v-model="password"
				placeholder="loginRegister_setpass_input_pass"  
				:bottomError="true" 
				type="password" name="pass"
				:autofocus="true"/>

        <v-btn  @click="login"
                :disabled="!isValid"
                :loading="isEmailLoading"
                color="#304FFE" large round 
                class="white--text">
                <span v-language:inner="'loginRegister_setpass_btn'"></span>
                </v-btn>
        
        <span class="bottom" @click="goForgotPassword" v-language:inner="'loginRegister_setpass_forgot'"/>
    </div>    
</template>

<script>
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import { mapGetters, mapActions } from 'vuex';

export default {
    name: 'setPassword',
    components:{
        SbInput
    },
    data() {
        return {
            pass: '',
        }
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
            }
        },
        password:{
            get(){},
            set(val){
                this.pass = val
                this.updatePassword(val)
            }
        },
        isValid(){
            return this.getEmail1 && this.pass.length > 7
        }
        
    },
    methods: {
        ...mapActions(['updatePassword','updateEmail','logIn','updateStep']),
        login(){
            this.logIn()
        },
        goForgotPassword(){
            this.updateStep('forgotPass')
        }
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';

    .setPassword{
        text-align: center;
        p {
        font-size: 28px;
        line-height: 1.54;
        letter-spacing: -0.51px;
        color: #434c5f;
        margin-bottom: 62px;
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

