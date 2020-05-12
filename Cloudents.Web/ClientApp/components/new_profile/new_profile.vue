<template>
  <div class="profilePage">
    <cover></cover>
    <profileDialogs />
    <div class="profilePage_main profile-page-container">
      <profileUserBox :globalFunctions="globalFunctions" :key="componentRenderKey" />
      <shareContent
        sel="share_area"
        :link="shareContentParams.link"
        :twitter="shareContentParams.twitter"
        :whatsApp="shareContentParams.whatsApp"
        :email="shareContentParams.email"
        class="mb-2 mb-sm-3 shareContentProfile"
        v-if="getProfile"
      />
      <calendarTab
        ref="calendarTab"
        v-if="showCalendarTab"
        class="mt-sm-12 mt-2 mx-auto calendarSection"
        :globalFunctions="globalFunctions"
      />
      <profileLiveClasses :id="id" v-if="isTutor" />
      <profileBecomeTutor v-if="showBecomeTutor" class="mb-3 d-lg-none" />
      <profileFindTutor v-if="showFindTutor" class="mb-3 d-lg-none" />
      <profileItemsBox v-if="isMyProfile || showItems" class="mt-sm-12 mt-2" />
      <profileEarnMoney class="mt-0 mt-sm-5" v-if="showEarnMoney" />
      <profileReviewsBox v-if="showReviewBox" class="mt-sm-10 mt-2" />
     
    </div>
    <!-- SIDE -->
   
    <sb-dialog
      :onclosefn="closeCouponDialog"
      :activateOverlay="false"
      :showDialog="isShowCouponDialog"
      :maxWidth="'410px'"
      :popUpType="'coupon'"
      :content-class="'coupon-dialog'"
      :isPersistent="true"
    >
      <v-card class="pb-4 coupon-dialog-card" :class="{'d-block': $vuetify.breakpoint.xsOnly}">
        <v-layout class="header py-6">
          <v-flex
            class="text-xs-center coupon-dialog-header"
            :class="{'mt-5': $vuetify.breakpoint.xsOnly}"
          >
            <span v-t="'coupon_title'"></span>
            <v-icon @click="closeCouponDialog" class="coupon-close" v-html="'sbf-close'" />
          </v-flex>
        </v-layout>
        <v-layout class="px-4" column>
          <v-flex class="mb-2">
            <div class="coupon coupon__dialog" v-if="isTutor && !isMyProfile">
              <div class="text-xs-right">
                <div class="coupon__dialog--flex">
                  <input
                    type="text"
                    @keyup.enter="applyCoupon"
                    v-model="coupon"
                    :placeholder="couponPlaceholder"
                    class="profile-coupon_input"
                    autofocus
                  />
                  <button
                    class="profile-coupon_btn white--text"
                    :disabled="disableApplyBtn"
                    @click="applyCoupon"
                    v-t="'coupon_apply_btn'"
                  ></button>
                </div>
                <div
                  class="profile-coupon_error"
                  v-t="'coupon_apply_error'"
                  v-if="getCouponError"
                ></div>
              </div>
            </div>
          </v-flex>
        </v-layout>
      </v-card>
    </sb-dialog>
  </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

import analyticsService from '../../services/analytics.service';
import { LanguageService } from "../../services/language/languageService";
import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue'
import storeService from '../../services/store/storeService';
import couponStore from '../../store/couponStore';
import chatService from '../../services/chatService.js';

import profileUserBox from './components/profileUserBox/profileUserBox.vue';
import profileDialogs from './components/profileDialogs/profileDialogs.vue';
import profileReviewsBox from './components/profileReviewsBox/profileReviewsBox.vue';
import profileEarnMoney from './components/profileEarnMoney/profileEarnMoney.vue';
import profileBecomeTutor from './components/profileBecomeTutor/profileBecomeTutor.vue';
import profileFindTutor from './components/profileFindTutor/profileFindTutor.vue';
import profileItemsBox from './components/profileItemsBox/profileItemsBox.vue';
import profileLiveClasses from './components/profileLiveClasses/profileLiveClasses.vue'
import calendarTab from '../calendar/calendarTab.vue';
import cover from "./components/cover.vue";



