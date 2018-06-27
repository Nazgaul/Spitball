<template>
    <div class="wrapper chat-wrapper">
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
<style src="./conversations.less" lang="less"></style>
