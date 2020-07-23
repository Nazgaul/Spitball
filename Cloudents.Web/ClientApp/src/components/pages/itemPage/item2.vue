<template>
  <div class="itemPage">
    <div class="itemPage__main">
      <div class="itemPage__main__document">
        <mainItem :document="document"></mainItem>
        <v-card class="itemActions pt-sm-11 pt-4 px-4 elevation-0">
          <div class="docWrapper d-block d-sm-flex justify-sm-center text-center pb-4">
            <template v-if="getDocumentPrice && !getIsPurchased">
              <div class="d-flex align-end me-4 justify-center mb-2 mb-sm-0">
                <template v-if="isFree || getDocumentPriceTypeHasPrice">
                  <div class="me-1 price">{{priceWithComma}}</div>
                  <span class="points" v-t="'documentPage_points'"></span>
                </template>
              </div>
            </template>

            <v-btn v-if="!getIsPurchased" class="itemPage__side__btn white--text"
              :loading="getBtnLoading" @click="openPurchaseDialog"
              height="42" color="#4c59ff" depressed rounded large>
              <span v-if="isVideo">{{unlockVideoBtnText}}</span>
              <span v-else>{{unlockDocumentBtnText}}</span>
            </v-btn>

            <v-btn v-if="!isVideo && getIsPurchased"
              class="itemPage__side__btn white--text"
              tag="a" :href="downloadUrl" target="_blank"
              :loading="getBtnLoading" @click="downloadDoc"
              color="#4c59ff" rounded depressed height="42" large>
              <span v-t="'documentPage_download_btn'"></span>
            </v-btn>

          </div>
        </v-card>
      </div>
    </div>

    <unlockDialog :document="document"></unlockDialog>

    <v-snackbar v-model="snackbar" :top="true" :timeout="8000">
      <div class="d-flex justify-space-between align-center" >
        <span v-t="'resultNote_unsufficient_fund'"></span>
        <v-btn class="px-4" outlined rounded @click="openBuyTokenDialog">
          <span v-t="'dashboardPage_my_sales_action_need_btn'"></span>
        </v-btn>
      </div>
    </v-snackbar>

  </div>
</template>

<script>
import { mapActions, mapGetters } from "vuex";


//services
import * as dialogNames from "../global/dialogInjection/dialogNames.js";
import * as routeNames from "../../../routes/routeNames";

//store
import storeService from "../../../services/store/storeService";
import studyDocumentsStore from "../../../store/studyDocuments_store";

// components
import mainItem from "./components/mainItem/mainItem.vue";

