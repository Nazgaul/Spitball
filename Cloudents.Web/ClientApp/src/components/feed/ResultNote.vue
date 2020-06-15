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
          <span class="date-area">{{$d(item.dateTime, 'short')}}</span>
        </div>
      </div>
      <div class="document-header-small-sagment">
        <documentPrice :price="item.price" v-if="!isMobile" :isSubscribed="isSubscribed" />

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
              <v-list-item-title style="cursor:pointer;" @click="prop.action()" v-t="prop.title"></v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
      </div>
    </div>

    <slot name="descriptionTitle"></slot>

    <v-flex grow class="top-row">
      <template v-if="!fromItemPage">
        <v-skeleton-loader
          v-if="!isPreviewReady"
          class="document-body-card-img"
          type="image"
          :height="isMobile ? '108' : '162'"
          :min-width="isMobile ? '110' : '200'"
        ></v-skeleton-loader>
        <div class="document-body-card" :class="{'subscribed': isSubscribed && isPreviewReady}">
          <intersection>
            <img
              class="document-body-card-img"
              @load="isPreviewReady = true"
              :src="docPreviewImg"
              alt
            />
          </intersection>
          <div class="overlay text-center px-2 px-sm-5" v-if="isSubscribed && isPreviewReady">
              <div class="unlockText white--text mb-3">{{subscribeText}}</div>
              <v-btn class="btn" color="#fff" @click.prevent="subscribe" rounded block>
                <span>{{subscribeBtnText}}</span>
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
                  <span class="item-course font-weight-bold" v-t="'resultNote_course'"></span>
                  <h2 class="item-course">{{item.course}}</h2>
                </span>
                <div class="videoType" v-show="(isVideo && item.itemDuration) && isPreviewReady">
                  <span class="vidTime mr-1">{{item.itemDuration}}</span>
                  <vidSVG class="videoIcon" width="16" />
                </div>
              </div>
              <v-divider v-show="item.snippet" class="my-2"></v-divider>
              <div class="doc-snippet" v-show="item.snippet">
                <h6 class="doc-snippet-h6">{{item.snippet}}</h6>
              </div>
            </v-flex>
          </div>

          <documentLikes v-if="!isMobile && !fromItemPage" :item="item" />
      </div>
      
    </v-flex>

    <documentLikes v-if="isMobile && !fromItemPage" :item="item" />

    <sb-dialog
      :showDialog="showReport"
      :maxWidth="'438px'"
      :popUpType="'reportDialog'"
      :content-class="`reportDialog`"
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
                :currency="$t('app_currency_dynamic')"
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

import * as routeNames from '../../routes/routeNames';

const sbDialog = () => import("../wrappers/sb-dialog/sb-dialog.vue");
const reportItem = () => import("../results//helpers/reportItem/reportItem.vue");
const documentLikes = () => import("../results/resultDocument/documentLikes.vue");
const intersection = () => import('../pages/global/intersection/intersection.vue');
const documentPrice = () => import("../pages/global/documentPrice/documentPrice.vue");
import VueNumeric from 'vue-numeric'

import vidSVG from "../results/svg/vid.svg";

