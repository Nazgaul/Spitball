<template>
    <!--<main-header class="elevation-0" :toolbarHeight="$vuetify.breakpoint.mdAndUp ? 140 : 182" layoutClass="header-elements">-->
    <!--<nav class="item-header elevation-0 mt-2" slot="extraHeader">-->
    <!--<div class="item-header-content">-->
    <!--<v-layout row align-center justify-space-between>-->
    <!--<h1 class="item-name">{{item.name}}</h1>-->
    <!--<item-actions></item-actions>-->
    <!--</v-layout>-->
    <!--<v-flex class="item-meta mt-2">-->
    <!--<v-layout row align-center justify-space-between>-->
    <!--<div class="author">{{item.owner}}</div>-->
    <!--<div class="date">{{item.date | mediumDate}}</div>-->
    <!--</v-layout>-->
    <!--</v-flex>-->
    <!--</div>-->
    <!--</nav>-->
    <!--</main-header>-->
    <div class="header-wrap">
        <nav class="item-header doc-header  mt-2" slot="extraHeader">
            <div class="item-header-content">
                <v-layout row align-center justify-space-between>
                    <h1 class="item-name">{{item.name}}</h1>
                    <div class="doc-details">
                        <div class="author">
                        <span class="upload-by">
                            <v-icon class="sb-person mr-2">sbf-person</v-icon>
                            <span class="mr-2">By</span>
                            <span class="name mr-2">{{item.user.name}},</span>
                        </span>
                        </div>
                        <div class="date">
                            {{item.date | mediumDate}}
                        </div>
                    </div>
                    <item-actions></item-actions>
                </v-layout>
                <v-flex class="item-meta mt-2">
                    <v-layout row align-center justify-space-between>
                        <!--<div class="author">{{item.owner}}</div>-->
                        <!--<div class="date">{{item.date | mediumDate}}</div>-->
                    </v-layout>
                </v-flex>
            </div>
        </nav>
        <div class="details-content">
            <v-layout class="details-wrap" row align-center justify-start>
                <div class="doc-type pr-2">
                    <v-icon class="doc-type-icon">sbf-textbook-note</v-icon>
                    <span class="doc-type-text">{{item.docType}}</span>
                </div>
                <div class="details" >
                    <div class="school detail-cell">
                            <v-icon class="scool-icon icon mr-2">sbf-university</v-icon>
                        <span class="detail-name mr-2">School</span>
                        <span class="detail-title">{{item.university}}</span>
                    </div>
                    <div class="class detail-cell">
                        <v-icon class="class-icon icon mr-2">sbf-classes-new</v-icon>
                        <span class="detail-name mr-3">Class</span>
                        <span class="detail-title">{{item.course}}</span>

                    </div>
                    <div class="prof detail-cell">
                        <v-icon class="prof-icon icon mr-2">sbf-professor</v-icon>
                        <span class="detail-name mr-3">Prof.</span>
                        <span class="detail-title">{{item.course}}</span>
                    </div>
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
    import itemActions from './itemActions.vue'
    import mainHeader from '../helpers/header.vue'
    import { mapGetters } from 'vuex'

    export default {
        components: {
            mainHeader,
            itemActions
        },
        computed: {
            ...mapGetters([ 'getDocumentDetails']),
            item(){

                return this.getDocumentDetails
            }

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