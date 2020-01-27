<template>
  <div>
    <banner v-if="getBannerParams"></banner>
  </div>
</template>

<script>
const banner = () => import("./banner.vue");
import { mapGetters } from "vuex";
export default {
  components: {
    banner
  },
  computed: {
    ...mapGetters(["getBannerParams"])
  }
};
</script>