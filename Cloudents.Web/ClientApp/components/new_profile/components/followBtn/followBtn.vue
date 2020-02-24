<template>
  <button :class="['followBtn',getProfile.user.isFollowing?'following':'follow']" @click="followToggler">
    <followSVG v-if="!getProfile.user.isFollowing" class="mr-1" />
    <span v-language:inner="getProfile.user.isFollowing? 'profile_following' :'profile_follow'"/>
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
    ...mapGetters(['getProfile','getUserLoggedInStatus'])
  },
  methods: {
    ...mapActions(['toggleProfileFollower','updateLoginDialogState']),
    followToggler(){
      
      if(this.getUserLoggedInStatus){
        if(this.isLoading) return
        let self = this;
        this.isLoading = true;
        this.toggleProfileFollower(!this.getProfile.user.isFollowing).then(()=>{
          self.isLoading = false;
        })
      }else{
        this.updateLoginDialogState(true);
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
  font-size: 12px;
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
   border: solid 1px #4c59ff;
  }
}
</style>