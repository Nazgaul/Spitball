<template>
  <div class="mainDocument-container">
    <h2 class="courseName font-weight-bold text-truncate pt-1 hidden-sm-and-up">{{courseName}}</h2>
    <v-layout row class="mainDocument-header pt-1 pb-2" align-center>
      <div class="main-header-wrapper">
        <v-icon
          class="grey--text"
          :class="['arrow-back','hidden-sm-and-down',isRtl? 'arrow-back-rtl': '']"
          @click="closeDocument"
        >sbf-arrow-back-chat</v-icon>
        <h2 class="courseName font-weight-bold text-truncate ml-3 hidden-sm-and-down">{{courseName}}</h2>
        <v-spacer class="hidden-sm-and-down"></v-spacer>
        <span v-if="docViews" class="grey-text" :class="[isSmAndDown ? 'pr-3' : 'pr-5']">
          {{docViews}}
          <span class="" v-language:inner="docViews > 1 ? 'resultNote_views' : 'resultNote_view'"/> 
        </span>
        <v-spacer class="hidden-sm-and-up"></v-spacer>
        <span class="grey-text date" :class="{'pl-3': isSmAndDown}">{{documentDate}}</span>

        <v-menu
          class="menu-area"
          lazy
          bottom
          left
          content-class="card-user-actions"
          v-model="showMenu"
        >
          <template v-slot:activator="{on}">
            <v-btn
              :depressed="true"
              v-on:click.native.stop.prevent="showReportOptions()"
              class="mr-0"
              icon
            >
              <v-icon class="verticalMenu">sbf-3-dot</v-icon>
            </v-btn>
          </template>
          <v-list>
            <v-list-item
              v-show="item.isVisible(item.visible)"
              :disabled="item.isDisabled()"
              v-for="(item, i) in actions"
              :key="i"
            >
              <v-list-item-title style="cursor:pointer;" @click="item.action()">{{ item.title }}</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
        <sb-dialog
          :showDialog="showReport"
          :maxWidth="'438px'"
          :popUpType="'reportDialog'"
          :content-class="`reportDialog ${isRtl? 'rtl': ''}` "
        >
          <report-item :closeReport="closeReportDialog" :itemType="'Document'" :itemId="itemId"></report-item>
        </sb-dialog>
        <sb-dialog
          :showDialog="priceDialog"
          :maxWidth="'438px'"
          :popUpType="'priceUpdate'"
          :onclosefn="closeNewPriceDialog"
          :activateOverlay="true"
          :isPersistent="true"
          :content-class="`priceUpdate ${isRtl ? 'rtl': ''}`"
        >
          <v-card class="price-change-wrap">
            <v-flex align-center justify-center class="relative-pos">
              <div class="title-wrap">
                <span class="change-title pr-1" v-language:inner="'resultNote_change_for'"></span>
                <span class="change-title">&nbsp;"{{courseName}}"</span>
              </div>
              <div class="input-wrap d-flex row align-center justify-center">
                <div :class="['price-wrap', isRtl ? 'reversed' : '']">
                  <vue-numeric
                    :currency="currentCurrency"
                    class="sb-input-upload-price"
                    :minus="false"
                    :min="0"
                    :precision="2"
                    :max="1000"
                    :currency-symbol-position="'suffix'"
                    separator=","
                    v-model="documentPrice"
                  ></vue-numeric>
                </div>
              </div>
            </v-flex>
            <div class="change-price-actions">
              <button @click="closeNewPriceDialog()" class="cancel mr-2">
                <span v-language:inner>resultNote_action_cancel</span>
              </button>
              <button @click="submitNewPrice()" class="change-price">
                <span v-language:inner>resultNote_action_apply_price</span>
              </button>
            </div>
          </v-card>
        </sb-dialog>
        <sb-dialog
          :showDialog="showPurchaseConfirmation"
          :popUpType="'purchaseConfirmation'"
          :activateOverlay="true"
          :isPersistent="true"
          :content-class="`confirmation-purchase-dialog`"
        >
          <v-card class="confirm-purchase-card">
            <v-card-title class="confirm-headline">
              <span v-html="$Ph('preview_about_to_buy', [docPrice, uploaderName])"></span>
            </v-card-title>
            <v-card-actions class="card-actions">
              <div class="doc-details">
                <div class="doc-type" v-if="!isVideo">
                  <v-icon class="doc-type-icon">sbf-document-note</v-icon>
                </div>
                <div class="doc-title">
                  <div class="text-truncate">{{courseName}}</div>
                </div>
              </div>
              <div class="purchase-actions">
                <v-btn text class="cancel" @click.native="updatePurchaseConfirmation(false)">
                  <span v-language:inner>preview_cancel</span>
                </v-btn>
                <v-btn rounded class="submit-purchase" @click.native="unlockDocument">
                  <span class="hidden-xs-only" v-language:inner>preview_buy_btn</span>
                  <span
                    class="hidden-sm-and-up text-uppercase"
                    v-language:inner
                  >preview_itemActions_buy</span>
                </v-btn>
              </div>
            </v-card-actions>
          </v-card>
        </sb-dialog>
      </div>
    </v-layout>
    <div class="document-wrap">
      <div class="text-center" v-if="!videoLoader">
        <img :style="{'width': `${dynamicWidthAndHeight.width}px`, 'height': `${dynamicWidthAndHeight.height}px`}" :src="require('./doc-preview-animation.gif')" alt="Photo" :class="{'video_placeholder': $vuetify.breakpoint.smAndDown}">
        <!-- <v-progress-circular v-if="isVideo" :style="{'width': `${dynamicWidthAndHeight.width}px`}"
            :class="{'video_placeholder': $vuetify.breakpoint.smAndDown}"
            width="3"
            :size="videoHeight" 
            indeterminate
            color="#4452fc"/> -->
      </div>
      
      <template  v-if="isVideo && videoSrc">
        <div style="margin: 0 auto;background:black" class="text-center main-header-wrapper mb-4">
          <sbVideoPlayer 
              @videoEnded="showAfterVideo = true"
              :id="`${document.details.id}`"
              :height="videoHeight" 
              :width="videoWidth" 
              style="margin: 0 auto" 
              :isResponsive="true" 
              :src="videoSrc"
              :title="courseName"
              :poster="`${document.preview.poster}?width=${videoWidth}&height=${videoHeight}&mode=crop&anchorPosition=bottom`"
          />
        </div>
          <div class="docPreviewCarousel mb-4" v-if="$vuetify.breakpoint.smAndDown && getTutorList.length">
            <h3 class="subtitle-1 mb-4 text-center" v-language:inner="'resultTutor_title'"/>
            <sbCarousel class="carouselDocPreview" @select="enterTutorCard" 
                        :arrows="false"
                        :gap="20">
              <tutorCardCarousel :fromCarousel="true" v-for="(tutor, index) in getTutorList" :tutor="tutor" :key="index"/>
            </sbCarousel>
          </div>
      </template>
      
      <div v-else>
        <div class="text-center" v-for="(page, index) in docPreview" :key="index">
          <v-lazy-image 
            v-if="page"
            :style="`height:${dynamicWidthAndHeight.height}px; width:${dynamicWidthAndHeight.width}px`"
            class="document-wrap-content mb-4"
            :src="page"
            :src-placeholder="isObserver(page)"
            :alt="document.content"
          />
          <div class="docPreviewCarousel mb-4" v-if="$vuetify.breakpoint.smAndDown && getTutorList.length && index === 0">
            <h3 class="subtitle-1 mb-4" v-language:inner="'resultTutor_title'"/>
            <sbCarousel class="carouselDocPreview" @select="enterTutorCard" 
                        :arrows="false"
                        :gap="20">
              <tutorCardCarousel :fromCarousel="true" v-for="(tutor, index) in getTutorList" :tutor="tutor" :key="index"/>
            </sbCarousel>
          </div>
        </div>
      </div>
    <transition-group name="slide-x-transition">
      <div :key="'12'"
        :class="[{'unlockBoxAfterVideo':showAfterVideo && isPrice },'unlockBox','headline']"
        v-if="isShowPurchased || !accountUser"
      >
        <div class="inner">
          <p class="text-center hidden-sm-and-down" v-language:inner="unlockDocDictionary"/>
          <div class="aside-top-btn align-center" v-if="!isLoading && accountUser" @click="accountUser? updatePurchaseConfirmation(true) :updateLoginDialogState(true)">
            <span class="font-weight-bold text-center disabled" v-if="isPrice" >{{docPrice | currencyLocalyFilter}}</span>
            <span class="white--text pa-3 font-weight-bold text-center" v-language:inner="'documentPage_unlock_btn'"/>
          </div>
          <div class="aside-top-btn-not align-center" v-if="!isLoading && !accountUser" @click="updateLoginDialogState(true)">
            <span class="white--text pa-3 font-weight-bold text-center" v-language:inner="'documentPage_unlock_btn'"/>
          </div>
          <v-progress-circular
            class="unlock_progress"
            v-if="isLoading"
            indeterminate
            color="#4452fc"
          ></v-progress-circular>
        </div>
      </div>
      </transition-group>
      <a
        class="btn-download justify-center elevation-5"
        :href="`${$route.path}/download`"
        target="_blank"
        @click="downloadDoc"
        :class="{'mt-2': !isShowPurchased, 'v-hidden': isShowVideo && !isVideo}"
        v-if="!isShowPurchased && !isLoading && accountUser"
      >
        <v-icon color="#fff" class="pr-3">sbf-download-cloud</v-icon>
        <span
          class="white--text py-4 font-weight-bold"
          v-language:inner="'documentPage_download_btn'"
        ></span>
      </a>
      </div>
  </div>