export default {
  components: {
    sbDialog,
    reportItem,
    documentLikes,
    vidSVG,
    intersection,
    documentPrice,
    VueNumeric
  },
  data() {
    return {
      defOpen:false,
      isExpand: false,
      isPreviewReady: false,
      loading: false,
      actions: [
        {
          title: 'questionCard_Report',
          action: this.reportItem,
          isDisabled: this.isDisabled,
          isVisible: this.isVisible,
          visible: true
        },
        {
          title: 'resultNote_change_price',
          action: this.showPriceChangeDialog,
          isDisabled: this.isOwner,
          isVisible: this.isVisible,
          icon: "sbf-delete",
          visible: true
        },
        {
          title: 'resultNote_action_delete_doc',
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
    ...mapGetters(["accountUser"]),

    subscribeText() {
      return this.isMobile ? this.$t('resultNote_subscribe_mobile_text') : this.$t('resultNote_subscribe_desktop_text')
    },
    subscribeBtnText() {
      let price = this.$price(this.subscribedPrice, 'USD')
      return this.isMobile ? this.$t('resultNote_subscribe_mobile_btn', [price]) : this.$t('resultNote_subscribe_desktop_btn', [price])
    },
    subscribedPrice() {
      return this.item.price
    },
    isSubscribed() {
      return this.item.priceType === 'Subscriber'
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
    cardOwner() {
      let userAccount = this.accountUser;
      if (userAccount && this.item.user) {
        return userAccount.id === this.item.user.id; // will work once API call will also return userId
      } else {
        return false;
      }
    },
    textLimit(){
        return this.isMobile ? 30 : 0;
    },
    isOpen :{
      get(){
        return this.defOpen
      },
      set(val){
        this.defOpen = val
      }
    },
  },
  methods: {
    ...mapActions(["updateToasterParams", "removeItemFromList", "removeDocItemAction"]),
    subscribe() {
      this.$router.push({
        name: routeNames.Profile,
        params: {
          id: this.item.user.id,
          name: this.item.user.name
        },
        hash: '#subscription'
      })
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
      let owner = this.cardOwner;
      return !owner;
    },
    showPriceChangeDialog() {
      this.priceDialog = true;
    },
    isDisabled() {
      return this.cardOwner || !this.accountUser;
    },
    reportItem() {
      this.itemId = this.item.id;
      this.showReport = !this.showReport;
    },
    deleteDocument() {
      let id = this.item.id;
      documentService.deleteDoc(id).then(() => {
          this.updateToasterParams({
            toasterText: this.$t("resultNote_deleted_success"),
            showToaster: true
          });
          if (this.$route.name === routeNames.Document) {
            this.$router.replace({name: routeNames.Feed});
            return
          }
          this.removeItemFromList(id);
          this.removeDocItemAction({ id });
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
  filters: {
    truncate(val, isOpen, suffix, textLimit){
        if (val.length > textLimit && !isOpen) {
            return val.substring(0, textLimit) +  suffix + ' ';
        } 
        if (val.length > textLimit && isOpen) {
            return val + ' ';
        }
        return val;
    },
    restOfText(val, isOpen, suffix, textLimit){
        if (val.length > textLimit && !isOpen) {
            return val.substring(textLimit) ;
        }
        if (val.length > textLimit && isOpen) {
            return '';
        }
    }
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

<style lang="less">
@import "../../styles/mixin.less";
@import "../dialogs/changePrice/changePrice.less";

@colorUploadDate: rgba(0, 0, 0, 0.38);
@placeholderGrey: rgba(74, 74, 74, 0.25);
@colorPrice: rgba(74, 74, 74, 0.87);
.relative-pos {
  position: relative;
  .input-wrap {
    position: absolute;
    display: flex;
    justify-content: center;
    align-items: center;
    left: 0;
    bottom: -12px;
    right: 0; 
  }
}

.note-block {
  background-color: #FFF;
  min-height: auto;
  // box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
  //background: @color-white;
  border-radius: 8px;
  //width: 100%;
  cursor: pointer;
  //padding: 14px 16px !important;

  &.no-cursor {
    cursor: text;
  }

  .doc-type-text {
    font-size: 13px;
    font-weight: 600;
    
    color: @color-blue-new;
  }

  i, .doc-type-text {
    &.type-lecture {
      color: @colorTypeLecture;
    }
    &.type-textbook {
      color: @color-blue-new;
    }
    &.type-exam {
      color: @colorTypeExam;
    }
    &.type-document {
      color: @colorTypeDocThirdParty;
    }

  }

  .document-header-container {
    display: flex;
    flex-direction: row;
    line-height: 35px;
    justify-content: space-between;
    white-space: nowrap;
    .document-header-large-sagment {
      display: flex;
      // width: 90%;
      min-width: 0;
      flex-grow: 1;
      align-items: end;

      &--arrow {
        align-self: center;
        color: #69687d;
      }

      .document-header-name-container {
        display: flex;
        flex-direction: column;
        margin-left: 12px;
        min-width: 0;
        .document-header-name {
          line-height: normal;
            font-size: 14px;
            color: @global-purple;
            text-transform: capitalize;
        }
        .date-area {
          line-height: normal;
          font-size: 13px;
          color: #a0a0a0;
        }
      }
      .sold-area {
        .sold-container {
          width: 65px;
          display: flex;
          margin-left: -10px;
          background-color: #2bcea9;
          border-radius: 15px;
          height: 15px;
          margin-top: 10px;
          line-height: 15px;
          //justify-content: space-evenly;
          justify-content: space-around;

          span {
            
            font-size: 12px;
            font-weight: 600;
            font-style: italic;
            color: #ffffff;
          }
          i {
            color: #fff;
            font-size: 11px;
          }
        }
      }
    }
    .document-header-small-sagment {
      display: flex;
      // align-items: baseline;
      margin-top: -8px;
      margin-right: -16px;
      .price-area {
          font-size: 18px;
          font-weight: bold;
          color: #5158af;
        &.sold {
          color: rgba(0, 0, 0, 0.38);
        }
      }
      .menu-area {
        width: 20px;
        @media (max-width: @screen-xs) {
          width: 24px;
        }
      }
      .menu-area-btn {
        i {
          font-size: 16px;
          color: @colorTypeDocDefault;
        }
      }
    }
  }
  .documentPrice {
    .docFree {
      font-size: 18px;
      color: @global-purple;
      font-weight: 600;
    }
    .docIcon {}
  }
  .bottom-row{
    display: flex;
    justify-content: space-between;
    align-items: baseline;
    font-weight: bold;
    line-height: 1;
    color: @global-purple;
    padding-top: 6px;
    @media (max-width: @screen-xs) {
      padding-top: 20px;
      justify-content: initial;
      align-items: flex-end;
    }
    .left{
      .views-cont{
        margin-right: 22px;
      }
      .downloads{
        margin-left: 4px;
      @media (max-width: @screen-xs) {
        margin-right: 22px;
      }
      }
    }
    .right{
      display: flex;
      align-items: flex-end;
      .likeSVG{
        margin-right: 4px;
        fill: @global-purple;
      }
      
    }
    .price-area {
      @media (max-width: @screen-xs) {
        font-size: 16px;
        font-weight: bold;
        color: #5158af;
        margin-right: -2px;
      }
    &.sold {
      color: rgba(0, 0, 0, 0.38);
    }
  
  }
  }
    .top-row {
      display: flex;
      flex-direction: row;
      align-items: flex-start;
      padding-top: 18px;
      @media (max-width: @screen-xs) {
      }
      .document-body-card{
        position: relative;

        @media (max-width: @screen-xs) {
        }
        .document-body-card-img{
          object-fit: cover;
          border: 1px solid #d8d8d8;
          @media (max-width: @screen-xs) {
            object-position: top;
          }
            
        }
        &.subscribed {
          &:before {
            content: '';
            position: absolute;
            background: rgba(0, 0, 0, .7);
            height: 100%;
            width: 100%;
          }
          .overlay {
            position: absolute;
            top: 50%;
            right: 0;
            left: 0;
            transform: translate(0,-50%);
            .unlockText {
              white-space: pre;
              .responsive-property(font-size, 15px, null, 13px);                
              font-weight: 600;
              line-height: 1.47;
              @media (max-width: @screen-xs) {
                white-space: unset;
              }
            }
            .btn {
              width: 100%;
              color: @global-purple;
              font-weight: 600;
            }
          }
        }
      }
      .type-wrap {
        flex-grow: 1;
        display: flex;
        flex-direction: column;
        padding-left: 12px;
        min-width: 0;
        @media (max-width: @screen-xs) {
          padding-left: 8px;
        }
        .wrapHeight {
          height: 140px;
          @media (max-width: @screen-xs) {
            height: auto;
          }
        }
        &--noPadding {
          padding-left: 0;
          .wrapHeight {
            height: auto;
          }
          .doc-snippet-h6{
            .giveMeEllipsis(3, 22) !important;
            @media (max-width: @screen-xs) {
              .giveMeEllipsis(4, 16) !important;
            }
          }
        }
        .videoType{
          .videoIcon {
            vertical-align: middle;
            path {
              fill: #69687d;
            }
          }
          .vidTime{
              font-size: 12px;
          }
        }
      }
    }
    .details-row {
      margin-bottom: 26px;
      .details-wrap {
        padding-top: 0px;
        .aligned {
          font-size: 12px;
          color: @colorBlackNew;
          float: left;
          margin-right: 4px;
          display: unset;
          .sb-icon-arrow {
            font-size: 12px;
            height: 16px;
            width: 16px;
            color: @colorGreyLight;
          }
        }
      }
    }
    .data-row {
      display: flex;
      flex-direction: column;
      height: 100%;
      margin-right: 10px;
      .doc-snippet{
          .doc-snippet-h6{
            font-size: 14px;
            font-weight: normal;
            line-height: 1.57;
            color:@global-purple;
            @media (max-width: @screen-xs) {
              font-size: 12px;
            }
            .giveMeEllipsis(2, 22);
            @media (max-width: @screen-xs) {
              .giveMeEllipsis(3, 16);
            }
          }
          
      }
      .content-wrap {
        display: flex;
        flex-direction: column;
        .item-title{
          font-size: 18px;
          font-weight: bold;
          color: @global-purple;
        }
        .item-course{
          line-height: 1.8;
          color: @global-purple;
          font-size: 14px;
          font-weight: normal;
          display: initial;
          @media (max-width: @screen-xs) {
            font-size: 12px;
            line-height: 1.9; 
          }
        }
        span{
          color: @global-purple;
          font-size: 14px;
          @media (max-width: @screen-xs) {
            font-size: 12px;
          }

          

        }
        &.type-lecture {
          background: fade(@colorTypeLecture, 12%)
        }
        &.type-textbook {
          background: fade(@color-blue-new, 9%);

        }
        &.type-exam {
          background: fade(@colorTypeExam, 10%);
        }
        &.type-document {
          background: fade(@colorTypeDocDefault, 7%);
        }
        .title-wrap {
          display: flex;
          align-items: flex-start;
          word-break: break-all;
          word-break: break-word;
          i.doc {
            color: @colorBlackNew;
            font-size: 14px;
            padding-top: 3px;
            padding-right: 4px;
          }
          .doc-title {
            font-size: 13px;
            font-weight: 600;
            color: @textColor;
            margin-bottom: 0;
            flex-grow: 1;
            display: inline-flex;
            //firefox fix for line clamp, needs specific max-height
            &.foxLineClamp {
              max-height: 56px !important;
              display: block !important;
            }
            @media (max-width: @screen-xs) {
              line-height: 20px;
            }
          }
        }
        .content-text {
          padding-top: 9px;
          span {
            color: @colorBlackNew;
            font-size: 13px;
            line-height: 1.5;
            letter-spacing: -0.2px;
            word-break: break-all;
            word-break: break-word;
          }
        }
      }
    }
    .doc-details {
      display: flex;
      flex-direction: row;
      justify-content: flex-end;
      &.doc-actions-info {
       // display: flex;
        //flex-direction: row;
        align-items: center;

        .sb-doc-icon {
          vertical-align: middle;
          color: @colorGreyLight;
          font-size: 12px;

        }
        .sb-doc-info {
          color: @colorBlackNew;
          font-size: 12px;
          opacity: 0.9;
          
          &.downloads {
            margin-right: 12px;
          }
        }
      }
    }

  //}
}


</style>
