<template>
   <v-app-bar app clipped-right color="#4c59ff" class="studyRoomHeader">
      <a @click="resetItems()">
         <logoComponent/>
      </a>

      <div class="roundShape mr-2"></div>
      <v-toolbar-title class="white--text">Live</v-toolbar-title>

      <v-btn depressed class="ma-2" @click="setClass()">Class</v-btn>
      <v-btn depressed class="ma-2" @click="setWhiteboard()">Whiteboard</v-btn>
      <v-btn depressed class="ma-2" @click="setShareScreen()">Share Screen</v-btn>
      <v-btn depressed class="ma-2" @click="setTextEditor()">Text Editor</v-btn>
      <v-btn depressed class="ma-2" @click="setCodeEditor()">Code Editor</v-btn>
      <v-spacer></v-spacer>
      <v-btn depressed class="ma-2" @click="MuteAll()">Mute All</v-btn>
      <v-btn rounded depressed class="ma-2" @click="EndSeesion()">End</v-btn>
      <v-btn icon>
         <v-icon>sbf-3-dot</v-icon>
         <!--Need to open record ( if avaible and setting)-->
      </v-btn>
   </v-app-bar>
</template>

<script>
import logoComponent from "../../../app/logo/logo.vue";
export default {
   components:{
      logoComponent,
   },
   computed: {
      roomModes(){
         return this.$store.getters.getRoomModeConsts;
      }
      
   },
   methods: {
      resetItems(){
         debugger
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
      setClass() {},
      setShareScreen() {},
   },
}
</script>
<style lang="less">
   .studyRoomHeader {
      .logo {
         fill: #fff;
      }
      .roundShape {
         width: 8px;
         height: 8px;
         background-color: #fff;
         border-radius: 50%;
      }
   }
</style>