<template>
    <div class="header-wrap">
        <nav class="item-header doc-header  mt-2" slot="extraHeader">
            <div class="item-header-content">
                <v-layout row align-center justify-space-between>
                    <h1 class="item-name">{{item ? item.name : ''}}</h1>
                    <div class="doc-details">
                        <div class="author">
                        <span class="upload-by">
                            <v-icon class="sb-person mr-2">sbf-person</v-icon>
                            <span class="mr-2" v-language:inner>item_by</span>
                            <span class="name mr-2">{{uploaderName}},</span>
                        </span>
                        </div>
                        <div class="date">
                            {{item.date | mediumDate}}
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
                <div class="details" v-if="$vuetify.breakpoint.smAndUp">
                    <div class="school detail-cell">
                        <v-icon class="scool-icon icon mr-2">sbf-university</v-icon>
                        <span class="detail-name mr-2" v-language:inner>item_school</span>
                        <span class="detail-title">{{item ? item.university: ''}}</span>
                    </div>
                    <div class="class detail-cell">
                        <v-icon class="class-icon icon mr-2">sbf-classes-new</v-icon>
                        <span class="detail-name mr-3" v-language:inner>item_class</span>
                        <span class="detail-title">{{item ? item.course: ''}}</span>

                    </div>
                    <div class="prof detail-cell">
                        <v-icon class="prof-icon icon mr-2">sbf-professor</v-icon>
                        <span class="detail-name mr-3" v-language:inner>item_prof</span>
                        <span class="detail-title">{{item.professor}}</span>
                    </div>
                </div>
                <div class="details mobile" v-else>
                    <document-details :item="item"></document-details>
                </div>
                <div class="views details">
                    <div class="detail-cell views-cell">
                        <div class="viewed">
                            <v-icon class="views-icon icon mr-2">sbf-views</v-icon>
                            <span class="viewed-text">{{item.views}}</span>
                        </div>
                        <div class="ml-4 downloaded">
                            <v-icon class="upload-icon icon mr-2">sbf-download-cloud</v-icon>
                            <span class="downloaded-text">{{item.views}}</span>
                        </div>

                    </div>
                </div>

            </v-layout>
        </div>

    </div>
</template>

<script>
    import itemActions from './itemActions.vue';
    import mainHeader from '../helpers/header.vue';
    import { mapGetters } from 'vuex';
    import { documentTypes } from '../results/helpers/uploadFiles/consts';
    import documentDetails from '../results/helpers/documentDetails/documentDetails.vue'

    export default {
        components: {
            mainHeader,
            itemActions,
            documentDetails
        },
        computed: {
            ...mapGetters(['getDocumentDetails']),
            item() {
                return this.getDocumentDetails
            },
            doc() {
                let self = this;
                if(self.item && self.item.docType) {
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