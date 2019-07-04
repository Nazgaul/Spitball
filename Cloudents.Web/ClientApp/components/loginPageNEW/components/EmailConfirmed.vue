<template>
    <div class="EmailConfirmed">
        <div class="top" v-language:inner="isRegisterPath? 'loginRegister_emailconfirm_title':'loginRegister_emailconfirm_title_reset'"/>
        <div class="middle" v-if="isRegisterPath">
            <span>
                <span v-language:inner="'loginRegister_emailconfirm_to'"/>
                <span class="font-weight-bold">{{userEmail}}</span>
            </span>
            <p class="font-weight-bold" @click="goToRegister()" v-language:inner="'loginRegister_emailconfirm_notyou'"/>
        </div>
        <div>
            <div class="bottom">
                <span v-language:inner="isRegisterPath? 'loginRegister_emailconfirm_bottom' : 'loginRegister_emailconfirm_bottom_reset'"/>
                <span v-if="!isRegisterPath" v-language:inner="'loginRegister_emailconfirm_bottom_reset_or'"/>
                <span class="link" @click="resend()" v-language:inner="'loginRegister_emailconfirm_resend'"/>
                <span v-if="isRegisterPath" v-language:inner="'loginRegister_emailconfirm_rest'"/>
            </div>
        </div>
    </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'

export default {
    name: 'EmailConfirmed',
    computed: {
        ...mapGetters(['getEmail1','getProfileData']),
        userEmail(){
            return this.getEmail1
        },
        isRegisterPath(){
            return (this.$route.path === '/register')
        }
    },
    methods: {
        ...mapActions(['resetState','resendEmail','resendEmailPassword']),
        goToRegister(){
            this.resetState()
        },
        resend(){
            if(this.isRegisterPath){
                this.resendEmail()
            } else{
                this.resendEmailPassword()
            }
        }
    }
}
</script>

<style lang='less'>
.EmailConfirmed{
    display: flex;
    flex-direction: column;
    align-items: center;
    .top{
        text-align: center;
        padding: 0 0 64px;
        font-size: 28px;
        letter-spacing: -0.51px;
        color: #434c5f;
    }
    .middle{
        display: flex;
        flex-direction: column;
        align-items: center;
        color: #000000;
        font-size: 18px;
        padding-bottom: 50px;
        p{
            margin: 5px 0 0;
            font-size: 14px;
            color: #4452fc;
            cursor: pointer;
        }
    }
    .bottom{
        font-size: 14px;
        letter-spacing: -0.37px;
        text-align: center;
        color: #000000;
        line-height: 25px;
        .link{
            cursor: pointer;
            color: #4452fc;
        }
    }
}

</style>
