<template>
  <div class="profileFooter d-sm-flex d-block text-center align-center justify-space-around pa-3 pa-sm-0">
    <div class="mb-3 mb-sm-0">{{tutorName}}</div>
    <shareContent
        sel="share_area"
        :link="shareContentParams.link"
        :twitter="shareContentParams.twitter"
        :whatsApp="shareContentParams.whatsApp"
        :email="shareContentParams.email"
        class="shareContentProfile justify-center"
    />
  </div>
</template>

<script>
const shareContent = () => import(/* webpackChunkName: "shareContent" */'../../../pages/global/shareContent/shareContent.vue');
export default {
    name: 'profileFooter',
    components: {
        shareContent
    },
    computed: {
        shareContentParams(){
            let urlLink = `${global.location.origin}/p/${this.$route.params.id}?t=${Date.now()}`;
            let userName = this.$store.getters.getProfile?.user?.name;
            let paramObJ = {
                link: urlLink,
                twitter: this.$t('shareContent_share_profile_twitter',[userName,urlLink]),
                whatsApp: this.$t('shareContent_share_profile_whatsapp',[userName,urlLink]),
                email: {
                    subject: this.$t('shareContent_share_profile_email_subject',[userName]),
                    body: this.$t('shareContent_share_profile_email_body',[userName,urlLink]),
                }
            }
            return paramObJ
        },
        tutorName() {
            return this.$store.getters.getProfileTutorName
        }
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin';

.profileFooter {
    background: #eeeff0;
    @media (max-width: @screen-xs) {
        background: #f8f8f8;
    }
    color: #000000;
    font-size: 16px;
    font-weight: bold;
    @media (max-width: @screen-xs) {
        .shareContentProfile {
            height: auto;
            padding: 0;
        }
    }
}
</style>