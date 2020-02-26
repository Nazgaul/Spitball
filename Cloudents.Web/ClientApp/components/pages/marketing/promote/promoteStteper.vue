<template>
    <v-stepper class="stepperWrap">
        <div class="text-right pa-4 pb-0">
          <router-link :to="{name: routeNames.Marketing}" class="">
            <v-icon size="12" color="#adadba" class="">sbf-close</v-icon>
          </router-link>
        </div>

        <v-stepper-header class="elevation-0">
            <v-stepper-step class="stepStteper pl-8" :class="[step === 1 ? 'active' : 'noActive']" step="1">
                {{$t('promote_choose')}} {{$t('promote_your_content')}}
            </v-stepper-step>

            <v-divider></v-divider>

            <v-stepper-step class="stepStteper pl-8" :class="[step === 2 ? 'active' : 'noActive']" step="2">
                {{$t('promote_choose')}} {{$t('promote_your_content')}}
            </v-stepper-step>

            <v-divider></v-divider>

            <v-stepper-step class="stepStteper" :class="[step === 3 ? 'active' : 'noActive']" step="3">
                {{$t('promote_choose')}} {{$t('promote_template')}}
            </v-stepper-step>
            <v-divider></v-divider>

            <v-stepper-step class="stepStteper pr-6" :class="[step === 4 ? 'active' : 'noActive']" step="4">
                {{$t('promote_publish')}}
            </v-stepper-step>
        </v-stepper-header>

        <!-- Mobile stepper label -->
        <!-- <div class="mobileLabels px-4 mt-n3 text-center d-md-none d-flex justify-space-between">
            <div class="label1 fontLabel"><div class="fontLabel">{{$t('promote_choose')}}</div>{{$t('promote_your_content')}}</div>
            <div class="label2 fontLabel"><div class="fontLabel">{{$t('promote_choose')}}</div>{{$t('promote_your_content')}}</div>
            <div class="label3 fontLabel mr-4"><div class="fontLabel">{{$t('promote_choose')}}</div>{{$t('promote_template')}}</div>
            <div class="label4 fontLabel">{{$t('promote_publish')}}</div>
        </div> -->

        <v-stepper-items>
            <v-stepper-content class="pa-4 pt-0" :step="step">
                <component
                  :is="stepComponent"
                  :template="template"
                  :video="video"
                  :dataType="dataType"
                  :resource="resource"
                  @selectedTemplate="selectedTemplate"
                  @selectedDocument="selectedDocument"
                  ref="childComponent">
                </component>
                <div class="text-right">
                  <v-alert type="error" v-show="error">
                    {{$t('promote_table_error')}}
                  </v-alert>
                  <v-btn class="white--text mt-10" width="120" v-if="step !== 1 && step !== 3" @click="nextStep" color="#4452fc" rounded>{{$t('promote_btn_next')}}</v-btn>
                </div>
            </v-stepper-content>
        </v-stepper-items>

   </v-stepper>
</template>
<script>
import * as routeNames from '../../../../routes/routeNames';

const marketingActions = () => import('../marketingActions/marketingActions.vue');
const promoteTable = () => import('./promoteTable.vue');
const promoteTemplate = () => import('./promoteTemplate.vue');
const promotePublish = () => import('./promotePublish.vue');

export default {
  components: {
    marketingActions,
    promoteTable,
    promoteTemplate,
    promotePublish
  },
  data() {
    return {
      routeNames,
      dataType: '',
      step: 1,
      error: false,
      template: null,
      video: null,
      stepComponent: 'marketingActions',
      stepComponents: {
        step1: 'marketingActions',
        step2: 'promoteTable',
        step3: 'promoteTemplate',
        step4: 'promotePublish'
      },
      resource: {
        box1: {
          title1: this.$t('promote_sharePost_title1'),
          title2: this.$t('promote_sharePost_title2'),
          image: require('../images/promoteProfile.png'),
          buttonText: this.$t('promote_lets_go'),
          action: this.promoteProfile
        },
        box2: {
          title1: this.$t('promote_createOffer_title1'),
          title2: this.$t('promote_createOffer_title2'),
          image: require('../images/promoteVideo.png'),
          buttonText: this.$t('promote_lets_go'),
          action: this.promoteVideos
        },
        box3: {
          title1: this.$t('promote_createVideo_title1'),
          title2: this.$t('promote_createVideo_title2'),
          image: require('../images/promoteContent.png'),
          buttonText: this.$t('promote_lets_go'),
          action: this.promoteDocuments
        }
      }
    }
  },
  methods: {
    nextStep() {
      let ref = this.$refs.childComponent;
      if(ref.selected || this.step === 1) {
        this.step += 1;
        this.stepComponent = this.stepComponents[`step${this.step}`]
        return;
      }
      this.error = true
    },
    selectedDocument(video) {
      this.video = video;
      this.error = false;
    },
    selectedTemplate(template) {
      this.template = template;
      this.nextStep()
    },
    promoteProfile() {
      this.step = 4;
      this.dataType = 'profile';
      this.stepComponent = this.stepComponents[`step${this.step}`]
    },
    promoteVideos () {
      this.dataType = 'Video';
      this.nextStep()
    },
    promoteDocuments() {
      this.dataType = 'Document';
      this.nextStep()
    },
  },
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';
.stepperWrap {
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;

  @media (max-width: @screen-xs) {
    box-shadow: none;
    border-radius: 0;
  }
  .stepStteper {
    padding: 24px 34px;
    @media (max-width: @screen-xs) {
      padding: 24px 30px;
    }
    .v-stepper__step__step {
      width: 26px;
      height: 26px;
      font-size: 14px;
      background: linear-gradient(53deg, #4452fc 27%, #3892e4 115%) !important;
      background: -webkit-linear-gradient(53deg, #4452fc 27%, #3892e4 115%) !important;
      padding-bottom: 2px;
    }
    .v-stepper__label {
      color: @global-purple !important;
    }
    &.noActive {
      .v-stepper__step__step {
        background: -webkit-linear-gradient(53deg, #4452fc 27%, #3892e4 115%) !important;
        -webkit-background-clip: text !important;
        -webkit-text-fill-color: transparent !important;
        border: 1.5px solid #4452fc !important;
        font-weight: 600;
      }
      .v-stepper__label {
        text-shadow: none;
      }
    }
    &.active {
      .v-stepper__step__step {
        padding-right: 1px;
      }
      .v-stepper__label {
        text-shadow: 0px 0px 0px black;
      }
    }

  }
  .mobileLabels {
    color: @global-purple !important; //vuetify

    .fontLabel {
      font-size: 12px;
    }
  }
}
</style>
