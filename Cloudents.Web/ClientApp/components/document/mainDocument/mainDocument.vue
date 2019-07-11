<template>
    <div class="main-container pb-5">
        <v-layout row class="main-header" :class="[isSmAndDown ? 'pt-3' : 'pb-2']" align-center>
            <div class="main-header-wrapper">
                <v-icon color="#000" :class="['arrow-back','hidden-sm-and-down',isRtl? 'arrow-back-rtl': '']" @click="closeDocument">sbf-arrow-back-chat</v-icon>
                <h2 class="courseName font-weight-bold text-truncate" :class="[isSmAndDown ? 'pr-5' : 'pl-3']">{{courseName}}</h2>
                <v-spacer></v-spacer>
                <span class="grey-text views mt-2" :class="[isSmAndDown ? 'pr-3' : 'pr-5']">{{docViews}}<v-icon class="pl-2 doc-views" small>sbf-views</v-icon></span>
                <span class="grey-text date mt-2" :class="{'pl-3': isSmAndDown}">{{documentDate}}</span>
                
                <v-menu class="menu-area" lazy bottom left content-class="card-user-actions" v-model="showMenu">
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
            </div>
        </v-layout>
        <div class="document-wrap">
            <v-progress-circular
                class="unlock_progress text-xs-center"
                v-if="docPreviewLoader"
                :size="70"
                :width="7"
                indeterminate
                color="#4452fc"
            ></v-progress-circular>
            <div class=" text-xs-center" v-for="(page, index) in docPreview" :key="index">
                <img
                    class="document-wrap-content" 
                    :src="page"
                    @load="handleDocWrap"
                    v-if="page"
                    :alt="document.content" />
                
            </div>
            <div class="unlockBox headline hidden-sm-and-down" v-if="isShowPurchased" @click="unlockDocument">
                <p class="text-xs-left" v-language:inner="'documentPage_unlock_document'"></p>
                <div class="aside-top-btn elevation-5 align-center" v-if="!isLoading">
                    <span class="pa-4 font-weight-bold text-xs-center disabled" v-if="isPrice">{{docPrice | currencyLocalyFilter}}</span>
                    <span class="white--text pa-4 font-weight-bold text-xs-center" v-language:inner="'documentPage_unlock_btn'"></span>
                </div>
                <v-progress-circular
                    class="unlock_progress"
                    v-if="isLoading"
                    indeterminate
                    color="#4452fc"
                ></v-progress-circular>
            </div>
        </div>

    </div>
</template>
<script>
import { mapActions, mapGetters } from 'vuex';
import { LanguageService } from "../../../services/language/languageService";
import sbDialog from "../../wrappers/sb-dialog/sb-dialog.vue";
import reportItem from "../../results/helpers/reportItem/reportItem.vue";
import utillitiesService from '../../../services/utilities/utilitiesService';
import documentService from '../../../services/documentService';