</template>
<script>
import { mapActions, mapGetters, mapMutations } from "vuex";
import { LanguageService } from "../../../services/language/languageService";
import sbDialog from "../../wrappers/sb-dialog/sb-dialog.vue";
import reportItem from "../../results/helpers/reportItem/reportItem.vue";
import utillitiesService from "../../../services/utilities/utilitiesService";
import documentService from "../../../services/documentService";
import sbVideoPlayer from '../../sbVideoPlayer/sbVideoPlayer.vue';
import { VList } from 'vuetify/lib';

import sbCarousel from '../../sbCarousel/sbCarousel.vue';
import tutorCardCarousel from '../../carouselCards/tutorCard.vue';

export default {
  name: "mainDocument",
  components: {
    sbCarousel,
    tutorCardCarousel,
    reportItem,
    sbDialog,
    sbVideoPlayer,
    VList
  },
  props: {
    document: {
      type: Object
    }
  },
  data() {
    return {
      showAfterVideo: false,
      isMounted: false,
      showMenu: false,
      currentCurrency: LanguageService.getValueByKey("app_currency_dynamic"),
      itemId: 0,
      priceDialog: false,
      showReport: false,
      isRtl: global.isRtl,
      newPrice: null,
      docWrap: null,
      docWrapContent: null,
      actions: [
        {
          title: LanguageService.getValueByKey("questionCard_Report"),
          action: this.reportItem,
          isDisabled: this.isDisabled,
          isVisible: this.isVisible,
          visible: true
        },
        {
          title: LanguageService.getValueByKey("resultNote_change_price"),
          action: this.showPriceChangeDialog,
          isDisabled: this.isOwner,
          isVisible: this.isVisible,
          icon: "sbf-delete",
          visible: true
        },
        {
          title: LanguageService.getValueByKey("resultNote_action_delete_doc"),
          action: this.deleteDocument,
          isDisabled: this.isOwner,
          isVisible: this.isVisible,
          visible: true
        }
      ]
    };
  },
  computed: {
    ...mapGetters([
      "getBtnLoading",
      "accountUser",
      "getPurchaseConfirmation",
      "getRouteStack",
      "getDocumentLoaded",
      "getTutorList"
    ]),
    videoSrc(){
      if(this.document && this.document.preview && this.document.preview.locator){
        return this.document.preview.locator
      }
    },
    isShowVideo() {
      if(this.document && this.document.documentType && this.isMounted) {
        return true;
      }
      return false;
    },
    isVideo(){      
        return this.document.documentType === 'Video' 
    },
    videoHeight(){
       return Math.floor((this.videoWidth/16)*9); 
    },
    showPurchaseConfirmation() {
      return this.getPurchaseConfirmation;
    },
    uploaderName() {
      if (this.document.details && this.document.details.user.name) {
        return this.document.details.user.name;
      }
    },
    courseType() {
      if (this.document.details && this.document.details.name) {
        return this.document.details.course;
      }
    },
    courseName() {
      if (this.document.details && this.document.details.name) {
        return this.document.details.name;
      }
    },
    documentDate() {
      if (this.document.details && this.document.details.date) {
        let lang = `${global.lang}-${global.country}`;
        return new Date(this.document.details.date).toLocaleString(lang, {
          year: "numeric",
          month: "short",
          day: "numeric"
        });
      }
    },
    isPurchased() {
      if (!this.document.details) return true;
      if (this.document.details && this.document.details.isPurchased) {
        return this.document.details.isPurchased;
      }
    },
    docViews() {
      if (this.document.details && this.document.details.views) {
        return this.document.details.views;
      }
    },
    docPrice() {
      if (this.document.details && this.document.details.price >= 0) {
        return this.document.details.price.toFixed(2);
      }
    },
    videoWidth(){
      return this.calculateWidthByScreenSize()
    },
    docPreview() {
      if(this.isVideo)return
      // TODO temporary calculated width container
      if (this.document.preview && this.docWrap) {
        if(this.document.preview[0].indexOf("base64") > -1){
          return this.document.preview;
        }
        let result = this.document.preview.map(preview => {
          return utillitiesService.proccessImageURL(
            preview,
            this.dynamicWidthAndHeight.width,
            this.dynamicWidthAndHeight.height,
            "pad"
          );
        });
        return result;
      }
    },
    isSmAndDown() {
      return this.$vuetify.breakpoint.smAndDown;
    },
    documentPrice: {
      get() {
        if (this.newPrice !== null) {
          return this.newPrice;
        } else {
          return this.document.details ? this.document.details.price : 0;
        }
      },
      set(val) {
        this.newPrice = val;
      }
    },
    isLoading() {
      return this.getBtnLoading;
    },
    isPrice() {
      if (this.document.details && this.document.details.price > 0) {
        return true;
      } else {
        return false;
      }
    },
    isShowPurchased() {
      if (!this.isPurchased && this.isPrice > 0) {
        return true;
      }
      return false;
    },
    unlockDocDictionary() {
      if(!this.accountUser) {
        return 'documentPage_unlock_document_unregister'
      } else {
        return this.isVideo ? 'documentPage_unlock_video' : 'documentPage_unlock_document';
      } 
    }, 
    videoLoader() {
      if(this.getDocumentLoaded){
        if(this.isVideo){
          return !!(this.document && this.document.preview && this.document.preview.locator);
        }else{
          return true;
        }
      }else{
        return false;
      }
    },
    dynamicWidthAndHeight(){
      return {
        width: this.calculateWidthByScreenSize(),
        height: Math.ceil(this.calculateWidthByScreenSize() / 0.707)
      }
    }
  },
  methods: {
    ...mapActions([
      "updatePurchaseConfirmation",
      "purchaseDocument",
      "updateToasterParams",
      "setNewDocumentPrice",
      "updateLoginDialogState",
      "downloadDocument",
      "getTutorListCourse"
    ]),
    ...mapMutations(["UPDATE_SEARCH_LOADING"]),
        enterTutorCard(vueElm){
      if(vueElm.enterProfilePage){
        vueElm.enterProfilePage();
      }else{
        vueElm.$parent.enterProfilePage();
      }
    },
    calculateWidthByScreenSize(){
      let width = 0;
      if (this.$vuetify.breakpoint.xl) {
          width = 960;
        }
        if (this.$vuetify.breakpoint.lg) {
          width = 880;
        }
        if (this.$vuetify.breakpoint.md) {
          width = 560;
        }
        if (this.$vuetify.breakpoint.sm) {
          width = 730;
        }
        if (this.$vuetify.breakpoint.xs) {
          width = 400;
        }
        if (this.$vuetify.breakpoint.width === 375) {
          width = 375;
        }
        return width;
    },
    unlockDocument() {
      let item = {
        id: this.document.details.id,
        price: this.document.details.price
      };
      if (!this.isLoading) {
        this.purchaseDocument(item);
        this.updatePurchaseConfirmation(false);
      }
    },
    closeDocument() {
      let regRoute = 'registration';
      let routeStackLength = this.getRouteStack.length;
      let beforeLastRoute = this.getRouteStack[routeStackLength-2];
      
      if (routeStackLength > 1 && beforeLastRoute && beforeLastRoute !== regRoute) {
        this.$router.back();
      } else {
        this.$router.push({ path: "/feed" });
      }
    },
    showReportOptions() {
      this.showMenu = true;
    },
    cardOwner() {
      let userAccount = this.accountUser;
      if (userAccount && this.document.details && this.document.details.user) {
        return userAccount.id === this.document.details.user.userId;
      } else {
        return false;
      }
    },
    isVisible(val) {
      return val;
    },
    isDisabled() {
      let isOwner, account;
      isOwner = this.cardOwner();
      account = this.accountUser;

      if (isOwner || !account) {
        return true;
      }
    },
    isOwner() {
      let owner = this.cardOwner();
      return !owner;
    },
    reportItem() {
      this.itemId = this.document.details.id;
      this.showReport = !this.showReport;
    },
    showPriceChangeDialog() {
      this.priceDialog = true;
    },
    closeNewPriceDialog() {
      this.newPrice = null;
      this.priceDialog = false;
    },
    closeReportDialog() {
      this.showReport = false;
    },
    deleteDocument() {
      let id = this.document.details.id;

      documentService.deleteDoc(id).then(
        success => {
          this.updateToasterParams({
            toasterText: LanguageService.getValueByKey(
              "resultNote_deleted_success"
            ),
            showToaster: true
          });
          this.closeDocument();
        },
        error => {
          this.updateToasterParams({
            toasterText: error.response.data.error[0],
            showToaster: true,
            toasterType: "error-toaster"
          });
        }
      );
    },
    submitNewPrice() {
      let data = { id: this.document.details.id, price: this.newPrice };
      let self = this;
      documentService.changeDocumentPrice(data).then(
        success => {
          this.setNewDocumentPrice(self.newPrice);
          self.newPrice = null;
          self.closeNewPriceDialog();
        },
        error => {
          console.error("erros change price", error);
        }
      );
    },
    downloadDoc(e) {
      if (!this.accountUser) {
        e.preventDefault();
      }
      let item = {
        course: this.document.details.course,
        id: this.document.details.id
      };
      this.downloadDocument(item);
    },
    isObserver(page) {
      if("IntersectionObserver" in window) {
        return require('./doc-preview-animation.gif');
      } else {
        return page;
      }
    }
  },
  mounted() {
    this.docWrap = document.querySelector(".document-wrap");
    this.isMounted = true;
  },
  created() {
    if(this.$vuetify.breakpoint.smAndDown) {
        this.getTutorListCourse(this.courseType);
    }
  },
};
</script>
<style lang="less">
@import "../../../styles/mixin.less";
@import "../../dialogs/changePrice/changePrice.less";

