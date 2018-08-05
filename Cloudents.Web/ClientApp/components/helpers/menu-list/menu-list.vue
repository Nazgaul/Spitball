<template>
    <div>
        <v-list class="menu-list" v-if="!isAuthUser" content-class="s-menu-item">
            <user-block :user="user" :showExtended="true" :classType="'university'" v-if="isMobile" class="unsign">
                <div slot="icon" class="mb-3">
                    <v-avatar tag="v-avatar" class="Mask" size="32"><not-logged-in></not-logged-in></v-avatar>
                    </div>
                <template slot="text" class="mb-3"><span class="mb-4 blue--text"><router-link class="blue--text" to="/register">Sign up</router-link>  or  <router-link to="/signin" class="blue--text">Log in</router-link></span></template>
            </user-block>
            <template v-for="(item) in notRegMenu">
                <template v-if="item.name">
                    <router-link tag="v-list-tile" :to="{name:item.name}">
                        <v-list-tile-content>
                            <v-list-tile-title class="subheading">{{item.title}}</v-list-tile-title>
                        </v-list-tile-content>
                    </router-link>
                </template>
                <v-list-tile v-else @click="()=>item.click?item.click():''">
                    <v-list-tile-content>
                        <v-list-tile-title class="subheading">{{item.title}}</v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>
            </template>
        </v-list>
        <v-list class="menu-list" v-else>
            <user-block :user=user :classType="'university'" v-if=isMobile></user-block>
            <router-link tag="v-list-tile" :to="{name:'wallet'}">
                <v-list-tile-action>
                    <v-icon>sbf-wallet</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title class="subheading">My Wallet</v-list-tile-title>
                </v-list-tile-content>
            </router-link>
            <router-link tag="v-list-tile" :to="{name:'conversations'}">
                <v-list-tile-action>
                    <v-icon>sbf-comment</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title class="subheading">Messages</v-list-tile-title>
                </v-list-tile-content>
                <v-list-tile-avatar>
                    <span class="red-counter subheading" v-if="unreadMessages">{{unreadMessages}}</span>
                </v-list-tile-avatar>
            </router-link>
            <router-link tag="v-list-tile" :to="{name:'profile',params:{id:accountUser.id}}">
                <v-list-tile-action>
                    <v-icon>sbf-user</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title class="subheading">My Profile</v-list-tile-title>
                </v-list-tile-content>
            </router-link>
            <v-list-tile @click="startIntercom" >
                <v-list-tile-action>
                    <v-icon>sbf-feedback</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title class="subheading">Feedback</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
            <v-list-tile @click="logout">
                <v-list-tile-action class="tile-logout">
                    <v-icon>sbf-logout</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title class="subheading">Logout</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
            <v-divider class="my-3"></v-divider>

            <router-link tag="v-list-tile" :to="{name:'about'}">
                <v-list-tile-content>
                    <v-list-tile-title class="subheading">About Spitball</v-list-tile-title>
                </v-list-tile-content>
            </router-link>
            <router-link tag="v-list-tile" :to="{name:'faq'}">
                <v-list-tile-content>
                    <v-list-tile-title class="subheading">Help</v-list-tile-title>
                </v-list-tile-content>
            </router-link>
            <router-link tag="v-list-tile" :to="{name:'terms'}">
                <v-list-tile-content>
                    <v-list-tile-title class="subheading">Terms of Service</v-list-tile-title>
                </v-list-tile-content>
            </router-link>
            <router-link tag="v-list-tile" :to="{name:'privacy'}">
                <v-list-tile-content>
                    <v-list-tile-title class="subheading">Privacy Policy</v-list-tile-title>
                </v-list-tile-content>
            </router-link>
        </v-list>
        <!--<v-dialog v-if="showSettingsFirst" v-model="showSettings" content-class="settings-dialog" max-width="610">-->
            <!--<user-settings v-model="showSettings"></user-settings>-->
        <!--</v-dialog>-->
    </div>

</template>


<script>

    import { mapGetters, mapActions } from 'vuex';
    import notLoggedIn from "../img/not-logged-in.svg";
    import {notRegMenu} from '../../settings/consts';
    import userBlock from "../user-block/user-block.vue"


    export default {
        components: {userBlock,notLoggedIn},
        props: {
            counter: {
                required: false,
                type: Object
            },
            isAuthUser: {
                type: Boolean,
                default: false
            }
        },
        methods: {
            currentTemplate(val) {
                return val ? 'router-link' : 'v-list-tile';
            },
            ...mapActions(['logout']),
            startIntercom(){
                Intercom('showNewMessage')
            }
        },
        data() {
            return {
                notRegMenu,
                showSettingsFirst:false,
                showSettings: false
            }
        },
        computed: {
            ...mapGetters(['unreadMessages', 'accountUser', 'getUniversityName']),
            isMobile() {
                return this.$vuetify.breakpoint.xsOnly;
            },
            user(){
                return {...this.accountUser, universityName: this.getUniversityName}
            }
        },
    }
</script>

<style src="./menu-list.less" lang="less"></style>