<template>
    <div class="doc-preview-container">
        <doc-header></doc-header>
        <div class="item document-wrap">
            <div class="item-content">
                <span class="img-placeholder-text" v-if="isPlaceholder"v-language:inner>preview_doc_placeholder_text</span>
                <!--<v-carousel class="mobile-docs-carousel" :cycle="false" hide-delimiters hide-controls  v-if="$vuetify.breakpoint.smAndDown" height="100%"-->
                <!--style="max-height: 300px;">-->
                <!--<v-carousel-item v-for="(page, index) in preview">-->
                <!--<div class="page text-xs-center" :key="index">-->
                <!--<component class="page-content elevation-1" :is="currentComponent" :src="page"-->
                <!--:class="item.contentType+'-content'"></component>-->
                <!--</div>-->
                <!--</v-carousel-item>-->
                <!--</v-carousel>-->
                <!--<div v-else class="page text-xs-center" v-for="(page, index) in preview" :key="index">-->
                <div class="page text-xs-center" style="position: relative;" v-for="(page, index) in preview" :key="index">
                    <component class="page-content elevation-1" :is="currentComponent" :src="page"
                               :class="item.contentType+'-content'"></component>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import { mapActions, mapGetters } from 'vuex';
    import docHeader from "./headerDocument.vue";

    export default {
        components: {
            docHeader: docHeader
        },
        data() {
            var actions = [
                // {
                //     id: 'info',
                //     click: function () {
                //     }
                // },
                // {id: 'download'},
                // {id: 'print'},
                // {id: 'more'},
                {id: 'close'}
            ];
            return {
                actions,
            }
        },
        props: {
            id: 0
        },

        methods: {
            ...mapActions([
                'setDocumentPreview',
                'clearDocPreview',
                'updateLoginDialogState'
            ]),
            showBuyFull(index){
                if(index ===0){
                    return this.isPlaceholder
                }
            }

        },

        computed: {
            ...mapGetters(["accountUser", "getDocumentItem"]),

            currentComponent() {
                if (this.item && this.item.contentType) {
                    return this.item.contentType === "html" ? "iframe" : "img";
                    // if (['link', 'text'].find((x) => x === type.toLowerCase())) return 'iframe'
                }
            },
            item() {
                return this.getDocumentItem;
            },
            preview() {
                if (!!this.item && !!this.item.preview) {
                    return this.item.preview
                }
            },
            isPlaceholder() {
                if (!!this.item && !!this.item.details && this.item.details.isPlaceholder) {
                    return this.item.details.isPlaceholder
                }

            },
        },

        created() {
            this.setDocumentPreview({type: 'item', id: this.id})
                .then((response) => {

                });
        },
        //clean store document item on destroy component
        beforeDestroy() {
            this.clearDocPreview();
        }
    }
</script>
<style src="./item.less" lang="less"></style>