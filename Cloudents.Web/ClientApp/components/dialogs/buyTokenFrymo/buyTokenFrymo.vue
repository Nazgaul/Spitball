<template>
    <div class="buyTokenFrymo">
        <v-icon class="buyTokenFrymo__close" @click="closeModal">sbf-close</v-icon>
        <h2 class="buyTokenFrymo__h2">Buy Points</h2>
        <h4 class="buyTokenFrymo__h4">Points help you access teachers, documents. </h4>
        <h3 class="buyTokenFrymo__h3">Points Packs</h3>
        <div class="buyTokenFrymo__box">
            <img v-if="$vuetify.breakpoint.xsOnly" src="./images/group-2-price.png" alt="">
            <template v-else>
                <div class="buyTokenFrymo__box--100"></div>
                <div class="buyTokenFrymo__box--500"></div>
                <div class="buyTokenFrymo__box--1000"></div>
                    
            </template>
        </div>
        <div class="buyTokenFrymo__warn">
            <img class="buyTokenFrymo__warn--img" src="../buyTokens/img/warning.png" alt=""/>
            <p class="buyTokenFrymo__warn--p">Please try with smaller packs we do not offer any refunds</p>
        </div>
        <div class="buyTokenFrymo__payCard">
            <div class="buyTokenFrymo__payCard--phone"></div>
            <div class="buyTokenFrymo__payCard--or">OR</div>
            <div class="buyTokenFrymo__payCard--scan"></div>
        </div>
        <h5 class="buyTokenFrymo__h5">If any queries contact us at 
            <a href="mailto:support@frymo.com">support@frymo.com</a>
        </h5>
    </div>
</template>

<script>
import { mapActions } from 'vuex';

export default {
    name: "buyTokenFrymo",
    methods: {
        ...mapActions(['updateShowBuyDialog']),
        closeModal(){
            this.updateShowBuyDialog(false);
        },
    }
} 
</script>

<style lang="less">
    
    @import '../../../styles/mixin';

    .buyTokenFrymo {
        width: 100%;
        margin: 0 auto;
        text-align: center;
        background: #fff;

        @media (max-width: @screen-xs) {
            padding: 6px;
        }
        &__close {
            display: flex !important;
            justify-content: flex-end !important;
            font-size: 18px !important;
            margin: 10px 10px 0 10px;

            @media (max-width: @screen-xs) {
                padding: 10px;
                margin: 4px 10px 0 10px;
            }
        }

        &__h2 {
            color: #43425d;
            font-size: 24px;
            margin-bottom: 8px;
            font-weight: 600;

            @media (max-width: @screen-xs) {
                font-size: 20px;
            }
        }

        &__h3 {
            color: #5158af;
            font-size: 20px;
            margin-bottom: 10px;
            font-weight: 600;

            @media (max-width: @screen-xs) {
                font-size: 18px;
                margin-bottom: 0;
            }
        }

        &__h4 {
            color: #43425d;
            font-size: 18px;
            margin-bottom: 24px;
            font-weight: 600;
            @media (max-width: @screen-xs) {
                font-size: 14px;
                margin-bottom: 20px;
            }
        }

        &__box {
            display: flex;
            justify-content: center;
            margin-bottom: 6px;

            @media (max-width: @screen-xs) {
                margin-bottom: 0;
            }
            img {
                @media (max-width: @screen-xs) { 
                width: 100%;
            }
            }
            &--100 {
                background-image: url(./images/group-6.png);
                background-position: center;
                width: 220px;
                height: 140px;
                border-radius: 6px;
                @media (max-width: @screen-sm) {
                    background-size: 100%;
                }
                @media (max-width: @screen-xs) {
                    display: none;
                }
            }

            &--500 {
                background-image: url(./images/group-7.png);
                background-position: center;
                width: 220px;
                height: 140px;
                border-radius: 6px;
                @media (max-width: @screen-sm) {
                    background-size: 100%;
                }
                @media (max-width: @screen-xs) {
                    display: none;
                }
            }

            &--1000 {
                background-image: url(./images/group-8.png);
                background-position: center;
                width: 220px;
                height: 140px;
                border-radius: 6px;
                @media (max-width: @screen-sm) {
                    background-size: 100%;
                }
                @media (max-width: @screen-xs) {
                    display: none;
                }
            }

        }

        &__warn {
            display: flex;
            justify-content: center;
            margin-bottom: 30px;
            color: #7a798c;
            font-weight: 600;

            @media (max-width: @screen-xs) {
                width: 90%;
                margin: 0 auto;
                margin-bottom: 0;
            }

            &--img {
                width: 18px;
                height: 16px;
                margin-right: 10px;

                @media (max-width: @screen-xs) {
                    margin-right: 8px;
                }
            }

            &--p {
                @media (max-width: @screen-xs) {
                    font-size: 13px;
                    display: inline;
                }
            }
        }

        &__payCard {
            display: flex;
            justify-content: center;
            align-items: center;
            margin-bottom: 38px;

            @media (max-width: @screen-sm) {
                margin: 0 10px 38px 10px;
            }
            @media (max-width: @screen-xs) {
                flex-direction: column;
                margin-bottom: 0;
            }
            &--phone {
                background-image: url(./images/group-9.png);
                background-position: center;
                width: 270px;
                height: 186px;
                background-size: 100%;
                @media (max-width: @screen-xs) {
                    background-image: url(./images/group-9-mobile.png);
                    background-size: 240px;
                    height: 160px;
                }
            }
            &--or {
                border-radius: 50%;
                border: 2px solid #b8bcda;
                width: 46px;
                height: 43px;
                font-size: 16px;
                font-weight: bold;
                display: flex;
                align-items: center;
                justify-content: center;
                margin: 0 10px;

                @media (max-width: @screen-xs) {
                    width: 36px;
                    height: 36px;
                    font-size: 12px;
                }
            }
            &--scan {
                background-image: url(./images/scanAndPlay.png);
                background-position: center;
                width: 270px;
                height: 186px;
                background-size: 100%;
                @media (max-width: @screen-xs) {
                    background-image: url(./images/scanAndPlay-mobile.png);
                    background-size: 240px;
                    // height: 160px;
                }
            }
        }

        &__h5 {
            font-size: 13px;
            margin-bottom: 13px;
            font-weight: 600;
        }

    }

</style>