<template>
  <router-link v-if="url" class="d-block note-block" :to="url">
        <div class="document-header-container">
          <div class="document-header-large-sagment">
              <user-avatar size="34" v-if="authorName" :userImageUrl="userImageUrl" :user-name="authorName" :user-id="authorId"/>
            <div class="document-header-name-container">
              <span class="document-header-name"> 
                <span v-if="isTutor" v-html="$Ph('resultNote_privet',[authorName])"/>
                <span v-else>{{authorName}}</span> 
              </span>
              <span class="date-area">{{uploadDate}}</span>
            </div>
          </div>
          <div class="document-header-small-sagment">
            <div v-if="!isMobile" v-show="item.price" class="price-area" :class="{'isPurchased': isPurchased}">
                {{item.price ? item.price.toFixed(0): ''}}
                <span v-language:inner>app_currency_dynamic</span>
            </div>
            
              <v-menu class="menu-area" lazy bottom left content-class="card-user-actions" v-model="showMenu">
                <v-btn :depressed="true" @click.native.stop.prevent="showReportOptions()" slot="activator" icon>
                  <v-icon>sbf-3-dot</v-icon>
                </v-btn>
                <v-list>
                  <v-list-tile v-show="item.isVisible(item.visible)" :disabled="item.isDisabled()" v-for="(item, i) in actions" :key="i">
                    <v-list-tile-title @click="item.action()">{{ item.title }}</v-list-tile-title>
                  </v-list-tile>
                </v-list>
              </v-menu>
          </div>
        </div>

        <v-flex grow class="top-row">

          <div class="document-body-card">
            <v-progress-circular v-if="!item.preview" class="document-body-card-img" :style="isMobile? 'height:108px; width:100%;' : 'height:130px'"  indeterminate v-bind:size="164" width="2" color="#514f7d"/>
            <img v-else class="document-body-card-img" :src="docPreviewImg(item.preview)" alt="">
          </div>

          <div class="type-wrap">
            <v-flex grow class="data-row">
               <div class="content-wrap">
                <span class="item-title text-truncate">{{item.title}}</span>
                <span class="item-course text-truncate" v-html="$Ph('resultNote_course',[item.course])"/>
                <span class="item-university text-truncate" v-html="$Ph('resultNote_university',[item.university])"/>
              </div>
              <v-divider v-if="item.snippet && !isMobile" class="my-2"></v-divider>
              <div class="doc-snippet" v-if="item.snippet && !isMobile">
                <span v-line-clamp="2">{{item.snippet}}</span>
              </div>
            </v-flex>
          </div>
        </v-flex>

        <v-flex grow class="bottom-row">
          <div class="left">
            <span v-if="docViews" class="views-cont">
              <span>{{docViews}}</span>
              <span class="views" v-language:inner="docViews > 1 ? 'resultNote_views' : 'resultNote_view'"/> 
            </span>
            <span v-if="docDownloads && !item.price">
              <span>{{docDownloads}}</span>
              <span class="downloads" v-language:inner="docDownloads > 1 ? 'resultNote_downloads' : 'resultNote_download'"/> 
            </span>
            <span v-if="docPurchased && item.price">
              <span>{{docPurchased}}</span>
              <span class="downloads" v-language:inner="docPurchased > 1 ? 'resultNote_purchaseds' : 'resultNote_purchased'"/> 
            </span>
          </div>
            <span class="right">
              <likeFilledSVG v-if="isLiked" @click.native.stop.prevent="upvoteDocument" class="likeSVG"/>
              <likeSVG v-if="!isLiked"  @click.native.stop.prevent="upvoteDocument" class="likeSVG"/> 
              <span v-if="item.votes>0">{{item.votes}}</span>
            </span>
            <v-spacer v-if="isMobile"></v-spacer>
            <div v-if="isMobile" v-show="item.price" class="price-area" :class="{'isPurchased': isPurchased}">
                {{item.price ? item.price.toFixed(0): ''}}
                <span v-language:inner>app_currency_dynamic</span>
            </div>
        </v-flex>


    <sb-dialog
      :showDialog="showReport"
      :maxWidth="'438px'"
      :popUpType="'reportDialog'"
      :content-class="`reportDialog ${isRtl? 'rtl': ''}` "
    >
      <report-item :closeReport="closeReportDialog" :itemType="item.template" :itemId="itemId"></report-item>
    </sb-dialog>
    <sb-dialog
      :showDialog="priceDialog"
      :maxWidth="'438px'"
      :popUpType="'priceUpdate'"
      :onclosefn="closeNewPriceDialog"
      :activateOverlay="true"
      :isPersistent="true"
      :content-class="`priceUpdate ${isRtl? 'rtl': ''}`">
      <v-card class="price-change-wrap">
        <v-flex align-center justify-center class="relative-pos">
          <div class="title-wrap">
            <span class="change-title" v-language:inner>resultNote_change_for</span>
            <span
              class="change-title"
              style="max-width: 150px;"
              v-line-clamp="1"
            >&nbsp;"{{item.title}}"</span>
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
  </router-link>
