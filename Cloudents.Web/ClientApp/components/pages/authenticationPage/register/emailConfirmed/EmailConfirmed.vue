<template>
    <div class="EmailConfirmed">
        <div class="top" v-language:inner="isRegisterPath? 'loginRegister_emailconfirm_title':'loginRegister_emailconfirm_title_reset'"/>
        <div class="middle" v-if="isRegisterPath">
            <span>
                <!-- <span v-language:inner="'loginRegister_emailconfirm_to'"/> -->
                <span class="email"> {{userEmail}}</span>
            </span>
            <p class="notYou font-weight-bold" @click="goToRegister()" v-language:inner="'loginRegister_emailconfirm_notyou'"/>
        </div>
        <div>
            <div class="bottom">
                <span v-language:inner="isRegisterPath? 'loginRegister_emailconfirm_bottom' : 'loginRegister_emailconfirm_bottom_reset'"/>
                <span v-if="!isRegisterPath" v-language:inner="'loginRegister_emailconfirm_bottom_reset_or'"/>
                <div>
                    <span class="link" @click="resend()" v-language:inner="'loginRegister_emailconfirm_resend'"/>&nbsp;
                    <span v-if="isRegisterPath" v-language:inner="'loginRegister_emailconfirm_rest'"/>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'
import { RegiserEmailConfirmed }from '../../../../../routes/routeName'

export default {
    // name: 'EmailConfirmed',
    computed: {
        ...mapGetters(['getEmail1']),
        userEmail(){
            return this.getEmail1
        },
        isRegisterPath(){
            return (this.$route.name === RegiserEmailConfirmed)
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
@import '../../../../../styles/mixin.less';
@import '../../../../../styles/colors.less';
.EmailConfirmed{
    display: flex;
    flex-direction: column;
    align-items: center;
    .top{
        .responsive-property(font-size, 28px, null, 22px);
        .responsive-property(letter-spacing, -0.51px, null, -0.4px);
        .responsive-property(margin-bottom, 28px, null, 38px);
        .responsive-property(margin-top, null, null, 42px);
        text-align: center;
        color: @color-login-text-title;
    }
    .middle{
        display: flex;
        flex-direction: column;
        align-items: center;
        color: #000000;
        font-size: 18px;
        .responsive-property(margin-bottom, 85px, null, 42px);
        .email {
            color: #4d4b69;
        }
        .notYou {
            margin: 5px 0 0;
            font-size: 14px;
            color: @global-blue;
            cursor: pointer;
        }
    }
    .bottom{
        @media (max-width: @screen-xs) {
            padding: 0 40px;
            line-height: inherit;
        }
        font-size: 14px;
        letter-spacing: -0.37px;
        text-align: center;
        color: #4d4b69;
        line-height: 25px;
        .link{
            cursor: pointer;
            color: #5e68ff;
        }
        div{
            .responsive-property(margin-top, inherit, null, 36px);
        }
    }
}

</style>
