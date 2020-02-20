<template>
   <div class="shareContent">
      <span class="pr-1">{{$t('shareContent_title')}}</span> |
      <facebookSVG style="width:9px" class="option ml-3" @click="shareOnSocialMedia('facebook')"/>
      <whatsappSVG style="width:20px" class="option ml-7 ml-sm-6" @click="shareOnSocialMedia('whatsapp')"/>
      <twitterSVG style="width:20px" class="option ml-5" @click="shareOnSocialMedia('twitter')"/>
      <emailSVG style="width:21px" class="option ml-5" @click="shareOnSocialMedia('email')"/>
      <linkSVG style="width:20px" class="option ml-5" @click="shareOnSocialMedia('link')"/>
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
   components:{facebookSVG,whatsappSVG,emailSVG,twitterSVG,linkSVG},
   methods: {
      shareOnSocialMedia(socialMedia) {
         let msg = {
            url: window.location.href,
            title:'bla bla title',
            text: `bla bla text:`
         }
         switch (socialMedia) {
            case 'whatsApp':
               global.open(`https://web.whatsapp.com/send?text=${msg.url}`, "_blank");
               break;
            case 'facebook':
               global.open(`https://www.facebook.com/sharer.php?u=${encodeURIComponent(msg.url)}`, "_blank");
               break;
            case 'twitter':
               global.open(`https://twitter.com/intent/tweet?url=${msg.url}&text=${msg.text}`, "_blank");
               break;
            case 'email':
               global.open(`https://mail.google.com/mail/?view=cm&su=${msg.title}&body=${msg.text}`, "_blank");
               break;
         }
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
}
</style>