<template>
    <div>
        <v-toolbar app :fixed="!isMobile" :height="height" class="header">

            <v-layout column :class="layoutClass?layoutClass:'header-elements'" class="mx-0">
                <div class="main">
                    <v-flex class="line top">
                        <v-layout row>
                            <!-- :class="{'auth-user':isAuthUser}" -->
                            <v-toolbar-title>
                                <router-link class="logo-link" to="/ask">
                                    <app-logo class="logo"></app-logo>
                                </router-link>
                            </v-toolbar-title>
                            <v-toolbar-items>
                                <search-input v-if="$vuetify.breakpoint.smAndUp" :user-text="userText"
                                              :placeholder="this.$options.placeholders[currentSelection]"
                                              :submit-route="submitRoute"></search-input>
                                <!--<form v-if="$vuetify.breakpoint.mdAndUp" @submit.prevent="submit">-->
                                <!--<v-text-field type="search" light solo class="search-b" :placeholder="placeholders[currentSelection]" v-model="msg" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>-->
                                <!--<div v-for="(s,index) in suggestList">{{s}}</div>-->
                                <!--</form>-->
                                <v-spacer v-if="$vuetify.breakpoint.xsOnly"></v-spacer>
                                <div class="settings-wrapper d-flex align-center">
                                    <!-- <v-menu bottom left>
        <v-btn class="share-btn" icon slot="activator">
            <share-icon></share-icon>
        </v-btn>
        <v-list class="sharing-list">
            <v-list-tile @click="facebookShare">
                <v-list-tile-action>
                    <facebook-icon class="facebook-icon"></facebook-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>
                        <span>Facebook</span>
                    </v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
            <v-list-tile @click="twitterShare">
                <v-list-tile-action>
                    <twitter-icon class="twitter-icon"></twitter-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>Twitter</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
            <div @click="$ga.social('Whatsapp', 'Share')">
            <v-list-tile :href="whatsappLink()" class="btn-copy hidden-sm-and-up" v-if="$vuetify.breakpoint.xs">
                <v-list-tile-action>
                    <whatsapp-icon></whatsapp-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>Whatsapp</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
            </div>
            <v-list-tile @click="copyToClipboard" class="btn-copy">
                <v-list-tile-action>
                    <copy-link-icon></copy-link-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>Copy link</v-list-tile-title>
                    <input type="text" id="input-url" value="Copied!">
                </v-list-tile-content>
            </v-list-tile>
        </v-list>
    </v-menu>
    <v-menu bottom left>
        <v-btn icon slot="activator">
            <v-icon>sbf-3-dot</v-icon>
        </v-btn>
        <v-list class="settings-list">
            <v-list-tile @click="$_currentClick(item)" v-for="(item,index) in settingMenu" :key="index" :id="item.id">
                <v-list-tile-content>
                    <v-list-tile-title>{{item.id==='university'&&getUniversityName?getUniversityName:item.name}}</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
        </v-list>
    </v-menu> -->
                                    <!--<div class="header-comments" v-if="loggedIn && !$vuetify.breakpoint.smAndDown">-->
                                    <!--<router-link :to="{name:'conversations'}">-->
                                    <!--<v-icon>sbf-comment</v-icon>-->
                                    <!--<span class="red-counter" v-if="unreadMessages">{{unreadMessages}}</span> -->
                                    <!--</router-link>                                    -->
                                    <!--</div>-->

                                    <router-link to="/wallet" class="header-wallet" v-if="loggedIn">
                                        <span class="bold">{{accountUser.balance | currencyLocalyFilter}} SBL</span>
                                        <span>$ {{accountUser.balance | dollarVal}}</span>
                                    </router-link>


                                    <div class="header-rocket" v-if="loggedIn">
                                        <v-menu bottom left offset-y>
                                            <!--<v-btn icon slot="activator" @click.native="drawer = !drawer">-->
                                            <!--<v-icon>sbf-rocket</v-icon>-->
                                            <!--</v-btn>-->
                                            <user-avatar slot="activator" @click.native="drawer = !drawer" size="32"
                                                         :user-name="accountUser.name"/>
                                            <menu-list :isAuthUser="loggedIn"
                                                       v-if=!$vuetify.breakpoint.xsOnly></menu-list>
                                        </v-menu>
                                        <span class="red-counter" v-if="unreadMessages">{{unreadMessages}}</span>
                                    </div>

                                    <a v-if="!loggedIn" class="header-login body-1" href="/register">Sign Up</a>
                                    <a v-if="!loggedIn" class="header-login body-1" href="/signin">Login</a>


                                    <v-menu bottom left offset-y class="gamburger"
                                            v-if="!loggedIn && $vuetify.breakpoint.xsOnly">
                                        <v-btn icon slot="activator" @click.native="drawer = !drawer">
                                            <v-icon>sbf-menu</v-icon>
                                        </v-btn>
                                        <menu-list :isAuthUser="loggedIn"
                                                   v-if="$vuetify.breakpoint.smAndUp"></menu-list>
                                    </v-menu>

                                </div>
                            </v-toolbar-items>
                        </v-layout>
                    </v-flex>
                    <v-flex v-if="$vuetify.breakpoint.xsOnly" class="line search-wrapper">
                        <search-input :user-text="userText" :placeholder="this.$options.placeholders[currentSelection]"
                                      :submit-route="submitRoute"></search-input>
                        <!--<form @submit.prevent="submit">-->
                        <!--<v-text-field type="search" light solo class="search-b" :placeholder="placeholders[currentSelection]" v-model="msg" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>-->
                        <!--</form>-->
                    </v-flex>
                </div>
                <slot name="extraHeader"></slot>
            </v-layout>
            <v-snackbar absolute :timeout="toasterTimeout" :value="getShowToaster">
                <div class="text-wrap" v-html="getToasterText"></div>
            </v-snackbar>
            <personalize-dialog ref="personalize" :value="clickOnce"></personalize-dialog>


        </v-toolbar>

        <v-navigation-drawer temporary v-model="drawer" light right fixed app v-if=$vuetify.breakpoint.xsOnly
                             width="280">
            <menu-list :isAuthUser="loggedIn"></menu-list>
        </v-navigation-drawer>

    </div>
