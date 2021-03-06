<template>
    <div class="profilePage">
      <profileDrawer v-if="isMyProfile && $store.getters.getProfileCoverDrawer" />
      <div class="flex-grow-1">
        <div class="coverWrapper" id="profileCover" :key="componentRenderKey">
            <profileCover />
            <profileCoverActions
              @setCalendarActive="val => calendarActive = val"
              @handleFollowMyProfile="handleSnackBarError"
              v-if="isCoverImageLoaded"
            />
        </div>
        <profileStats v-if="showProfileStats" />
        <profileParagraph />
        <div class="profilePage_main mx-0 mx-sm-5">
            <profileCalendarTab
              ref="calendarTab"
              v-if="showCalendarTab"
              :showCalendarTab="calendarActive"
              class="mt-sm-12 mt-2 mx-auto"
            />
            <profileSubscription
              id="subscription"
              :userId="id"
              v-if="showProfileSubscription"
              ref="profileSubscription"
              @handleFollowMyProfile="handleSnackBarError"
            />
            <profileBroadcasts id="broadcast" :userId="id" ref="profileLiveClassesElement" :key="componentRenderKey" />
            <profileReviewsBox v-if="showProfileReviews"/>
            <!-- <profileFAQ /> -->
        </div>

      </div>
      <profileFloatingBtn  
        @handleFollowMyProfile="handleSnackBarError"
      />
      <profileFooter />
          
      <v-snackbar
        v-model="snack.state"
        :timeout="6000"
        color="error"
        top
      >
        <div class="white--text text-center">{{snack.text}}</div>
      </v-snackbar>
    </div>
</template>

<script>
import profileDrawer from './profileDrawer/profileDrawer.vue';
import profileCover from './components/profileCover/profileCover.vue';
import profileCoverActions from './components/profileCoverActions/profileCoverActions.vue';
import profileStats from './components/profileStats/profileStats.vue';
import profileParagraph from './components/profileParagraph/profileParagraph.vue';
const profileCalendarTab = () => import('../calendar/calendarTab.vue');
const profileSubscription = () => import('./components/profileSubscription/profileSubscription.vue');
import profileBroadcasts from './components/profileLiveClasses/profileBroadcasts.vue'
import profileReviewsBox from './components/profileReviewsBox/profileReviewsBox.vue';
// import profileFAQ from './components/profileFAQ/profileFAQ.vue';
import profileFooter from './components/profileFooter/profileFooter.vue';
import profileFloatingBtn from './components/profileFloatingBtn.vue';

export default {
    name: "new_profile",
    components: {
        profileDrawer,
        profileCover,
        profileCoverActions,
        profileStats,
        profileParagraph,
        profileCalendarTab,
        profileSubscription,
        profileBroadcasts,
        profileReviewsBox,
        // profileFAQ,
        profileFooter,
        profileFloatingBtn,
    },
    props: {
        id: {
            // Number
        }
    },
    data() {
        return {
            snack: {
              state: false,
              text: ''
            },
            calendarActive: false,
            componentRenderKey: 0
        };
    },
    methods: {
        handleSnackBarError(text) {
          this.snack.state = true
          this.snack.text = text
        },
        fetchData() {
            let self = this
            this.$store.dispatch('syncProfile', this.id)
              .then(() => {
                this.getProfileDataItems()
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
        getProfileDataItems() {
          this.$store.dispatch('updateProfileReviews', this.id)
        }
    },
    computed: {
        showProfileSubscription() {
            let profileTutorSubscription = this.$store.getters.getProfileTutorSubscription
            let isTutorSubscription = profileTutorSubscription && Object.keys(profileTutorSubscription).length > 0
            return isTutorSubscription && !this.$store.getters.getIsSubscriber
        },
        isMyProfile(){
          return this.$store.getters.getIsMyProfile
        },
        showCalendarTab() {
            let isCalendar = this.$store.getters.getProfileIsCalendar
            if(this.isMyProfile) {
                return !isCalendar
            }
            return isCalendar && this.calendarActive
        },
        isCoverImageLoaded() {
          return this.$store.getters.getProfileCoverLoading
        },
        showProfileStats(){
          if(this.$store.getters.getProfile){
            let stats = [
              this.$store.getters.getProfileStatsHours,
              this.$store.getters.getProfileStatsReviews,
              this.$store.getters.getProfileStatsFollowers,
              this.$store.getters.getProfileStatsResources,
            ]
            return stats.reduce((a, b) => a + b, 0) > 4
          }else return false;
        },
        showProfileReviews(){
          return this.$store.getters.getProfileStatsReviews;
        }
    },
    watch: {
      calendarActive(val) {
        if(val) {
          setTimeout(() => {
            this.$vuetify.goTo(this.$refs.calendarTab)
          }, 200)
        }
      },
        "$route.params.id": function(val, oldVal) {
            let old = Number(oldVal,10);
            let newVal = Number(val,10);
            this.componentRenderKey += 1;
            if (newVal !== old) {
                this.$store.commit('resetProfile');
                this.fetchData();
            }
        },
        "$route.hash": {
          immediate: true,
          handler(val) {
            if(val) {
              let hashes = {
                broadcast: '#broadcast',
                subscription: '#subscription',
                calendarTab: '#calendarTab'
              }
              let self = this
              this.$nextTick(() => {
                // added time out for component that load after getting data from server and not immediate live
                setTimeout(() => {
                  if(self.$route.hash === hashes[self.$route.hash.slice(1)]) {
                    if(self.$route.hash === '#calendarTab') {
                      self.calendarActive = true
                    }
                    self.$vuetify.goTo(self.$route.hash, {offset: 20})
                  }                
                }, 400)
              })
            }
          }
        },
        isCoverImageLoaded(val) {
          // render for cover image skeletong to trigger when update new cover image from tutorEditInfo
          if(!val) {
            this.componentRenderKey +=1
          }
        }
    },
    beforeRouteLeave(to, from, next) {
        this.$store.dispatch('updateToasterParams', {
            showToaster: false
        });
        this.$store.commit('resetProfile');
        next();
    },
    created() {
        const hash = sessionStorage.getItem('hash');
        if (hash) {
         this.$router.push({hash:hash});
         sessionStorage.clear();
      }
      
      this.getProfileDataItems()
    }
}
</script>

<style lang="less">
@import "../../styles/mixin.less";
.profilePage {
    // min-height: calc(~"100vh - 52px");
    height: 100%;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    position: relative;
  .coverWrapper {
    position: relative;
  }
  .profilePage_main {
    max-width: 1920px;
  }
}
</style>