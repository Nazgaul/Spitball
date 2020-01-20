<template>
    <div :class="['earnSection']">
        <div class="earnImg">
            <div class="earn-title" v-language:inner="'homePage_earn_title'"/>
            <div class="earn-subtitle" v-language:inner="'homePage_earn_subtitle'"/>
            <v-btn tag="a" :href="href" class="btn-earn" rounded color="#4c59ff">
                <span v-language:inner="'homePage_btn_earn'"/>
            </v-btn>
        </div>
    </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
    computed: {
        ...mapGetters(['isFrymo']),

        href() {
            if(this.isFrymo) {
                return 'https://www.frymo.com/register';
            }
            return 'https://help.spitball.co/en/article/making-your-profile';
        }
    }
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
.earnSection{
    padding: 64px 0;
    .responsiveLandingPage(1354px,80px);
    @media (max-width: @screen-xs) {
        width: 100%;
        padding: 10px 0;
    }
    .earnImg{
        width: 100%;
        height: 317px;
        @media (max-width: @screen-xs) {
            height: unset;
            padding: 15px 0;
        }
        background-image: url('../images/earn.png');
        background-repeat: no-repeat;
        background-size: cover; 
        background-position: center;
        color: #ffffff;
        text-align: center;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        .earn-title{
            padding: 0 20px;
            text-shadow: 0 2px 15px rgba(0, 0, 0, 0.6);
            font-size: 28px;
            font-weight: bold;
            max-width: 714px;
            margin-bottom: 12px;
        }
        .earn-subtitle{
            padding: 0 60px;
            max-width: 714px;
            font-size: 18px;
            font-weight: normal;
            line-height: 1.55;
            text-align: center;
            margin-bottom: 30px;
            @media (max-width: @screen-xs) {
                margin-bottom: 10px;
                padding: 0 20px;
            }
        }
        .btn-earn{
            @media (max-width: @screen-xs) {
                height: 42px;
                padding: 20px;
            }
            font-size: 18px;
            font-weight: 600;
            font-stretch: normal;
            font-style: normal;
            line-height: 1.33;
            letter-spacing: normal;
            text-align: center;
            color: #ffffff;
            padding: 0 60px;
            height: 52px;
            text-transform: capitalize !important;
            box-shadow: 2px 8.8px 21px 0 rgba(0, 0, 0, 0.42);
        }
    }
}
</style>