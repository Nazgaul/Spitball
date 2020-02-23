<template>
    <div class="promotePublish mx-auto pa-4">
        <div class="wrap text-center" :key="template.id">
            <img class="img" :src="template.imageLarge" alt="">
            <div class="bottom">
                <div class="shareIt text-left">Share it</div>
                <div class="btnWrap">
                    <v-btn small medium class="elevation-0 ma-2 ml-0" color="#305d98">
                        <facebookIcon />
                    </v-btn>
                    <v-btn small class="elevation-0 ma-2" color="#2cb742">
                        <whatsappIcon />
                    </v-btn>
                    <v-btn small class="elevation-0 ma-2" color="#45ceff">
                        <twitterIcon />
                    </v-btn>
                    <v-btn small class="elevation-0 ma-2 mr-0" color="#878693">
                        <emailIcon />
                    </v-btn>
                </div>
                <div class="copyBtn mt-3">
                    <div class="wrap">
                        <input type="text" class="copy text-truncate" name="" :value="video.product" ref="copy">
                        <button type="button" class="buttonCopy px-5" @click="copyLink" name="button">Copy</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import facebookIcon from './images/facebook.svg';
import whatsappIcon from './images/whatsapp.svg';
import twitterIcon from './images/twitter.svg';
import emailIcon from './images/email.svg';

export default {
  components: {
    facebookIcon,
    whatsappIcon,
    twitterIcon,
    emailIcon
  },
  props: {
    template: {
      type: Object,
      default: () => ({})
    },
    video: {
      type: Object,
      default: () => ({})
    }
  },
  data() {
    return {
      
      }
  },
  methods: {
    copyLink() {
      const copyText = this.$refs.copy;
      copyText.select();
      copyText.setSelectionRange(0, 99999)
      document.execCommand("copy");
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
