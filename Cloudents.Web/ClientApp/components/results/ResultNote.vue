<template>
    <a
            :target="($vuetify.breakpoint.xsOnly || isOurs)?'_self':'_blank'"
            @click.native="(isOurs ? $_spitball($event, url): '')"
            :href="url"
            id="sb-link"
            :class="['d-block', 'note-block']"
    >
        <v-container
                class="pa-0"
                @click="$ga.event('Search_Results', $route.path.slice(1),`#${index+1}_${item.source}`)"
        >
            <v-flex class="wrapper">
                <div class="document-header-container">
                    <div class="document-header-large-sagment">
                        <div class="avatar-area">
                            <user-avatar v-if="authorName" :user-name="authorName" :user-id="authorId"/>
                        </div>
                        <div class="rank-date-container">
                            <div :class="['rank-area', !isOurs ? 'thirdParty' : '']">
                                <user-rank v-if="isOurs" :score="userRank"></user-rank>
                                <span class="doc-type-text type-document" v-else>{{item.source}}</span>
                            </div>
                            <div class="date-area">{{uploadDate}}</div>
                        </div>
                    </div>
                    <div class="document-header-small-sagment">
                        <!--<div class="price-area" :class="{'sold': isSold}">-->
                        <!--{{item.price.toFixed(2)}}-->
                        <!--<span>SBL</span>-->
                        <!--</div>-->
                        <div class="menu-area">
                            <v-menu bottom left content-class="card-user-actions">
                                <v-btn :depressed="true" @click.prevent slot="activator" icon>
                                    <v-icon>sbf-3-dot</v-icon>
                                </v-btn>
                                <v-list>
                                    <v-list-tile :disabled="item.isDisabled() || !isOurs" v-for="(item, i) in actions"
                                                 :key="i">
                                        <v-list-tile-title @click="item.action()">{{ item.title }}</v-list-tile-title>
                                    </v-list-tile>
                                </v-list>
                            </v-menu>
                        </div>
                    </div>
                </div>
                <v-flex grow class="top-row">
                    <div class="upvotes-counter" v-if="item.votes !== null">
            <span class="document-reputation upvote-arrow" @click.prevent="upvoteDocument()">
              <v-icon :class="{'voted': item.upvoted}">sbf-arrow-up</v-icon>
            </span>
                        <span class="document-reputation document-score" :dir="isRtl ? `ltr` : ''">{{item.votes}}</span>
                        <span class="document-reputation downvote-arrow" @click.prevent="downvoteDocument()">
              <v-icon :class="{'voted': item.downvoted}">sbf-arrow-down</v-icon>
            </span>
                    </div>
                    <div :class="['type-wrap', !isOurs ? 'thirdPartyItem' : '']">
                        <span v-if="isOurs" :class="[ 'doc-type-text', 'type-'+typeID]">{{typeTitle}}</span>
                        <document-details :item="item"></document-details>
                        <v-flex grow :class="['data-row', !isOurs ? 'thirdParty' : '']">
                            <div :class="['content-wrap', 'type-'+typeID]">
                                <div class="title-wrap">
                                    <p
                                            :class="['doc-title', isFirefox ? 'foxLineClamp' : '']"
                                            v-line-clamp:20="$vuetify.breakpoint.xsOnly ? 2 : 2 "
                                    >{{item.title}}</p>
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
                        <v-icon class="sb-doc-icon mr-1">sbf-download-cloud</v-icon>
                        <span class="sb-doc-info downloads">{{docDownloads}}</span>
                        <v-icon class="sb-doc-icon mr-1">sbf-views</v-icon>
                        <span class="sb-doc-info views">{{docViews}}</span>
                    </div>
                </v-flex>
            </v-flex>
        </v-container>
        <sb-dialog
                :showDialog="showReport"
                :maxWidth="'438px'"
                :popUpType="'reportDialog'"
                :content-class="`reportDialog ${isRtl? 'rtl': ''}` "
        >
            <report-item :closeReport="closeReportDialog" :itemType="item.template" :itemId="itemId"></report-item>
        </sb-dialog>
    </a>
