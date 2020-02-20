<template>
   <div class="shareContent">
      <span class="pr-1">{{$t('shareContent_title')}}</span> |
      <facebookSVG style="width:9px" class="option ml-3" @click="shareOnSocialMedia('facebook')"/>
      <whatsappSVG style="width:20px" class="option ml-7 ml-sm-6" @click="shareOnSocialMedia('whatsApp')"/>
      <twitterSVG style="width:20px" class="option ml-5" @click="shareOnSocialMedia('twitter')"/>
      <emailSVG style="width:21px" class="option ml-5" @click="shareOnSocialMedia('email')"/>
      <v-tooltip v-model="showCopyToolTip" top transition="fade-transition">
         <template v-slot:activator="{ on }">
            <linkSVG style="width:20px" class="option ml-5" @click="shareOnSocialMedia('link')"/>
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
import * as routeNames from '../../../../routes/routeNames.js';

export default {
   name: 'shareContent',
   data() {
      return {
         showCopyToolTip:false,
      }
   },
   components:{facebookSVG,whatsappSVG,emailSVG,twitterSVG,linkSVG},
   methods: {
      shareOnSocialMedia(socialMediaName) {
         let self = this;
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
             
            let itemObj = {
               link: urlLink,
               twitter: this.$t('shareContent_share_item_twitter',[courseName,urlLink]),
               whatsApp: this.$t('shareContent_share_item_whatsapp',[courseName,urlLink]),
               email: {
                  subject: this.$t('shareContent_share_item_email_subject',[courseName]),
                  body: this.$t('shareContent_share_item_email_body',[itemType,courseName,urlLink]),
               }
            }
            _share(itemObj);
            return
         }
         function _share(infoObject){
            switch (socialMediaName) {
               case 'link':
                  self.$copyText(infoObject.link).then(() => {
                     self.showCopyToolTip = true;
                     setTimeout(()=>{
                        self.showCopyToolTip = false;
                     },2000)
                  })
                  break;
               case 'email':
                  window.location.href = `mailto:?subject=${encodeURIComponent(infoObject[socialMediaName].subject)}&body=${encodeURIComponent(infoObject[socialMediaName].body)}`
                  break;
               case 'facebook':
                  global.open(`https://www.facebook.com/sharer.php?u=${infoObject.link}`, "_blank");
                  break;
               case 'whatsApp':
                  global.open(`https://wa.me/?text=${encodeURIComponent(infoObject[socialMediaName])}`, "_blank");
                  break;
               case 'twitter':
                  global.open(`https://twitter.com/intent/tweet?text=${encodeURIComponent(infoObject[socialMediaName])}`, "_blank");
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