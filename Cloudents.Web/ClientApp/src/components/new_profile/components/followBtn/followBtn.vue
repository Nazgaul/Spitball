<template>
  <button :class="['followBtn',getProfile.user.isFollowing?'following':'follow']" @click="followToggler">
    <followSVG width="20" v-if="!getProfile.user.isFollowing" class="me-1" />
    <span>{{followingText}}</span>
  </button>
</template>

<script>
import followSVG from "./images/follow.svg";
import { mapActions, mapGetters } from 'vuex';
export default {
  data() {
    return {
      isLoading:false,
    }
  },
  components: { followSVG },
  computed: {
    ...mapGetters(['getProfile','getUserLoggedInStatus']),

    followingText() {
      if(this.getProfile.user.isFollowing) {
        return this.$t('profile_following')
      }
      return this.$t('profile_follow')
    }
  },
  methods: {
    ...mapActions(['toggleProfileFollower']),
    followToggler(){
      
      if(this.getUserLoggedInStatus){
        if(this.isLoading) return
        let self = this;
        this.isLoading = true;
        this.toggleProfileFollower(!this.getProfile.user.isFollowing).then(()=>{
          self.isLoading = false;
        })
      }else{
        // this.$openDialog('login')
        this.$store.commit('setComponent', 'register')
      }
    }
  },
};
</script>

<style lang="less">
.followBtn {
   max-height: 26px;
   display: inline-flex !important;
   outline: none;
  font-size: 14px;
  font-weight: 600;
  font-stretch: normal;
  font-style: normal;
  line-height: 1.67;
  letter-spacing: normal;
  text-align: center;
  border-radius: 6px;
  padding: 2px 8px;
  max-width: fit-content;
  display: flex;
  align-items: center;
  text-transform: capitalize;
  &.following{
   color: #43425d;
   border: solid 1px #43425d;
  }
  &.follow{
   color: #4c59ff;
  //  border: solid 1px #4c59ff;
  }
}
</style>