import unlockDialog from "./components/dialog/unlockDialog.vue";
export default {
  name: "itemPage",
  components: {
    mainItem,
    unlockDialog,
  },
  props: {
    id: {
      // type: String
    }
  },
  computed: {
    ...mapGetters([
      "accountUser",
      "getDocumentDetails",
      // "getDocumentName",
      "getDocumentPrice",
      "getIsPurchased",
      // "getRelatedDocuments",
      "getShowItemToaster",
      "getBtnLoading",
      "getDocumentPriceTypeFree",
      "getDocumentPriceTypeHasPrice",
      "getDocumentPriceTypeSubscriber"
    ]),
    isFree() {
      return this.getDocumentPriceTypeFree;
    },
    unlockDocumentBtnText() {
      if(this.isFree || this.getDocumentPriceTypeHasPrice) {
        return this.$t('documentPage_unlock_document_btn')
      }
      return this.$t('documentPage_unlock_document_btn_subscribe', [this.$price(this.getDocumentPrice, 'USD')])
    },
    unlockVideoBtnText() {
      if(this.isFree || this.getDocumentPriceTypeHasPrice) {
        return this.$t('documentPage_unlock_video_btn')
      }
      return this.$t('documentPage_unlock_video_btn_subscribe', [this.$price(this.getDocumentPrice, 'USD')])
    },
    snackbar: {
      get() {
        return this.getShowItemToaster;
      },
      set(val) {
        this.updateItemToaster(val);
      }
    },
    document() {
      if (this.getDocumentDetails) {
        return this.getDocumentDetails;
      }
      return {};
    },
    doucmentDetails() {
      //TODO why explaind why not use document()
      if (this.getDocumentDetails && this.getDocumentDetails.details) {
        return this.getDocumentDetails.details;
      }
      return {};
    },
    isVideo() {
      return this.document.documentType === "Video";
    },
    priceWithComma() {
      if (this.document && this.document.details) {
        return this.document.details.price.toLocaleString();
      }
      return null;
    },
    downloadUrl(){
      return `/document/${this.id}/download`;
    }
  },
  methods: {
    ...mapActions([
      "documentRequest",
      "clearDocument",
      // "getStudyDocuments",
      "updateItemToaster",
      "updatePurchaseConfirmation",
      "downloadDocument"
    ]),

    openBuyTokenDialog() {
      this.updateItemToaster(false);
      this.$openDialog(dialogNames.BuyPoints);
    },
    openPurchaseDialog() {
      if (this.getDocumentPriceTypeSubscriber) {
        this.$router.push({
          name: routeNames.Profile,
          params: {
            id: this.doucmentDetails.tutor.userId,
            name: this.doucmentDetails.tutor.name
          },
          hash: "#subscription"
        });
        return;
      }
      if (this.accountUser) {
        this.updatePurchaseConfirmation(true);
      } else {
        this.$store.commit("setComponent", "register");
      }
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

  beforeDestroy() {
    this.clearDocument();
  },
  mounted() {
    this.documentRequest(this.id).catch(()=>{
      this.$store.dispatch('updateCurrentItem');
    });
  },
  created() {
    storeService.lazyRegisterModule(this.$store,"studyDocumentsStore",studyDocumentsStore);
  }
};
</script>

<style lang="less">
@import "../../../styles/mixin";

.itemPage {
  //hacks to finish this fast
  .price-area,
  .content-wrap,
  hr,
  .spacer {
    display: none !important;
  }
  .bottom-row,
  .data-row {
    margin-right: 30% !important;
    @media (max-width: @screen-xs) {
      margin-right: auto !important;
      justify-content: space-between;
    }
  }
  .azuremediaplayer {
    background: #fff !important;
  }
  position: relative;
  margin: 0 auto;
  max-width: 960px;
  @media (max-width: @screen-md) {
    margin: 20px;
  }
  @media (max-width: @screen-xs) {
    margin: 0;
    display: block;
  }
  .sticky-item {
    position: sticky;
    height: fit-content;
    top: 80px;
    &.sticky-item_bannerActive {
      top: 150px;
    }
  }
  &__main {
    @media (max-width: @screen-sm) {
      margin-right: 0;
      max-width: auto;
    }
    &__document {
      width: 100%;
      margin: 0 auto 16px;

      @media (max-width: @screen-sm) {
        width: auto;
        margin: 0 auto 8px;
      }
      .itemActions {
        color: @global-purple;
        @media (max-width: @screen-xs) {
          border-top: 1px solid #ddd;
        }
        .docWrapper {
          font-weight: 600;
          .price {
            .responsive-property(font-size, 30px, null, 18px);
          }
          .points {
            .responsive-property(font-size, 14px, null, 18px);
          }
        }
      }
      &__tutor {
        display: flex;
        align-items: center;
        flex-wrap: wrap;
        font-weight: 600;
        font-size: 14px;
        &__link {
          @media (max-width: @screen-md) {
            margin-bottom: 6px;
          }
          &--title1 {
            display: inline-block;
            color: #5560ff;
            cursor: pointer;
            @media (max-width: @screen-xs) {
              white-space: nowrap;
              display: block;
            }
          }
          &--title2 {
            color: #4d4b69;
            display: inline-block;
            cursor: text;
            @media (max-width: @screen-xs) {
              white-space: nowrap;
              display: block;
            }
          }
          @media (max-width: @screen-xs) {
            flex-direction: column;
            justify-content: center;
            margin-bottom: 10px;
          }
        }
        &--btn {
          border: solid 1px #4452fc;
          border-radius: 28px;
          background: #fff !important; //vuetify
          @media (max-width: @screen-xs) {
            padding: 0 10px;
          }
          div {
            color: #4452fc;
            font-size: 13px;
            font-weight: 600;
            text-transform: initial;
            margin-bottom: 1px;
          }
        }
      }
      &--loader {
        display: flex;
        align-items: center;
        justify-content: center;
        min-height: 160px;
      }
    }
  }
}
</style>