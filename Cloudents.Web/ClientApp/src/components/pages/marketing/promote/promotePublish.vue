<template>
    <div class="promotePublish mx-auto pa-sm-4">
        <div class="d-block d-sm-none publishTitle mb-4">{{$t('promote_publish')}}</div>
        <div class="wrap text-center">
            <v-skeleton-loader type="image" width="100%" v-if="loading"></v-skeleton-loader>
            <img class="img" @load="onLoad" v-show="!loading" :src="publishImage" alt="">
            <div class="shareIt my-3 text-left">{{$t('promote_shareIt')}}</div>
            <shareContent  :defaultStyle="true"
              :link="shareContentParams.link"
              :twitter="shareContentParams.twitter"
              :whatsApp="shareContentParams.whatsApp"
              :email="shareContentParams.email"
              :large="true"
            />
        </div>
    </div>
</template>
<script>
const shareContent = () => import(/* webpackChunkName: "shareContent" */'../../global/shareContent/shareContent.vue');

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
    },
    resource: {
      required: false
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
        urlLink = `${global.location.origin}/course/${this.document.id}?t=${Date.now()}`;
      } else {
        urlLink = `${global.location.origin}/p/${this.user.id}?t=${Date.now()}`;
      }
      return urlLink;
    },
    shareContentParams(){
      let urlLink = this.urlLink;
      let user = this.user;
      if(this.document) {
        let courseName = this.$store.getters.getCourseName
        let courseDescription = this.$store.getters.getDescription
        return {
          link: urlLink,
          twitter: this.$t('shareContent_course_twitter_whatsapp',[courseName]),
          whatsApp: this.$t('shareContent_course_twitter_whatsapp',[courseName]),
          email: {
            subject: this.$t('shareContent_course_email_subject',[courseName]),
            body: this.$t('shareContent_course_email_body',[courseName, courseDescription]),
          }
        }
      }
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
      let rtl = global.country === 'IL';
      let version = parseInt(this.$store.getters.getCourseCoverImage.split('?')[1].split('=')[1])
      //TODO: move to store
      if(this.dataType === 'profile') {
        return `${window.functionApp}/api/share/profile/${user.id}?width=420&height=220&rtl=${rtl}`
      } 
      return `${window.functionApp}/api/image/studyRoom/${this.document.id}?version=${version++}&width=420&height=220&mode=crop`
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
  margin-top: 25px;
  @media (max-width: @screen-xs) {
    border: none;
    margin-top: 10px;
  }
  .publishTitle {
    color: @global-purple;
    font-weight: 600;
    font-size: 20px;
    @media (max-width: @screen-xs) {
        font-size: 18px;
    }
  }
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
