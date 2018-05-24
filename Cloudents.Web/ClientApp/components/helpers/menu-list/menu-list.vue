<template>
    <div>
        <v-list class="menu-list" v-if="!isAuthUser">
            <v-list-tile v-for="(item,index) in notRegMenu" :key="index" :id="item.id">
                <v-list-tile-content>
                    <v-list-tile-title>{{item.name}}</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
        </v-list> 

        <v-list class="menu-list" v-else>
            <v-list-tile>
                <v-list-tile-action>
                    <v-icon>sbf-wallet</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>My Wallet</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>   
            <v-list-tile>
                <v-list-tile-action>
                    <v-icon>sbf-comment</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>Messages</v-list-tile-title>
                </v-list-tile-content>
                <v-list-tile-avatar>
                    <span class="red-counter">5</span>  
                </v-list-tile-avatar>
            </v-list-tile>   
            <v-list-tile>
                <v-list-tile-action>
                    <v-icon>sbf-notifications</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>Notifications</v-list-tile-title>
                </v-list-tile-content>
                <v-list-tile-avatar>
                    <span class="red-counter">1</span>  
                </v-list-tile-avatar>
            </v-list-tile>   
            <v-list-tile>
                <v-list-tile-action>
                    <v-icon>sbf-user</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>My Profile</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>   
            <v-list-tile>
                <v-list-tile-action>
                    <v-icon>sbf-wallet</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>Settings</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>   
            <v-list-tile>
                <v-list-tile-action class="tile-logout">
                    <v-icon>sbf-logout</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                    <v-list-tile-title>Logout</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile> 

            <v-divider class="my-3"></v-divider>

            <v-list-tile>
                <v-list-tile-content>
                    <v-list-tile-title>About Spitball</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>   
            <v-list-tile>
                <v-list-tile-content>
                    <v-list-tile-title>Help</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>   
            <v-list-tile>
                <v-list-tile-content>
                    <v-list-tile-title>Terms of Service</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>   
            <v-list-tile>
                <v-list-tile-content>
                    <v-list-tile-title>Privacy Policy</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile> 
        </v-list>

    </div>

</template>


<script>

    import { notRegMenu } from '../../settings/consts';

    export default {
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
        data() {
            return {
                notRegMenu
            }
        },
        methods:{
            
        }    
    }
</script>

<style src="./menu-list.less" lang="less"></style>