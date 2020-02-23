<template>
    <div class="promoteTemplate">
        <div class="top  d-none d-sm-flex align-center">
            <div class="topWrap">
                <div class="textWrap mr-4">
                  <div class="text1">{{$t('promote_template_title1')}}</div>
                  <div class="text2">{{$t('promote_template_title2')}}</div>
                </div>
            </div>
            <div class="bottomWrap text-center" >
                <img class="img" :src="$proccessImageUrl('https://spitball-function-dev2.azurewebsites.net/api/share/document/50997?theme=1', 422, 220 )" alt="">
                <div class="btnWrap">
                  <v-btn class="useBtn" @click="useTemplate(recommended)" height="34" rounded outlined color="#4c59ff">{{$t('promote_use_template')}}</v-btn>
                </div>
            </div>
        </div>

        <div class="">{{$t('promote_choose_template')}}</div>

        <div class="bottom">
          <div v-for="n in 3" class="bottomWrap text-center" :key="n">
              <img class="img" :src="$proccessImageUrl(`https://spitball-function-dev2.azurewebsites.net/api/share/document/50997?theme=${n+1}`, 292, 150)" alt="">
              <div class="btnWrap">
                <v-btn class="useBtn" @click="useTemplate(template)" height="34" rounded outlined color="#4c59ff">
                    <span class="text-truncate">{{$t('promote_use_template')}}</span>
                </v-btn>
              </div>
          </div>
          <!-- <div v-for="template in resource" class="bottomWrap text-center" :key="template.id">
              <img class="img" :src="" alt="">
              <div class="btnWrap">
                <v-btn class="useBtn" @click="useTemplate(template)" height="34" rounded outlined color="#4c59ff">
                    <span class="text-truncate">{{$t('promote_use_template')}}</span>
                </v-btn>
              </div>
          </div> -->
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
    video : {
      type: Object,
      default: () => ({})
    }
  },
  data() {
    return {
      selected: 0,
    }
  },
  methods: {
    useTemplate(template) {
      this.selected = template.id
      this.$emit('selectedTemplate', template);
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
  }

  .bottom {
    display: grid;
    grid-template-columns: repeat(auto-fill, 292px);
    grid-gap: 18px;
    @media (max-width: @screen-xs) {
      grid-template-columns: repeat(auto-fill, 100%);
    }
  }

  .bottomWrap {
    width: max-content;
    @media (max-width: @screen-xs) {
      width: 100%;
    }
    .img {
      vertical-align: bottom;
      width: 100%;
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
    }
  }
}
</style>
