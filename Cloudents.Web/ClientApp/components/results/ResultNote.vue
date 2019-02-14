<template>
    <router-link
               :class="['d-block', 'note-block']"
               :to="url">
        <v-container
                class="pa-0"
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
                    </div>
                    <div class="document-header-small-sagment">
                        <div v-show="item.price" class="price-area" :class="{'isPurchased': isPurchased}">
                            <bdi>
                                {{item.price ? item.price.toFixed(2): ''}}
                                <span v-language:inner>app_currency_dynamic</span>
                            </bdi>
                        </div>
                        <div v-show="!item.price" class="price-area" :class="{'isPurchased': isPurchased}"
                             v-language:inner>resultNote_free
                        </div>
                        <div class="menu-area">
                            <v-menu bottom left content-class="card-user-actions" v-model="showMenu">
                                <v-btn :depressed="true" @click.native.stop.prevent="showReportOptions()"
                                       slot="activator" icon>
                                    <v-icon>sbf-3-dot</v-icon>
                                </v-btn>
                                <v-list>
                                    <v-list-tile v-show="item.isVisible(item.visible)"
                                                 :disabled="item.isDisabled()" v-for="(item, i) in actions"
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
            <span class="document-reputation upvote-arrow" @click.stop.prevent="upvoteDocument()">
              <v-icon :class="{'voted': item.upvoted}">sbf-arrow-up</v-icon>
            </span>
                        <span class="document-reputation document-score" :dir="isRtl ? `ltr` : ''">{{item.votes}}</span>
                        <span class="document-reputation downvote-arrow" @click.stop.prevent="downvoteDocument()">
              <v-icon :class="{'voted': item.downvoted}">sbf-arrow-down</v-icon>
            </span>
                    </div>
                    <div class="type-wrap">
                        <!--<span :class="[ 'doc-type-text']">{{type}}</span>-->
                        <!--<document-details :item="item"></document-details>-->
                        <v-flex grow class="data-row">
                            <div :class="['content-wrap']">
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
                        <document-details :item="item" class="document-details"></document-details>
                    </div>
                </v-flex>

                <v-flex grow class="doc-details">
                    <div class="doc-actions-info">
                        <span :class="[ 'doc-type-text mr-3']">{{type}}</span>
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
        <sb-dialog
                :showDialog="priceDialog"
                :maxWidth="'438px'"
                :popUpType="'priceUpdate'"
                :onclosefn="closeNewPriceDialog"
                :activateOverlay="true"
                :isPersistent="true"
                :content-class="`priceUpdate ${isRtl? 'rtl': ''}`">
            <v-card class="price-change-wrap">
                <v-flex align-center justify-center class="relative-pos">
                    <div class="title-wrap">
                        <span class="change-title" v-language:inner>resultNote_change_for</span>
                        <span class="change-title" style="max-width: 150px;"
                              v-line-clamp="1">&nbsp;"{{item.title}}"</span>
                    </div>
                    <div class="input-wrap d-flex row align-center justify-center">
                        <div :class="['price-wrap', isRtl ? 'reversed' : '']">
                            <!--updating document obj inside -->
                            <vue-numeric  :currency="currentCurrency"
                                          class="sb-input-upload-price"
                                          :minus="false"
                                          :min="0"
                                          :precision="2"
                                          :max="99"
                                          :currency-symbol-position="'suffix'"
                                          separator=","
                                          v-model="newPrice"></vue-numeric>


                            <!--<sbl-currency v-model="newPrice"-->
                                          <!--class="sb-input-upload-price">-->
                            <!--</sbl-currency>-->
                            <!--<div class="sbl-suffix" v-language:inner>app_currency_dynamic</div>-->
                        </div>
                    </div>
                </v-flex>
                <div class="change-price-actions">
                    <button @click="closeNewPriceDialog()" class="cancel mr-2"><span v-language:inner>resultNote_action_cancel</span>
                    </button>
                    <button @click="submitNewPrice()" class="change-price"><span v-language:inner>resultNote_action_apply_price</span>
                    </button>
                </div>
            </v-card>
        </sb-dialog>
    </router-link>
