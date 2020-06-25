<template>
  <div class="profilePage" :key="componentRenderKey">
    <div class="coverWrapper">
      <cover />
      <profileCoverActions @setCalendarActive="val => calendarActive = val" />
    </div>
    <profileStats />
    <div class="profileEdit text-right pa-2" v-if="isMyProfile">
      <v-btn @click="openTutorEditInfo" width="122" color="#e6e8e9" height="40" depressed>
        <editSVG class="editIcon" />
        <span class="text ms-2" v-t="'edit'"></span>
      </v-btn>
    </div>
    <!-- <profileParagraph /> -->
    <div class="profilePage_main profile-page-container">
      <calendarTab
        ref="calendarTab"
        v-if="showCalendarTab"
        class="mt-sm-12 mt-2 mx-auto calendarSection"
      />
      <profileSubscription :id="id" v-if="showProfileSubscription" ref="profileSubscription" />
      <profileBroadcasts :id="id" @isComponentReady="val => goToLiveClasses = true" ref="profileLiveClassesElement" />
      <profileItemsBox v-if="isMyProfile || showItems" class="mt-sm-12 mt-2" />
      <profileReviewsBox class="mt-sm-10 mt-2" />
    </div>
    <profileFooter />
    <profileDialogs />
  </div>
</template>

<script>
// import * as componentConsts from '../pages/global/toasterInjection/componentConsts.js'
import { mapGetters } from 'vuex';

import cover from "./components/cover.vue";
import profileCoverActions from './components/profileCoverActions/profileCoverActions.vue';
import profileDialogs from './components/profileDialogs/profileDialogs.vue';
import profileStats from './components/profileStats/profileStats.vue';
// import profileParagraph from './components/profileParagraph/profileParagraph.vue';
import profileReviewsBox from './components/profileReviewsBox/profileReviewsBox.vue';
import profileItemsBox from './components/profileItemsBox/profileItemsBox.vue';
import profileBroadcasts from './components/profileLiveClasses/profileBroadcasts.vue'
import calendarTab from '../calendar/calendarTab.vue';
import profileSubscription from './components/profileSubscription/profileSubscription.vue';
import profileFooter from './components/profileFooter/profileFooter.vue';

import editSVG from './images/edit.svg';

export default {
    name: "new_profile",
    components: {
        cover,
        profileDialogs,
        profileCoverActions,
        profileStats,
        // profileParagraph,
        profileReviewsBox,
        profileItemsBox,
        profileBroadcasts,
        profileSubscription,
        calendarTab,
        profileFooter,
        editSVG
    },
    props: {
        id: {
            // Number
        }
    },
    data() {
        return {
            goToLiveClasses: false,
            activeTab: 1,
            calendarActive: false,
            componentRenderKey: 0
        };
    },
    methods: {
        fetchData() {
            let options = {
              id: this.id,
              pageSize: this.$vuetify.breakpoint.xsOnly ? 3 : 8
            }
            let self = this
            this.$store.dispatch('syncProfile', options).catch((ex) => {
                console.error(ex);
                let currentRoute = self.$store.getters.getRouteStack[self.$store.getters.getRouteStack.length - 2] // check if there is last route that user come from
                if(currentRoute) {
                  self.$router.go(-1)
                  return
                }
                self.$router.push('/')
            })
        },
        openTutorEditInfo() {
          this.$store.commit('setEditDialog', true)
          // this.$store.commit('addComponent', componentConsts.TUTOR_EDIT_PROFILE)
          // removeComponent
        }
    },
    computed: {
        ...mapGetters([
            "getProfile",
            'getIsSubscriber',// profile isSubscriber
            'getProfileTutorSubscription',
        ]),
        showProfileSubscription() {
            if(this.getProfileTutorSubscription && Object.keys(this.getProfileTutorSubscription).length > 0 && !this.getIsSubscriber) {
              return true
            }
            return false
        },
        showReviewBox(){
            if(this.getProfile?.user?.tutorData?.rate){
              return true;
            }else{
              return false
            }
        },
        isMyProfile(){
          return this.$store.getters.getIsMyProfile
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
            return isCalendar && this.calendarActive
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
        showProfileSubscription: {
          deep: true,
          immediate: true,
          handler(val) {
          if(val) {
            this.$nextTick(() => {
              let profileSubscriptionElement = this.$refs.profileSubscription
              if(profileSubscriptionElement && this.$route.hash === '#subscription') {
                this.$vuetify.goTo(this.$route.hash, {offset: 20})
              }
            })
          }
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
    beforeRouteLeave(to, from, next) {
        this.$store.dispatch('updateToasterParams', {
            showToaster: false
        });
        this.$store.commit('resetProfile');
        next();
    }
}
</script>

<style lang="less">
@import "../../styles/mixin.less";
.profilePage {
  position: relative;
  @media (max-width: @screen-md) {
    justify-content: center;
  }
  @media (max-width: @screen-xs) {
    margin: 0;
    display: block;
  }
  .coverWrapper {
    position: relative;
  }
  .profileEdit {
    .editIcon {
      //temporary solution till new icon
      path:first-child {
        fill: #131415;
      }
    }
    .text {
      font-size: 16px;
      font-weight: 600;
      color: #131415;
    }
  }
  .profilePage_main {
    max-width: 1920px;
    margin: 0 20px;
    @media (max-width: @screen-xs) {
      margin: 0;
    }
    &.profile-page-container {
      @media (max-width: @screen-xs) {
        margin-left: 0;
        padding: 0;
      }
    }
  }
  .calendarSection {
    max-width: 960px;
    border-radius: 8px !important;
  }
}
</style>