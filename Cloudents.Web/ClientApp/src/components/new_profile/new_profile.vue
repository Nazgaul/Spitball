<template>
  <div class="profilePage" :key="componentRenderKey" >
    <cover></cover>
    <profileDialogs />
    <div class="profilePage_main profile-page-container">
      <!-- <profileUserBox :globalFunctions="globalFunctions"/> -->
      <!-- <shareContent
        sel="share_area"
        :link="shareContentParams.link"
        :twitter="shareContentParams.twitter"
        :whatsApp="shareContentParams.whatsApp"
        :email="shareContentParams.email"
        class="mb-2 mb-sm-3 shareContentProfile"
        v-if="getProfile"
      /> -->
      <calendarTab
        ref="calendarTab"
        v-if="showCalendarTab"
        class="mt-sm-12 mt-2 mx-auto calendarSection"
        :globalFunctions="globalFunctions"
      />
      <profileSubscription :id="id" v-if="showProfileSubscription" ref="profileSubscription" />
      <profileBroadcasts :id="id" @isComponentReady="val => goToLiveClasses = true" ref="profileLiveClassesElement" />
      <!-- <profileLiveClasses :id="id" @isComponentReady="val => goToLiveClasses = true" ref="profileLiveClassesElement" /> -->
      <!-- <profileFindTutor class="mb-3 d-lg-none" /> -->
      <profileItemsBox v-if="isMyProfile || showItems" class="mt-sm-12 mt-2" />
      <!-- <profileEarnMoney class="mt-0 mt-sm-5" v-if="showEarnMoney" />  -->
      <profileReviewsBox v-if="showReviewBox" class="mt-sm-10 mt-2" />
    </div>
  </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

import chatService from '../../services/chatService.js';

// import profileUserBox from './components/profileUserBox/profileUserBox.vue';
import profileDialogs from './components/profileDialogs/profileDialogs.vue';
import profileReviewsBox from './components/profileReviewsBox/profileReviewsBox.vue';
// import profileEarnMoney from './components/profileEarnMoney/profileEarnMoney.vue';
// import profileFindTutor from './components/profileFindTutor/profileFindTutor.vue';
import profileItemsBox from './components/profileItemsBox/profileItemsBox.vue';
import profileBroadcasts from './components/profileLiveClasses/profileBroadcasts.vue'
// import profileLiveClasses from './components/profileLiveClasses/profileLiveClasses.vue'
import calendarTab from '../calendar/calendarTab.vue';
import profileSubscription from './components/profileSubscription/profileSubscription.vue';
import cover from "./components/cover.vue";
import * as routeNames from '../../routes/routeNames.js';

