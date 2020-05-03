<template>
  <v-sheet height="100%" width="100%" class="flex-column d-flex canvas-window">
    <div class="d-flex flex-grow-1">
      <whiteBoard></whiteBoard>
      <whiteBoardFloatingTools/>
    </div>
    <v-slide-y-transition>
    <div class="d-flex flex-grow-0 flex-shrink-0 tabs">
      <v-btn-toggle v-model="text" group dense borderless color="#4c59ff"  background-color="#00FF00" mandatory>
        <template v-for="n in 8">
          <v-btn color="#00FF00" :key="n" :value="'tab'+n" active-class="tab-active">Tab {{n}}</v-btn>
        </template>

        <!-- <v-btn value="center">Center</v-btn>

        <v-btn value="right">Right</v-btn>

        <v-btn value="justify">Justify</v-btn>-->
      </v-btn-toggle>
    </div>
    </v-slide-y-transition>
  </v-sheet>
</template>
<script>
import whiteBoard from  "./../../whiteboard/WhiteBoard.vue"
import whiteBoardFloatingTools from './whiteBoardFloatingTools.vue'
export default {
  components : {
    whiteBoard,
    whiteBoardFloatingTools
  },
  data() {
    return {
      text: "tab1",
      tabsCount: 8
    };
  }
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
  .tab-active {
   // color: #4c59ff !important;
  }
}
</style>