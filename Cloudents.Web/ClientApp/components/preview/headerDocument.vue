<template>
    <div class="header-wrap">
        <nav class="item-header doc-header" slot="extraHeader">
            <div class="item-header-content">
                <v-layout row align-center justify-space-between class="wrap-doc-name">
                    <h1 class="item-name">{{itemName}} <span class="doc-extension" v-show="item && item.extension">({{item ? item.extension : ''}})</span></h1>
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
                        <span class="downloaded-text">{{item.views}}</span>
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
                    <v-layout column  fill-height justify-space-between>
                        <v-flex>
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
            <v-layout row  fill-height justify-end>
            <div class="detail-cell views-cell" v-if="$vuetify.breakpoint.smAndUp">
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
    </div>
</template>
<script>
    import itemActions from './itemActions.vue';
    import mainHeader from '../helpers/header.vue';
    import { mapGetters, mapActions } from 'vuex';
    import { documentTypes } from '../results/helpers/uploadFiles/consts';
    import documentDetails from '../results/helpers/documentDetails/documentDetails.vue'

    export default {
        components: {
            mainHeader,
            itemActions,
            documentDetails
        },
        methods: {
            ...mapActions([
                'updateLoginDialogState',
            ]),
            ...mapGetters(['accountUser']),
            downloadDoc() {
                let url = this.$route.path + '/download';
                if (!!this.accountUser()) {
                    global.location.href = url;
                } else {
                    this.updateLoginDialogState(true)
                }
            },
        },
        computed: {
            ...mapGetters(['getDocumentDetails']),
            item() {
                return this.getDocumentDetails
            },
            itemName(){
                if(this.item && this.item.name)
                return this.item.name.replace(/\.[^/.]+$/, "")
            },

            doc() {
                let self = this;
                if(self.item && self.item.docType && documentTypes) {
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
             uploadDate(){
              if(this.item && this.item.date){
                 return this.$options.filters.fullMonthDate(this.item.date);
              }else{
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