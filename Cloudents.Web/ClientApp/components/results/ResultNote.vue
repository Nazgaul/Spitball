<template>
  <router-link v-show="url && !fromItemPage" class="d-block note-block" :class="{'no-cursor': fromItemPage}" :to="!fromItemPage ? url : ''">
    <div class="document-header-container">
      <div class="document-header-large-sagment">
        <user-avatar
          size="34"
          v-if="authorName"
          :userImageUrl="userImageUrl"
          :user-name="authorName"
          :user-id="authorId"
        />
        <div class="document-header-name-container">
          <span class="document-header-name text-truncate">
            <span>{{authorName}}</span>
          </span>
          <span class="date-area">{{$d(new Date(item.dateTime), 'short')}}</span>
        </div>
      </div>
      <div class="document-header-small-sagment">
        <div
          v-if="!isMobile"
          v-show="item.price"
          class="price-area"
          :class="{'isPurchased': isPurchased}"
        >
          {{item.price}}
          <span v-t="'app_currency_dynamic'"></span>
        </div>

        <v-menu
          class="menu-area"
          bottom
          left
          content-class="card-user-actions"
          v-model="showMenu"
        >
          <template v-slot:activator="{  }">  
            <v-btn
              class="menu-area-btn"
              :depressed="true"
              @click.native.stop.prevent="showReportOptions()"
              icon
            >
              <v-icon>sbf-3-dot</v-icon>
            </v-btn>
          </template>
          <v-list>
            <v-list-item
              v-for="(prop, i) in actions"
              v-show="prop.isVisible(prop.visible)"
              :disabled="prop.isDisabled()"
              :key="i"
            >
              <v-list-item-title style="cursor:pointer;" @click="prop.action()">{{ prop.title }}</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
      </div>
    </div>

    <slot name="descriptionTitle"></slot>

    <v-flex grow class="top-row">
      <template v-if="!fromItemPage">
        <v-progress-circular
          v-show="!isPreviewReady"
          class="document-body-card-img"
          :style="isMobile? 'height:108px; width:100%;' : 'height:130px'"
          indeterminate
          v-bind:size="164"
          width="2"
          color="#514f7d"
        />
        <div class="document-body-card" :class="{'subscribed': isSubscribed}">
          <span v-show="(isVideo && item.itemDuration) && isPreviewReady" class="videoType">
            <vidSVG  />
            <span class="vidTime">{{item.itemDuration}}</span>
          </span>
          <intersection>
            <img
              class="document-body-card-img"
              @load="isPreviewReady = true"
              :src="docPreviewImg"
              alt
            />
          </intersection>
          <div class="overlay text-center px-5" v-if="isSubscribed">
              <div class="unlockText white--text mb-3" v-t="'resultNote_subscribe_unlock_btn'"></div>
              <v-btn class="btn" color="#fff" rounded block>
                <span v-t="{path: 'resultNote_subscribe_unlock_btn', args: { 0: subscribedPrice }}"></span>
              </v-btn>
          </div>
        </div>
      </template>

      <div class="type-wrap" :class="{'type-wrap--noPadding': fromItemPage}">
          <div class="wrapHeight">
            <v-flex grow class="data-row">
              <div class="content-wrap">
                <h1 class="item-title text-truncate">{{item.title}}</h1>
                <span class="item-course text-truncate">
                  <span class="item-course font-weight-bold">{{$t('resultNote_course')}}</span>
                  <h2 class="item-course">{{item.course}}</h2>
                </span>
              </div>
              <v-divider v-show="item.snippet" class="my-2"></v-divider>
              <div class="doc-snippet" v-show="item.snippet">
                <h6 class="doc-snippet-h6">{{item.snippet}}</h6>
              </div>
            </v-flex>
          </div>

          <documentLikes v-if="!isMobile" :item="item" />
      </div>
      
    </v-flex>

    <documentLikes v-if="isMobile" :item="item" />

    <sb-dialog
      :showDialog="showReport"
      :maxWidth="'438px'"
      :popUpType="'reportDialog'"
      :content-class="`reportDialog` "
    >
      <report-item v-if="showReport" :closeReport="closeReportDialog" :itemType="'Document'" :itemId="itemId"></report-item>
    </sb-dialog>
    <sb-dialog
      :showDialog="priceDialog"
      :maxWidth="'438px'"
      :popUpType="'priceUpdate'"
      :onclosefn="closeNewPriceDialog"
      :activateOverlay="true"
      :isPersistent="true"
      :content-class="`priceUpdate`"
    >
      <v-card class="price-change-wrap">
        <v-flex align-center justify-center class="relative-pos">
          <div class="title-wrap">
            <span class="change-title" v-t="'resultNote_change_for'"></span>
            <span class="change-title" style="max-width: 150px;">&nbsp;"{{item.title}}"</span>
          </div>
          <div class="input-wrap align-center justify-center">
            <div class="price-wrap">
              <vue-numeric
                :currency="currentCurrency"
                class="sb-input-upload-price"
                :minus="false"
                :min="0"
                :precision="2"
                :max="1000"
                :currency-symbol-position="'suffix'"
                separator=","
                v-model="newPrice"
              ></vue-numeric>
            </div>
          </div>
        </v-flex>
        <div class="change-price-actions">
          <button @click="closeNewPriceDialog()" class="cancel mr-2">
            <span v-t="'resultNote_action_cancel'"></span>
          </button>
          <button @click="submitNewPrice()" class="change-price">
            <span v-t="'resultNote_action_apply_price'"></span>
          </button>
        </div>
      </v-card>
    </sb-dialog>

    <slot name="isTutor"></slot>

  </router-link>
