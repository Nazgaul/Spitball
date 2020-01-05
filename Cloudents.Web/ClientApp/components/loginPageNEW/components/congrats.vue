<template>
    <div class="congrats">
        <div class="top">
            <p v-language:inner="'loginRegister_congrats_title'"/>
            <span v-language:inner="'loginRegister_congrats_subtitle'"/>
        </div>
        <img src="../images/group-4.png" alt="">
        <div class="bottom" v-language:inner="'loginRegister_congrats_bottom'"/>
        <v-btn @click="finish" large rounded class="white--text btn-login" v-language:inner="'loginRegister_congrats_btn'"/>
    </div>
</template>

<script>
import { mapActions } from 'vuex';
export default {
    name: 'congrats',
    methods: {
        ...mapActions(['finishRegister']),
        finish(){
            this.finishRegister()
        }
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
@import '../../../styles/colors.less';
.congrats{
    display: flex;
    flex-direction: column;
    align-items: center;
    .top{
        .responsive-property(font-size, 28px, null, 22px);
      @media (max-width: @screen-xs) {
       line-height: inherit;
      }
        font-weight: 600;
        line-height: 1.54;
        text-align: center;
        color: @color-login-text-title;
        .responsive-property(margin-bottom, 32px, null, 48px);
        p{
        .responsive-property(letter-spacing, -0.51px, null, -0.4px);
            margin: 0 0 5px;
        }
    }
    img{
        .responsive-property(width, 336px, null, 272px);
        .responsive-property(height, auto, null, 167px);
    }
    .bottom{
        .responsive-property(font-size, 20px, null, 16px);
            line-height: 1.5;
            margin:34px 0 56px;
        @media (max-width: @screen-xs) {
            padding: 0px 42px;
            margin: 48px 0 47px;
            line-height: 1.6;
        }
        letter-spacing: -0.36px;
        text-align: center;
        color: @color-login-text-title;
    }
    button{
        margin: 0;
        .responsive-property(width, 100%, null, 72%);
        font-size: 16px;
        font-weight: 600;
        letter-spacing: -0.42px;
        text-align: center;
        text-transform: none !important;
    }
}
</style>

