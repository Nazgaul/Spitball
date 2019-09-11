<template>
    <div :class="['header-wrap', isEdgeRtl ? 'position-static' : '']">
        <nav class="item-header doc-header" slot="extraHeader">
            <div class="item-header-content">
                <v-layout row align-center justify-space-between class="wrap-doc-name">
                    <!--<div class="gap ma-0"></div>-->
                    <h1 class="item-name">
                        <span class=" text-truncate">{{itemName}} </span>
                    </h1>
                    <div class="doc-details">
                        <div class="author">
                            <div>
                              <user-avatar class="avatar-circle width24 mr-2" :userImageUrl="userImageUrl" :user-name="uploaderName" :user-id="uploaderID"/>
                            </div>
                            <user-rank class="mr-2"
                                       :score="uploaderScore"></user-rank>

                        </div>
                        <div class="date">
                            {{uploadDate}}
                        </div>
                    </div>
                    <item-actions></item-actions>
                </v-layout>
            </div>
        </nav>
        <div class="details-content">
            <v-layout class="details-wrap" row align-center justify-start>
                <div class="doc-type pr-2">
                    <v-icon class="doc-type-icon">sbf-document-note</v-icon>
                    <span class="doc-type-text">{{docType}}</span>
                </div>
                <div class="detail-cell views-cell" v-if="$vuetify.breakpoint.xsOnly">
                    <div class="viewed">
                        <v-icon class="views-icon icon mr-2">sbf-views</v-icon>
                        <span class="viewed-text">{{ item && item.views ? item.views : 0}}</span>
                    </div>
                    <div class="ml-4 downloaded">
                        <v-icon class="upload-icon icon mr-2">sbf-download-cloud</v-icon>
                        <span class="downloaded-text">{{item && item.downloads ? item.downloads : 0}}</span>
                    </div>
                </div>
                <div class="details text-truncate" v-if="$vuetify.breakpoint.smAndUp">
                    <div class="school detail-cell">
                        <v-icon class="scool-icon icon mr-2">sbf-university</v-icon>
                        <span class="detail-name mr-2" v-language:inner>headerDocument_item_school</span>
                        <span class="detail-title">{{item ? item.university: ''}}</span>
                    </div>
                    <div class="class detail-cell">
                        <v-icon class="class-icon icon mr-2">sbf-classes-new</v-icon>
                        <span class="detail-name mr-3" v-language:inner>headerDocument_item_class</span>
                        <span class="detail-title">{{item ? item.course: ''}}</span>

                    </div>
                    <div class="prof detail-cell text-truncate">
                        <v-icon class="prof-icon icon mr-2" v-show=" item && item.professor">sbf-professor</v-icon>
                        <span v-show="item && item.professor" class="detail-name mr-3" v-language:inner>headerDocument_item_prof</span>
                        <span class="detail-title text-truncate">{{item ?  item.professor : ''}}</span>
                    </div>
                </div>
                <div class="views details">
                    <v-layout column fill-height justify-space-between>
                        <v-flex v-show="!isPurchased">
                            <a target="_blank" @click="showPurchaseConfirm()">
                                <div class="buy-action-container">
                                    <div class="buy-text-wrap">
                                        <span class="equals-to-dollar hidden-xs-only">
                                            <span v-language:inner>headerDocument_item_pay_student</span>
                                          </span>
                                        <span class="buy-text-price">
                                            <span class="mobile-buy-text hidden-sm-and-up" v-language:inner>preview_itemActions_buy</span>
                                            {{item && item.price ? item.price.toFixed(2) : '00.00'}}
                                            <span class="sbl-suffix" v-language:inner>app_currency_dynamic</span>
                                        </span>
                                        <span class="equals-to-dollar hidden-xs-only">
                                            <span v-language:inner>preview_price_equals_to</span>
                                          </span>
                                    </div>
                                    <div class="buy-btn-wrap">
                                        <span class="buy-text" v-language:inner>preview_itemActions_buy</span>
                                    </div>
                                </div>
                            </a>
                        </v-flex>
                        <v-flex v-show="isPurchased">
                            <a target="_blank" @click="downloadDoc()">
                                <div class="download-action-container">
                                    <div class="text-wrap">
                                        <span class="download-text" v-language:inner>preview_itemActions_download</span>
                                        <v-icon class="download-icon-mob ml-2" v-if="$vuetify.breakpoint.xsOnly">
                                            sbf-download-cloud
                                        </v-icon>
                                    </div>
                                    <div class="btn-wrap">
                                        <v-icon class="sb-download-icon">sbf-download-cloud</v-icon>
                                    </div>
                                </div>
                            </a>
                        </v-flex>
                    </v-layout>
                </div>
            </v-layout>
            <v-layout row fill-height justify-end class="pt-4" v-if="$vuetify.breakpoint.smAndUp">
                <div class="detail-cell views-cell">
                    <div class="viewed">
                        <v-icon class="views-icon icon mr-2">sbf-views</v-icon>
                        <span class="viewed-text">{{item  ? item.views : ''}}</span>
                    </div>
                    <div class="ml-4 downloaded">
                        <v-icon class="upload-icon icon mr-2">sbf-download-cloud</v-icon>
                        <span class="downloaded-text">{{item? item.downloads: ''}}</span>
                    </div>
                </div>
            </v-layout>
            <div class="details mobile" v-if="!$vuetify.breakpoint.smAndUp">
                <document-details :item="item"></document-details>
            </div>
        </div>
        <sb-dialog :showDialog="true"
                   :popUpType="'purchaseConfirmation'"
                   :isPersistent="true"
                   :activateOverlay="true"
                   :content-class="'confirmation-purchase-dialog'">
            <v-card class="confirm-purchase-card">
                <v-card-title class="confirm-headline">
                    <span v-html="$Ph('preview_about_to_buy', [price, uploaderName])"></span>
                    <!--<span v-language:inner>preview_about_to_buy</span>-->
                    <!--<span>&nbsp;{{doc ? doc.title: ''}}</span>-->
                </v-card-title>
                <v-card-actions class="card-actions">
                    <div class="doc-details">
                        <div class="doc-type">
                            <v-icon class="doc-type-icon">sbf-document-note</v-icon>
                            <span class="doc-type-text">{{docType}}</span>
                        </div>
                        <div class="doc-title">
                            <div>{{itemName}}</div>
                        </div>
                    </div>
                    <div class="purchase-actions">
                        <v-btn flat class="cancel" @click.native="confirmPurchaseDialog = false"><span v-language:inner>preview_cancel</span>
                        </v-btn>
                        <v-btn round class="submit-purchase" @click.native="purchaseDocument(item.id)">
                            <span class="hidden-xs-only" v-language:inner>preview_buy_btn</span>
                            <span class="hidden-sm-and-up text-uppercase"
                                  v-language:inner>preview_itemActions_buy</span>
                        </v-btn>
                    </div>
                </v-card-actions>
            </v-card>

        </sb-dialog>
    </div>
