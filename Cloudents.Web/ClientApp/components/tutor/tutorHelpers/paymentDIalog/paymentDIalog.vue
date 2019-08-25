<template>
    <v-layout column class="payme-popup">
        <v-icon class="exit-btn cursor-pointer" @click="closePaymentDialog()">sbf-close</v-icon>
        <div class="payme-popup-top">
            <span v-language:inner="'payme_title_1'"/>
            <span>{{tutorName}}</span>
            <span v-language:inner="'payme_title_2'"/>
        </div>
        <iframe class="payment-iframe" width="100%" height="475" :src="paymentUrl"></iframe>
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
    data() {
        return {
            tutorName: ''
        }
    },
    computed: {
        ...mapGetters(['getPaymentURL','getProfile','getStudyRoomData']),
        // tutorName(){
            // let studyRoomData = this.getStudyRoomData
            // if(!!studyRoomData){
            //     return studyRoomData.tutorName
            // }else{
            //     return this.getProfile.user.tutorData.firstName
            // }
        // },
        paymentUrl(){
            return this.getPaymentURL
        }
    },
    methods: {
        ...mapActions(['updatePaymentDialog']),
        closePaymentDialog(){
            this.updatePaymentDialog(false)
        },
    },
    created() {
        if(this.$route.name === 'profile'){
            this.tutorName = this.getProfile.user.tutorData.firstName
        } else{
            this.tutorName = this.getStudyRoomData.tutorName
        }

    },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.payme-popup{
    position: relative;
    max-width: 800px;
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
        padding: 40px 40px 15px 22px;
        font-size: 18px;
        font-weight: 700;
        line-height: 1.5;
        color: @global-purple;
    }
    .payme-popup-bottom{
        display: flex;
        align-items: center;
        justify-content: space-between;
        background-color: #f0f0f7;
        padding: 16px 22px;
        p{
            font-weight: 700;
            line-height: 1.5;
            letter-spacing: normal;
            color: #5158af;
            max-width: 83%;
            margin: 0;
        }
        img{
            height: 45px;
        }
    }
}
</style>