const shareContent = () => import(/* webpackChunkName: "shareContent" */'../pages/global/shareContent/shareContent.vue');
export default {
    name: "new_profile",
    components: {
        profileUserBox,
        profileDialogs,
        profileReviewsBox,
        profileEarnMoney,
        profileBecomeTutor,
        profileFindTutor,
        profileItemsBox,
        profileLiveClasses,
        calendarTab,
        cover,
        sbDialog,
        shareContent,
    },
    props: {
        id: {
            // Number
        }
    },
    data() {
        return {
            globalFunctions:{
                sendMessage: this.sendMessage,
                openCalendar: this.openCalendar,
                closeCalendar: this.closeCalendar,
                openCoupon: this.openCoupon
            },
            coupon: '',
            couponPlaceholder: LanguageService.getValueByKey('coupon_placeholder'),
            disableApplyBtn: false,
            activeTab: 1,
            componentRenderKey: 0
        };
    },
    methods: {
        ...mapActions([
            'updateCouponDialog',
            'updateCoupon',
            'updateCurrTutor',
            'setTutorRequestAnalyticsOpenedFrom',
            'updateRequestDialog',
            'setActiveConversationObj',
            'openChatInterface',


            'syncProfile',
            'resetProfileData',
            'updateToasterParams'
        ]),
        closeCouponDialog() {
            this.coupon = ''
            this.updateCouponDialog(false);
        },
        openCoupon(){
            if(this.getUserLoggedInStatus) {
            if(this.accountUser) {          
                if(this.$route.params.id != this.accountUser.id) {
                    this.updateCouponDialog(true)
                    analyticsService.sb_unitedEvent('Tutor_Engagement', 'Click_Redeem_Coupon', `${this.$route.path}`);
                }
            }
            } else {
                this.$store.commit('setComponent', 'register')
            }
        },
        applyCoupon() {
            if(this.isTutor) {
                this.disableApplyBtn = true;
                let tutorId = this.getProfile.user.id;
                let coupon = this.coupon;
                let self = this
                this.updateCoupon({coupon, tutorId}).finally(() => {
                self.coupon = ''
                self.disableApplyBtn = false;
                if(!self.getCouponError) {
                    analyticsService.sb_unitedEvent('Tutor_Engagement', 'Redeem_Coupon_Success', `${this.$route.path}`);
                }
                })
            }
        },
        sendMessage(){
            if(this.isMyProfile) {return}
            if(this.accountUser == null) {
               analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:GUEST`);
               let profile = this.getProfile
               this.updateCurrTutor(profile.user)    
               this.setTutorRequestAnalyticsOpenedFrom({
                  component: 'profileContactBtn',
                  path: this.$route.path
               });
               this.updateRequestDialog(true);
            } else {
               analyticsService.sb_unitedEvent('Request Tutor Submit', 'Send_Chat_Message', `${this.$route.path}`);
               let currentProfile = this.getProfile;
               let conversationObj = {
                  userId: currentProfile.user.id,
                  image: currentProfile.user.image,
                  name: currentProfile.user.name,
                  conversationId: chatService.createConversationId([currentProfile.user.id, this.accountUser.id]),
               }
               let currentConversationObj = chatService.createActiveConversationObj(conversationObj)
               this.setActiveConversationObj(currentConversationObj);
               this.openChatInterface();                    
            }
        },
        fetchData() {
            let syncObj = {
                id: this.id,
                type:'documents',
                params:{
                    page: 0,
                    pageSize:this.$vuetify.breakpoint.xsOnly? 3 : 8,
                }
            }
            this.syncProfile(syncObj);
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
            'getCouponDialog',
            'getCouponError',
            "getProfile",
            'getBannerParams',
            'getUserLoggedInStatus'
        ]),
        shareContentParams(){
            let urlLink = `${global.location.origin}/p/${this.$route.params.id}?t=${Date.now()}` ;
            let userName = this.getProfile.user?.name;
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
        isShowCouponDialog(){
            // if(this.getCouponDialog){
            //     setTimeout(() => {
            //         document.querySelector('.profile-coupon_input').focus()
            //     }, 100);
            // }
            return this.getCouponDialog;
        },
        showReviewBox(){
            if((!!this.getProfile && this.getProfile.user.isTutor) && (this.getProfile.user.tutorData.rate)){
                return true;
            }else{
                return false
            }
        },
        isTutor(){
            return !!this.getProfile && this.getProfile.user.isTutor
        },
        isMyProfile(){
            return !!this.getProfile && !!this.accountUser && this.accountUser?.id == this.getProfile?.user?.id
        },
        showEarnMoney(){
            return this.isMyProfile && this.isTutor && !!this.uploadedDocuments && !!this.uploadedDocuments.result && !this.uploadedDocuments.result.length;
        },
        showItemsEmpty(){
            return !this.isMyProfile && this.isTutor && !!this.uploadedDocuments && !!this.uploadedDocuments.result && !this.uploadedDocuments.result.length;
        },
        showItems(){
            if(!!this.getProfile){
                // if(this.isTutor) {
                //     return true;
                // }else{
                    return this.uploadedDocuments?.result?.length
                // }
            }
            return false
        },
        showCalendarTab() {
            if(!this.isTutor) return false;
            
            let isCalendar = this.getProfile?.user.calendarShared
            if(this.isMyProfile) {
                return !isCalendar || (this.activeTab === 5 && isCalendar) 
            }
            return this.activeTab === 5 && isCalendar
        },
        isTutorPending(){
            return this.isMyProfile && (!!this.accountUser && this.accountUser.isTutorState === "pending")
        },
        showBecomeTutor(){
            return this.isMyProfile && !this.isTutor && !this.isTutorPending;
        },
        showFindTutor(){
            return (!this.isMyProfile && !this.isTutor)
        },
        profileData() {
            //if (!!this.getProfile) {
                return this.getProfile;
            //}
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
                this.resetProfileData();
                if((newVal == this.accountUser.id) && this.accountUser.isTutorState === "pending"){
                    this.updateToasterParams({
                        toasterText: LanguageService.getValueByKey("becomeTutor_already_submitted"),
                        showToaster: true,
                        toasterTimeout: 3600000
                    });
                }else{
                    this.updateToasterParams({
                        showToaster: false
                    }); 
                }
                this.fetchData();
            }
        },
        coupon(val) {
            if(val && this.getCouponError) {
                this.$store.commit('setCouponError', false)
            }
        }
    },
    beforeRouteLeave(to, from, next) {
        this.updateToasterParams({
            showToaster: false
        });
        this.resetProfileData();
        next();
    },
    beforeDestroy(){
        this.closeCouponDialog();
        storeService.unregisterModule(this.$store, 'couponStore');
     },
    created() {
        this.fetchData();
        storeService.registerModule(this.$store, 'couponStore', couponStore);
        if(!!this.$route.query.coupon) {
            setTimeout(() => {
                this.openCoupon();
            },200)
        }
        if(this.$route.params.openCalendar) {
            this.openCalendar();
        }
    },
    mounted() {
        setTimeout(()=>{
            if((this.$route.params && this.$route.params.id) && 
               (this.$route.params.id == this.accountUser.id) && 
               this.accountUser.isTutorState === "pending"){
                this.updateToasterParams({
                    toasterText: LanguageService.getValueByKey("becomeTutor_already_submitted"),
                    showToaster: true,
                    toasterTimeout: 3600000
                });
            }
        },200);
    }
}
</script>

<style lang="less">
@import "../../styles/mixin.less";
.profilePage {
  position: relative;
  // display: flex;
  // margin: 24px 0;
  //  justify-content: center;

  margin-bottom: 30px;
 
  // margin: 24px 70px 26px 34px;

  @media (max-width: @screen-md) {
    // margin: 20px;
    justify-content: center;
  }
  @media (max-width: @screen-xs) {
    margin: 0;
    // margin-bottom: 40px;
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
      @media (max-width: @screen-md-plus) {
        // margin-left: 0;
      }
      @media (max-width: @screen-xs) {
        margin-left: 0;
        padding: 0;
        // margin-bottom: 60px;
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
.coupon-dialog {
  border-radius: 6px;
  background: white;
  display: flex;
  flex-direction: column;
  .coupon-dialog-header {
    text-align: center;
    font-weight: 600;
    font-size: 18px;
    color: #43425d;
    .coupon-close {
      position: absolute;
      right: 10px;
      top: 10px;
      font-size: 12px;
      fill: #adadba;
      cursor: pointer;
    }
  }
  .coupon {
    display: flex;
    width: 100%;
    justify-content: center;
    &__dialog {
      &--flex {
        display: flex;
      }
      .profile-coupon_input {
        outline: none;
        border-radius: 6px 0 0 6px;
        width: 200px;
        border: 1px solid #bbb;
        padding: 10px 2px 11px 8px;
        margin-right: -5px;
      }
      .profile-coupon_btn {
        border-radius: 0 6px 6px 0;
        background-color: #4c59ff;
        font-size: 12px;
        padding: 8px 20px;
        font-weight: 600;
        outline: none;
      }
      .profile-coupon_error {
        width: 236px;
        margin-top: 4px;
        text-align: left;
        font-size: 11px;
        color: #ff5252;
      }
    }
  }
}
</style>