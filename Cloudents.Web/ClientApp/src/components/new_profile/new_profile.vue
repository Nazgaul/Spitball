<template>
  <div class="profilePage" :key="componentRenderKey">
    <div class="coverWrapper">
      <cover>
        <div class="imageLinear"></div>
      </cover>
      <profileCoverActions @setCalendarActive="val => calendarActive = val" />
    </div>
    <profileStats />
    <profileParagraph />
    <div class="profilePage_main profile-page-container">
      <calendarTab
        ref="calendarTab"
        v-if="showCalendarTab"
        :showCalendarTab="calendarActive"
        class="mt-sm-12 mt-2 mx-auto calendarSection"
      />
      <profileSubscription id="subscription" :userId="id" v-if="showProfileSubscription" ref="profileSubscription" />
      <profileBroadcasts id="broadcast" :userId="id" ref="profileLiveClassesElement" />
      <profileItemsBox v-if="showItems" class="mt-sm-12 mt-8" />
      <profileReviewsBox class="my-10 mt-2" />
    </div>
    <profileFooter />
    <profileDialogs />
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

import cover from "./components/cover.vue";
import profileCoverActions from './components/profileCoverActions/profileCoverActions.vue';
import profileDialogs from './components/profileDialogs/profileDialogs.vue';
import profileStats from './components/profileStats/profileStats.vue';
import profileParagraph from './components/profileParagraph/profileParagraph.vue';
import profileReviewsBox from './components/profileReviewsBox/profileReviewsBox.vue';
import profileItemsBox from './components/profileItemsBox/profileItemsBox.vue';
import profileBroadcasts from './components/profileLiveClasses/profileBroadcasts.vue'
import calendarTab from '../calendar/calendarTab.vue';
import profileSubscription from './components/profileSubscription/profileSubscription.vue';
import profileFooter from './components/profileFooter/profileFooter.vue';


export default {
    name: "new_profile",
    components: {
        cover,
        profileDialogs,
        profileCoverActions,
        profileStats,
        profileParagraph,
        profileReviewsBox,
        profileItemsBox,
        profileBroadcasts,
        profileSubscription,
        calendarTab,
        profileFooter,
    },
    props: {
        id: {
            // Number
        }
    },
    data() {
        return {
            calendarActive: false,
            componentRenderKey: 0
        };
    },
    methods: {
        fetchData() {
            let self = this
            this.$store.dispatch('syncProfile', this.id)
              .then(() => {
                this.getProfileData()
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
        getProfileData() {
          let options = {
            id: this.id,
            params: {
              page: 0,
              pageSize: this.$vuetify.breakpoint.xsOnly ? 3 : 8
            }
          }
          let items = this.$store.dispatch('updateProfileItemsByType', options)
          let reviews = this.$store.dispatch('updateProfileReviews', this.id)
          Promise.all([items, reviews]).catch(ex => {
            console.error(ex);
          })
        }
    },
    computed: {
        ...mapGetters([
            "getProfile",
            'getIsSubscriber',
            'getProfileTutorSubscription',
        ]),
        showProfileSubscription() {
            if(this.getProfileTutorSubscription && Object.keys(this.getProfileTutorSubscription).length > 0 && !this.getIsSubscriber) {
              return true
            }
            return false
        },
        isMyProfile(){
          return this.$store.getters.getIsMyProfile
        },
        showItems(){
            return this.$store.getters.getProfileDocuments?.result?.length
        },
        showCalendarTab() {
            let isCalendar = this.getProfile?.user?.calendarShared
            if(this.isMyProfile) {
                return !isCalendar
            }
            return isCalendar && this.calendarActive
        },
    },
    watch: {
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
                subscription: '#subscription'
              }
              let self = this
              this.$nextTick(() => {
                // added time out for component that load after getting data from server and not immediate live
                setTimeout(() => {
                  if(self.$route.hash === hashes[self.$route.hash.slice(1)]) {
                    self.$vuetify.goTo(self.$route.hash, {offset: 20})
                  }                
                }, 400)
              })
            }
          }
        },
    },
    beforeRouteLeave(to, from, next) {
        this.$store.dispatch('updateToasterParams', {
            showToaster: false
        });
        this.$store.commit('resetProfile');
        next();
    },
    created() {
      this.getProfileData()
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
   .imageLinear {
      position: absolute;
      bottom: 0;
      right: 0;
      left: 0;
      height: 100%;
      background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0), rgba(0, 0, 0, 0.06), rgba(0, 0, 0, 0.26), rgba(0, 0, 0, 0.89));
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