</template>
<script>
    import itemActions from './itemActions.vue';
    import mainHeader from '../helpers/header.vue';
    import { mapGetters, mapActions } from 'vuex';
    import documentDetails from '../results/helpers/documentDetails/documentDetails.vue';
    import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';
    import analyticsService from '../../services/analytics.service';
    import userAvatar from '../helpers/UserAvatar/UserAvatar.vue';
    import userRank from '../helpers/UserRank/UserRank.vue'
    import { LanguageService } from "../../services/language/languageService";

    export default {
        components: {
            mainHeader,
            itemActions,
            documentDetails,
            sbDialog,
            userAvatar,
            userRank
        },
        data() {
            return {
                confirmPurchaseDialog: false,
                isEdgeRtl: global.isEdgeRtl
            }
        },
        methods: {
            ...mapActions([
                'updateLoginDialogState',
                'purchaseAction',
                'updateDownloadsCount'
            ]),
            ...mapGetters(['accountUser']),
            downloadDoc() {
                let isLogedIn = this.accountUser();
                if(!isLogedIn){
                    this.updateLoginDialogState(true)
                    return;
                }
                let url = this.$route.path + '/download';
                let user = this.accountUser();
                let userId = user.id;
                let course = this.item && this.item.course ? this.item.course : '';
                analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_DOWNLOAD', `USER_ID: ${userId}, DOC_ID: ${this.item.id}, DOC_COURSE:${course}`);
                if (!!this.accountUser()) {
                    global.location.href = url;
                    this.updateDownloadsCount()
                } else {
                    this.updateLoginDialogState(true)
                }
            },
            showPurchaseConfirm() {
                let isLogedIn = this.accountUser();
                if (isLogedIn) {
                    this.confirmPurchaseDialog = true;
                    analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_PURCHASE_INTENT');
                } else {
                    this.updateLoginDialogState(true);
                }
            },
            purchaseDocument() {
                let item = this.item;
                this.purchaseAction(item);
                this.confirmPurchaseDialog = false;
            },
            isGuest(){
                return this.accountUser() === null;
            }
        },
        computed: {
            ...mapGetters(['getDocumentDetails']),
            item() {
                if (!!this.getDocumentDetails)
                    return this.getDocumentDetails
            },
            itemName() {
                if (this.item && this.item.name)
                    return this.item.name.replace(/\.[^/.]+$/, "")
            },
            isPurchased: {
                get() {
                    if(this.isGuest){
                        if(!!this.item && !this.item.price){
                            return true;
                        }
                    }
                    return this.item && this.item.isPurchased
                },
                set(val) {
                    return this.item.isPurchased = val
                }

            },
            docType() {
             let self = this;
             return  self.item && self.item.docType ? self.item.docType : LanguageService.getValueByKey("upload_multiple_files_type_default");
            },
            price(){
              if(this.item && this.item.price){
                  return this.item.price
              }
            },
            uploaderName() {
                if (this.item && this.item.user && this.item.user.name)
                    return this.item.user.name
            },
            uploaderID() {
                if (this.item && this.item.user && this.item.user.id)
                    return this.item.user.id
            },
            uploaderScore(){
                if (this.item && this.item.user && this.item.user.score){
                    return this.item.user.score
                }else{
                    return 0
                }
            },
            userImageUrl(){
                if(this.item && this.item.user &&  this.item.user.image && this.item.user.image.length > 1){
                    return `${this.item.user.image}`
                }
                return ''
            },
            uploadDate() {
                if (this.item && this.item.date) {
                    return this.$options.filters.fullMonthDate(this.item.date);
                } else {
                    return ''
                }
            },
        },
        filters: {
            mediumDate: function (value) {
                if (!value) return '';
                let date = new Date(value);
                return date.toLocaleString('en-US', {year: 'numeric', month: 'short', day: 'numeric'});
            }
        },


    }
</script>
<style src="./headerDocument.less" lang="less"></style>