<template>
  <div class="shared-document-container">
    <iframe v-if="sharedDocUrl" :src="sharedDocUrl" frameborder="0"></iframe>
  </div>
</template>

<script>
export default {
  name: "sharedDocument",
  data() {
    return {};
  },
  props: {
    roomId: {
      required: false,
      type: String
    }
  },
  computed: {
    sharedDocUrl() {
      return this.$store.getters.getRoomOnlineDocument;
    }
  }
};
</script>

<style scoped lang="less">
.shared-document-container {
    position: relative;
//   .iframe-container {
//     position: relative;
//     width: ~"calc(100% - 333px)"; //minus chat& vide width
//     height: ~"calc(100vh - 106px)"; // minus toop toolbar menu height
//     margin-top: 2px;
//   }
   iframe {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    width: 100%;
    height: 100%;
    border: 0;
  }

  /* 4x3 Aspect Ratio */
//   .iframe-container-4x3 {
//     padding-top: 75%;
//   }
}
</style>