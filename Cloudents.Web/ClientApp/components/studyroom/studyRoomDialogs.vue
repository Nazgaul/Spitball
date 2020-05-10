<template>
   <div>
      <v-dialog 
         v-model="getReviewDialogState" 
         max-width="596px"
         :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
         :persistent="$vuetify.breakpoint.smAndUp">
         <leave-review/>
      </v-dialog>

      <v-dialog 
         v-model="isBrowserSupportDialog" 
         max-width="612.5px"
         :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
         :persistent="$vuetify.breakpoint.smAndUp"
         :content-class="'browser-dialog-unsupport'"
         >
          <browserSupport></browserSupport>
      </v-dialog>
   </div>
</template>

<script>
import leaveReview from "./tutorHelpers/leaveReview/leaveReview.vue";
import browserSupport from "./tutorHelpers/browserSupport/browserSupport.vue";

import { mapGetters } from 'vuex';

export default {
   data() {
      return {
         isBrowserSupportDialog:false,
      }
   },
   components:{
      leaveReview,
      browserSupport
   },
   computed: {
      ...mapGetters(['getReviewDialogState'])
   },
   methods: {
      isBrowserSupport(){
         let agent = navigator.userAgent;
         if(agent.match(/Edge/)){
            return false;
         }
         return agent.match(/Firefox|Chrome|Safari/);
      }
   },
   created() {
      if (!this.isBrowserSupport()) {
         let self = this.isBrowserSupportDialog
         this.$nextTick(()=>{
            self.isBrowserSupportDialog = true;
         })
         return;
      }
   },
}
</script>
<style lang="less">
.browser-dialog-unsupport {
  flex: auto
}
</style>