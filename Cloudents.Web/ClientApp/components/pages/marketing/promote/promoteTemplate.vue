<template>
    <div class="promoteTemplate">
        <div class="top d-none d-md-flex align-center">
            <div class="topWrap">
                <div class="textWrap mr-4">
                  <div class="text1 mb-1">{{$t('promote_template_title1')}}</div>
                  <div class="text2">{{$t('promote_template_title2')}}</div>
                </div>
            </div>

            <div class="bottomWrap text-center">
                <v-skeleton-loader type="image" width="422" v-if="loading"></v-skeleton-loader>
                <img class="img" @load="onLoad" v-show="!loading" width="422" height="220" :src="`${domain}/api/share/document/${documentId}?theme=1&width=422&height=220&rtl=${rtl}`" alt="">
                <div class="btnWrap">
                  <v-btn class="useBtn" :disabled="loading" @click="useTemplate(1)" height="34" rounded outlined color="#4c59ff">{{$t('promote_use_template')}}</v-btn>
                </div>
            </div>
        </div>


        <div class="bottom">
          <div class="centerTitle d-block d-sm-none">{{$t('promote_choose_template')}}</div>
          <div v-for="n in 3" class="bottomWrap text-center" :key="n">
              <v-skeleton-loader type="image" width="292" v-if="loading"></v-skeleton-loader>
              <img class="img" @load="onLoad" v-show="!loading" width="292" height="150" :src="`${domain}/api/share/document/${documentId}?theme=${n+1}&width=292&height=150&rtl=${rtl}`" alt="">
              <div class="btnWrap">
                <v-btn class="useBtn" :disabled="loading" @click="useTemplate(n+1)" height="34" rounded outlined color="#4c59ff">
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
    document : {
      type: Object,
      default: () => ({})
    },
    resource: {
      required: false
    }
  },
  data() {
    return {
      selected: 0,
      loading: true,
      domain: ''
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
    useTemplate(templateNumber) {
      this.selected = templateNumber
      this.$emit('selectedTemplate', templateNumber);
    },
    onLoad() {
      this.loading = false
    }
  },
  created() {
    this.domain = window.functionApp;
  }
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';
.promoteTemplate {
  .top {
    margin: 50px 0;
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
          line-height: 1.57;
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
    font-weight: 600;
  }

  .bottom {
    display: grid;
    grid-template-columns: repeat(auto-fill, 292px);
    grid-gap: 18px;
    @media (max-width: @screen-sm) {
      margin-top: 50px;
    }
    @media (max-width: @screen-xs) {
      grid-template-columns: repeat(auto-fill, 100%);
      margin-top: 0;
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
