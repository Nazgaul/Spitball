<template>
  <router-link v-show="url && !fromItemPage" class="d-block note-block" :class="{'no-cursor': fromItemPage}" :to="!fromItemPage ? url : ''">
    <div class="document-header-container">
      <div class="document-header-large-sagment">
        <slot name="arrowBack"></slot>
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
          <span class="date-area">{{uploadDate}}</span>
        </div>
      </div>
      <div class="document-header-small-sagment">
        <div
          v-if="!isMobile"
          v-show="item.price"
          class="price-area"
          :class="{'isPurchased': isPurchased}"
        >
          {{item.price ? item.price.toFixed(0): ''}}
          <span v-language:inner>app_currency_dynamic</span>
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
        <div class="document-body-card">
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
        </div>
      </template>

      <div class="type-wrap" :class="{'type-wrap--noPadding': fromItemPage}">
        <v-flex grow class="data-row">
          <div class="content-wrap">
            <h1 class="item-title text-truncate">{{item.title}}</h1>
            <span class="item-course text-truncate">
              <span class="item-course" v-language:inner="'resultNote_course'"/>
              <h2 class="item-course">{{item.course}}</h2>
            </span>
            <span v-if="item.university" class="item-university text-truncate">
              <span class="item-university" v-language:inner="'resultNote_university'"/>
              <h3 class="item-university">{{item.university}}</h3>
            </span>
          </div>
          <v-divider v-show="item.snippet" class="my-2"></v-divider>
          <div class="doc-snippet" v-show="item.snippet">
            <h6 class="doc-snippet-h6">{{item.snippet}}</h6>
          </div>
        </v-flex>
      </div>
    </v-flex>

    <v-flex grow class="bottom-row">
      <div class="left">
        <span v-if="docViews" class="views-cont">
          <span>{{docViews}}</span>
          <span
            class="views"
            v-language:inner="docViews > 1 ? 'resultNote_views' : 'resultNote_view'"
          />
        </span>
        <span v-if="docDownloads && !item.price">
          <span>{{docDownloads}}</span>
          <span
            class="downloads"
            v-language:inner="docDownloads > 1 ? 'resultNote_downloads' : 'resultNote_download'"
          />
        </span>
        <span v-if="docPurchased && item.price">
          <span>{{docPurchased}}</span>
          <span
            class="downloads"
            v-language:inner="docPurchased > 1 ? 'resultNote_purchaseds' : 'resultNote_purchased'"
          />
        </span>
      </div>
      <span class="right" style="cursor:pointer">
        <likeFilledSVG v-if="isLiked" @click.stop.prevent="upvoteDocument" class="likeSVG" />
        <likeSVG v-if="!isLiked" @click.stop.prevent="upvoteDocument" class="likeSVG" />
        <span v-if="item.votes>0">{{item.votes}}</span>
      </span>
      <v-spacer v-if="isMobile"></v-spacer>
      <div
        v-if="isMobile"
        v-show="item.price"
        class="price-area"
        :class="{'isPurchased': isPurchased}"
      >
        {{item.price ? item.price.toFixed(0): ''}}
        <span v-language:inner>app_currency_dynamic</span>
      </div>
    </v-flex>

    <sb-dialog
      :showDialog="showReport"
      :maxWidth="'438px'"
      :popUpType="'reportDialog'"
      :content-class="`reportDialog` "
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
      :content-class="`priceUpdate`"
    >
      <v-card class="price-change-wrap">
        <v-flex align-center justify-center class="relative-pos">
          <div class="title-wrap">
            <span class="change-title" v-language:inner>resultNote_change_for</span>
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
            <span v-language:inner>resultNote_action_cancel</span>
          </button>
          <button @click="submitNewPrice()" class="change-price">
            <span v-language:inner>resultNote_action_apply_price</span>
          </button>
        </div>
      </v-card>
    </sb-dialog>

    <slot name="isTutor"></slot>

  </router-link>
</template>
<script>
import { mapGetters, mapActions, mapMutations } from "vuex";

import studyDocumentsStore from "../../store/studyDocuments_store";
import storeService from "../../services/store/storeService";
import documentService from "../../services/documentService";

import { LanguageService } from "../../services/language/languageService";
import utilitiesService from "../../services/utilities/utilitiesService.js"; // cannot async, js error

const sbDialog = () => import("../wrappers/sb-dialog/sb-dialog.vue");
const reportItem = () => import("./helpers/reportItem/reportItem.vue");
const likeSVG = () => import("./img//like.svg");
const likeFilledSVG = () => import("./img/like-filled.svg");
const vidSVG = () => import("./svg/vid.svg");
const intersection = () => import('../pages/global/intersection/intersection.vue');
import VueNumeric from 'vue-numeric'

