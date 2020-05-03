<template>
  <v-sheet height="100%" width="100%" class="flex-column d-flex canvas-window">
    <div class="d-flex flex-grow-1">
      <whiteBoard></whiteBoard>
      <whiteBoardFloatingTools/>
    </div>
    <v-slide-y-transition>
    <div class="d-flex flex-grow-0 flex-shrink-0 tabs">
      <whiteBoardTabs/>
    </div>
    </v-slide-y-transition>
  </v-sheet>
</template>
<script>
import whiteBoard from  "./../../whiteboard/WhiteBoard.vue"
import whiteBoardFloatingTools from './whiteBoardFloatingTools.vue'
import whiteBoardTabs from './whiteBoardTabs.vue';
export default {
  components : {
    whiteBoard,
    whiteBoardFloatingTools,
    whiteBoardTabs
  },
};
</script>
<style lang="less">
.canvas-window {
  background: transparent; //overide vuetify
  background-size: 20px 20px;
  background-image: linear-gradient(to right, #f5f5f5 1px, transparent 1px),
    linear-gradient(to bottom, #f5f5f5 1px, transparent 1px);

  .tabs {
    background: #e0e0e1;
  }
}
</style>