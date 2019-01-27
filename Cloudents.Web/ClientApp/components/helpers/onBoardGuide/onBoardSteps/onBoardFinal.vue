<template>
    <div class="final-onboard-wrap">
        <div class="present-row">
            <span class="step-title d-flex" v-language:inner>onboard_final_title</span>
            <presentStars  class="present-img"></presentStars>
        </div>
        <div class="text-row">
            <div class="sub-title-wrap">
                    <span class="step-subtitle"v-language:inner>onboard_final_added</span>
                <bdi>
                    <span class="ammount">&nbsp;{{tokensAmmount | currencyLocalyFilter}}&nbsp;</span>
                </bdi>
                    <span class="step-subtitle" v-language:inner>onboard_final_to_wallet</span>

            </div>
        </div>
        <v-divider class="divider-line"></v-divider>
        <div class="sml-text-row">
            <div class="" v-show="tokensAmmount < ammountCalcFrom">

                    <span v-language:inner>onboard_final_only</span>
                <bdi>
                    <span>&nbsp;{{sblAway}} SBL&nbsp;</span>
                </bdi>
                    <span v-language:inner>onboard_final_away</span>

            </div>
            <div class="" v-show="tokensAmmount >= ammountCalcFrom">
                <bdi>
                    <span v-language:inner>onboard_final_enter_wallet</span>
                </bdi>
            </div>
        </div>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex';
    import presentStars from '../images/present-stars.svg';


    export default {
        name: "onBoardFinal",
        components: {presentStars},
        data() {
            return {
                ammountCalcFrom: 1000

            }
        },
        props: {},
        computed: {
            ...mapGetters(['accountUser']),
            tokensAmmount() {
                return this.accountUser.balance;
            },
            sblAway() {
                if(this.tokensAmmount < this.ammountCalcFrom){
                    return this.ammountCalcFrom - this.tokensAmmount;
                }else{
                    return this.tokensAmmount;
                }
            },

        },
        methods: {
            name() {

            }
        },
    }
</script>

<style scoped lang="less">
    @import '../../../../styles/mixin.less';

    @finalYellow: #ffca54;
    @colorTextLight: #dfe0ef;
    .final-onboard-wrap {
        display: flex;
        flex: 1;
        justify-content: center;
        flex-direction: column;
        align-items: center;
        height: 100%;
        .present-row {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            @media (max-width: @screen-xs) {
                max-width: 100%;
                line-height: 1.1;
                display: flex;
                flex-direction: column;
                align-items: center;
            }
            .present-img {
                height: auto;
                width: 100%;
                max-width: 198px;
                @media (max-width: @screen-xs) {
                    max-width: 198px;
                }
            }
        }
        .text-row {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            width: 100%;

        }
        .sml-text-row {
            padding-top: 32px;
            display: flex;
            font-family: @fontFiraSans;
            font-size: 18px;
            font-style: italic;
            line-height: 1.5;
            text-align: center;
            color: @colorTextLight;
            max-width: 254px;
            @media (max-width: @screen-xs) {
                font-size: 16px;

            }
        }
        .step-title {
            opacity: 0.9;
            font-family: @fontFiraSans;
            font-size: 36px;
            font-weight: 600;
            font-style: italic;
            font-stretch: normal;
            line-height: 1.25;
            letter-spacing: normal;
            text-align: center;
            color: @finalYellow;
            padding: 32px 0;
            @media (max-width: @screen-xs) {
                font-size: 30px;
                line-height: 1.1;
            }
        }
        .sub-title-wrap {
            display: flex;
            flex-direction: column;
            padding-bottom: 32px;
            padding-top: 24px;
            text-align: center;
            max-width: 90%;
            @media (max-width: @screen-xs) {
                padding-top: 24px;
                max-width: 90%;
                text-align: center;
                line-height: 1.38;
            }
        }
        .step-subtitle {
            font-family: @fontFiraSans;
            font-size: 28px;
            font-style: italic;
            font-stretch: normal;
            line-height: 1.32;
            text-align: center;
            color: @colorTextLight;
            @media (max-width: @screen-xs) {
                font-size: 24px;
                line-height: 1.38;
                font-weight: 600;
            }
        }
        .ammount {
            color: @finalYellow;
            font-weight: 600;
            line-height: 1.59;
            font-size: 28px;
            font-style: italic;
            display: inline-block;
            word-break: keep-all;
            @media (max-width: @screen-xs) {
                line-height: 1.38;
                font-size: 24px;
                font-style: italic;
            }
        }
        .divider-line {
            background: fade(@color-white, 48%);
            width: 136px;
            height: 1px;
        }
    }

</style>