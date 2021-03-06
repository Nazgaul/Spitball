<template>
  <v-dialog :value="true" content-class="itemDialog" :fullscreen="$vuetify.breakpoint.xsOnly"
    persistent :overlay="false" max-width="974px" transition="dialog-transition" scrollable>
    <v-card>
      <v-card-title class="itemTitle px-4 pt-6 pt-sm-3 pb-0 pb-sm-3">
        <h1 class="itemTitleText" :class="{'text-truncate':!isMobile}">{{getDocumentName}}</h1>
        <v-spacer v-if="!isMobile"></v-spacer>
        <template v-if="$store.getters.getDocumentLoaded">
          <template v-if="!getIsPurchased">
            <v-btn :loading="getBtnLoading" @click="openPurchaseDialog" rounded outlined class="font-weight-bold me-0 me-sm-8 mb-3 mb-sm-0" color="#4c59ff">
              <span>{{$t('documentPage_unlock_video_btn')}}</span>
            </v-btn>
          </template>
          <template v-if="!isVideo && getIsPurchased">
            <v-btn :loading="getBtnLoading" tag="a" :href="downloadUrl" target="_blank" @click="downloadDoc" rounded outlined class="font-weight-bold me-0 me-sm-8 mb-3 mb-sm-0" color="#4c59ff">
              <span v-t="'documentPage_download_btn'"></span>
            </v-btn>
          </template>
        </template>

        <v-icon @click="closeItem" class="closeIcon" size="14" v-text="'sbf-close'"/> 
      
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text class="pt-sm-4 pt-3 px-sm-4 px-3 pb-0">
        <itemForDialog :id="id"/>
      </v-card-text>
      <v-card-actions class="justify-center pa-sm-4 pa-0" :class="{'pb-sm-0':isVideo}">
        <div class="paging" v-if="!isVideo && $store.getters.getDocumentLoaded">
          <v-layout class="actions">
            <button class="actions--left" @click="prevDoc()" v-if="showDesktopButtons">
                <v-icon class="actions--img" v-html="'sbf-arrow-left-carousel'"/>
            </button>
            <div class="mx-4 paging--text justify-center">{{$t('documentPage_docPage', [$store.getters.getCurrentPage + 1, documentPreviews.length])}}</div>          
            <button class="actions--right" @click="nextDoc()" v-if="showDesktopButtons">
                <v-icon class="actions--img" v-html="'sbf-arrow-right-carousel'"/>
            </button>
          </v-layout>
        </div>
      </v-card-actions>
    </v-card>

  </v-dialog>
</template>

<script>
import itemForDialog from '../../itemPage/itemForDialog.vue';
import { mapGetters, mapActions } from 'vuex';

export default {
  components:{
    itemForDialog
  },
  computed: {
    ...mapGetters([
      'getDocumentName',
      'getIsPurchased',
      'getBtnLoading',
      'getDocumentDetails',
      'getUserLoggedInStatus',
      ]),
    id(){
      return this.$store.getters.getCurrentItemId;
    },
    isVideo() {
      return this.getDocumentDetails?.documentType === "Video";
    },
    downloadUrl(){
      return `/document/${this.id}/download`;
    },
    showDesktopButtons(){
      if(this.documentPreviews) {
          return !(this.isMobile || this.documentPreviews?.length < 2);
      }
      return false;
    },
    documentPreviews(){
      return this.getDocumentDetails?.preview || [];
    },
    isMobile(){
      return this.$vuetify.breakpoint.xsOnly;
    }
  },
  methods: {
    ...mapActions(['downloadDocument']),
    closeItem(){
      this.$store.dispatch('updateCurrentItem')
    },
    openPurchaseDialog() {
      if (this.getUserLoggedInStatus) {
        let courseId = this.getDocumentDetails.courseId;
        this.$store.dispatch('updateEnrollCourse',courseId)
      } else {
        this.$store.commit("setComponent", "register");
      }
    },
    downloadDoc(e) {
      if (!this.getUserLoggedInStatus) {
        e.preventDefault();
      }
      let item = {
        id: this.getDocumentDetails?.id
      };
      this.downloadDocument(item);
    },
    prevDoc(){
      this.$store.dispatch('updateItemPaging',false)
    },
    nextDoc(){
      this.$store.dispatch('updateItemPaging',true)
    }
  },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.itemDialog{
  background: white;
  max-height: 70vh;
  height: auto;
  @media (max-width: @screen-xs) {
    height: 100%;
    max-height: initial;
  }
  .itemTitle{
    height: 62px;
    position: relative;
    @media (max-width: @screen-xs) {
      height: auto;
      padding-top: 28px;
      flex-direction: column;
    }
    .itemTitleText{
      max-width: 70%;
      @media (max-width: @screen-xs) {
        max-width: 100%;
        padding-bottom: 16px;
        text-align: center;
        padding-right: 10px;
        padding-left: 10px;
      }
      word-break: break-word;
      font-size: 18px;
      font-weight: 600;
      color: #43425d;
    }
    .closeIcon{
      position: absolute;
      right: 16px;
      opacity: 0.34;
      color: #43425d;
      @media (max-width: @screen-xs) {
        top: 10px;
        right: 10px;
      }
    }
  }
  .paging{
    .actions {
      display: flex;
      justify-content: center;
      background: #fff;
      padding: 14px 0;
      border-radius: 0 0 8px 8px;
      &--img {
        transform: none /*rtl:scaleX(-1)*/;
        color: #4c59ff !important; //vuetify
        font-size: 14px !important; //vuetify
        font-weight: 600;
          &:before {
            font-weight: 600 !important;
          }
        }
      &--left {
        width: 32px;
        padding: 2px 6px 6px 6px;
        border-radius: 38px 0 0 38px;
        border: solid 1px #d7d7d7;
        outline: none;
        background: #fff;
      }
      &--right {
        width: 32px;
        padding: 2px 6px 6px 6px;
        border-radius: 0 38px 38px 0;
        border: solid 1px #d7d7d7;
        outline: none;
        background: #fff;
      }
    }
    &--text {
        min-width: 120px;
        text-align: center;
        display: flex;
        align-items: center;
        font-size: 16px;
        color: #4d4b69;
        font-weight: 600;
    }
  }
}
</style>