</template>
<script>
    // v14 do we need this ?
    import FlashcardDefault from "../helpers/img/flashcard.svg";
    import AskDefault from "../helpers/img/ask.svg";
    import NoteDefault from "../helpers/img/document.svg";
    //
    import userAvatar from "../helpers/UserAvatar/UserAvatar.vue";
    import userRank from "../helpers/UserRank/UserRank.vue";
    import documentDetails from "./helpers/documentDetails/documentDetails.vue";
    import sbDialog from "../wrappers/sb-dialog/sb-dialog.vue";
    import reportItem from "./helpers/reportItem/reportItem.vue";
    import { mapGetters, mapActions, mapMutations } from "vuex";
    import { LanguageService } from "../../services/language/languageService";
    import SbInput from "../question/helpers/sbInput/sbInput";
    import sblCurrency from "./helpers/sblCurrency/sbl-currency.vue";
    import documentService from "../../services/documentService";


    export default {
        components: {
            SbInput,
            AskDefault,
            NoteDefault,
            FlashcardDefault,
            documentDetails,
            sbDialog,
            reportItem,
            userAvatar,
            userRank,
            sblCurrency

        },
        data() {
            return {
                isFirefox: global.isFirefox,
                currentCurrency: LanguageService.getValueByKey("app_currency_dynamic"),
                actions: [
                    {
                        title: LanguageService.getValueByKey("questionCard_Report"),
                        action: this.reportItem,
                        isDisabled: this.isDisabled,
                        isVisible: this.isVisible,
                        visible: true,
                    },
                    {
                        title: LanguageService.getValueByKey("resultNote_change_price"),
                        action: this.showPriceChangeDialog,
                        isDisabled: this.isOwner,
                        isVisible: this.isVisible,
                        icon: 'sbf-delete',
                        visible: true,
                    },
                    {
                        title: LanguageService.getValueByKey("resultNote_action_delete_doc"),
                        action: this.deleteDocument,
                        isDisabled: this.isOwner,
                        isVisible: this.isVisible,
                        visible: true,
                    },
                ],
                itemId: 0,
                showReport: false,
                isRtl: global.isRtl,
                showMenu: false,
                priceDialog: false,
                newPrice: this.item.price ? this.item.price : 0,
                rules: {
                    required: value => !!value || 'Required.',
                    max: value => value.$options.filter <= 1000 || 'max is 1000',
                }
            };
        },

        props: {item: {type: Object, required: true}, index: {Number}},
        computed: {
            isProfile() {
                return this.$route.name === "profile"
            },
            userRank() {
                if (!!this.item.user) {
                    return this.item.user.score;
                }
            },
            isPurchased() {
                return this.item.isPurchased
            },
            type() {
                return this.item.type || ''

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
            docViews() {
                if (this.item) {
                    return this.item.views;
                }
            },
            docDownloads() {
                if (this.item) {
                    return this.item.downloads;
                }
            },
            uploadDate() {
                if (this.item && this.item.dateTime) {
                    return this.$options.filters.fullMonthDate(this.item.dateTime);
                } else {
                    return "";
                }
            },
            url() {
                return this.item.url;
            },
            isOurs() {
                let ours;
                if (this.item && this.item.source) {
                    ours = this.item.source.toLowerCase().includes("cloudents");
                }
                return ours
            },
        },
        methods: {
            ...mapMutations({updateLoading: "UPDATE_LOADING", updateSearchLoading: "UPDATE_SEARCH_LOADING",}),
            ...mapActions([
                "documentVote",
                "updateLoginDialogState",
                "updateToasterParams",
                "syncProfile"
            ]),
            ...mapGetters(["accountUser"]),
            cardOwner() {
                let userAccount = this.accountUser();
                if (userAccount && this.item.user) {
                    return userAccount.id === this.item.user.id; // will work once API call will also return userId
                } else {
                    return false
                }
            },
            updateItemPrice(val) {
                if (val || val ===0) {
                    return this.item.price = val;
                }
            },
            isVisible(val) {
                return val
            },
            showEvent(event) {
                console.log(event)
            },
            submitNewPrice() {
                let data = {id: this.item.id, price: this.newPrice};
                let self = this;
                documentService.changeDocumentPrice(data).then(
                    (success) => {
                        self.updateItemPrice(self.newPrice);
                        self.closeNewPriceDialog();
                    },
                    (error) => {
                        console.error('erros change price', error)
                    });
            },
            closeNewPriceDialog() {
                this.priceDialog = false;
            },
            isOwner() {
                // return true
                let owner = this.cardOwner();
                return !owner
            },
            showPriceChangeDialog() {
                this.priceDialog = true;

            },
            isDisabled() {
                let isOwner, account, notEnough;
                isOwner = this.cardOwner();
                account = this.accountUser();
                if (isOwner || !account || notEnough) {
                    return true
                }
            },
            reportItem() {
                this.itemId = this.item.id;
                this.showReport = !this.showReport;
            },
            //check if profile and refetch data after doc deleted
            updateProfile() {
                let account, id;
                if (this.isProfile) {
                    account = this.accountUser();
                    id = account.id ? account.id : '';
                    this.syncProfile(id);
                }
            },
            deleteDocument() {
                documentService.deleteDoc(this.item.id).then(
                    (success) => {
                        this.updateToasterParams({
                            toasterText: LanguageService.getValueByKey("resultNote_deleted_success"),
                            showToaster: true,
                        });
                        this.updateProfile();

                    },
                    (error) => {
                        this.updateToasterParams({
                            toasterText: LanguageService.getValueByKey("resultNote_error_delete"),
                            showToaster: true,
                        });


                    }
                )
            },
            closeReportDialog() {
                this.showReport = false;
            },
            showReportOptions() {
                this.showMenu = true
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
