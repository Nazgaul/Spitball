<template>
    <v-layout column class="payme-popup">
        <v-icon class="exit-btn cursor-pointer" @click="closeDialog">sbf-close</v-icon>
        <div class="payme-popup-top pt-4">
            <div class="payme-top-title" v-t="'payme_top_title'"/>
            <v-layout wrap :class="['payme-content',isMobile? 'pt-4 pb-2':'pt-5 pb-4']">
                <v-flex xs12 sm3 :class="['payme-content-div']">
                    <img :class="['payme-content-img']" src="./images/timer.png">
                    <span :class="['payme-content-txt',{'pt-2':!isMobile}]" v-t="'payme_content_txt_time'"/>
                </v-flex>
                <v-flex xs12 sm3 :class="['payme-content-div',{'mx-4':!isMobile}]">
                    <img :class="['payme-content-img']" src="./images/sheild.png" >
                    <span :class="['payme-content-txt',{'pt-2':!isMobile}]" v-t="'payme_content_txt_sheild'"/>
                </v-flex>
                <v-flex xs12 sm3 :class="['payme-content-div']">
                    <img :class="['payme-content-img']" src="./images/hands.png">
                    <span :class="['payme-content-txt',{'pt-2':!isMobile}]" v-t="'payme_content_txt_hands'"/>
                </v-flex>
            </v-layout>
        </div>
        <iframe :class="['payment-iframe']" width="100%" height="475" :src="paymentUrl"></iframe>
        <div class="payme-popup-bottom">
            <p v-t="'payme_bottom'"/>
            <img src="./images/card.png" alt="">
        </div>
    </v-layout>
</template>

<script>
import { mapGetters } from 'vuex';
import * as componentConsts from '../../../../toasterInjection/componentConsts.js';

import * as routeNames from '../../../../../../../routes/routeNames';
export default {
   name:'paymentIL',
    computed: {
        ...mapGetters(['getPaymentURL']),
        paymentUrl(){
            return this.getPaymentURL
        },
        isMobile(){
        return this.$vuetify.breakpoint.xsOnly;
      }
    },
    methods: {
        closeDialog(){
            let isStudyRoom = this.$store.getters.getRoomIdSession && this.$route.name === routeNames.StudyRoom;
            if(isStudyRoom){
                let isExit = confirm(this.$t("payme_are_you_sure_exit"))
                if(isExit){
                    this.$router.push('/');
                    this.$store.commit('removeComponent',componentConsts.PAYMENT_DIALOG)
                }
            }else{
                this.$store.commit('removeComponent',componentConsts.PAYMENT_DIALOG)
            }
        }
    },
    beforeCreate() {
        let self = this;
        this.$store.dispatch('requestPaymentURL').then(()=>{}).catch(()=>{
            self.$store.dispatch('updateToasterParams', {
                    toasterText: self.$t("buyTokens_failed_transaction"),
                    showToaster: true,
                    toasterTimeout: 5000
                });
            self.$store.commit('removeComponent',componentConsts.PAYMENT_DIALOG)
        })
    },
    beforeDestroy() {
        this.$store.dispatch('updatePaymentLink',null)
    },
}
</script>

<style lang="less">

@import '../../../../../../../styles/mixin.less';
.payme-popup{
    position: relative;
    border-radius: 4px;
    background-color: #ffffff;
    -webkit-overflow-scrolling: touch;
    .exit-btn{
        position: absolute;
        top: 16px;
        right: 16px;
        font-size: 12px !important;
        color: rgba(0, 0, 0, 0.541);
    }
    .payment-iframe{
        @media (max-width: @screen-xs) {
            height: 100%;
        }
        border: none;
        padding: 0 6px;
    }
    .payme-popup-top{
        text-align: center;

        .payme-top-title{
            font-size: 20px;
            font-weight: bold;
            color:@global-purple; 
            @media (max-width: @screen-xs) {
                font-size: 18px;
            }
        }
        // .payme-top-desc{
        //     font-size: 14px;
        //     font-weight: 600;
        //     line-height: 1.5;
        //     letter-spacing: -0.17px;
        //     color:@global-purple; 
        //     @media (max-width: @screen-xs) {
        //         padding: 0 24px;
        //     }
        // }
        .payme-content{
            @media (max-width: @screen-xs) {
                padding-left: 12px;
            }
            display: flex;
            justify-content: center;
            .payme-content-div{
                @media (max-width: @screen-xs) {
                    flex-direction: row;
                    padding-bottom: 12px;
                }
                display: flex;
                flex-direction: column;
                align-items: center;
                .payme-content-img{
                    width: 32px;
                    height: 32px;
                    @media (max-width: @screen-xs) {
                        width: 26px;
                        height: 26px;
                        margin-right: 12px;
                    }
                    object-fit: contain; 
                }
                .payme-content-txt{
                    font-size: 14px;
                    font-weight: 600;
                    line-height: 1.5;
                    letter-spacing: -0.17px;
                    color:@global-purple;
                }
            }

        }
    }
    .payme-popup-bottom{
        @media (max-width: @screen-xs) {
            .flexSameSize();
            flex-direction: column-reverse;
            align-items: start;
        }
        display: flex;
        align-items: center;
        justify-content: space-between;
        background-color: #f0f0f7;
        padding: 16px 22px;
        p{
            line-height: 1.8;
            color: @global-purple;
            max-width: 83%;
            margin: 0;
            font-size: 14px;
            letter-spacing: 0.1px;
            @media (max-width: @screen-xs) {
                font-size: 12px;
                max-width: inherit;
            }
        }
        img{
            @media (max-width: @screen-xs) {
                height: 32px;
                margin-bottom: 12px;
                align-self: center;
            }
            height: 54px;
        }
    }
}
</style>