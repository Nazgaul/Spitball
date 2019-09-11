<template>
  <div class="mainDocument-container">
    <v-layout row class="mainDocument-header" :class="[isSmAndDown ? 'pt-3' : 'pb-2']" align-center>
      <div class="main-header-wrapper">
        <v-icon
          class="grey--text"
          :class="['arrow-back','hidden-sm-and-down',isRtl? 'arrow-back-rtl': '']"
          @click="closeDocument"
        >sbf-arrow-back-chat</v-icon>
        <h2
          class="courseName font-weight-bold text-truncate"
          :class="[isSmAndDown ? 'pr-5' : 'pl-3']"
        >{{courseName}}</h2>
        <v-spacer></v-spacer>
        <span class="grey-text views" :class="[isSmAndDown ? 'pr-3' : 'pr-5']">
          {{docViews}}
          <v-icon class="pl-2 doc-views" small>sbf-views</v-icon>
        </span>
        <span class="grey-text date" :class="{'pl-3': isSmAndDown}">{{documentDate}}</span>

        <v-menu
          class="menu-area"
          lazy
          bottom
          left
          content-class="card-user-actions"
          v-model="showMenu"
        >
          <v-btn
            :depressed="true"
            @click.native.stop.prevent="showReportOptions()"
            slot="activator"
            class="mr-0"
            icon
          >
            <v-icon class="verticalMenu">sbf-3-dot</v-icon>
          </v-btn>
          <v-list>
            <v-list-tile
              v-show="item.isVisible(item.visible)"
              :disabled="item.isDisabled()"
              v-for="(item, i) in actions"
              :key="i"
            >
              <v-list-tile-title @click="item.action()">{{ item.title }}</v-list-tile-title>
            </v-list-tile>
          </v-list>
        </v-menu>
        <sb-dialog
          :showDialog="showReport"
          :maxWidth="'438px'"
          :popUpType="'reportDialog'"
          :content-class="`reportDialog ${isRtl? 'rtl': ''}` "
        >
          <report-item :closeReport="closeReportDialog" :itemType="itemType" :itemId="itemId"></report-item>
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
                <span class="change-title" v-language:inner="'resultNote_change_for'"></span>
                <span
                  class="change-title"
                  style="max-width: 150px;"
                  v-line-clamp="1"
                >&nbsp;"{{courseName}}"</span>
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
                <div class="doc-type">
                  <v-icon class="doc-type-icon">sbf-document-note</v-icon>
                  <span class="doc-type-text">{{itemType}}</span>
                </div>
                <div class="doc-title">
                  <div class="text-truncate">{{courseName}}</div>
                </div>
              </div>
              <div class="purchase-actions">
                <v-btn flat class="cancel" @click.native="updatePurchaseConfirmation(false)">
                  <span v-language:inner>preview_cancel</span>
                </v-btn>
                <v-btn round class="submit-purchase" @click.native="unlockDocument">
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
      <div class="text-xs-center" v-for="(page, index) in docPreview" :key="index">
        <v-lazy-image
          :style="`height:${imgHeight}px; width:${imgWidth}px`"
          class="document-wrap-content mb-4"
          :src="page"
          :src-placeholder="require('./doc-preview-animation.gif')"
          v-if="page"
          :alt="document.content"
        />

        <tutor-result-card-carousel v-if="(index === 0 && $vuetify.breakpoint.smAndDown)" :courseName="courseType" />
      </div>
      <div
        class="unlockBox headline hidden-sm-and-down"
        v-if="isShowPurchased || !accountUser"
        @click="accountUser? updatePurchaseConfirmation(true) :updateLoginDialogState(true)"
      >
        <div class="inner">
          <p
            class="text-xs-center"
            v-language:inner="!accountUser? 'documentPage_unlock_document_unregister' :'documentPage_unlock_document'"
          ></p>
          <div class="aside-top-btn align-center" v-if="!isLoading && accountUser">
            <span
              class="font-weight-bold text-xs-center disabled"
              v-if="isPrice"
            >{{docPrice | currencyLocalyFilter}}</span>
            <span
              class="white--text pa-3 font-weight-bold text-xs-center"
              v-language:inner="'documentPage_unlock_btn'"
            ></span>
          </div>
          <div class="aside-top-btn-not align-center" v-if="!isLoading && !accountUser">
            <span
              class="white--text pa-3 font-weight-bold text-xs-center"
              v-language:inner="'documentPage_unlock_btn'"
            ></span>
          </div>
          <v-progress-circular
            class="unlock_progress"
            v-if="isLoading"
            indeterminate
            color="#4452fc"
          ></v-progress-circular>
        </div>
      </div>
      <a
        class="btn-download justify-center elevation-5"
        :href="`${$route.path}/download`"
        target="_blank"
        @click="downloadDoc"
        :class="{'mt-2': !isShowPurchased}"
        v-if="!isShowPurchased && !isLoading && !isSmAndDown && accountUser"
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
import tutorResultCardCarousel from "../../../components/results/tutorCards/tutorResultCardCarousel/tutorResultCardCarousel.vue";

