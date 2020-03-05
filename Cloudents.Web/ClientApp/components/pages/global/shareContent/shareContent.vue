<template>
  <div class="shareContent" :class="{'marketingBtn': large}">
    <span class="pr-1" v-if="!large">{{$t('shareContent_title')}} |</span>

    <div class="d-flex align-center btnWrapper">
      <v-btn
        class="shareBtns"
        :icon="!large"
        depressed
        color="#305d98"
        :ripple="false"
        @click="shareOnSocialMedia('facebook')"
      >
        <facebookSVG right style="width:9px" class="option facebook" />
      </v-btn>
      <v-btn
        class="shareBtns"
        :icon="!large"
        depressed
        color="#2cb742"
        :ripple="false"
        @click="shareOnSocialMedia('whatsApp')"
      >
        <whatsappSVG right style="width:20px" class="option whatsapp mr-2" />
      </v-btn>
      <v-btn
        class="shareBtns"
        :icon="!large"
        depressed
        color="#45ceff"
        :ripple="false"
        @click="shareOnSocialMedia('twitter')"
      >
        <twitterSVG right style="width:20px" class="option twitter mr-2" />
      </v-btn>
      <v-btn
        class="shareBtns"
        :icon="!large"
        depressed
        color="#878693"
        :ripple="false"
        @click="shareOnSocialMedia('email')"
      >
        <emailSVG right style="width:21px" class="option email mr-2" />
      </v-btn>
    </div>

    <v-tooltip v-model="showCopyToolTip" top transition="fade-transition">
      <template v-slot:activator="{}">
        <div class="copyBtn mt-3" v-if="large">
          <div class="wrap">
            <input type="text" class="copy text-truncate" name :value="link" ref="copy" readonly />
            <button
              type="button"
              class="buttonCopy px-5"
              @click="shareOnSocialMedia('link')"
              name="button"
            >{{$t('shareContent_copy')}}</button>
          </div>
        </div>
        <linkSVG
          style="width:20px"
          class="option link ml-4"
          @click="shareOnSocialMedia('link')"
          v-else
        />
      </template>
      <span>{{$t('shareContent_copy_tool')}}</span>
    </v-tooltip>
  </div>
</template>

<script>
import analyticService from '../../../../services/analytics.service'

import emailSVG from "./images/email.svg";
import facebookSVG from "./images/facebook.svg";
import whatsappSVG from "./images/whatsapp.svg";
import twitterSVG from "./images/twitter.svg";
import linkSVG from "./images/link.svg";

export default {
  name: "shareContent",
  data() {
    return {
      showCopyToolTip: false
    };
  },
  props: {
    large: {
      required: false,
      type: Boolean
    },
    link: {
      required: true,
      type: String
    },
    twitter: {
      required: true,
      type: String
    },
    whatsApp: {
      required: true,
      type: String
    },
    email: {
      required: true,
      type: Object
    }
  },
  components: { facebookSVG, whatsappSVG, emailSVG, twitterSVG, linkSVG },
  methods: {
    shareOnSocialMedia(socialMediaName) {
      let self = this;
      let linkLabel = '';
      const windowSizes =
        "menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=450,width=583";

      function openLink(url) {
        global.open(url, "", windowSizes);
      }

      function socialShareEvent(linkLabel) {
        analyticService.sb_unitedEvent('Share', socialMediaName, linkLabel);
      }

      let mediaUrl = {
        link: () => {
          self.$copyText(self.link).then(({text}) => {
            self.showCopyToolTip = true;
            socialShareEvent(text);
            setTimeout(() => {
              self.showCopyToolTip = false;
            }, 2000);
          });
        },
        email: () => {
          linkLabel = `mailto:?subject=${encodeURIComponent(self.email.subject)}&body=${encodeURIComponent(self.email.body)}`
          openLink(linkLabel)
          socialShareEvent(linkLabel)
        },
        facebook: () => {
          linkLabel = `https://www.facebook.com/sharer.php?u=${self.link}`,
          openLink(linkLabel)
          socialShareEvent(linkLabel)
        },
        whatsApp: () => {
          linkLabel = `https://wa.me/?text=${encodeURIComponent(self.whatsApp)}`
          openLink(linkLabel)
          socialShareEvent(linkLabel)
        },
        twitter: () => {
          linkLabel = `https://twitter.com/intent/tweet?text=${encodeURIComponent(self.twitter)}`
          openLink(linkLabel)
          socialShareEvent(linkLabel)
        }
      };

      let funcToInvoke = mediaUrl[socialMediaName];
      funcToInvoke();

      // switch (socialMediaName) {
      //    case 'link':
      //       self.$copyText(self.link).then(() => {
      //          self.showCopyToolTip = true;
      //          setTimeout(()=>{
      //             self.showCopyToolTip = false;
      //          },2000)
      //       })
      //       break;
      //    case 'email':
      //       openLink(`mailto:?subject=${encodeURIComponent(self.email.subject)}&body=${encodeURIComponent(self.email.body)}`);
      //       break;
      //    case 'facebook':
      //       openLink(`https://www.facebook.com/sharer.php?u=${self.link}`);
      //       break;
      //    case 'whatsApp':
      //       openLink(`https://wa.me/?text=${encodeURIComponent(self.whatsApp)}`);
      //       break;
      //    case 'twitter':
      //       openLink(`https://twitter.com/intent/tweet?text=${encodeURIComponent(self.twitter)}`);
      //       break;
      // }
    }
  },
  created() {
    if (process.env.NODE_ENV == "development") {
      if (!this.link) {
        console.error("one or more params are missed in ShareContent: link");
      }
      if (!this.twitter) {
        console.error("one or more params are missed in ShareContent: twitter");
      }
      if (!this.whatsApp) {
        console.error(
          "one or more params are missed in ShareContent: whatsApp"
        );
      }
      if (!this.email) {
        console.error("one or more params are missed in ShareContent: email");
      }
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
.shareContent {
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
    min-width: 100%;
  }
  .share-title {
    font-size: 14px;
  }
  .option {
    cursor: pointer;
  }
  .shareBtns {
    &::before {
      display: none;
    }
    .v-btn__content {
      justify-content: center !important;
    }
  }
  .btnWrapper {
    .option {
      fill: #6f6e82;
      margin: 0 !important;
    }
  }
  &.marketingBtn {
    height: unset;
    flex-wrap: wrap;
    box-shadow: none;
    padding: 0 !important;

    .btnWrapper {
      width: 100%;
      .shareBtns {
        margin: 0 10px;
        flex: 1;
        @media (max-width: @screen-xs) {
          min-width: 46px;
          margin: 0 5px;
        }
        .option {
          fill: #ffffff;
        }
        &:first-child {
          margin-left: 0;
        }
        &:last-child {
          margin-right: 0;
        }
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
          opacity: 0.5;
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