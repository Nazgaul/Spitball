<template>
   <v-card :color="messageColor" tile class="remoteMessageContainer pa-2 pb-1">
      <v-card-title class="messageTitle pa-0" v-text="userName"/>
      <v-card-text v-strLinkify="''" v-if="isTextMessage" dir="auto" class="messageText px-0 pb-0 pt-2" v-text="message.text"/>
      <div class="pt-2" v-else>
         <a :href="message.href" target="_blank">
            <v-img :src="fileSrc" height="140px" width="190px"/>
         </a>
      </div>
      <v-card-actions class="messageBottom d-flex align-baseline justify-end px-0 pb-0 pt-2">
         <span class="messageDate">{{messageDate}}</span>
      </v-card-actions>
   </v-card>
</template>

<script>
import * as routeNames from '../../../../routes/routeNames.js'

export default {
   props:{
      message:{
         type:Object,
         required:true
      }
   },
   computed: {
      messageColor(){
         return this.$route.name == routeNames.MessageCenter ? '#ffffff' : '#EFEFF0';
      },
      isTextMessage(){
         return this.message.type == 'text'
      },
      userName(){
         return this.message.name
      },
      fileSrc(){
         return this.$proccessImageUrl(this.message.src, {width:190, height:140})
      },
      messageDate(){
         let momentDate = this.$moment(this.message.dateTime);
         let isToday = momentDate.isSame(this.$moment(), 'day');
         if(isToday){
            return momentDate.format('LT');
         }else{
            if (this.$moment().diff(momentDate, 'days') >= 1) {
               return momentDate.format('l');
            }else{
               return momentDate.calendar();
            }
         }
      }
   },

}
</script>

<style lang="less">
.remoteMessageContainer{
   &.v-card {
      // overide vuetify new border radius on v-card 
      border-radius: 8px 8px 8px 0 !important;
   }
   max-width: 70%;
   width: -moz-fit-content;
   width: fit-content;

   box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.25) !important;
   margin-right: auto;
   margin-left: unset;
   .messageTitle{
      font-size: 14px;
      font-weight: 600;
      color: black;
      line-height: 1;
   }
   .messageText{
      font-size: 14px;
      color: black !important;
      line-height: normal;
      white-space: pre-wrap;
      word-break: break-word;
   }
   .messageBottom{
      .messageDate{
         color:rgba(0, 0, 0, 0.6);
         font-size: 11px;
      }
   }
}
</style>