export default {
  name: "mainDocument",
  components: {
    reportItem,
    sbDialog,
    tutorResultCardCarousel
  },
  props: {
    document: {
      type: Object
    }
  },
  data() {
    return {
      imgHeight: 0,
      imgWidth: 0,
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
      "getRouteStack"
    ]),
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
    docPreview() {
      // TODO temporary calculated width container
      if (this.document.preview && this.docWrap) {
        if (this.$vuetify.breakpoint.xl) {
          this.imgWidth = 960;
        }
        if (this.$vuetify.breakpoint.lg) {
          this.imgWidth = 880;
        }
        if (this.$vuetify.breakpoint.md) {
          this.imgWidth = 560;
        }
        if (this.$vuetify.breakpoint.sm) {
          this.imgWidth = 730;
        }
        if (this.$vuetify.breakpoint.xs) {
          this.imgWidth = 400;
        }
        if (this.$vuetify.breakpoint.width === 375) {
          this.imgWidth = 375;
        }
        this.imgHeight = this.imgWidth / 0.707;
        let result = this.document.preview.map(preview => {
          return utillitiesService.proccessImageURL(
            preview,
            this.imgWidth,
            Math.ceil(this.imgHeight),
            "pad"
          );
        });
        return result;
      }
    },
    isSmAndDown() {
      return this.$vuetify.breakpoint.smAndDown;
    },
    itemType() {
      if (this.document) {
        // return this.document.details.template
      }
      return "note";
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
    }
  },
  methods: {
    ...mapActions([
      "updatePurchaseConfirmation",
      "purchaseDocument",
      "updateToasterParams",
      "setNewDocumentPrice",
      "updateLoginDialogState",
      "downloadDocument"
    ]),
    ...mapMutations(["UPDATE_SEARCH_LOADING"]),
    
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
      this.UPDATE_SEARCH_LOADING(true);
      let routeStackLength = this.getRouteStack.length;
      if (routeStackLength > 1) {
        this.$router.back();
      } else {
        this.$router.push({ path: "/note" });
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
    }
  },
  beforeDestroy() {},
  mounted() {
    this.docWrap = document.querySelector(".document-wrap");
  }
};
</script>
<style lang="less">
@import "../../../styles/mixin.less";

.mainDocument-container {
  margin-bottom: 80px;
  flex: 5;
  @media (max-width: @screen-sm) {
    order: 2;
  }
  .mainDocument-header {
    justify-content: center;
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
      .courseName {
        font-size: 18px;
                    color: @global-purple;
        line-height: initial !important;
        max-width: 0;
        min-width: 60%;
        @media (max-width: @screen-xs) {
          max-width: 200px;
          min-width: unset;
        }
        @media (max-width: @screen-xss) {
          max-width: 160px;
          min-width: unset;
        }
        @media (max-width: 320px) {
          max-width: 110px;
          min-width: unset;
        }
      }
      .arrow-back {
        font-size: 24px;
        margin-top: 3px;
      }
      .arrow-back-rtl {
        transform: scaleX(-1);
      }
      .grey-text {
        opacity: 0.6;
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
  }
  .document-wrap {
    // position: relative;
    .unlockBox {
      position: fixed;
      left: 0;
      right: 330px;
      bottom: 30px;
      margin: auto;
      text-align: center;
      z-index: 5;
      .inner {
          
        min-width: 450px;
        padding: 20px;
        box-shadow: 0 3px 55px -1px rgba(0, 0, 0, 0.2),
          0 5px 8px 0 rgba(0, 0, 0, 0.14), 0 1px 14px 0 rgba(0, 0, 0, 0.12) !important;
        cursor: pointer;
        border-radius: 4px;
        background: #fff;
        display: inline-block;
      }

      //width: 450px;

      p {
        padding: 0 0 30px 0;
        margin: 0;
        font-size: 19px;
      }
      .aside-top-btn-not {
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
        @media (max-width: @screen-sm) {
          width: auto;
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
      position: fixed;
      bottom: 30px;
      left: 50%;
      padding: 14px 72px;
      border-radius: 5.5px;
                background-color: @global-blue;
      z-index: 9;
      margin-left: -340px;
      i {
        font-size: 32px;
      }
      span {
        font-size: 26px;
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
      font-family: @fontFiraSans;
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
        flex-direction: column;
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
            font-family: @fontFiraSans;
          }
        }
        .doc-title {
          font-family: @fontFiraSans;
          color: @textColor;
          text-align: center;
          font-size: 16px;
          font-weight: 600;
          max-width: 400px;
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
            font-family: @fontOpenSans;
            color: fade(@color-black, 72%);
          }
        }
      }
    }
  }
}
</style>