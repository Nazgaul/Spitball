<template>
    <!--to apply border left, for cell based on type, add this to class---- 'type-'+typeID  -->
    <a :target="($vuetify.breakpoint.xsOnly)?'_self':'_blank'"
       @click.native="(isOurs ? $_spitball($event):'')"
       :href="url"
       :class="['d-block', 'note-block']">
        <v-container class="pa-0"
                     @click="$ga.event('Search_Results', $route.path.slice(1),`#${index+1}_${item.source}`)">
            <v-flex class="wrapper">
                <div class="document-header-container">
                    <div class="document-header-large-sagment">
                        <div class="avatar-area">
                            <user-avatar v-if="authorName" :user-name="authorName" :user-id="authorId"/>
                        </div>
                        <div class="rank-date-container">
                            <div class="rank-area">
                                <user-rank :score="userRank"></user-rank>
                            </div>
                            <div class="date-area">{{uploadDate}}</div>
                        </div>
                        <!--<div class="sold-area" v-if="isSold">-->
                        <!--<div class="sold-container">-->
                        <!--<span>SOLD</span>-->
                        <!--<v-icon>sbf-curved-arrow</v-icon>-->
                        <!--</div>-->
                        <!--</div>-->
                    </div>
                    <div class="document-header-small-sagment">
                        <!--<div class="price-area" :class="{'sold': isSold}">-->
                        <!--{{item.price.toFixed(2)}}-->
                        <!--<span>SBL</span>-->
                        <!--</div>-->
                        <div class="menu-area">
                            <v-menu bottom left>
                                <v-btn :depressed="true" @click.prevent slot="activator" icon>
                                    <v-icon>sbf-3-dot</v-icon>
                                </v-btn>
                                <v-list>
                                    <v-list-tile v-for="(action, i) in actions" :key="i" @click>
                                        <v-list-tile-title>{{ action.title }}</v-list-tile-title>
                                    </v-list-tile>
                                </v-list>
                            </v-menu>
                        </div>
                    </div>
                </div>
                <v-flex grow class="top-row">
                    <div class="upvotes-counter">
                          <span class="document-reputation upvote-arrow">
                           <v-icon>sbf-arrow-up</v-icon>
                          </span>
                        <span class="document-reputation document-score">{{item.votes}}</span>
                        <span class="document-reputation downvote-arrow">
                                 <v-icon>sbf-arrow-down</v-icon>
                        </span>
                    </div>
                    <div class="type-wrap">
                        <span :class="[ 'doc-type-text', 'type-'+typeID]">{{typeTitle}}</span>
                        <document-details :item="item"></document-details>
                        <v-flex grow class="data-row">
                            <div :class="['content-wrap', 'type-'+typeID]">
                                <div class="title-wrap">
                                    <p :class="['doc-title', isFirefox ? 'foxLineClamp' : '']"
                                       v-line-clamp:13="$vuetify.breakpoint.xsOnly ? 2 : 2 ">
                                        {{item.title}}
                                    </p>
                                    <v-icon class="doc">sbf-document-note</v-icon>
                                </div>
                                <div class="content-text" v-show="item.snippet">
                                    <span v-line-clamp="2">{{item.snippet}}</span>
                                </div>
                            </div>
                        </v-flex>
                    </div>
                </v-flex>



                <v-flex grow class="doc-details">
                    <div class="doc-actions-info">
                        <v-icon class="sb-doc-icon  mr-1">sbf-download-cloud</v-icon>
                        <span class="sb-doc-info downloads">{{docDownloads}}</span>
                        <v-icon class="sb-doc-icon mr-1">sbf-views</v-icon>
                        <span class="sb-doc-info views">{{docViews}}</span>
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
    import userAvatar from "../helpers/UserAvatar/UserAvatar.vue";
    import userRank from "../helpers/UserRank/UserRank.vue";
    import { documentTypes } from "./helpers/uploadFiles/consts.js"
    import documentDetails from "./helpers/documentDetails/documentDetails.vue"

    export default {
        components: {
            AskDefault,
            NoteDefault,
            FlashcardDefault,
            documentDetails,
            userAvatar,
            userRank

        },
        data() {
            return {
                isFirefox: global.isFirefox,
                actions: [{title: "Flag"}],
            }
        },
        props: {item: {type: Object, required: true}, index: {Number}},
        computed: {
            userRank() {
                if(!!this.item.user){
                    return this.item.user.score
                }
            },
            documentUpvotes() {
                return Math.floor(Math.random() * 100);
            },
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
            authorName() {
                if (!!this.item.user) {
                    return this.item.user.name
                }
            },
            authorId() {
                if (!!this.item && !!this.item.user && !!this.item.user.id) {
                    return this.item.user.id
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
            docViews() {
                if (this.item) {
                    return this.item.views || 0
                }
            },
            docDownloads() {
                if (this.item) {
                    return this.item.downloads || 0
                }
            },
            uploadDate() {
                if (this.item && this.item.dateTime) {
                    return this.$options.filters.fullMonthDate(this.item.dateTime);
                } else {
                    return ''
                }
            },

            isOurs() {
                if (this.item && this.item.source)
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
                this.$router.push(this.url);
                setTimeout(() => {
                    if (this.item && this.item.views) {
                        this.item.views = this.item.views + 1;
                    }
                }, 100)
            }
        },

    }
</script>
<style src="./ResultNote.less" lang="less"></style>
