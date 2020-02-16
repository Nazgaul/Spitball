<template>
   <div class="shareContent">
      <div class="share-icon-container">
         <v-list>
            <v-list-tile>
               <span @click="shareOnSocialMedia('facebook')" class="share-btn facebook-share-btn mb-1">
                  <v-icon class="share-icon">sbf-facebook-share</v-icon>
                  <span class="share-text" v-language:inner>referralDialog_share_facebook</span>
               </span>
            </v-list-tile>
            <v-list-tile>
               <span @click="shareOnSocialMedia('twitter')" class="share-btn twitter-share-btn mb-1">
                  <v-icon class="share-icon referral-twitter-icon">sbf-tweeter-share</v-icon>
                  <span class="share-text referral-twitter" v-language:inner>referralDialog_share_tweeter</span>
               </span>    
            </v-list-tile>
            <v-list-tile>
               <span @click="shareOnSocialMedia('gmail')" class="share-btn gmail-share-btn mb-1">
                  <v-icon class="share-icon referral-gmail-icon">sbf-google-share</v-icon>
                  <span class="share-text" v-language:inner>referralDialog_share_google</span>
               </span>
            </v-list-tile>
            <v-list-tile>
               <span @click="shareOnSocialMedia('whatsApp')" class="share-btn whatsup-share-btn">
                  <v-icon class="share-icon">sbf-whatsup-share</v-icon>
                  <span class="share-text" v-language:inner>referralDialog_share_whatsup</span>
               </span>
            </v-list-tile>
         </v-list>
      </div>
   </div>
</template>

<script>
export default {
   name: 'shareContent',
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
               global.open(`https://www.facebook.com/sharer.php?u=${msg.url}`, "_blank");
               break;
            case 'linkedin':
               global.open(`https://www.linkedin.com/shareArticle?mini=true&url=${msg.url}&title=${msg.title}&summary=${msg.text}`, "_blank");
               break;
            case 'twitter':
               global.open(`https://twitter.com/intent/tweet?url=${msg.url}&text=${msg.text}`, "_blank");
               break;
            case 'gmail':
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
      padding: 12px;
   position: sticky;
   top: 80px;
   min-width: 292px;
   width: 292px;
   height: max-content;
   border-radius: 8px;
   box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
   background-color: #ffffff;
   text-align: center;
   font-size: 14px;
   color: #43425d;
       .share-icon-container {
      order: 1;
      display: flex;
      max-width: 372px;
      width: 100%;
      justify-content: space-around;
      .v-icon{

        &.share-icon {
          cursor: pointer;
          /*rtl:ignore*/
          direction: ltr;
          font-size: 16px;
          color: white;
          overflow: hidden;
        }
        &.referral-twitter-icon{
          overflow: inherit;
        }
        &.referral-gmail-icon{
          font-size: 14px;
        }
    }
      span {
        cursor: pointer;
        .share-icon {
          /*rtl:ignore*/
          direction: ltr;
          height: 16px;
        }
      }
      .share-btn{
        border-radius: 4px;
        display: flex;
        align-items: center;
        width: 80px;
        padding: 4px 12px 4px 8px;
        margin-right: 8px;
        &:last-child{
          margin-right: 0;
        }
        }
      .share-text{
        font-size: 12px;
        color: @color-white;
        margin-left: 8px;
        &.referral-twitter{
          margin-left:0;
        }

      }
      .whatsup-share-btn {
        background: rgb(76, 175, 80);
      }
      .facebook-share-btn {
        background: rgb(59, 89, 152);
      }
      .twitter-share-btn {
        background: rgb(85, 172, 238);
      }
      .gmail-share-btn {
        background: rgb(255, 40, 0);
      }

    }
}
</style>