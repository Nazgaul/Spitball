<template>
   <div class="shareContent" :class="{'btnWrap': fromMarketing}">
      <span class="pr-1" v-if="!fromMarketing">{{$t('shareContent_title')}} |</span>

      <div class="d-flex align-center">
         <facebookSVG style="width:9px" class="option facebook ml-3" @click="shareOnSocialMedia('facebook')"/>
         <whatsappSVG style="width:20px" class="option whatsapp ml-7 ml-sm-6" @click="shareOnSocialMedia('whatsApp')"/>
         <twitterSVG style="width:20px" class="option twitter ml-5" @click="shareOnSocialMedia('twitter')"/>
         <emailSVG style="width:21px" class="option email ml-5" @click="shareOnSocialMedia('email')"/>
      </div>

      <div class="copyBtn mt-3" v-if="fromMarketing">
         <div class="wrap">
            <input type="text" class="copy text-truncate" name="" :value="link" ref="copy" readonly>
            <button type="button" class="buttonCopy px-5" @click="shareOnSocialMedia('link')" name="button">Copy</button>
         </div>
      </div>

      <v-tooltip v-model="showCopyToolTip" top transition="fade-transition" v-else>
         <template v-slot:activator="{}">
            <linkSVG style="width:20px" class="option link ml-5" @click="shareOnSocialMedia('link')"/>
         </template>
         <span>{{$t('shareContent_copy_tool')}}</span>
      </v-tooltip>
   </div>
</template>

<script>
import emailSVG from './images/email.svg';
import facebookSVG from './images/facebook.svg';
import whatsappSVG from './images/whatsapp.svg';
import twitterSVG from './images/twitter.svg';
import linkSVG from './images/link.svg';

export default {
   name: 'shareContent',
   data() {
      return {
         showCopyToolTip:false,
      }
   },
   props:{
      fromMarketing: {
         required: false,
         type: Boolean
      },
      link:{
         required: true,
         type: String
      },
      twitter:{
         required: true,
         type: String
      },
      whatsApp:{
         required: true,
         type: String
      },
      email:{
         required: true,
         type: Object
      },
   },
   components:{facebookSVG,whatsappSVG,emailSVG,twitterSVG,linkSVG},
   methods: {
      shareOnSocialMedia(socialMediaName) {
         let windowSizes = 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=450,width=583';
         let self = this;
         switch (socialMediaName) {
            case 'link':
               self.$copyText(self.link).then(() => {
                  self.showCopyToolTip = true;
                  setTimeout(()=>{
                     self.showCopyToolTip = false;
                  },2000)
               })
               break;
            case 'email':
               window.location.href = `mailto:?subject=${encodeURIComponent(self.email.subject)}&body=${encodeURIComponent(self.email.body)}`
               break;
            case 'facebook':
               global.open(`https://www.facebook.com/sharer.php?u=${self.link}`,'', windowSizes);
               break;
            case 'whatsApp':
               global.open(`https://wa.me/?text=${encodeURIComponent(self.whatsApp)}`,'', windowSizes);
               break;
            case 'twitter':
               global.open(`https://twitter.com/intent/tweet?text=${encodeURIComponent(self.twitter)}`, '', windowSizes)
               break;
         }
      }
   },
   created() {
      if(!this.link){
         console.error('one or more params are missed in ShareContent: link')
      }
      if(!this.twitter){
         console.error('one or more params are missed in ShareContent: twitter') 
      }
      if(!this.whatsApp){
         console.error('one or more params are missed in ShareContent: whatsApp')
      }
      if(!this.email){
         console.error('one or more params are missed in ShareContent: email')
      }
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.shareContent{
   padding: 16px;
   min-width: 292px;
   width: 100%;
   height: 52px;
   border-radius: 8px;
   box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
   background-color: #ffffff;
   color: #43425d;
   display: flex;
   align-items: center;
   @media (max-width: @screen-xs) {
      padding: 14px;
      border-radius: 0;
      box-shadow: none;
   }
   .share-title{
      font-size: 14px;
   }
   .option{
      cursor: pointer;
   }
   &.btnWrap {
      height: unset;
      flex-wrap: wrap;
      button {
        min-width: 90px !important;
        flex: 1;
        @media (max-width: @screen-xs) {
          min-width: 46px !important;
        }
        svg {
          width: 20px;
        }
      }
      .copyBtn {
         width: 100%;
         .wrap {
            display: flex;
            justify-content: flex-end;
            border: solid 1px #dddddd;
            border-radius: 8px;
            height: 34px;
            .copy {
               flex-grow: 1;
               width: 100%;
               outline: none;
               padding: 0 10px;
               color: @global-purple;
               opacity: 0.5
            }
            .buttonCopy {
               outline: none;
               background: rgba(189, 192, 209, 0.5);
            }
         }
      }
    }
}
</style>