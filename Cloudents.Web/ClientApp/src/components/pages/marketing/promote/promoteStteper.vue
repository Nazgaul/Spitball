<template>
    <v-stepper class="stepperWrap">
        <v-stepper-header class="elevation-0">
            <v-stepper-step class="stepStteper ps-8" :class="[step === 1 ? 'active' : 'noActive']" step="1" @click="goStep(1)">
                {{$t('promote_title')}}
            </v-stepper-step>

            <v-divider></v-divider>

            <v-stepper-step class="stepStteper ps-8" :class="[step === 2 ? 'active' : 'noActive']" step="2" @click="goStep(3)">
                <!-- {{$t('promote_choose')}} {{$t('promote_your_content')}} -->
                {{$t('promote_choose')}} {{$t('promote_template')}}
            </v-stepper-step>

            <v-divider></v-divider>

            <v-stepper-step class="stepStteper" :class="[step === 3 ? 'active' : 'noActive']" step="3" @click="goStep(4)">
                <!-- {{$t('promote_choose')}} {{$t('promote_template')}} -->
                {{$t('promote_publish')}}
            </v-stepper-step>
            <!-- <v-divider></v-divider>

            <v-stepper-step class="stepStteper pe-6" :class="[step === 4 ? 'active' : 'noActive']" step="4" @click="goStep(4)">
                {{$t('promote_publish')}}
            </v-stepper-step> -->
        </v-stepper-header>

        <!-- Mobile stepper label -->
        <div class="mobileLabels px-5 mt-n3 text-center d-none d-sm-flex d-md-none justify-space-between">
            <div class="label1 fontLabel" :class="[step === 1 ? 'active' : 'noActive']"><div class="fontLabel">{{$t('promote_choose')}}</div>{{$t('promote_your_content')}}</div>
            <div class="label2 fontLabel" :class="[step === 2 ? 'active' : 'noActive']"><div class="fontLabel">{{$t('promote_choose')}}</div>{{$t('promote_your_content')}}</div>
            <div class="label3 fontLabel me-4" :class="[step === 3 ? 'active' : 'noActive']"><div class="fontLabel">{{$t('promote_choose')}}</div>{{$t('promote_template')}}</div>
            <div class="label4 fontLabel" :class="[step === 4 ? 'active' : 'noActive']">{{$t('promote_publish')}}</div>
        </div>

        <v-stepper-items>
            <v-stepper-content class="pa-4 pt-0" :step="step">
                <component
                  :is="stepComponent"
                  :theme="theme"
                  :document="document"
                  :dataType="dataType"
                  :resource="resource"
                  :currentCourseItem="currentCourseItem"
                  @selectedTemplate="selectedTemplate"
                  @selectedDocument="selectedDocument"
                  @setCurrentCourse="handleCurrentCourse"
                  ref="childComponent">
                </component>
                <div class="text-sm-end text-center">
                  <v-alert type="error" class="text-left mt-4 mb-0" v-show="error">{{$t('promote_table_error')}}</v-alert>
                  <v-btn class="white--text mt-10" width="120" depressed v-if="step === 2" @click="nextStep" color="#4452fc" rounded>{{$t('promote_btn_next')}}</v-btn>
                  <v-btn class="white--text mt-10" width="120" depressed v-if="step == 3" :to="{name: routeNames.Dashboard}" color="#4452fc" rounded>{{$t('promote_btn_done')}}</v-btn>
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
      currentCourseItem: {},
      routeNames,
      dataType: '',
      step: 1,
      error: false,
      theme: 0,
      document: null,
      stepComponent: 'marketingActions',
      stepComponents: {
        step1: 'marketingActions',
        // step2: 'promoteTable',
        step2: 'promoteTemplate',
        step3: 'promotePublish'
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
          title1: this.$t('promote_your_course'),
          title2: this.$t('promote_sharePost_title2'),
          image: require('../images/promoteVideo.png'),
          buttonText: this.$t('promote_lets_go'),
          action: this.promoteCourse
        }
        // box2: {
          // title1: this.$t('promote_createOffer_title1'),
          // title2: this.$t('promote_createOffer_title2'),
          // image: require('../images/promoteVideo.png'),
          // buttonText: this.$t('promote_lets_go'),
          // action: this.promoteVideos,
          // isDisabled:true
        // },
        // box3: {
        //   title1: this.$t('promote_createVideo_title1'),
        //   title2: this.$t('promote_createVideo_title2'),
        //   image: require('../images/promoteContent.png'),
        //   buttonText: this.$t('promote_lets_go'),
        //   action: this.promoteDocuments,
        //   isDisabled:true
        // }
      }
    }
  },
  methods: {
    nextStep() {
      let ref = this.$refs.childComponent;
      if(ref.selected || this.step === 1) {
        if(this.dataType === 'Courses') {
          this.step = 3
        } else {
          this.step += 1;
        }
        this.stepComponent = this.stepComponents[`step${this.step}`];
        return;
      }
      this.error = true
    },
    selectedDocument(document) {
      this.document = document;
      this.error = false;
    },
    handleCurrentCourse(item) {
      this.dataType = 'Courses';
      this.currentCourseItem = item
    },
    selectedTemplate(theme) {
      this.theme = theme;
      this.nextStep()
    },
    promoteProfile() {
      this.step = 3;
      this.document = null;
      this.dataType = 'profile';
      this.stepComponent = this.stepComponents[`step${this.step}`]
    },
    promoteCourse() {
      this.dataType = 'Courses';
      this.nextStep();
    },
    promoteVideos () {
      this.dataType = 'Video';
      this.nextStep()
    },
    promoteDocuments() {
      this.dataType = 'Document';
      this.nextStep()
    },
    goStep(step) {
      if(this.step < step) return
      if(this.dataType === 'profile' && this.step === 3 && step !== 1) return;

      this.error = false
      this.step = step;
      this.stepComponent = this.stepComponents[`step${this.step}`]
    }
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
    .v-stepper__label {
      color: @global-purple !important;
    }
    &.noActive {
      // cursor: pointer;
      .v-stepper__step__step {
        background: transparent !important;
        border: 2px solid #4452fc !important;


        font-weight: 600;
        color: #4c59ff;
      }
      .v-stepper__label {
        text-shadow: none;
      }
    }
    &.active {
      .v-stepper__step__step {
        background: -webkit-linear-gradient(53deg, #4452fc 27%, #3892e4 115%) !important;
      background: linear-gradient(53deg, #4452fc 27%, #3892e4 115%) !important;
      }
      .v-stepper__label {
        text-shadow: 0 0 0 black;
      }
    }

  }
  .mobileLabels {
    color: @global-purple !important; //vuetify

      .noActive {
        cursor: pointer;
      }
      .active {
        text-shadow: 0 0 0 black;
    }
    .fontLabel {
      font-size: 12px;
    }
  }
}
</style>
