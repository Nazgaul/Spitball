<template>
   <div>
      <v-dialog 
         v-model="getReviewDialogState" 
         max-width="596px"
         :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
         :persistent="$vuetify.breakpoint.smAndUp">
         <leave-review/>
      </v-dialog>   
   </div>
</template>

<script>
import leaveReview from "./tutorHelpers/leaveReview/leaveReview.vue";
import { mapGetters } from 'vuex';

export default {
   components:{
      leaveReview,
   },
   computed: {
      ...mapGetters(['getReviewDialogState'])
   },
}
</script>