<template>
    <div class="header-wrap">
        <nav class="item-header doc-header" slot="extraHeader">
            <div class="item-header-content">
                <v-layout row align-center justify-space-between class="wrap-doc-name">
                    <h1 class="item-name" >{{itemName}} <span class="doc-extension" v-show="item && item.extension">({{item ? item.extension : ''}})</span>
                    </h1>
                    <div class="doc-details">
                        <div class="author">
                        <span class="upload-by">
                            <v-icon class="sb-person mr-1" v-if="$vuetify.breakpoint.smAndUp">sbf-person</v-icon>
                            <span v-if="$vuetify.breakpoint.smAndUp" class="mr-2" v-language:inner>headerDocument_item_by</span>
                            <span class="name mr-2">{{uploaderName}},</span>
                        </span>
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
                    <v-icon class="doc-type-icon">{{doc ? doc.icon : 'sbf-document-note'}}</v-icon>
                    <span class="doc-type-text">{{doc ? doc.title: ''}}</span>
                </div>
                <div class="detail-cell views-cell" v-if="$vuetify.breakpoint.smAndDown">
                    <div class="viewed">
                        <v-icon class="views-icon icon mr-2">sbf-views</v-icon>
                        <span class="viewed-text">{{item.views}}</span>
                    </div>
                    <div class="ml-4 downloaded">
                        <v-icon class="upload-icon icon mr-2">sbf-download-cloud</v-icon>
                        <span class="downloaded-text">{{item.downloads}}</span>
                    </div>
                </div>
                <div class="details" v-if="$vuetify.breakpoint.smAndUp">
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
                    <div class="prof detail-cell">
                        <v-icon class="prof-icon icon mr-2" v-show=" item && item.professor">sbf-professor</v-icon>
                        <span v-show="item && item.professor" class="detail-name mr-3" v-language:inner>headerDocument_item_prof</span>
                        <span class="detail-title">{{item ?  item.professor : ''}}</span>
                    </div>
                </div>
                <div class="views details">
                    <v-layout column fill-height justify-space-between>
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
                        <v-flex v-show="!isPurchased">
                            <a target="_blank" @click="showPurchaseConfirm()">
                                <div class="buy-action-container">
                                    <div class="buy-text-wrap">
                                        <span class="buy-text-price">
                                            <span class="mobile-buy-text hidden-sm-and-up"  v-language:inner>preview_itemActions_buy</span>
                                            {{item ? item.price : ''}}
                                            <span class="sbl-suffix">SBL</span>
                                        </span>
                                        <span class="equals-to-dollar hidden-xs-only">
                                            <span v-language:inner>preview_price_equals_to</span>
                                            {{40 | dollarVal}}$</span>
                                    </div>
                                    <div class="buy-btn-wrap">
                                        <span class="buy-text" v-language:inner>preview_itemActions_buy</span>
                                    </div>
                                </div>
                            </a>
                        </v-flex>
                    </v-layout>
                </div>
            </v-layout>
            <v-layout row fill-height justify-end class="pt-4" v-if="$vuetify.breakpoint.smAndUp">
                <div class="detail-cell views-cell" >
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
        <sb-dialog :showDialog="confirmPurchaseDialog"
                   :popUpType="'purchaseConfirmation'"
                   :isPersistent="true"
                   :activateOverlay="true"
                   :content-class="'confirmation-purchase-dialog'">
            <v-card class="confirm-purchase-card">
                <v-card-title class="confirm-headline">
                    <span v-language:inner>preview_about_to_buy</span>
                    <span>{{doc ? doc.title: ''}}</span>
                </v-card-title>
                <v-card-actions class="card-actions">
                    <div class="doc-details">
                        <div class="doc-type">
                            <v-icon class="doc-type-icon">{{doc ? doc.icon : 'sbf-document-note'}}</v-icon>
                            <span class="doc-type-text">{{doc ? doc.title: ''}}</span>
                        </div>
                        <div class="doc-title">
                            <span v-line-clamp="1">{{itemName  ? itemName : ''}}</span>
                        </div>
                    </div>
                    <div class="purchase-actions">
                        <v-btn flat class="cancel" @click.native="confirmPurchaseDialog = false"><span>Cancel</span>
                        </v-btn>
                        <v-btn round class="submit-purchase" @click.native="purchaseDocument(item.id)">
                            <span class="hidden-xs-only" v-language:inner>preview_buy_download_btn</span>
                            <span class="hidden-sm-and-up text-uppercase" v-language:inner>preview_itemActions_buy</span>
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
    import { documentTypes } from '../results/helpers/uploadFiles/consts';
    import documentDetails from '../results/helpers/documentDetails/documentDetails.vue';
    import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue'

    export default {
        components: {
            mainHeader,
            itemActions,
            documentDetails,
            sbDialog
        },
        data() {
            return {
                 confirmPurchaseDialog: false
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
                let url = this.$route.path + '/download';
                if (!!this.accountUser()) {
                    global.location.href = url;
                    this.updateDownloadsCount()
                } else {
                    this.updateLoginDialogState(true)
                }
            },
            showPurchaseConfirm() {
                this.confirmPurchaseDialog = true;
            },
            purchaseDocument() {
                let item = this.item;
                this.purchaseAction(item);
                this.confirmPurchaseDialog = false;
            }
        },
        computed: {
            ...mapGetters(['getDocumentDetails']),
            item() {
                if(!!this.getDocumentDetails)
                return this.getDocumentDetails
            },
            itemName() {
                if (this.item && this.item.name)
                    return this.item.name.replace(/\.[^/.]+$/, "")
            },
            isPurchased:{
                get(){
                    return this.item && this.item.isPurchased
                },
                set(val){
                    return this.item.isPurchased = val
                }

            },
            doc() {
                let self = this;
                if (self.item && self.item.docType && documentTypes) {
                    return self.item.docType = documentTypes.find((singleType) => {
                        if (singleType.id.toLowerCase() === self.item.docType.toLowerCase()) {
                            return singleType
                        }
                    })
                }
            },
            uploaderName() {
                if (this.item && this.item.user && this.item.user.name)
                    return this.item.user.name
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