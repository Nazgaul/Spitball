<template>
   <v-app-bar height="62" app clipped-right color="#4c59ff" class="studyRoomHeader">
      <a @click="resetItems()">
         <logoComponent/>
      </a>
      <div class="roundShape mr-2"></div>
      <v-toolbar-title class="white--text">{{$t('studyRoom_live')}}</v-toolbar-title>
      <v-divider class="mx-6 divider" vertical inset color="white"></v-divider>
      <v-btn-toggle mandatory :value="currentEditorMode" :ripple="false" active-class="editorActive"  borderless group class="editors">
         <v-btn :value="roomModes.CLASS_MODE" text @click="setClass()">
            <span><v-icon class="mr-2">sbf-class</v-icon>{{$t('studyRoom_nav_class')}}</span>
         </v-btn>
         <v-btn :value="roomModes.CLASS_SCREEN" text @click="setClassScreen()">
            <span><v-icon class="mr-2">sbf-class</v-icon>{{$t('studyRoom_nav_class')}} screen</span>
         </v-btn>
         <v-btn :value="roomModes.WHITE_BOARD" text @click="setWhiteboard()">
            <span><v-icon class="mr-2">sbf-whiteboard</v-icon>{{$t('studyRoom_nav_whiteboard')}}</span>
         </v-btn>
         <v-btn :value="roomModes.SCREEN_MODE" text @click="setShareScreen()">
            <span><v-icon class="mr-2">sbf-shareScreen</v-icon>{{$t('studyRoom_nav_screen')}}</span>
         </v-btn>
         <v-btn :value="roomModes.TEXT_EDITOR" text @click="setTextEditor()">
            <span><v-icon class="mr-2">sbf-text</v-icon>{{$t('studyRoom_nav_text')}}</span>
         </v-btn>
         <v-btn :value="roomModes.CODE_EDITOR" text @click="setCodeEditor()">
            <span><v-icon class="mr-2">sbf-code</v-icon>{{$t('studyRoom_nav_code')}}</span>
         </v-btn>
      </v-btn-toggle>

      <v-spacer></v-spacer>
      <v-btn v-if="isRoomTutor"  depressed class="ma-2" @click="MuteAll()">Mute All</v-btn>
      <v-btn v-if="isRoomTutor" rounded depressed class="ma-2" @click="endSession()">End</v-btn>
      <v-btn icon>
         <v-icon>sbf-3-dot</v-icon>
         <!--Need to open record ( if avaible and setting)-->
      </v-btn>
      <sb-dialog
        :showDialog="getDialogRoomEnd"
        :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'"
        :popUpType="'endSessionConfirm'"
        :maxWidth="'356'"
        :onclosefn="closeEndDialog"
        :activateOverlay="false"
        :isPersistent="$vuetify.breakpoint.smAndUp"
        :content-class="'session-end-confirm'"
      >
        <endSessionConfirm :id="$route.params.id"></endSessionConfirm>
      </sb-dialog>

   </v-app-bar>
</template>

<script>
import sbDialog from "../../../wrappers/sb-dialog/sb-dialog.vue";
import endSessionConfirm from "../../tutorHelpers/endSessionConfirm/endSessionConfirm.vue";

import logoComponent from "../../../app/logo/logo.vue";
import { mapGetters } from 'vuex';
export default {
   components:{
      logoComponent,

      sbDialog,
      endSessionConfirm
   },
   computed: {
      ...mapGetters(['getDialogRoomEnd']),
      isRoomTutor(){
         return this.$store.getters.getRoomIsTutor;
      },
      roomModes(){
         return this.$store.getters.getRoomModeConsts;
      },
      currentEditorMode(){
         return this.$store.getters.getActiveNavEditor;
      }
   },
   methods: {
      closeEndDialog(){
         this.$store.dispatch('updateEndDialog',false)
      },
      resetItems(){
         let isExit = confirm(this.$t("login_are_you_sure_you_want_to_exit"),)
         if(isExit){
         this.$ga.event("tutoringRoom", 'resetItems');
         this.$router.push('/');
         }
      },
      setWhiteboard() {
         this.$store.dispatch('updateActiveNavEditor',this.roomModes.WHITE_BOARD)
      },
      setTextEditor() {
         this.$store.dispatch('updateActiveNavEditor',this.roomModes.TEXT_EDITOR)
      },
      setCodeEditor() {
         this.$store.dispatch('updateActiveNavEditor',this.roomModes.CODE_EDITOR)
      },
      setClass() {
         this.$store.dispatch('updateActiveNavEditor',this.roomModes.CLASS_MODE)
      },
      setClassScreen() {
         this.$store.dispatch('updateActiveNavEditor',this.roomModes.CLASS_SCREEN)
      },
      setShareScreen() {
         // this.$store.dispatch('updateActiveNavEditor',this.roomModes.SCREEN_MODE)
      },
      endSession() {
            this.$ga.event("tutoringRoom", "endSession");
            this.$store.dispatch('updateEndDialog',true)
      }
   },
}
</script>
<style lang="less">
   .studyRoomHeader {
      .v-toolbar__content{
         padding-bottom: 0;
      }
      .logo {
         fill: #fff;
      }
      .editors{
         button{
            font-weight: 600;
            color: #ffffff;
            margin-bottom: 0 !important;
         }
         .editorActive{
            background: white !important;
            color: #4c59ff !important;
         }
      }
            
      .divider{
         opacity: 0.28;
      }
      .roundShape {
         width: 8px;
         height: 8px;
         background-color: #fff;
         border-radius: 50%;
      }
   }
</style>