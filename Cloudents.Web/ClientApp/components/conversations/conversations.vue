<template>
    <div class="wrapper">
        <div id="chat-container" ></div>
    </div>
</template>

<script>
import { mapGetters } from "vuex";
import { debug } from "util";

export default {
  computed: {
    ...mapGetters(["talkSession"])
  },
  mounted() {
    if (this.talkSession) {
      const inbox = this.talkSession.createInbox({
        showFeedHeader: false
      });

      inbox.mount(document.getElementById("chat-container"));
    }
  }
};
</script>

<style lang="less" scoped>
// .wrapper {
  
// }
#chat-container {
  //margin: 0 auto;
//height: 100%;
  //max-height: 680px;
  margin-top: 24px;
  //max-width: 420px;
  //border: 2px solid red;
}
</style>
