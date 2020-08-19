<template>
   <v-card :color="messageColor" tile class="myMessageContainer pa-2 pb-1">
      <v-card-title class="messageTitle pa-0" v-text="userName"/>
      <v-card-text v-strLinkify="''" v-if="isTextMessage" dir="auto" class="messageText px-0 pb-0 pt-2" v-text="message.text"/>
      <div class="pt-2" v-else>
         <a :href="message.href" target="_blank">
            <v-img :src="fileSrc" height="140px" width="190px"/>
         </a>
      </div>
      <v-card-actions class="messageBottom d-flex align-baseline justify-end px-0 pb-0 pt-2">
         <span class="messageDate">{{messageDate}}</span>
         <v-icon :color="message.unreadMessage? '#00000077':'#4fc3f7'" class="ms-1" size="10">sbf-readIcon</v-icon>
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
         return this.$route.name == routeNames.MessageCenter ? '#deedff' : '#E4F1FE';
      },
      isTextMessage(){
         return this.message.type == 'text'
      },
      userName(){
         return this.$store.getters.accountUser?.name
      },
      fileSrc(){
         return this.$proccessImageUrl(this.message.src, 190, 140, 'crop')
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
   // methods: {
      // stringy(str,linkClassName = ''){
      //    let urlPattern = /\b(?:https?|ftp):\/\/[a-z0-9-+&@#\/%?=~_|!:,.;]*[a-z0-9-+&@#\/%=~_|]/gim;
      //    let pseudoUrlPattern = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
      //    let emailAddressPattern = /[\w.]+@[a-zA-Z_-]+?(?:\.[a-zA-Z]{2,6})+/gim;
      //    let modifiedText = str;
      //    let matchedResults = modifiedText.match(urlPattern) || modifiedText.match(pseudoUrlPattern) || modifiedText.match(emailAddressPattern) ;
      //    if(matchedResults){
      //       matchedResults.forEach(result=>{
      //          let prefix = result.toLowerCase().indexOf('http') === -1 && result.toLowerCase().indexOf('ftp') === -1 ? '//' : '';
      //          if(result.match(urlPattern)?.length){
      //             modifiedText = modifiedText.replace(result, `<a class="${linkClassName}" href="${prefix + result.trim()}" target="_blank">${result}</a>`)
      //          }
      //          if(result.match(pseudoUrlPattern)?.length){
      //             modifiedText = modifiedText.replace(result, `<a class="${linkClassName}" href="${prefix + result.trim()}" target="_blank">${result}</a>`)
      //          }
      //          if(result.match(emailAddressPattern)?.length){
      //             modifiedText = modifiedText.replace(result, `<a class="${linkClassName}" href="mailto:${result}">${result}</a>`)
      //          }
      //       }); 
      //    }
      //    return modifiedText
      // }
   // },
   // mounted() {
   //    let el = document.getElementById(`messageText_${this.message.index}`)
   //    if(el){
   //       el.innerHTML = this.stringy(this.message.text)
   //    }
   // },

}
</script>

<style lang="less">
 @import '../../../..styles/mixin.less';
.myMessageContainer{
   &.v-card {
      // overide vuetify new border radius on v-card 
      border-radius: 8px 8px 0 8px !important;
   }
   max-width: 70%;
   .widthFitContent();
   
   box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.25) !important;
   margin-right: unset;
   margin-left: auto;
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