</template>
<script>
    import FlashcardDefault from "../helpers/img/flashcard.svg";
    import AskDefault from "../helpers/img/ask.svg";
    import NoteDefault from "../helpers/img/document.svg";
    import userAvatar from "../helpers/UserAvatar/UserAvatar.vue";
    import userRank from "../helpers/UserRank/UserRank.vue";
    import { documentTypes } from "./helpers/uploadFiles/consts.js";
    import documentDetails from "./helpers/documentDetails/documentDetails.vue";
    import sbDialog from "../wrappers/sb-dialog/sb-dialog.vue";
    import reportItem from "./helpers/reportItem/reportItem.vue";
    import { mapGetters, mapActions } from "vuex";
    import { LanguageService } from "../../services/language/languageService";

    export default {
        components: {
            AskDefault,
            NoteDefault,
            FlashcardDefault,
            documentDetails,
            sbDialog,
            reportItem,
            userAvatar,
            userRank
        },
        data() {
            return {
                isFirefox: global.isFirefox,
                actions: [
                    {
                        title: LanguageService.getValueByKey("questionCard_Report"),
                        action: this.reportItem,
                        isDisabled: this.isDisabled,
                    }
                ],
                itemId: 0,
                showReport: false,
                isRtl: global.isRtl
            };
        },
        props: {item: {type: Object, required: true}, index: {Number}},
        computed: {
            userRank() {
                if (!!this.item.user) {
                    return this.item.user.score;
                }
            },

            type() {
                let self = this;
                if (!!self.item.type) {
                    return documentTypes.find(single => {
                        if (single.id.toLowerCase() === self.item.type.toLowerCase()) {
                            return single;
                        }
                    });
                } else {
                    return {
                        id: "document",
                        title: self.item.source,
                        icon: "sbf-document-note"
                    };
                }
            },
            authorName() {
                if (!!this.item.user) {
                    return this.item.user.name;
                }
            },
            authorId() {
                if (!!this.item && !!this.item.user && !!this.item.user.id) {
                    return this.item.user.id;
                }
            },
            typeID() {
                if (!!this.type) {
                    return this.type.id || "";
                }
            },
            typeTitle() {
                if (!!this.type) {
                    return this.type.title || "";
                }
            },
            docViews() {
                if (this.item) {
                    return this.item.views || 0;
                }
            },
            docDownloads() {
                if (this.item) {
                    return this.item.downloads || 0;
                }
            },
            uploadDate() {
                if (this.item && this.item.dateTime) {
                    return this.$options.filters.fullMonthDate(this.item.dateTime);
                } else {
                    return "";
                }
            },
            isOurs() {
                let ours;
                if (this.item && this.item.source) {
                    ours = this.item.source.toLowerCase().includes("cloudents") || this.item.source.toLowerCase().includes("spitball");
                }
                return ours
            },

            url() {
                return this.item.url;
            },
        },
        methods: {
            ...mapActions(["documentVote", "updateLoginDialogState"]),
            ...mapGetters(["accountUser"]),
            cardOwner() {
                let userAccount = this.accountUser();
                if (userAccount && this.item.user) {
                    return userAccount.id === this.item.user.id; // will work once API call will also return userId
                } else {
                    return false
                }
            },
            isDisabled(){
                let isOwner, account, notEnough;
                isOwner = this.cardOwner();
                account = this.accountUser();
                if(account && account.balance){
                    notEnough = account.balance < 400
                }
                if(isOwner || !account || notEnough){
                    return true
                }
            },
              reportItem() {
                this.itemId = this.item.id;
                this.showReport = !this.showReport;
            },
            closeReportDialog() {
                this.showReport = false;
            },
            $_spitball(event) {
                console.log('our event',event);
                event.preventDefault();
                this.$router.push(this.url);
                setTimeout(() => {
                    if (this.item && this.item.views) {
                        this.item.views = this.item.views + 1;
                    }
                }, 100);
            },

            isAuthUser() {
                let user = this.accountUser();
                if (user == null) {
                    this.updateLoginDialogState(true);
                    return false;
                }
                return true;
            },
            upvoteDocument() {
                if (this.isAuthUser()) {
                    let type = "up";
                    if (!!this.item.upvoted) {
                        type = "none";
                    }
                    let data = {
                        type,
                        id: this.item.id
                    };
                    this.documentVote(data);
                }
            },
            downvoteDocument() {
                if (this.isAuthUser()) {
                    let type = "down";
                    if (!!this.item.downvoted) {
                        type = "none";
                    }
                    let data = {
                        type,
                        id: this.item.id
                    };
                    this.documentVote(data);
                }
            }
        }
    };
</script>
<style src="./ResultNote.less" lang="less"></style>
