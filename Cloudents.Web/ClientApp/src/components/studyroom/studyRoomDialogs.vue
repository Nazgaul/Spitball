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
         v-model="getIsBrowserNotSupport" 
         max-width="612.5px"
         :fullscreen="$vuetify.breakpoint.xsOnly"
         :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
         persistent
         :content-class="'browser-dialog-unsupport'"
         >
          <browserSupport></browserSupport>
      </v-dialog>

      <v-dialog v-model="getShowAudioRecordingError" max-width="675px"
                  :persistent="$vuetify.breakpoint.smAndUp" 
                  :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'">
        <errorWithAudioRecording></errorWithAudioRecording>
      </v-dialog>

      <v-dialog v-model="getDialogUserConsent" max-width="356px"
                  :persistent="$vuetify.breakpoint.smAndUp" 
                  :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'">
          <studentConsentDialog></studentConsentDialog>
      </v-dialog>
      <v-dialog v-model="getDialogRoomEnd" max-width="356px"
                  :persistent="$vuetify.breakpoint.smAndUp" 
                  :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'">
         <endSessionConfirm/>
      </v-dialog>
      <v-dialog v-if="getAudioVideoDialog" v-model="getAudioVideoDialog" max-width="570px" content-class="studyRoomAudioVideoDialog"
                :fullscreen="$vuetify.breakpoint.xsOnly" persistent
                  :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'">
         <studyRoomAudioVideoDialog/>
      </v-dialog>
   </div>
</template>

<script>
import leaveReview from "./tutorHelpers/leaveReview/leaveReview.vue";
import browserSupport from "./tutorHelpers/browserSupport/browserSupport.vue";
import errorWithAudioRecording from './tutorHelpers/errorWithAudioRecording/errorWithAudioRecording.vue';
import studentConsentDialog from './tutorHelpers/studentConsentDialog/studentConsentDialog.vue';
import endSessionConfirm from "./tutorHelpers/endSessionConfirm/endSessionConfirm.vue";
import studyRoomAudioVideoDialog from './tutorHelpers/studyRoomSettingsDialog/studyRoomAudioVideoDialog/studyRoomAudioVideoDialog.vue';

import { mapGetters } from 'vuex';

export default {
   components:{
      leaveReview,
      browserSupport,
      errorWithAudioRecording,
      studentConsentDialog,
      endSessionConfirm,
      studyRoomAudioVideoDialog
   },
   computed: {
      ...mapGetters([
         'getIsBrowserNotSupport',
         'getAudioVideoDialog',
         'getReviewDialogState',
         'getShowAudioRecordingError',
         'getDialogUserConsent',
         'getDialogRoomEnd'
      ])
   }
}
</script>
<style lang="less">
.browser-dialog-unsupport {
  flex: auto
}
</style>