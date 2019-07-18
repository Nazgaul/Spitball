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
        <v-dialog v-if="confirmExit"
        v-model="confirmExit"
        content-class="payme-popup-exit"
        :fullscreen="$vuetify.breakpoint.xsOnly" persistent>


        <v-layout align-center column class="payme-popup-exit">
            <div class="payme-popup-exit-top">
                <span>are you sure you want to exit?</span>
            </div>
            <div class="payme-popup-exit-btns pb-3">
                <v-btn depressed @click="confirmExit=false">cancle</v-btn>
                <v-btn depressed color="info" @click="setConfirmExit">yes</v-btn>
            </div>
        </v-layout>

      </v-dialog>
    </v-layout>
</template>

<script>
import { mapActions, mapGetters, mapMutations } from 'vuex';
import studyRoomService from '../../../../services/studyRoomsService.js'

export default {
    name: 'paymentDIalog',
    data() {
        return {
            confirmExit: false
        }
    },
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
            this.confirmExit = true;
        },
        setConfirmExit(){ 
           studyRoomService.skipNeedPayment({studyRoomId:this.getStudyRoomData.roomId })
           this.updatePaymentDialog(false)
        }
    },
}
</script>

<style lang="less">
.payme-popup-exit{
    width: 600px;
    border-radius: 4px;
    background-color: #ffffff;
        .payme-popup-exit-top{
        padding: 15px;
        font-size: 18px;
        font-weight: 700;
        line-height: 1.5;
        color: #43425d;
        text-align: center;
        }
        .payme-popup-exit-btns{

        }
    }
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
        color: #43425d;
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