</template>
<script>
import userAvatar from "../helpers/UserAvatar/UserAvatar.vue";
import sbDialog from "../wrappers/sb-dialog/sb-dialog.vue";
import reportItem from "./helpers/reportItem/reportItem.vue";
import { mapGetters, mapActions, mapMutations } from "vuex";
import { LanguageService } from "../../services/language/languageService";
import SbInput from "../question/helpers/sbInput/sbInput";
import documentService from "../../services/documentService";
import likeSVG from './img//like.svg';
import likeFilledSVG from './img/like-filled.svg';
import utilitiesService from '../../services/utilities/utilitiesService.js'

export default {
  components: {
    SbInput,
    sbDialog,
    reportItem,
    userAvatar,
    likeSVG,
    likeFilledSVG
  },
  data() {
    return {
      isLiked: false,
      isFirefox: global.isFirefox,
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
      isRtl: global.isRtl,
      showMenu: false,
      priceDialog: false,
      newPrice: this.item.price ? this.item.price : 0,
      rules: {
        required: value => !!value || "Required.",
        max: value => value.$options.filter <= 1000 || "max is 1000"
      }
    };
  },

  props: { item: { type: Object, required: true }, index: { Number } },
  computed: {
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
      }
    },
    authorId() {
      if (!!this.item && !!this.item.user && !!this.item.user.id) {
        return this.item.user.id;
      }
    },
    docViews() {
      if (this.item) {
        return this.item.views;
      }
    },
    docDownloads() {
      if (this.item) {
        return this.item.downloads;
      }
    },
    docPurchased(){
      if(this.item){
        return this.item.purchased;
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
    isTutor(){
      if (!!this.item) {
        return this.item.user.isTutor;
      }
    },
    isMobile(){
      return this.$vuetify.breakpoint.xs;
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
      "syncProfile",
      "documentVote",
      "removeItemFromList"
    ]),
    ...mapGetters(["accountUser"]),
    docPreviewImg(imgUrl){
      if(this.isMobile){
        return utilitiesService.proccessImageURL(imgUrl, 500, 108,'crop&anchorPosition=top');
      } else{
        return utilitiesService.proccessImageURL(imgUrl, 164, 130,'crop&anchorPosition=top');
      }
    },
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
        success => {
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
        success => {
          this.updateToasterParams({
            toasterText: LanguageService.getValueByKey(
              "resultNote_deleted_success"
            ),
            showToaster: true
          });
          this.removeItemFromList(id);
          this.updateProfile(id);
        },
        error => {
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
    },
  },
  created() {
    this.isLiked = this.item.upvoted;
  },
};
</script>
<style src="./ResultNote.less" lang="less"></style>
