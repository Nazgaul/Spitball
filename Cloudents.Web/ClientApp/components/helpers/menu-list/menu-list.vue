<template>
    <div>
        <v-list class="menu-list" v-if="!isAuthUser">
            <user-block :user=user :classType="'university'" v-if=isMobile></user-block>
            <template v-for="(item) in notRegMenu" >
                <template v-if="item.name">
                    <router-link tag="v-list-tile" :to="{name:item.name}">
                        <v-list-tile-content>
                            <v-list-tile-title>{{item.title}}</v-list-tile-title>
                        </v-list-tile-content>
                    </router-link>
                </template>
                <v-list-tile v-else>
                <v-list-tile-content>
                    <v-list-tile-title>{{item.title}}</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
            </template>
        </v-list>
        <v-list class="menu-list" v-else>
            <user-block :user=user :classType="'university'" v-if=isMobile></user-block>
            <v-list-tile>
                <v-list-tile-action>
                    <v-icon>sbf-wallet</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>My Wallet</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
            <router-link tag="v-list-tile" :to="{name:'conversations'}">
                <v-list-tile-action>
                    <v-icon>sbf-comment</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>Messages</v-list-tile-title>
                </v-list-tile-content>
                <v-list-tile-avatar>
                    <span class="red-counter" v-if="unreadMessages">{{unreadMessages}}</span>
                </v-list-tile-avatar>
            </router-link>
            <v-list-tile>
                <v-list-tile-action>
                    <v-icon>sbf-notifications</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>Notifications</v-list-tile-title>
                </v-list-tile-content>
                <v-list-tile-avatar>
                    <!--<span class="red-counter">1</span>  -->
                </v-list-tile-avatar>
            </v-list-tile>   
            <router-link tag="v-list-tile" :to="{name:'profile'}">
                <v-list-tile-action>
                    <v-icon>sbf-user</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>My Profile</v-list-tile-title>
                </v-list-tile-content>
            </router-link>
            <v-list-tile @click="showSettings=true">
                <v-list-tile-action>
                    <v-icon>sbf-settings</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>Settings</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
            <v-list-tile @click="logout">
                <v-list-tile-action class="tile-logout">
                    <v-icon>sbf-logout</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>Logout</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
            <v-divider class="my-3"></v-divider>

            <router-link tag="v-list-tile" :to="{name:'about'}">
                <v-list-tile-content>
                    <v-list-tile-title>About Spitball</v-list-tile-title>
                </v-list-tile-content>
            </router-link>
            <router-link tag="v-list-tile" :to="{name:'faq'}">
                <v-list-tile-content>
                    <v-list-tile-title>Help</v-list-tile-title>
                </v-list-tile-content>
            </router-link>
            <router-link tag="v-list-tile" :to="{name:'terms'}">
                <v-list-tile-content>
                    <v-list-tile-title>Terms of Service</v-list-tile-title>
                </v-list-tile-content>
            </router-link>
            <router-link tag="v-list-tile" :to="{name:'privacy'}">
                <v-list-tile-content>
                    <v-list-tile-title>Privacy Policy</v-list-tile-title>
                </v-list-tile-content>
            </router-link>
        </v-list>
        <v-dialog v-model="showSettings" content-class="settings-dialog" max-width="610">
        <user-settings v-model="showSettings"></user-settings>
        </v-dialog>
    </div>

</template>


<script>
    import {mapGetters,mapActions} from 'vuex'
    import { notRegMenu } from '../../settings/consts';
    import userBlock from "../user-block/user-block.vue"
    import userSettings from "../../settings/view/settings.vue"


    export default {
        components:{userBlock,userSettings},
        props:{
            counter:{
                required:false,
                type: Object
            },
            isAuthUser:{
                type: Boolean,
                default:false
            }
        },
        methods:{
            currentTemplate(val){return val?'router-link':'v-list-tile';},
            ...mapActions(['logout'])
        },
        data() {
            return {
                notRegMenu,
                showSettings:false,
                user:{
                    name: "User Name",
                    universityName: "Boston MA"
                }
            }
        },
        computed: {
            ...mapGetters(['unreadMessages']),
            isMobile(){return this.$vuetify.breakpoint.xsOnly;}
        },
    }
</script>

<style src="./menu-list.less" lang="less"></style>