.mainDocument-container {
  margin-bottom: 80px;
  // flex: 5;
  @media (max-width: @screen-sm) {
    order: 2;
  }
  .courseName {
        font-size: 18px;
        color: @global-purple;
        line-height: initial !important;
        @media (max-width: 1450px) {
        font-size: 16px;
        }
    }
  .mainDocument-header {
    justify-content: center;
  }
  .main-header-wrapper {
      display: flex;
      width: 100%;
      align-items: center;
      max-width: 960px;
      @media (max-width: 1450px) {
        max-width: 880px;
      }
      @media (max-width: @screen-md) {
        max-width: 560px;
      }
      @media (max-width: @screen-sm) {
        max-width: 939px;
      }
      .menu-area {
        margin-right: -10px;
      }
      .doc-views {
        margin-bottom: 2px;
        font-size: 13px !important;
      }
      
      .arrow-back {
        font-size: 24px;
        margin-top: 3px;
      }
      .arrow-back-rtl {
        transform: scaleX(-1);
      }
      .grey-text {
        color: #a0a0a0;
      }
      .verticalMenu {
        font-size: 16px;
        color: #aaa;
        @media (max-width: @screen-sm) {
          font-size: 16px;
        }
      }
      .date,
      .views {
        // display: flex;
        font-size: 14px;
      }
    }
  .document-wrap {
    // position: relative;
    .unlockBoxAfterVideo{
      bottom: 500px !important;
    }
    .video_placeholder {
      width: 100%;
    }
    .unlockBox {
      position: fixed;
      left: 0;
      right: 330px;
      bottom: 30px;
      margin: auto;
      text-align: center;
      z-index: 12;
      @media (max-width: @screen-xs) {
        right: 0;
        bottom: 0;
      }
      .inner {
        min-width: 450px;
        padding: 20px;
        box-shadow: 0 3px 55px -1px rgba(0, 0, 0, 0.2),
          0 5px 8px 0 rgba(0, 0, 0, 0.14), 0 1px 14px 0 rgba(0, 0, 0, 0.12) !important;
        border-radius: 4px;
        background: #fff;
        display: inline-block;
        @media (max-width: @screen-xs) {
          width: 100%;
          min-width: auto;
          padding: 0;
        }
      }
      p {
        padding: 0 0 30px 0;
        margin: 0;
        font-size: 19px;
      }
      .aside-top-btn-not {
        cursor: pointer;
        display: flex;
        border: 1px solid #ccc;
        border-radius: 4px;
        margin: 0 auto;
        width: 60%;
        line-height: 20px;
        font-size: 15px;
        @media (max-width: @screen-sm) {
          width: auto;
        }
        span {
          width: 100%;
          background-color: #4452fc;
          border-radius: 0 4px 4px 0;
        }
      }
      .aside-top-btn {
        display: flex;
        border: 1px solid #ccc;
        border-radius: 4px;
        margin: 0 auto;
        width: 60%;
        line-height: 20px;
        font-size: 15px;
        cursor: pointer;
        @media (max-width: @screen-sm) {
          width: auto;
          line-height: 32px;
        }
        span:first-child {
          flex-grow: 1;
        }
        span:nth-child(2) {
          .flexSameSize();
                        background-color: @global-blue;
          border-radius: 0 4px 4px 0;
        }
      }
    }
    .docPreviewCarousel{
      width: 100%;
      .carouselDocPreview{
        text-align: initial;
      }
    }
    .document-wrap-content {
      @media (max-width: @screen-sm) {
        width: 100%;
        height: unset !important;
        max-width: 100%;
      }
    }
    .unlock_progress {
      display: flex;
      margin: 0 auto;
    }
    .btn-download {
      visibility: hidden;
      position: fixed;
      bottom: 30px;
      left: 50%;
      padding: 14px 72px;
      border-radius: 5.5px;
                background-color: @global-blue;
      z-index: 12;
      margin-left: -340px;
      i {
        font-size: 32px;
      }
      span {
        font-size: 26px;
      }
      &.v-hidden{
        visibility: visible;
      }
      @media (max-width: @screen-sm) {
        bottom: 0;
        left: 0;
        right: 0;
        padding: 20px;
        text-align: center;
        border-radius: 0;
        margin: 0;
      }
    }
  }
}