// const shareContent = () => import(/* webpackChunkName: "shareContent" */'../pages/global/shareContent/shareContent.vue');
export default {
    name: "new_profile",
    components: {
        // profileUserBox,
        profileDialogs,
        profileReviewsBox,
        // profileEarnMoney,
        // profileFindTutor,
        profileItemsBox,
        // profileLiveClasses,
        profileBroadcasts,
        profileSubscription,
        calendarTab,
        cover,
        // shareContent,
    },
    props: {
        id: {
            // Number
        }
    },
    data() {
        return {
            goToLiveClasses: false,
            globalFunctions:{
                sendMessage: this.sendMessage,
                openCalendar: this.openCalendar,
                closeCalendar: this.closeCalendar,
            },
            activeTab: 1,
            componentRenderKey: 0
        };
    },
    methods: {
        ...mapActions([
            'updateCurrTutor',
            'setTutorRequestAnalyticsOpenedFrom',
            'updateRequestDialog',
            'setActiveConversationObj',
            'syncProfile',
            'updateToasterParams'
        ]),
        sendMessage(){
            if(this.isMyProfile) {return}
            if(this.accountUser == null) {
               this.$ga.event('Tutor_Engagement', 'contact_BTN_profile_page', `userId:GUEST`);
               let profile = this.getProfile
               this.updateCurrTutor(profile.user)    
               this.setTutorRequestAnalyticsOpenedFrom({
                  component: 'profileContactBtn',
                  path: this.$route.path
               });
               this.updateRequestDialog(true);
            } else {
               this.$ga.event('Request Tutor Submit', 'Send_Chat_Message', `${this.$route.path}`);
               let currentProfile = this.getProfile;
               let conversationObj = {
                  userId: currentProfile.user.id,
                  image: currentProfile.user.image,
                  name: currentProfile.user.name,
                  conversationId: chatService.createConversationId([currentProfile.user.id, this.accountUser.id]),
               }
              let isNewConversation = !(this.$store.getters.getIsActiveConversationTutor(conversationObj.conversationId))
              if(isNewConversation){
                let tutorInfo = {
                  id: currentProfile.user.id,
                  name: currentProfile.user.name,
                  image: currentProfile.user.image,
                  calendar: currentProfile.user.calendarShared,
                }
                this.$store.commit('ACTIVE_CONVERSATION_TUTOR',{tutorInfo,conversationId:conversationObj.conversationId})
              }
              

               let currentConversationObj = chatService.createActiveConversationObj(conversationObj)
               this.setActiveConversationObj(currentConversationObj);
               this.$router.push({name:routeNames.MessageCenter,params:{id:currentConversationObj.conversationId}})
            }
        },
        fetchData() {
            let syncObj = {
                id: this.id,
                type:'documents',
                params:{
                    page: 0,
                    pageSize: this.$vuetify.breakpoint.xsOnly ? 3 : 8,
                }
            }
            let self = this
            this.syncProfile(syncObj).then(() => {

            }).catch((ex) => {
                console.error(ex);
                let currentRoute = self.$store.getters.getRouteStack[self.$store.getters.getRouteStack.length - 2] // check if there is last route that user come from
                if(currentRoute) {
                  self.$router.go(-1)
                  return
                }
                self.$router.push('/')
            })
        },
        openCalendar() {
            if(!!this.accountUser) {
                this.activeTab = 5;
                this.$nextTick(() => {
                    this.$vuetify.goTo(this.$refs.calendarTab)
                })
            } else {
                this.$store.commit('setComponent', 'register')
                setTimeout(()=>{
                    document.getElementById(`tab-${this.activeTab}`).lastChild.click();
                },200);
            }
        },
        closeCalendar(){
            this.activeTab = null
        }
    },
    computed: {
        ...mapGetters([
            "accountUser",
            "getProfile",
            'getBannerParams',
            'getUserLoggedInStatus',
            'getIsSubscriber',// profile isSubscriber
            'getProfileTutorSubscription',
            'getIsMyProfile'// is my profile
        ]),
        showProfileSubscription() {
            if(this.getProfileTutorSubscription && Object.keys(this.getProfileTutorSubscription).length > 0 && !this.getIsSubscriber) {
              return true
            }
            return false
        },
        shareContentParams(){
            let urlLink = `${global.location.origin}/p/${this.$route.params.id}?t=${Date.now()}` ;
            let userName = this.getProfile?.user?.name;
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
        showReviewBox(){
            if(this.getProfile?.user?.tutorData?.rate){
              return true;
            }else{
              return false
            }
        },
        isMyProfile(){
          return this.getIsMyProfile
        },
        showItemsEmpty(){
            return !this.isMyProfile && !!this.uploadedDocuments && !!this.uploadedDocuments.result && !this.uploadedDocuments.result.length;
        },
        showItems(){
            if(!!this.getProfile){
              return this.uploadedDocuments?.result?.length
            }
            return false
        },
        showCalendarTab() {
            let isCalendar = this.getProfile?.user?.calendarShared
            if(this.isMyProfile) {
                return !isCalendar || (this.activeTab === 5 && isCalendar) 
            }
            // return isCalendar
            return this.activeTab === 5 && isCalendar
        },
        profileData() {
          return this.getProfile;
        },
        uploadedDocuments() {
            if(this.profileData && this.profileData.documents) {
                return this.profileData.documents;
            }
            return [];
        },
    },
    watch: {
        "$route.params.id": function(val, oldVal){
            let old = Number(oldVal,10);
            let newVal = Number(val,10);
            this.activeTab = 1;
            this.componentRenderKey += 1;
            if (newVal !== old) {
                this.$store.commit('resetProfile');
                this.fetchData();
            }
        },
        showProfileSubscription(val) {
          if(val) {
            this.$nextTick(() => {
              let profileSubscriptionElement = this.$refs.profileSubscription
              if(profileSubscriptionElement && this.$route.hash === '#subscription') {
                this.$vuetify.goTo(this.$route.hash)
              }
            })
          }
        },
        goToLiveClasses(val) {
          if(val) {
            this.$nextTick(() => {
              let profileLiveClassesElement = this.$refs.profileLiveClassesElement
              if(profileLiveClassesElement && this.$route.hash === '#broadcast') {
                this.$vuetify.goTo(this.$route.hash, {offset: 20})
              }
            })
          }
        }
    },
    // beforeRouteUpdate(to, from, next) {
    //     let old = Number(from.params.id, 10)
    //     let newVal = Number(to.params.id, 10)
    //     this.activeTab = 1;
    //     this.componentRenderKey += 1;
    //     if (newVal !== old) {
    //       this.$store.commit('resetProfile');
    //       let self = this
    //       let syncObj = {
    //           id: to.params.id,
    //           type:'documents',
    //           params:{
    //               page: 0,
    //               pageSize: this.$vuetify.breakpoint.xsOnly ? 3 : 8,
    //           }
    //       }
    //       this.syncProfile(syncObj).then(({user}) => {
    //         if(user.isTutor) return next()

    //         let previousLink = from.fullPath || '/';
    //         next(previousLink);
    //       }).catch((ex) => {
    //           console.error(ex);
    //           self.$router.push({name: routeNames.notFound})
    //       })
    //     }
    // },
    beforeRouteLeave(to, from, next) {
        this.updateToasterParams({
            showToaster: false
        });
        this.$store.commit('resetProfile');
        next();
    },
    created() {
      // this.fetchData()
        if(this.$route.params.openCalendar) {
            this.openCalendar();
        }
    },
}
</script>

<style lang="less">
@import "../../styles/mixin.less";
.profilePage {
  position: relative;
  margin-bottom: 30px;
  @media (max-width: @screen-md) {
    justify-content: center;
  }
  @media (max-width: @screen-xs) {
    margin: 0;
    display: block;
  }
  .profile-sticky {
    position: sticky;
    height: fit-content;
    top: 80px;
    &.profileUserSticky_bannerActive {
      top: 150px;
    }
  }
  .profilePage_main {
    max-width: 1920px;
    padding-top: 260px;
    margin: 0 20px;
    @media (max-width: @screen-xs) {
      margin: 0;
    }
    &.profile-page-container {
      &.content-center {
        margin: 0 auto;
      }
      @media (max-width: @screen-xs) {
        margin-left: 0;
        padding: 0;
      }
      .question-container {
        margin: unset;
      }
      .back-button {
        transform: none /*rtl:rotate(180deg)*/;
        position: absolute;
        top: 32px;
        left: 10px;
        z-index: 99;
        outline: none;
      }
      .limit-width {
        max-width: 500px;
      }
      .bio-wrap {
        align-items: flex-start;
        @media (max-width: @screen-xs) {
          align-items: unset;
        }
      }
      .limited-760 {
        max-width: 760px;
      }
    }

    .shareContentProfile {
      background: #fff;
      max-width: 292px;
      margin: 0 auto 0;
      border-radius: 8px;
      justify-content: center;
      @media (max-width: @screen-xs) {
        padding-bottom: 20px;
        max-width: 100%;
        border-radius: 0;
      }
    }
  }
  .calendarSection {
    max-width: 960px;
    border-radius: 8px !important;
  }
}
</style>