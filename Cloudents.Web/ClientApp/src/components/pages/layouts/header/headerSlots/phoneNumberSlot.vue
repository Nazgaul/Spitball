<template>
   <a :href="`https://wa.me/${fullNumber}?text=${text?encodeURIComponent(text) : encodeURIComponent(' ') }`"
   target="_blank" class="phoneNumberSlot pe-1 pe-sm-3 pe-md-6">
      <whatsAppIcon class="whatsApp-icon me-1"/>
      <span class="text-phone">{{previewNumber}}</span>
   </a>
</template>

<script>
// TODO we need to fix it with global class/object

import whatsAppIcon from './whatsAppIcon.svg';
export default {
   name:'phoneNumberSlot',
   components:{whatsAppIcon},
   data() {
      return {
         country:{
            // IL:{
            //    previewNumber:'052-507-5638',
            //    fullNumber:'972525075638',
            //    text: this.$(`headerSlots_phoneNumberSlot_text`),
            // },
            IN:{
               previewNumber:'+91 8618134279',
               fullNumber:'918618134279',
               text:''
            }
         }
      }
   },
   computed: {
      // currentCountry(){
      //    if(global.siteName === 'frymo'){
      //       return 'IN';
      //    }
      //    else{
      //       return 'IN'
      //    }
      // },
      previewNumber(){
         return this.country['IN'].previewNumber;
      },
      fullNumber(){
         return this.country['IN'].fullNumber;
      },
      text(){
         return this.country['IN'].text;
      }
   },
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';
.phoneNumberSlot{
   cursor: pointer;
   .whatsApp-icon{
      width: 24px;
      fill: #43425d;
      vertical-align: sub;
      @media (max-width: @screen-xs) {
         width: 20px;
      }
   }
   .text-phone{
      font-size: 22px;
      font-weight: 600;
      color: #43425d;
         @media (max-width: @screen-xs) {
            font-size: 16px;   
         }
   }
}
</style>