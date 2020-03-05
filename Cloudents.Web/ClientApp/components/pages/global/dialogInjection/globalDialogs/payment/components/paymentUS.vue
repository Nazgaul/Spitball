<template>
    <v-layout column class="payme-popup">
        <v-icon class="exit-btn cursor-pointer" @click="closeDialog">sbf-close</v-icon>
        <div class="payme-popup-top pt-4">
            <div class="payme-top-title" v-language:inner="'payme_top_title'"/>
            <v-layout wrap class="payme-content pt-4 pt-sm-5 pb-2 pb-sm-4">
                <v-flex xs12 sm3 class="payme-content-div">
                    <img class="payme-content-img" src="./images/timer.png">
                    <span class="payme-content-txt pt-0 pt-sm-2" v-language:inner="'payme_content_txt_time'"/>
                </v-flex>
                <v-flex xs12 sm3 class="payme-content-div mx-0 mx-sm-4">
                    <img class="payme-content-img" src="./images/sheild.png" >
                    <span class="payme-content-txt pt-0 pt-sm-2" v-language:inner="'payme_content_txt_sheild'"/>
                </v-flex>
                <v-flex xs12 sm3 class="payme-content-div">
                    <img class="payme-content-img" src="./images/hands.png">
                    <span class="payme-content-txt pt-0 pt-sm-2" v-language:inner="'payme_content_txt_hands'"/>
                </v-flex>
            </v-layout>
            <!-- <div class="payme-top-desc pb-4" v-language:inner="'payme_top_desc'"/> -->
            <div class="payme-top-desc pb-4 px-sm-4">
               Lorem ipsum dolor sit amet consectetur adipisicing elit. Iure velit nisi, fugiat necessitatibus optio in provident nesciunt eveniet! Laborum aspernatur vero non quasi reiciendis aperiam omnis quas culpa quia perspiciatis!
            </div>
            <v-skeleton-loader class="mb-4 px-sm-4" v-show="isLoading" width="100%" height="44" type="button"></v-skeleton-loader>
            <div v-show="!isLoading" id="paypal-button-container"></div>
        </div>
         <div class="payme-popup-bottom">
               <p v-language:inner="'payme_bottom'"/>
               <img src="./images/card.png" alt="">
         </div>
    </v-layout>
</template>

<script>
import * as routeNames from '../../../dialogNames.js';
export default {
   name:'paymentUS',
   data() {
      return {
         isLoading:false,
      }
   },
   methods: {
      closeDialog(){
         let isStudyRoom = this.$store.getters.getStudyRoomData?.roomId && this.$route.name === routeNames.StudyRoom.name;
         if(isStudyRoom){
               let isExit = confirm(this.$t("payme_are_you_sure_exit"))
               if(isExit){
                  this.$router.push('/');
               }
         }else{
               this.$closeDialog()
         }
      }
   },
  mounted() {
      let self = this;
      let paypalUrl = `https://www.paypal.com/sdk/js?client-id=${window.paypalClientId}&commit=false`;
      this.$loadScript(paypalUrl)
         .then(() => {
            window.paypal
            .Buttons({
                createOrder: function(data, actions) {
                  debugger
                  self.isLoading = true;
                  return actions.order.create({
                     purchase_units: [
                           {
                              reference_id: "PUHF",
                              amount: {
                                 value: 10,
                                 currency: 'USD'
                              }
                           },
                     ]
                  });
                },
                onApprove: function(data) {
                  let params = {
                     id: data.orderID,
                     roomId: self.$store.getters.getStudyRoomData.roomId,
                  }
                  self.$store.dispatch('updatePaypalStudyRoom',params).then(()=>{
                     debugger
                     self.isLoading = false
                  })
                }
            })
            .render('#paypal-button-container');
        });
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
    .payme-popup-top{
        text-align: center;
         .v-skeleton-loader__button {
               width: inherit;
               height: inherit;
         }

        .payme-top-title{
            font-size: 20px;
            font-weight: bold;
            color:@global-purple; 
            @media (max-width: @screen-xs) {
                font-size: 18px;
            }
        }
        .payme-top-desc{
            font-size: 14px;
            font-weight: 600;
            line-height: 1.5;
            letter-spacing: -0.17px;
            color:@global-purple; 
            @media (max-width: @screen-xs) {
                padding: 0 24px;
            }
        }
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