<template>
  <div class="settingsWatchRecorded">
    <div class="watch-top">
      <h1 class="watch-title" v-language:inner="'studyroomSets_watch_title'"/>
      <div class="watch-middle">
        <img class="watch-img" src="../image/watch.png" alt="">
        <div class="watch-middle-cont">
          <p class="watch-middle-txt" v-language:inner="'studyroomSets_watch-middle'"/>
          <div class="watch-middle-lesson">
            <span v-language:inner="'studyroomSets_watch_middle_lesson'"/>
            <a class="lessonFaq" :href="faqPageLink" target="_blank" v-language:inner="'studyroomSets_watch_middle_faq'"/>
          </div>
        </div>
      </div>
    </div>
    <v-divider class="watch-divider"/>
    <div class="watch-actions">
      <div class="actions-terms">
        <span v-language:inner="'studyroomSets_watch_actions_txt'"/> 
        <a class="actions-terms-link" :href="termPageLink" target="_blank" v-language:inner="'studyroomSets_watch_actions_term'"/>
      </div>
      <v-btn class="actions-yes" depressed round color="#4452fc" @click="gotoNextPage"><span v-language:inner="'studyroomSets_watch_actions_yes'"/></v-btn>
      <v-btn class="actions-no" depressed round @click="skipToStudyRoom"><span v-language:inner="'studyroomSets_watch_actions_no'"/></v-btn>
    </div>
  </div>
</template>

<script>
import satelliteService from '../../../services/satelliteService';
export default {
  props:{
    nextStep:{
      type:Function,
      required: true
    }
  },
  computed:{
    termPageLink(){
      return satelliteService.getSatelliteUrlByName('terms');
    },
    faqPageLink(){
      return satelliteService.getSatelliteUrlByName('faq');
    }
  },
  methods:{
    gotoNextPage(){
      this.nextStep('enableScreenStep')
    },
    skipToStudyRoom(){
      this.nextStep('studyRoom')
    },
  }
}
</script>

<style lang="less">
.settingsWatchRecorded{
  max-width: 968px;
  width: 968px;
  color: #43425d;
  .watch-top{
    max-width: 674px;
    margin: 0 auto;
    .watch-title{
      font-size: 38px;
      font-weight: 600;
      font-stretch: normal;
      font-style: normal;
      line-height: normal;
      letter-spacing: normal;
      text-align: center;
    }
    .watch-middle{
      margin-top: 64px;
      display: flex;
      align-items: center;
      .watch-middle-cont{
        padding-left: 30px;
        .watch-img{
          object-fit: contain;
        }
        .watch-middle-txt{
          margin: 0;
          padding: 0;
          padding-bottom: 18px;
          font-size: 16px;
          font-weight: 600;
          font-stretch: normal;
          font-style: normal;
          line-height: 1.63;
          letter-spacing: normal;
        }
        .watch-middle-lesson{
          font-size: 16px;
          font-weight: 600;
          line-height: 1.63;
          .lessonFaq{
            color: #5560ff;
            text-decoration: underline;
          }
        }
      }
    }
  }
  .watch-divider{
    margin-top: 100px;
    width: 100%;
  }
  .watch-actions{
    text-align: center;
    margin-top: 20px;
    .actions-terms{
      margin-bottom: 16px;
      font-size: 14px;
      font-weight: 600;
      font-stretch: normal;
      font-style: normal;
      line-height: 1.86;
      letter-spacing: normal;
      color: #595475;
      .actions-terms-link{
        color: #6870ff;
        text-decoration: underline;
      }
    }
    .v-btn{
      min-width: 178px;
      height: 40px !important;
      text-transform: capitalize  !important;
      font-size: 14px;
      font-weight: 600;
    }
    .actions-yes{
      color: white !important;
    }
    .actions-no{
      color: #4452fc;
      border: 1px solid #4452fc !important;
    }
  }
}
</style>