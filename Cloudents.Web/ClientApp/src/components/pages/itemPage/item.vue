<template>
  <div class="itemPage mt-sm-6 mt-2">
    <div class="itemPage__main mb-2 mb-sm-6">
      <div class="d-flex pa-2 pa-sm-0 justify-sm-space-between documentTitle">
        <v-icon
          size="16"
          class="hidden-md-and-up me-4 document-header-large-sagment--arrow"
          @click="closeDocument"
          v-html="'sbf-arrow-left-carousel'"
        ></v-icon>
        <h1 class="ps-sm-4 text-center text-sm-left text-truncate">{{getDocumentName}}</h1>
        <shareContent
          v-if="getDocumentDetails && !isMobile"
          :link="shareContentParams.link"
          :twitter="shareContentParams.twitter"
          :whatsApp="shareContentParams.whatsApp"
          :email="shareContentParams.email"
        />
      </div>

      <div class="itemPage__main__document">
        <mainItem :isLoad="isLoad" :document="document"></mainItem>

        <v-card class="itemActions pt-sm-11 pt-4 px-4 elevation-0">
          <div class="docWrapper d-block d-sm-flex justify-sm-center text-center pb-4">
            <template v-if="getDocumentPrice && !getIsPurchased">
              <div class="d-flex align-end me-4 justify-center mb-2 mb-sm-0">
                <template v-if="isFree || getDocumentPriceTypeHasPrice">
                  <div class="me-1 price">{{priceWithComma}}</div>
                  <span class="points" v-t="'documentPage_points'"></span>
                </template>
                <!-- <div class="me-1 price" v-else>{{$n(priceWithComma, 'currency', 'en')}}</div> -->
              </div>
              <!-- <div v-t="'documentPage_credit_uploader'"></div> -->
            </template>
            <v-btn
              class="itemPage__side__btn white--text"
              depressed
              rounded
              large
              :loading="getBtnLoading"
              @click="openPurchaseDialog"
              v-if="!getIsPurchased"
              height="42"
              color="#4c59ff"
            >
              <span v-if="isVideo">{{unlockVideoBtnText}}</span>
              <span v-else>{{unlockDocumentBtnText}}</span>
            </v-btn>
            <v-btn
              v-if="!isVideo && getIsPurchased"
              large
              tag="a"
              :href="`${$route.path}/download`"
              target="_blank"
              :loading="getBtnLoading"
              class="itemPage__side__btn white--text"
              height="42"
              depressed
              rounded
              @click="downloadDoc"
              color="#4c59ff"
            >
              <span v-t="'documentPage_download_btn'"></span>
            </v-btn>
          </div>
        </v-card>

        <resultNote
          v-if="doucmentDetails.feedItem"
          class="itemPage__main__document__doc"
          :item="doucmentDetails.feedItem"
          :fromItemPage="true"
        >
          <!-- <template #arrowBack> -->
          <v-icon
            class="hidden-md-and-up document-header-large-sagment--arrow"
            @click="closeDocument"
            v-html="'sbf-arrow-left-carousel'"
          ></v-icon>
          <!-- </template> -->
          <template #descriptionTitle v-if="doucmentDetails.snippet">
            <div class="mt-5 descriptionTitle" v-t="'documentPage_description'"></div>
          </template>
        </resultNote>

        <template v-else>
          <v-sheet color="#fff" class="pb-2 skeletonWarp">
            <v-skeleton-loader max-width="250" type="list-item-avatar-two-line"></v-skeleton-loader>
            <v-skeleton-loader max-width="500" type="list-item-three-line, list-item"></v-skeleton-loader>
          </v-sheet>
        </template>

        <div class="mobileShareContent d-flex justify-center mt-2" v-if="isMobile">
          <shareContent
            class="d-flex justify-center"
            v-if="getDocumentDetails"
            :link="shareContentParams.link"
            :twitter="shareContentParams.twitter"
            :whatsApp="shareContentParams.whatsApp"
            :email="shareContentParams.email"
          />
        </div>
      </div>

      <div
        v-if="itemList.length"
        class="itemPage__main__carousel"
        :class="{'itemPage__main__carousel--margin': !docTutor && !docTutor.isTutor && $vuetify.breakpoint.xsOnly}"
      >
        <div class="itemPage__main__carousel__header">
          <div
            class="itemPage__main__carousel__header__title"
            v-t="'documentPage_related_content'"
          ></div>
        </div>

        <div>
          <v-slide-group
            v-model="model"
            class="pa-0 itemSlider"
            active-class="success"
            :next-icon="$vuetify.icons.values.next"
            :prev-icon="$vuetify.icons.values.prev"
            show-arrows
          >
            <v-slide-item v-for="(item, index) in itemList" :key="index" v-slot:default="{ }">
              <itemCard class="itemCard-itemPage" :fromCarousel="true" :item="item" :key="index" />
            </v-slide-item>
          </v-slide-group>
        </div>
      </div>

      <div
        class="itemPage__main__tutorCard"
        v-if="docTutor.isTutor"
        :class="{'itemPage__main__tutorCard--margin': docTutor.isTutor && $vuetify.breakpoint.xsOnly, 'itemPage__main__tutorCard--marginT': !itemList.length}"
      >
        <tutorResultCardMobile v-if="$vuetify.breakpoint.xsOnly" :tutorData="docTutor"></tutorResultCardMobile>
        <tutorResultCard v-else :tutorData="docTutor"></tutorResultCard>
      </div>
      <!-- <mobileUnlockDownload :sticky="true" v-if="$vuetify.breakpoint.md || $vuetify.breakpoint.sm" :document="document"></mobileUnlockDownload> -->
    </div>
    <!-- <mobileUnlockDownload v-if="$vuetify.breakpoint.xsOnly" :document="document"></mobileUnlockDownload> -->
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

