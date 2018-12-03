<template>
    <a :target="($vuetify.breakpoint.xsOnly)?'_self':'_blank'"
       @click="(isOurs ? $_spitball($event):'')" :href="url" :class="['d-block', 'note-block',  'type-'+typeID]">
        <v-container class="pa-0"
                     @click="$ga.event('Search_Results', $route.path.slice(1),`#${index+1}_${item.source}`)">
            <v-flex class="wrapper">
                <v-flex grow class="top-row">
                    <div class="type-wrap">
                        <v-icon :class="['type-'+typeID]">{{type ? type.icon : ''}}</v-icon>
                        <span :class="[ 'doc-type-text', 'ml-2', 'type-'+typeID]">{{typeTitle}}</span>
                    </div>
                </v-flex>
                <document-details :item="item"></document-details>
                   <v-flex grow class="data-row">
                    <div class="upvotes-counter">
                        <!--will follow-->
                    </div>
                    <div class="content-wrap">
                        <div class="title-wrap">
                            <p :class="['doc-title', isFirefox ? 'foxLineClamp' : '']" v-line-clamp:13="$vuetify.breakpoint.xsOnly ? 2 : 2 ">
                                <v-icon class="doc mr-2">sbf-document-note</v-icon>{{item.title}}
                            </p>
                        </div>
                        <div class="content-text" v-show="item.snippet">
                            <span v-line-clamp="2">{{item.snippet}}</span>
                        </div>
                    </div>
                   </v-flex>

                <v-flex grow class="doc-details">
                    <div class="author-info-date">
                        <div class="autor">
                            <span v-show="authorName"  v-language:inner>headerDocument_item_by</span>
                            <span>&nbsp;{{authorName}},&nbsp;</span>
                            </div>

                        <span class="date"v-show="uploadDate">{{uploadDate}}</span>
                    </div>
                    <div class="doc-actions-info">
                        <v-icon class="sb-doc-icon mr-1">sbf-views</v-icon>
                        <span class="sb-doc-info views">{{docViews}}</span>
                        <v-icon class="sb-doc-icon mr-1">sbf-download-cloud</v-icon>
                        <span class="sb-doc-info">{{docDownloads}}</span>
                    </div>
                </v-flex>
            </v-flex>
        </v-container>
    </a>
</template>
<script>

    import FlashcardDefault from '../helpers/img/flashcard.svg';
    import AskDefault from '../helpers/img/ask.svg';
    import NoteDefault from '../helpers/img/document.svg';
    import { documentTypes } from "./helpers/uploadFiles/consts.js"
    import documentDetails from "./helpers/documentDetails/documentDetails.vue"

    export default {
        components: {AskDefault, NoteDefault, FlashcardDefault, documentDetails},
        data() {
            return {
                isFirefox: global.isFirefox
            }
        },
        props: {item: {type: Object, required: true}, index: {Number}},
        computed: {

            //change when server will return id of document type and not title of type
            type() {
                let self = this;
                if (!!self.item.type) {
                    return documentTypes.find((single) => {
                        if (single.id.toLowerCase() === self.item.type.toLowerCase()) {
                            return single
                        }
                    })
                } else {
                    return {
                        id: 'document',
                        title: self.item.source,
                        icon: 'sbf-document-note'
                    }
                }
            },
            authorName(){
                if(!!this.item.user){
                    return this.item.user.name
                }
            },
            typeID() {
                if (!!this.type) {
                    return this.type.id || ''
                }
            },
            typeTitle() {
                if (!!this.type) {
                    return this.type.title || ''
                }
            },
            docViews(){
              if(this.item){
                  return this.item.views || 0
              }
            },
            //TODO downloads for now is same as views till server will handle it
            docDownloads(){
                if(this.item){
                    return this.item.views || 0
                }
            },
            uploadDate(){
              if(this.item && this.item.dateTime){
                 return this.$options.filters.fullMonthDate(this.item.dateTime);
              }else{
                  return ''
              }
            },

            isOurs() {
                return this.item.source.includes('Cloudents') || this.item.source.includes('Spitball')
            },
            isCloudents() {
                return this.item.source.includes('Cloudents')
            },
            isSpitball() {
                return this.item.source.includes('Spitball')
            },
            url: function () {
                return this.item.url
            },

        },
        methods: {
            $_spitball(event) {
                event.preventDefault();
                this.$router.push(this.url)
            }
        },
    }
</script>
<style src="./ResultNote.less" lang="less"></style>
