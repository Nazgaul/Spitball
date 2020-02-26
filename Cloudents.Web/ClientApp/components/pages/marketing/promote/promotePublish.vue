<template>
    <div class="promotePublish mx-auto pa-4">
        <div class="wrap text-center">
            <v-skeleton-loader type="image" width="100%" v-if="loading"></v-skeleton-loader>
            <img class="img" @load="onLoad" v-show="!loading" :src="publishImage" alt="">
            <shareContent 
              :link="shareContentParams.link"
              :twitter="shareContentParams.twitter"
              :whatsApp="shareContentParams.whatsApp"
              :email="shareContentParams.email"
              :fromMarketing="true"
            />
            <!-- <div class="bottom mt-3">
                <div class="shareIt text-left">{{$t('promote_shareIt')}}</div>
                <div class="btnWrap">
                    <v-btn class="elevation-0 ma-2 ml-0" color="#305d98" @click="shareOnSocialMedia('facebook')">
                        <facebookIcon />
                    </v-btn>
                    <v-btn class="elevation-0 ma-2" color="#2cb742" @click="shareOnSocialMedia('whatsApp')">
                        <whatsappIcon />
                    </v-btn>
                    <v-btn class="elevation-0 ma-2" color="#45ceff" @click="shareOnSocialMedia('twitter')">
                        <twitterIcon />
                    </v-btn>
                    <v-btn class="elevation-0 ma-2 mr-0" color="#878693" @click="shareOnSocialMedia('email')">
                        <emailIcon />
                    </v-btn>
                </div>
                <div class="copyBtn mt-3">
                    <div class="wrap">
                        <input type="text" class="copy text-truncate" name="" :value="''" ref="copy" readonly>
                        <button type="button" class="buttonCopy px-5" @click="copyLink" name="button">Copy</button>
                    </div>
                </div>
            </div> -->
        </div>
    </div>
</template>
<script>
// import facebookIcon from './images/facebook.svg';
// import whatsappIcon from './images/whatsapp.svg';
// import twitterIcon from './images/twitter.svg';
// import emailIcon from './images/email.svg';
import shareContent from '../../global/shareContent/shareContent.vue';
export default {
  components: {
    shareContent,
    // facebookIcon,
    // whatsappIcon,
    // twitterIcon,
    // emailIcon
  },
  props: {
    theme: {
      type: Number,
    },
    document: {
      type: Object,
      default: () => ({})
    },
    dataType: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      loading: true
    }
  },
  computed: {
    user() {
      return this.$store.getters.accountUser;
    },
    urlLink() {
      let urlLink;
      if(this.document) {
        urlLink = `${global.location.origin}/d/${this.user.id}?t=${Date.now()}&theme=${this.theme}`;
      } else {
        urlLink = `${global.location.origin}/p/${this.user.id}?t=${Date.now()}`;
      }
      return urlLink;
    },
    shareContentParams(){
      let urlLink = this.urlLink;
      let user = this.user;
      return {
        link: urlLink,
        twitter: this.$t('shareContent_share_profile_twitter',[user.name,urlLink]),
        whatsApp: this.$t('shareContent_share_profile_whatsapp',[user.name,urlLink]),
        email: {
          subject: this.$t('shareContent_share_profile_email_subject',[user.name]),
          body: this.$t('shareContent_share_profile_email_body',[user.name,urlLink]),
        }
      }
    },
    publishImage() {
      let user = this.user;
      let rtl = global.country === 'IL' ? true : false;
      //TODO: move to store
      if(this.dataType === 'profile') {
        return `${window.functionApp}/api/share/profile/${user.id}?width=420&amp;height=220&amp;rtl=${rtl}`
      }
      return `${window.functionApp}/api/share/document/${this.document.id}?theme=${this.theme}&width=420&amp;height=220&amp;rtl=${rtl}`
    },
  },
  methods: {
    // copyLink() {
    //   const copyText = this.$refs.copy;
    //   copyText.select();
    //   copyText.setSelectionRange(0, 99999)
    //   document.execCommand("copy");
    // },
    // shareOnSocialMedia(socialMediaName) {
    //   let windowSizes = 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=450,width=583';
    //   let user = this.$store.getters.accountUser;
    //   let urlLink = `${global.location.origin}/p/${user.id}?t=${Date.now()}?theme=${this.theme}`;
    //   let shareContent = this.shareContentParams;
    //   debugger
    //   switch (socialMediaName) {
    //     case 'email':
    //       window.location.href = `mailto:?subject=${encodeURIComponent(shareContent.email.subject)}&body=${encodeURIComponent(shareContent.email.body)}`;
    //       break;
    //     case 'facebook':
    //       global.open(`https://www.facebook.com/sharer.php?u=${urlLink}`,'', windowSizes);
    //       break;
    //     case 'whatsApp':
    //       global.open(`https://wa.me/?text=${encodeURIComponent(shareContent.whatsApp)}`,'', windowSizes);
    //       break;
    //     case 'twitter':
    //       global.open(`https://twitter.com/intent/tweet?text=${encodeURIComponent(shareContent.twitter)}`, '', windowSizes);
    //       break;
    //   }
    // },
    onLoad() {
      this.loading = false
    }
  }
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';
.promotePublish {
  max-width: 454px;
  border-radius: 8px;
  border: solid 1px #dddddd;

  .wrap {
    width: 100%;
    .img {
      width: 100%;
    }
  }
  .bottom {
    .shareIt {
        font-size: 16px;
        font-weight: 600;
        color: @global-purple;
    }
    .btnWrap {
      display: flex;
      justify-content: space-between;
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
    }
    .copyBtn {
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
