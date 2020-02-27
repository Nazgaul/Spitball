<template>
    <div class="promotePublish mx-auto pa-4">
        <div class="wrap text-center">
            <v-skeleton-loader type="image" width="100%" v-if="loading"></v-skeleton-loader>
            <img class="img" @load="onLoad" v-show="!loading" :src="publishImage" alt="">
            <div class="shareIt text-left">{{$t('promote_shareIt')}}</div>
            <shareContent 
              :link="shareContentParams.link"
              :twitter="shareContentParams.twitter"
              :whatsApp="shareContentParams.whatsApp"
              :email="shareContentParams.email"
              :fromMarketing="true"
            />
        </div>
    </div>
</template>
<script>
import shareContent from '../../global/shareContent/shareContent.vue';

export default {
  components: { shareContent },
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
        urlLink = `${global.location.origin}/d/${this.document.id}?t=${Date.now()}&theme=${this.theme}`;
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
  .shareIt {
      font-size: 16px;
      font-weight: 600;
      color: @global-purple;
  }
}
</style>