</template>
<script>
import { mapGetters, mapActions } from "vuex";

import studyDocumentsStore from "../../store/studyDocuments_store";
import storeService from "../../services/store/storeService";
import documentService from "../../services/documentService";

const sbDialog = () => import("../wrappers/sb-dialog/sb-dialog.vue");
const reportItem = () => import("./helpers/reportItem/reportItem.vue");
const documentLikes = () => import("./resultDocument/documentLikes.vue");
const intersection = () => import('../pages/global/intersection/intersection.vue');
import VueNumeric from 'vue-numeric'

import vidSVG from "./svg/vid.svg";

export default {
  components: {
    sbDialog,
    reportItem,
    documentLikes,
    vidSVG,
    intersection,
    VueNumeric
  },
  data() {
    return {
      isPreviewReady: false,
      loading: false,
      currentCurrency: this.$t("app_currency_dynamic"),
      actions: [
        {
          title: this.$t("questionCard_Report"),
          action: this.reportItem,
          isDisabled: this.isDisabled,
          isVisible: this.isVisible,
          visible: true
        },
        {
          title: this.$t("resultNote_change_price"),
          action: this.showPriceChangeDialog,
          isDisabled: this.isOwner,
          isVisible: this.isVisible,
          icon: "sbf-delete",
          visible: true
        },
        {
          title: this.$t("resultNote_action_delete_doc"),
          action: this.deleteDocument,
          isDisabled: this.isOwner,
          isVisible: this.isVisible,
          visible: true
        }
      ],
      itemId: 0,
      showReport: false,
      showMenu: false,
      priceDialog: false,
      newPrice: this.item.price ? this.item.price : 0,
    }
  },
  props: {
    item: { type: Object, required: true },
    index: { Number },
    fromItemPage: {
      type: Boolean,
      default: false
    }
  },
  watch: {
    priceDialog(val) {
      if (!val) {
        this.newPrice = this.item.price;
      }
    }
  },
  computed: {
    subscribedPrice() {
      return '$15'
    },
    isSubscribed() {
      return this.item?.subscribed
    },
    isVideo() {
      return this.item.documentType === "Video";
    },
    userImageUrl() {
      return this.item?.user.image
    },
    isPurchased() {
      return this.item.isPurchased;
    },
    authorName() {
      if (!!this.item.user) {
        return this.item.user.name;
      }else{
        return null;
      }
    },
    authorId() {
      return this.item?.user.id
    },
    url() {
      return this.item.url;
    },
    isMobile() {
      return this.$vuetify.breakpoint.xs;
    },
    docPreviewImg() {
      let size = this.isMobile ? [110, 108] : [200, 162]
      return this.$proccessImageUrl(this.item.preview,...size)
    },
    isPreview() {
      if (this.item && this.item.preview && this.loading) {
        return false;
      }
      return true;
    }
  },
  methods: {
    ...mapActions(["updateToasterParams", "removeItemFromList", "removeDocItemAction"]),
    ...mapGetters(["accountUser"]),

    cardOwner() {
      let userAccount = this.accountUser();
      if (userAccount && this.item.user) {
        return userAccount.id === this.item.user.id; // will work once API call will also return userId
      } else {
        return false;
      }
    },
    updateItemPrice(val) {
      if (val || val === 0) {
        return (this.item.price = val);
      }
    },
    isVisible(val) {
      return val;
    },
    submitNewPrice() {
      let data = { id: this.item.id, price: this.newPrice };
      let self = this;
      documentService.changeDocumentPrice(data).then(
        () => {
          self.updateItemPrice(self.newPrice);
          self.closeNewPriceDialog();
        },
        error => {
          console.error("erros change price", error);
        }
      );
    },
    closeNewPriceDialog() {
      this.priceDialog = false;
    },
    isOwner() {
      let owner = this.cardOwner();
      return !owner;
    },
    showPriceChangeDialog() {
      this.priceDialog = true;
    },
    isDisabled() {
      let isOwner, account, notEnough;
      isOwner = this.cardOwner();
      account = this.accountUser();
      if (isOwner || !account || notEnough) {
        return true;
      }
    },
    reportItem() {
      this.itemId = this.item.id;
      this.showReport = !this.showReport;
    },
    deleteDocument() {
      let id = this.item.id;
      documentService.deleteDoc(id).then(
        () => {
          this.updateToasterParams({
            toasterText: this.$t("resultNote_deleted_success"),
            showToaster: true
          });
          if (this.$route.name === "document") {
            this.$router.replace({name:"feed"});
            return
          }
          this.removeItemFromList(id);
          let objToDelete = { id };
          this.removeDocItemAction(objToDelete);
        },
        () => {
          this.updateToasterParams({
            toasterText: this.$t("resultNote_error_delete"),
            showToaster: true
          });
        }
      );
    },
    closeReportDialog() {
      this.showReport = false;
    },
    showReportOptions() {
      this.showMenu = true;
    }
  },
  beforeDestroy() {
    // storeService.unregisterModule(this.$store, 'studyDocumentsStore');
  },
  created() {
    storeService.lazyRegisterModule(
      this.$store,
      "studyDocumentsStore",
      studyDocumentsStore
    );
    this.$nextTick(() => {
      this.loading = true;
    });
  }
};
</script>
<style src="./ResultNote.less" lang="less"></style>
