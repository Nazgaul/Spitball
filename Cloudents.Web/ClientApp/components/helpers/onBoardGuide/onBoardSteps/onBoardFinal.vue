<template>
    <div class="final-onboard-wrap">
        <div class="present-row">
            <span class="step-title d-flex" v-language:inner>onboard_final_title</span>
            <presentStars  class="present-img"></presentStars>
        </div>
        <div class="text-row">
            <div class="sub-title-wrap" v-show="tokensAmmount !== 0">
                    <span class="step-subtitle"v-language:inner>onboard_final_added</span>
                <bdi>
                    <span class="ammount">&nbsp;{{tokensAmmount | currencyLocalyFilter}}&nbsp;</span>
                </bdi>
                    <!--<span class="step-subtitle" v-language:inner>onboard_final_to_wallet</span>-->

            </div>
        </div>

        <v-divider class="divider-line" v-show="tokensAmmount !== 0"></v-divider>
        <div class="sml-text-row">
            <span v-language:inner>onboard_final_bottom_text</span>
        </div>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex';
    import presentStars from '../images/SVG_step_3.svg';


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
                width: 100%;
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
            font-size: 24px;
            font-weight: 600;
            font-style: italic;
            font-stretch: normal;
            line-height: normal;
            letter-spacing: -0.5px;
            text-align: center;
            color: @color-white;
            padding-top: 32px;
            display: flex;
            font-family: @fontFiraSans;
            @media (max-width: @screen-xs) {
                font-size: 16px;
                max-width: 254px;

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
            @media (max-width: @screen-xs) {
                font-size: 30px;
                line-height: 1.1;
                justify-content: center;
                width: 100%;
            }
        }
        .sub-title-wrap {
            display: flex;
            flex-direction: row;
            align-items: baseline;
            padding-bottom: 32px;
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
            color:@color-white;
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