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
import * as routeNames from '../../../../routes/routeNames.js';

export default {
   name: 'shareContent',
   components:{facebookSVG,whatsappSVG,emailSVG,twitterSVG,linkSVG},
   methods: {
      shareOnSocialMedia(socialMediaName) {
         if(this.$route.name === routeNames.Profile){
            let teacherName = this.$store.getters.getProfile.user.name;
            let urlLink = `${global.location.origin}/p/${this.$store.getters.getProfile.user.id}`;

            let profileObj = {
               link: urlLink,
               twitter: this.$t('shareContent_share_profile_twitter',[teacherName,urlLink]),
               whatsApp: this.$t('shareContent_share_profile_whatsapp',[teacherName,urlLink]),
               email: {
                  subject: this.$t('shareContent_share_profile_email_subject',[teacherName]),
                  body: this.$t('shareContent_share_profile_email_body',[teacherName,urlLink]),
               }
            }
            _share(profileObj);
            return
         }
         if(this.$route.name === routeNames.Document){
            let urlLink = `${global.location.origin}/d/${this.$route.params.id}`;
            let courseName = this.$route.params.courseName;
            let itemType = this.$store.getters.getDocumentDetails.documentType;
         this.$t('')
            let itemObj = {
               twitter: 'I found great content for {cousename} check it out on Spitball {content_link}',
               whatsapp: 'I found great content for {cousename} check it out on Spitball {content_link}',
               email: {
                  subject: 'Think this will really help you with {coursename}',
                  body: 
                        `Hey, 
                        I found a great {content_type} for {cousename} check it out on Spitball {content_link}`
               }
            }
            _share(itemObj);
            return
         }
         function _share(infoObject){
            switch (socialMediaName) {
               case 'link':
                  this.$copyText(link).then(() => {
                     debugger
                  })
                  break;
               case 'email':
                  global.open(`https://mail.google.com/mail/?view=cm&su=${infoObject[socialMediaName].subject}&body=${infoObject[socialMediaName].body}`, "_blank");
                  break;
               case 'facebook':
                  global.open(`https://www.facebook.com/sharer.php?u=${encodeURIComponent(infoObject.link)}`, "_blank");
                  break;
               case 'twitter':
                  global.open(`https://twitter.com/intent/tweet?url=${infoObject.link}&text=${infoObject[socialMediaName]}`, "_blank");
                  break;
               case 'whatsApp':
                  global.open(`https://web.whatsapp.com/send?text=${infoObject.link}`, "_blank");
                  break;
            }
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