export default {
  components: {
    sbDialog,
    reportItem,
    likeSVG,
    likeFilledSVG,
    vidSVG,
    intersection,
    VueNumeric
  },
  data() {
    return {
      isPreviewReady: false,
      isLiked: false,
      loading: false,
      currentCurrency: LanguageService.getValueByKey("app_currency_dynamic"),
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
      ],
      itemId: 0,
      showReport: false,
      showMenu: false,
      priceDialog: false,
      newPrice: this.item.price ? this.item.price : 0,
      rules: {
        required: value => !!value || "Required.",
        max: value => value.$options.filter <= 1000 || "max is 1000"
      }
    };
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
    isVideo() {
      return this.item.documentType === "Video";
    },
    userImageUrl() {
      if (
        this.item.user &&
        this.item.user.image &&
        this.item.user.image.length > 1
      ) {
        return `${this.item.user.image}`;
      }
      return "";
    },
    isProfile() {
      return this.$route.name === "profile";
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
      if (!!this.item && !!this.item.user && !!this.item.user.id) {
        return this.item.user.id;
      }else{
        return null;
      }
    },
    docViews() {
      if (this.item) {
        return this.item.views;
      }else{
        return null;
      }
    },
    docDownloads() {
      if (this.item) {
        return this.item.downloads;
      }else{
        return null;
      }
    },
    docPurchased() {
      if (this.item) {
        return this.item.purchased;
      }else{
        return null;
      }
    },
    uploadDate() {
      if (this.item && this.item.dateTime) {
        return this.$options.filters.fullMonthDate(this.item.dateTime);
      } else {
        return "";
      }
    },
    url() {
      return this.item.url;
    },
    isOurs() {
      let ours;
      if (this.item && this.item.source) {
        ours = this.item.source.toLowerCase().includes("cloudents");
      }
      return ours;
    },
    isMobile() {
      return this.$vuetify.breakpoint.xs;
    },
    docPreviewImg() {
      if (this.isMobile) {
        return utilitiesService.proccessImageURL(
          this.item.preview,
          100,
          106,
          "crop&anchorPosition=top"
        );
      } else {
        return utilitiesService.proccessImageURL(
          this.item.preview,
          148,
          130,
          "crop&anchorPosition=top"
        );
      }
    },
    isPreview() {
      if (this.item && this.item.preview && this.loading) {
        return false;
      }
      return true;
    }
  },
  methods: {
    ...mapMutations({
      updateLoading: "UPDATE_LOADING",
      updateSearchLoading: "UPDATE_SEARCH_LOADING"
    }),
    ...mapActions([
      "updateLoginDialogState",
      "updateToasterParams",
      "removeItemFromProfile",
      "documentVote",
      "removeItemFromList",
      "removeDocItemAction"
    ]),
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
      // return true
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
    //check if profile and refetch data after doc deleted
    updateProfile(id) {
      if (this.isProfile) {
        this.removeItemFromProfile({ id: id });
      }
    },
    deleteDocument() {
      let id = this.item.id;
      
      
      documentService.deleteDoc(id).then(
        () => {
          this.updateToasterParams({
            toasterText: LanguageService.getValueByKey(
              "resultNote_deleted_success"
            ),
            showToaster: true
          });
          if (this.$route.name === "document") {
            this.$router.replace({name:"feed"});
            return
          }
          this.removeItemFromList(id);
          this.updateProfile(id);
          let objToDelete = { id };
          this.removeDocItemAction(objToDelete);
        },
        () => {
          this.updateToasterParams({
            toasterText: LanguageService.getValueByKey(
              "resultNote_error_delete"
            ),
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
    },

    isAuthUser() {
      let user = this.accountUser();
      if (user == null) {
        this.updateLoginDialogState(true);
        return false;
      }
      return true;
    },
    upvoteDocument(e) {
      e.stopImmediatePropagation();
      if (this.isAuthUser()) {
        this.isLiked = true;
        let type = "up";
        if (!!this.item.upvoted) {
          type = "none";
          this.isLiked = false;
        }
        let data = {
          type,
          id: this.item.id
        };
        this.documentVote(data);
      }
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
    this.isLiked = this.item.upvoted;
    this.$nextTick(() => {
      this.loading = true;
    });
  }
};
</script>
<style src="./ResultNote.less" lang="less"></style>
