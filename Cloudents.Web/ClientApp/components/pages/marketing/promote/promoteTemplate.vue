<template>
    <div class="promoteTemplate">
        <div class="top d-none d-md-flex align-center">
            <div class="topWrap">
                <div class="textWrap mr-4">
                  <div class="text1">{{$t('promote_template_title1')}}</div>
                  <div class="text2">{{$t('promote_template_title2')}}</div>
                </div>
            </div>

            <div class="bottomWrap text-center">
                <v-skeleton-loader type="image" width="422" v-if="loading"></v-skeleton-loader>
                <img class="img" @load="onLoad" v-show="!loading" :src="`https://spitball-function-dev2.azurewebsites.net/api/share/document/${documentId}?theme=1&width=422&amp;height=220&amp;rtl=${rtl}`" alt="">
                <div class="btnWrap">
                  <v-btn class="useBtn" :disabled="loading" @click="useTemplate()" height="34" rounded outlined color="#4c59ff">{{$t('promote_use_template')}}</v-btn>
                </div>
            </div>
        </div>

        <div class="centerTitle">{{$t('promote_choose_template')}}</div>

        <div class="bottom">
          <div v-for="n in 3" class="bottomWrap text-center" :key="n">
              <v-skeleton-loader type="image" width="292" v-if="loading"></v-skeleton-loader>
              <img class="img" @load="onLoad" v-show="!loading" :src="`https://spitball-function-dev2.azurewebsites.net/api/share/document/${documentId}?theme=${n+1}&width=292&amp;height=150&amp;rtl=${rtl}`" alt="">
              <div class="btnWrap">
                <v-btn class="useBtn" :disabled="loading" @click="useTemplate(template)" height="34" rounded outlined color="#4c59ff">
                    <span class="text-truncate">{{$t('promote_use_template')}}</span>
                </v-btn>
              </div>
          </div>
        </div>

    </div>
</template>
<script>
export default {
  name: "promoteTemplate",
  props: {
    template : {
      type: Object,
      default: () => ({})
    },
    document : {
      type: Object,
      default: () => ({})
    }
  },
  data() {
    return {
      selected: 0,
      loading: true,
    }
  },
  computed: {
    rtl() {
      return global.country === 'IL' ? 'True' : 'False';
    },
    documentId() {
      return this.document.id;
    }
  },
  methods: {
    useTemplate() {
      this.selected = this.document.id
      this.$emit('selectedTemplate', this.document);
    },
    onLoad() {
      this.loading = false
    }
  }
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';
.promoteTemplate {
  .top {
    margin-bottom: 60px;
    .topWrap {
      flex-grow: 1;
      .textWrap {
        max-width: 330px;
        .text1 {
          font-size: 24px;
          font-weight: 600;
          color: @global-purple;
        }
        .text2 {
          color: @global-purple;
        }
      }
    }
    .bottomWrap {
      width: 422px;
      .img {
        vertical-align: bottom;
        @media (max-width: @screen-xs) {
          width: 100%;
        }
      }
    }
  }

  .centerTitle {
    color: @global-purple;
    font-size: 18px;
    margin-bottom: 14px;
    font-weight: 600;
  }

  .bottom {
    display: grid;
    grid-template-columns: repeat(auto-fill, 292px);
    grid-gap: 18px;
    @media (max-width: @screen-xs) {
      grid-template-columns: repeat(auto-fill, 100%);
    }
    .bottomWrap {
      .img {
        vertical-align: bottom;
        @media (max-width: @screen-xs) {
          width: 100%;
        }
      }
    }
  }

  .btnWrap {
    border: 1px solid #dddddd;
    border-top: 0;
    border-radius: 0 0 8px 8px;
    padding: 16px;
    .useBtn {
      min-width: 120px;
      text-transform: initial;
      font-weight: 600;
      letter-spacing: normal;

      span {
        margin-bottom: 2px;
      }
    }
  }
}
</style>