.confirmation-purchase-dialog {
  max-width: 544px !important;
  @media (max-width: @screen-xs) {
    max-width: 338px !important;
  }
  .confirm-purchase-card {
    justify-content: center;
    align-items: flex-start;
    border-radius: 4px;
    box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.33);
    overflow: hidden;
    @media (max-width: @screen-xs) {
      align-items: center;
    }
    .confirm-headline {
      font-size: 18px;
      line-height: 1.56;
      color: @color-blue-new;
      display: flex;
      flex-grow: 1;
      align-items: center;
      justify-content: center;
      width: 100%;
      padding: 32px 16px 24px 16px;
      @media (max-width: @screen-xs) {
        text-align: center;
        padding-top: 48px;
        padding-bottom: 32px;
      }
    }

    .card-actions {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      width: 100%;
      background: #f7f7f7;
      padding: 24px 16px 16px 16px;
      @media (max-width: @screen-xs) {
        padding: 32px 16px 16px 16px;
      }
      .doc-details {
        display: flex;
        // flex-direction: column;
        justify-content: center;
        align-items: center;

        .doc-type {
          display: flex;
          flex-direction: row;
          align-items: center;
          padding: 0 0 12px;
          .doc-type-icon {
            margin-right: 8px;
            color: @color-blue-new;
            font-size: 24px;
          }
          .doc-type-text {
            color: @color-blue-new;
            font-size: 13px;
          }
        }
        .doc-title {
          color: @textColor;
          text-align: center;
          font-size: 16px;
          font-weight: 600;
          // max-width: 400px;
          div {
            max-width: 200px;
          }
        }
      }
      .purchase-actions {
        display: flex;
        flex-direction: row;
        padding-top: 28px;
        @media (max-width: @screen-xs) {
          padding-top: 48px;
        }
        button {
          height: unset;
          background-color: transparent;
          font-size: 16px;
          text-transform: capitalize;
          &.submit-purchase {
            .sb-rounded-medium-btn();
            font-size: 16px;
          }
          &.cancel {
            font-size: 14px;
            color: fade(@color-black, 72%);
          }
        }
      }
    }
  }
}
</style>