export default {
    name: 'mainDocument',
    components: {
        reportItem,
        sbDialog
    },
    props: {
        document: {
            type: Object
        },
    },
    data() {
        return {
            showMenu: false,
            docPreviewLoader: false,
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
            ],
        }
    },
    computed: {
        ...mapGetters(['getBtnLoading', 'accountUser']),

        courseName() {
            if(this.document.details && this.document.details.name) {
                return this.document.details.name
            }
        },
        documentDate() {
            if(this.document.details && this.document.details.date) {
                let lang = `${global.lang}-${global.country}`;
                return new Date(this.document.details.date).toLocaleString(lang, {year: 'numeric', month: 'short', day: 'numeric'})
            }
        },
        isPurchased() {
            if(!this.document.details) return true;
            if(this.document.details && this.document.details.isPurchased) {
                return this.document.details.isPurchased;
            }
        },
        docViews() {
            if(this.document.details && this.document.details.views) {
                return this.document.details.views
            }
        },
        docPrice() {
            if(this.document.details && this.document.details.views) {
                return this.document.details.price.toFixed(2)
            }
        },
        docPreview() {
            // TODO temporary calculated width container
            this.docPreviewLoader = true;
            if(this.document.preview && this.docWrap) {
                let width;
                if (this.$vuetify.breakpoint.xl) {
                    width = 1540
                }
                if (this.$vuetify.breakpoint.lg) {
                    width = 900
                }
                if (this.$vuetify.breakpoint.md) {
                    width = 600
                }
                if (this.$vuetify.breakpoint.sm) {
                    width = 750
                }
                if (this.$vuetify.breakpoint.xs) {
                    width = 400
                }                
                let height = width / 0.707;                
                let result = this.document.preview.map(preview => {                    
                    return utillitiesService.proccessImageURL(preview, width, height.toFixed(0), 'pad')
                })
                return result;
            }
        },
        isSmAndDown() {
            return this.$vuetify.breakpoint.smAndDown
        },
        itemType() {
            if(this.document) {
                // return this.document.details.template
            }
            return 'note'
        },
        documentPrice:{
            get(){
                if(this.newPrice !== null){
                    return this.newPrice;
                }else{
                    return this.document.details ? this.document.details.price : 0;
                }
            },
            set(val){
                this.newPrice = val;
            }   
        },
        isLoading() {            
            return this.getBtnLoading
        },
        isPrice() {
            if(this.document.details && this.document.details.price > 0) {
                return true
            } else {
                return false
            }
        },
        isShowPurchased() {
            if(!this.isPurchased && this.isPrice > 0) {
                return true
            }
            return false
        }
    },
    methods: {
        ...mapActions(['clearDocument', 'purchaseDocument', 'updateToasterParams', 'setNewDocumentPrice','updateLoginDialogState']),

        unlockDocument() {
            if(this.accountUser == null) {
                this.updateLoginDialogState(true);
            } else{
                let item = {id: this.document.details.id, price: this.document.details.price}
                if(!this.isLoading) {
                    this.purchaseDocument(item)
                }
            }
        },
        closeDocument() {
            this.clearDocument();
            this.$router.push({path: '/note'})
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

        documentService.deleteDoc(id).then(success => {
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
                    toasterType: 'error-toaster'
                });
            })
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
        handleDocWrap(e){
            this.docPreviewLoader = false;
        }
    },
    beforeDestroy() {
        console.log("beforeDestroy")
        this.clearDocument();
    },
    mounted(){
        this.docWrap = document.querySelector('.document-wrap');        
    },
}
</script>
<style lang="less">
    @import "../../../styles/mixin.less";

    .main-container {
        flex: 5;
        @media (max-width: @screen-sm) {
            order: 2;
        }
        .main-header {
            justify-content: center;
            .main-header-wrapper{
                display: flex;
                width: 100%;
                align-items: center;
                max-width: 960px;
                .menu-area {
                    margin-right: -10px;
                }
                .doc-views {
                    margin-bottom: 1px;
                    font-size: 13px !important;
                }
                .courseName {
                    font-size: 18px;
                    line-height: initial !important;
                    max-width: 800px;
                    @media (max-width: @screen-xs) {
                        max-width: 200px;
                    }
                    @media (max-width: @screen-xss) {
                        max-width: 160px;
                    }
                    @media (max-width: 320px) {
                        max-width: 110px;
                    }
                }
                .arrow-back {
                    font-size: 34px;
                }
                .arrow-back-rtl{
                    transform: scaleX(-1);
                }
                .grey-text {
                    opacity: .6;
                }
                .verticalMenu {
                    font-size: 16px;
                    color: #aaa;
                    @media (max-width: @screen-sm) {
                        font-size: 16px;
                    }
                }
                .date, .views {
                    font-size: 11px;
                }
            }
        }
        .document-wrap {
            position: relative;
            .unlockBox {
                cursor: pointer;
                background: #fff;
                position: fixed;
                border: 2px solid #000;
                padding: 20px;
                left: 0;
                right: 330px;
                bottom: 30px;
                height: 200px;
                width: 550px;
                margin: auto;
                p {
                    width: 80%;
                     @media (max-width: @screen-sm) {
                         width: auto;
                    }
                }
                .aside-top-btn {
                    display: flex;
                    border-radius: 4px;
                    margin: 0 0 0 auto;
                    width: 60%;
                    line-height: 20px;
                    @media (max-width: @screen-sm) {
                         width: auto;
                    }
                    span:first-child {
                        flex: 2;
                        font-size: 18px;
                    }
                    span:nth-child(2) {
                        flex: 1;
                        background-color: #4452fc;
                        font-size: 15px;
                        border-radius: 0 4px 4px 0
                    }
                }
                
            }
            .document-wrap-content {
                width: 100%;
                max-width: 960px;
            }
            .unlock_progress {
                    display: flex;
                    margin: 0 auto;
                }
        }
        
    }
</style>