import * as routeNames from "../../../routes/routeNames";

//services
import * as dialogNames from "../global/dialogInjection/dialogNames.js";

//store
import storeService from "../../../services/store/storeService";
import studyDocumentsStore from "../../../store/studyDocuments_store";

// components
import mainItem from "./components/mainItem/mainItem.vue";
import resultNote from "../../results/ResultNote.vue";
import itemCard from "../../carouselCards/itemCard.vue";
const tutorResultCard = () =>
  import(
    /* webpackChunkName: "tutorResultCard" */ "../../results/tutorCards/tutorResultCard/tutorResultCard.vue"
  );
const tutorResultCardMobile = () =>
  import(
    /* webpackChunkName: "tutorResultCardMobile" */ "../../results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue"
  );
import unlockDialog from "./components/dialog/unlockDialog.vue";
const shareContent = () =>
  import(
    /* webpackChunkName: "shareContent" */ "../global/shareContent/shareContent.vue"
  );
export default {
  name: "itemPage",
  components: {
    resultNote,
    tutorResultCard,
    tutorResultCardMobile,
    itemCard,
    mainItem,
    unlockDialog,
    shareContent
  },
  props: {
    id: {
      type: String
    }
  },
  data() {
    return {
      model: null,
      docPage: 1,
      isLoad: false
    };
  },
  watch: {
    "$route.params.id"() {
      this.clearDocument();
      this.documentRequest(this.id);
      this.getStudyDocuments({
        course: this.$route.params.courseName,
        id: this.id
      });
    }
  },
  computed: {
    ...mapGetters([
      "getBannerParams",
      "accountUser",
      "getDocumentDetails",
      "getDocumentName",
      "getDocumentPrice",
      "getIsPurchased",
      "getRelatedDocuments",
      "getRouteStack",
      "getPurchaseConfirmation",
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
    shareContentParams() {
      let urlLink = `${global.location.origin}/d/${
        this.$route.params.id
      }?t=${Date.now()}`;
      let itemType = this.getDocumentDetails.documentType;
      let courseName = this.courseName;
      let paramObJ = {
        link: urlLink,
        twitter: this.$t("shareContent_share_item_twitter", [
          courseName,
          urlLink
        ]),
        whatsApp: this.$t("shareContent_share_item_whatsapp", [
          courseName,
          urlLink
        ]),
        email: {
          subject: this.$t("shareContent_share_item_email_subject", [
            courseName
          ]),
          body: this.$t("shareContent_share_item_email_body", [
            itemType,
            courseName,
            urlLink
          ])
        }
      };
      return paramObJ;
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

    docTutor() {
      //TODO etc above
      if (
        this.getDocumentDetails &&
        this.getDocumentDetails.details &&
        this.getDocumentDetails.details.tutor
      ) {
        return this.getDocumentDetails.details.tutor;
      }
      return {};
    },
    itemList() {
      return this.getRelatedDocuments || 0;
    },
    firstName() {
      let user = this.docTutor;
      if (user.name) {
        return this.docTutor.name.split(" ")[0];
      }
      return "";
    },
    showFirstName() {
      let maxChar = 5;
      let user = this.docTutor;
      if (user.name) {
        let name = user.name.split(" ")[0];
        if (name.length > maxChar) {
          return this.$t("resultTutor_message_me");
        }
        return name;
      }
      return null;
    },
    courseName() {
      if (this.document && this.document.details) {
        return this.document.details.course;
      }
      return null;
    },
    isMyProfile() {
      if (!!this.docTutor && !!this.accountUser) {
        return (
          this.docTutor.isTutor && this.docTutor.userId == this.accountUser.id
        );
      }
      return false;
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
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    }
  },
  methods: {
    ...mapActions([
      "documentRequest",
      "clearDocument",
      "getStudyDocuments",
      "updateItemToaster",
      "updatePurchaseConfirmation",
      "downloadDocument"
    ]),

    // enterItemCard(vueElm){
    //     //TODO DUplicate code
    //     if(vueElm.enterItemPage){
    //         vueElm.enterItemPage();
    //     }else{
    //         vueElm.$parent.enterItemPage();
    //     }
    //     this.isLoad = true;
    //     setTimeout(()=>{
    //         this.isLoad = false;
    //     })
    //     this.$nextTick(() => {
    //         this.documentRequest(this.id);
    //     })
    // },
    closeDocument() {
      let regRoute = "registration";
      let routeStackLength = this.getRouteStack.length;
      let beforeLastRoute = this.getRouteStack[routeStackLength - 2];
      if (
        routeStackLength > 1 &&
        beforeLastRoute &&
        beforeLastRoute.name !== regRoute &&
        beforeLastRoute.name !== "document"
      ) {
        this.$router.back();
      } else {
        this.$router.push({ name: "feed" });
      }
    },
    // moveDownToTutorItem() {
    //   let elem = this.$el.querySelector(".itemPage__main__tutorCard");
    //   elem.scrollIntoView({ behavior: "smooth", block: "center" });
    // },
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
    this.documentRequest(this.id);
    this.getStudyDocuments({
      course: this.$route.params.courseName,
      id: this.id
    });
  },
  created() {
    storeService.lazyRegisterModule(
      this.$store,
      "studyDocumentsStore",
      studyDocumentsStore
    );
   // storeService.registerModule(this.$store, "document", document);
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
  //end hacks to finish this fast
  .documentTitle {
    background: #fff;
    & > div {
      // .flexSameSize();
    }
    & > h1 {
      // .flexSameSize();
      font-size: 18px;
      font-weight: 600;
      color: #43425d;
      align-self: center;
    }
    .document-header-large-sagment {
      &--arrow {
        transform: none /*rtl:scaleX(-1)*/;
        color: @global-purple;
      }
    }
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
          border-bottom: 1px solid #ddd;
          font-weight: 600;
          @media (max-width: @screen-xs) {
            border-bottom: none;
          }
          .price {
            .responsive-property(font-size, 30px, null, 18px);
          }
          .points {
            .responsive-property(font-size, 14px, null, 18px);
          }
        }
      }
      &__doc {
        padding: 12px 16px 12px 12px;
        border-radius: 0;
        .descriptionTitle {
          font-size: 16px;
          color: @global-purple;
          font-weight: 600;
        }
      }
      .skeletonWarp {
        .v-skeleton-loader__avatar {
          border-radius: 50%;
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
      .mobileShareContent {
        background: #fff;
      }
    }
    &__carousel {
      margin: 38px 0 34px 0;

      @media (max-width: @screen-xs) {
        background: #fff;
        padding: 16px 11px;
        margin: 0 0 8px 0;
      }
      &__header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 14px;
        font-weight: 600;
        &__title {
          color: #43425d;
          font-size: 18px;
          font-weight: 700;
        }
      }
      &--margin {
        margin-bottom: 100px;
      }
      .itemSlider {
        .v-slide-group__content {
          white-space: normal;
        }
        // .item-cont {
        //   direction: ltr; /* rtl:direction:ltr */
        // }
        .itemCard-itemPage {
          margin: 10px;
          border: none;
          box-shadow: 0 1px 4px 0 rgba(0, 0, 0, 0.15);
          display: block;
          &:first-child {
            margin-left: 0;
          }
          &:last-child {
            margin-right: 0;
          }
        }
      }
    }
    &__tutorCard {
      &--marginT {
        margin-top: 34px;
      }
      &--marginTop {
        margin-top: 34px;
      }
    }
  }
}
</style>