</template>

<script>
    import {settingMenu, notRegMenu} from '../settings/consts';
    import SearchInput from '../helpers/searchInput.vue';
    import UserAvatar from '../helpers/UserAvatar/UserAvatar.vue';
    import menuList from "./menu-list/menu-list.vue";

    import {mapActions, mapGetters} from 'vuex';
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";

    const PersonalizeDialog = () => import('./ResultPersonalize.vue');
    import ShareIcon from "./img/share-icon.svg";
    import FacebookIcon from "../home/svg/facebook-icon.svg"
    import TwitterIcon from "../home/svg/twitter-icon.svg"
    import WhatsappIcon from "./svg/whatsapp-icon.svg"
    import CopyLinkIcon from "./svg/copy-link-icon.svg"

    export default {
        components: {
            PersonalizeDialog,
            ShareIcon,
            FacebookIcon,
            TwitterIcon,
            WhatsappIcon,
            CopyLinkIcon,
            AppLogo,
            SearchInput,
            UserAvatar,
            menuList
        },
        placeholders: {
            job: "Your field of expertise...",
            tutor: "Find a tutor...",
            note: "Find study documents in...",
            book: "Textbook title or ISBN...",
            ask: "Ask anything...",
            flashcard: "Look for flashcards...",
            food: "Search for deals..."
        },
        data() {
            return {
                settingMenu,
                notRegMenu,
                clickOnce: false,
                drawer: null,
                toasterTimeout: 5000
                // isAuthUser:true
            }
        },
        props: {
            currentSelection: {type: String, default: 'note'},
            userText: {type: String},
            submitRoute: {type: String, default: '/ask'},
            toolbarHeight: {},
            layoutClass: {}
        },
        computed: {
            ...mapGetters(['getUniversityName', 'accountUser', 'unreadMessages', 'getShowToaster', 'getToasterText']),
            isMobile() {
                return this.$vuetify.breakpoint.xsOnly;
            },
            loggedIn() {
                return this.accountUser !== null
            },
            //myMoney(){return this.accountUser.balance / 40}

        },
        watch: {
            toolbarHeight(val) {
                this.height = val;
            },
            getShowToaster: function (val) {
                if (val) {
                    var self = this;
                    setTimeout(function () {
                        self.updateToasterParams({
                            showToaster: false
                        })
                    }, this.toasterTimeout)
                }
            }

        },
        methods: {
            ...mapActions(['updateToasterParams']),
            $_currentClick({id, name}) {
                if (name === 'Feedback') {
                    Intercom('showNewMessage', '');
                } else {
                    this.clickOnce = true;
                    this.$nextTick(() => {
                        this.$refs.personalize.openDialog(id);
                    })
                }
            },
            facebookShare() {
                const shareFb = 'https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(window.location.href);
                this.$ga.social('Facebook', 'Share');
                window.open(shareFb, "pop", "width=600, height=400, scrollbars=no");
            },

            twitterShare() {
                const shareTwiiter = "https://twitter.com/intent/tweet?text=" + encodeURIComponent(window.location.href);
                this.$ga.social('Twitter', 'Share');
                window.open(shareTwiiter, "pop", "width=600, height=400, scrollbars=no");
            },

            copyToClipboard() {
                let el = document.getElementById("input-url");
                el.value = window.location.href;
                this.$ga.event('CopyClipboard');

                // handle iOS as a special case
                if (navigator.userAgent.match(/ipad|ipod|iphone/i)) {
                    // convert to editable with readonly to stop iOS keyboard opening
                    el.contentEditable = true;
                    el.readOnly = true;

                    // create a selectable range
                    var range = document.createRange();
                    range.selectNodeContents(el);

                    // select the range
                    var selection = window.getSelection();
                    selection.removeAllRanges();
                    selection.addRange(range);
                    el.setSelectionRange(0, 999999);
                }
                else {
                    el.select();
                }
                // execute copy command
                document.execCommand('copy');
            },


            whatsappLink() {
                return "whatsapp://send?text=" + encodeURIComponent(window.location.href);
            },
        },
        created() {
            this.$root.$on("personalize",
                (type) => {
                    this.clickOnce = true;
                    this.$nextTick(() => {
                        this.$refs.personalize.openDialog(type);
                    })
                });
            let headerHeight = this.toolbarHeight ? this.toolbarHeight : (this.$vuetify.breakpoint.smAndUp ? 60 : 115)
            this.height = headerHeight;

            // this.isAuthUser =
        }
    }
</script>
<style src="./header.less" lang="less"></style>
