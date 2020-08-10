<template>
  <div class="itemPage">
    <div class="itemPage__main">
      <div class="itemPage__main__document">
        <mainItem :document="getDocumentDetails"></mainItem>
      </div>
    </div>
  </div>
</template>

<script>
import { mapActions, mapGetters } from "vuex";

// components
import mainItem from "./components/mainItem/mainItem.vue";

export default {
  name: "itemPage",
  components: {
    mainItem,
  },
  props: {
    id: {}
  },
  computed: {
    ...mapGetters([
      'getDocumentDetails'
    ]),
  },
  methods: {
    ...mapActions([
      "documentRequest",
      "clearDocument",
    ]),
  },

  beforeDestroy() {
    this.clearDocument();
  },
  mounted() {
    this.documentRequest(this.id).catch(()=>{
      this.$store.dispatch('updateCurrentItem');
    });
  },
};
</script>

<style lang="less">
@import "../../../styles/mixin";

.itemPage {
  //hacks to finish this fast
  .price-area,
  hr,
  .spacer {
    display: none !important;
  }
  .azuremediaplayer {
    background: #fff !important;
  }
  position: relative;
  margin: 0 auto;
  max-width: 960px;
  @media (max-width: @screen-md) {
    margin: 20px;
  }
  @media (max-width: @screen-xs) {
    margin: 0;
    display: block;
  }
  .sticky-item {
    position: sticky;
    height: fit-content;
    top: 80px;
  }
  &__main {
    @media (max-width: @screen-sm) {
      margin-right: 0;
      max-width: auto;
    }
    &__document {
      width: 100%;
      margin: 0 auto;
      @media (max-width: @screen-sm) {
        width: auto;
        margin: 0 auto;
      }
      &__tutor {
        display: flex;
        align-items: center;
        flex-wrap: wrap;
        font-weight: 600;
        font-size: 14px;
        &__link {
          @media (max-width: @screen-md) {
            margin-bottom: 6px;
          }
          &--title1 {
            display: inline-block;
            color: #5560ff;
            cursor: pointer;
            @media (max-width: @screen-xs) {
              white-space: nowrap;
              display: block;
            }
          }
          &--title2 {
            color: #4d4b69;
            display: inline-block;
            cursor: text;
            @media (max-width: @screen-xs) {
              white-space: nowrap;
              display: block;
            }
          }
          @media (max-width: @screen-xs) {
            flex-direction: column;
            justify-content: center;
            margin-bottom: 10px;
          }
        }
        &--btn {
          border: solid 1px #4452fc;
          border-radius: 28px;
          background: #fff !important; //vuetify
          @media (max-width: @screen-xs) {
            padding: 0 10px;
          }
          div {
            color: #4452fc;
            font-size: 13px;
            font-weight: 600;
            text-transform: initial;
            margin-bottom: 1px;
          }
        }
      }
      &--loader {
        display: flex;
        align-items: center;
        justify-content: center;
        min-height: 160px;
      }
    }
  }
}
</style>