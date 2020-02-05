<template>
   <v-flex class="right-sidebar" :class="{'bannerActive':getBannerParams}">
      <v-flex xs12 class="card-block">
         <marketing-box/>
      </v-flex>
   </v-flex>
</template>

<script>
import { mapGetters } from "vuex";
import marketingBox from "../marketingBox/marketingBox.vue";
export default {
   components: {
      marketingBox
   },
   computed: {
      ...mapGetters(["getBannerParams"])
   }
};
</script>

<style lang="less">
@import "../../../../../styles/mixin.less";

.cross-browser-sticky(~".right-sidebar");
.right-sidebar {
   top: 80px;
   &.bannerActive {
      top: 150px;
   }

   @media (max-width: @screen-xs) {
      display: none;
   }

   .card-block {
      width: 100%;
   }
}
</style>
