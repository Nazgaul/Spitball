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
  methods: {
    initMathjax(){
      let scriptUrl = 'https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.5/MathJax.js?config=TeX-AMS_SVG'
      this.$loadScript(scriptUrl)
          .then(() => {
            MathJax.Hub.Config({
              showMathMenu: false,
              SVG: {
                useGlobalCache: false,
                useFontCache: false
              }
            });
            MathJax.AuthorInit = function(texstring, callback) {
              var input = texstring;
              var wrapper = document.createElement("div");
              wrapper.innerHTML = input;
              var output = { svg: "" };
              MathJax.Hub.Queue(["Typeset", MathJax.Hub, wrapper]);
              MathJax.Hub.Queue(function() {
                var mjOut = wrapper.getElementsByTagName("svg")[0];
                if (!mjOut) {
                  return null;
                }
                mjOut.setAttribute("xmlns", "http://www.w3.org/2000/svg");
                output.svg = mjOut.outerHTML;
                callback(output);
              });
            };
        });
    },
  },
  created() {
    this.initMathjax()
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