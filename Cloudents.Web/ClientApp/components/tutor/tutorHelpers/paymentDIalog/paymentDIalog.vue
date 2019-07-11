<template>
    <v-layout column class="payme-popup">
        <v-icon class="exit-btn cursor-pointer" @click="closePaymentDialog()">sbf-close</v-icon>
        <div class="payme-popup-top">
            <span v-language:inner="'payme_title_1'"/>
            <span>{{tutorName}}</span>
            <span v-language:inner="'payme_title_2'"/>
        </div>
        <iframe class="payment-iframe" width="100%" height="500" :src="paymentUrl"></iframe>
        <div class="payme-popup-bottom">
            <p v-language:inner="'payme_bottom'"/>
            <img src="./images/card.png" alt="">
        </div>
    </v-layout>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
export default {
    name: 'paymentDIalog',
    computed: {
        ...mapGetters(['getPaymentUrl','getStudyRoomData']),
        tutorName(){
            let studyRoomData = this.getStudyRoomData
            if(studyRoomData){
                return studyRoomData.tutorName
            }
        },
        paymentUrl(){
            return this.getPaymentUrl
        }
    },
    methods: {
        ...mapActions(['updatePaymentDialog']),
        closePaymentDialog(){
            this.updatePaymentDialog(false)
        }
    }
}
</script>

<style lang="less">
.payme-popup{
    position: relative;
    max-width: 728px;
    border-radius: 4px;
    background-color: #ffffff;
    .exit-btn{
        position: absolute;
        top: 16px;
        right: 16px;
        font-size: 12px;
        color: rgba(0, 0, 0, 0.541);
    }
    .payment-iframe{
        border: none;
        padding: 0 6px;
    }
    .payme-popup-top{
        padding: 40px 84px 26px 22px;
        font-size: 20px;
        font-weight: bold;
        line-height: 2;
        color: #43425d;
    }
    .payme-popup-bottom{
        display: flex;
        align-items: center;
        justify-content: space-between;
        background-color: #f0f0f7;
        padding: 16px 22px;
        p{
            font-weight: bold;
            line-height: 1.86;
            letter-spacing: normal;
            color: #5158af;
            max-width: 78%;
            margin: 0;
        }
        img{
            width: 92px;
            height: 54px;
        }
    }
}
</style>