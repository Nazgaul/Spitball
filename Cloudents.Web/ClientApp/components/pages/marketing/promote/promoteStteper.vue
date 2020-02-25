<template>
    <v-stepper class="stepperWrap elevation-0">
        <router-link :to="{name: routeNames.Marketing}" class="d-block text-right pa-2">
          <v-icon size="12" color="#adadba">sbf-close</v-icon>
        </router-link>
        <v-alert type="error" v-show="error">
            {{$t('promote_table_error')}}
        </v-alert>
        <v-stepper-header class="elevation-0">
            <v-stepper-step class="stepStteper pl-8" :class="[step === 1 ? 'active' : 'noActive']" step="1">
                {{$t('promote_choose')}} {{$t('promote_your_content')}}
            </v-stepper-step>

            <v-divider></v-divider>

            <v-stepper-step class="stepStteper" :class="[step === 2 ? 'active' : 'noActive']" step="2">
                {{$t('promote_choose')}} {{$t('promote_template')}}
            </v-stepper-step>
            <v-divider></v-divider>

            <v-stepper-step class="stepStteper pr-6" :class="[step === 3 ? 'active' : 'noActive']" step="3">
                {{$t('promote_publish')}}
            </v-stepper-step>
        </v-stepper-header>

        <div class="mobileLabels px-4 mt-n3 text-center d-md-none d-flex justify-space-between">
            <div class="label1 fontLabel"><div class="fontLabel">{{$t('promote_choose')}}</div>{{$t('promote_your_content')}}</div>
            <div class="label2 fontLabel mr-4"><div class="fontLabel">{{$t('promote_choose')}}</div>{{$t('promote_template')}}</div>
            <div class="label3 fontLabel">{{$t('promote_publish')}}</div>
        </div>

        <v-stepper-items>
            <v-stepper-content class="pa-4" :step="step">
                <component
                  :is="stepComponent"
                  :template="template"
                  :video="video"
                  :dataType="dataType"
                  @selectedTemplate="selectedTemplate"
                  @selectedVideo="selectedVideo"
                  ref="childComponent">
                </component>
                <div class="text-right">
                  <v-btn class="white--text mt-10" width="120" v-if="step !== 2" @click="nextStep" color="#4452fc" rounded>{{$t('promote_btn_next')}}</v-btn>
                </div>
            </v-stepper-content>
        </v-stepper-items>

   </v-stepper>
</template>
<script>
import * as routeNames from '../../../../routes/routeNames';

const promoteTable = () => import('./promoteTable.vue');
const promoteTemplate = () => import('./promoteTemplate.vue');
const promotePublish = () => import('./promotePublish.vue');

export default {
  name: "",
  components: {
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
      stepComponent: 'promoteTable',
      stepComponents: {
        step1: 'promoteTable',
        step2: 'promoteTemplate',
        step3: 'promotePublish',
      }
    }
  },
  methods: {
    nextStep() {
      let ref = this.$refs.childComponent;
      if(ref.selected) {
        this.step += 1;
        this.stepComponent = this.stepComponents[`step${this.step}`]
        return;
      }
      this.error = true
    },
    selectedVideo(video) {
      this.dataType = 'video';
      this.video = video;
      this.error = false
    },
    selectedTemplate(template) {
      this.template = template;
      this.dataType = 'document';
      this.nextStep()
    }
  },
}
</script>
<style lang="less">
.stepperWrap {

  .stepStteper {
    padding: 24px 34px;
    @media (max-width: 599px) {
      padding: 24px 30px;
    }
    .v-stepper__step__step {
      width: 30px;
      height: 30px;
      font-size: 16px;
      background: linear-gradient(53deg, #4452fc 27%, #3892e4 115%) !important;
      background: -webkit-linear-gradient(53deg, #4452fc 27%, #3892e4 115%) !important;

    }
    .v-stepper__label {
      color: #43425d !important;
    }
    &.noActive {
      .v-stepper__step__step {
        background: -webkit-linear-gradient(53deg, #4452fc 27%, #3892e4 115%) !important;
        -webkit-background-clip: text !important;
        -webkit-text-fill-color: transparent !important;
        border: 1.5px solid #4452fc !important;
        font-weight: 600;
      }
    }
    &.active {
      .v-stepper__step__step {
        padding-right: 1px;
      }
    }

  }
  .mobileLabels {
    color: #43425d !important; //vuetify

    .fontLabel {
      font-size: 12px;
    }
  }
}
</style>
