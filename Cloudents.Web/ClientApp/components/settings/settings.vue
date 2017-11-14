<template>
    <v-container class="pa-0">
        <v-layout row>
            <h5>Settings</h5>
            <v-list class="pt-0 pb-0">
                <template v-for="(item,index) in menuData">
                    <v-list-tile :to="{name:item.id}">
                        <v-list-tile-action>
                            <component :is="item.id+'-icon'"></component>
                        </v-list-tile-action>
                        <v-list-tile-content>
                            <v-list-tile-title>{{item.id==='university'&&getUniversityName?getUniversityName:item.name}}</v-list-tile-title>
                        </v-list-tile-content>
                        <v-list-tile-action>
                            <arrow-button-icon></arrow-button-icon>
                        </v-list-tile-action>
                    </v-list-tile>
                    <v-divider></v-divider>
                </template>
            </v-list>
            <div class="pa-2" style="width:320px; height:240px;" >
                <img src="http://lorempixel.com/320/240/" />
                <img src="http://lorempixel.com/320/240/" />

                <adsense ad-client="ca-pub-1215688692145777"
                         ad-slot="3866041406"
                         ad-style="display: block; width:320px; height:240px;"
                         ad-format="auto">
                </adsense>
            </div>
        </v-layout>
    </v-container>
</template>
<script>
    import AboutUsIcon from './svg/about-us-icon.svg'
    import  ArrowButtonIcon from './svg/arrow-button.svg'
    import MyCoursesIcon from './svg/my-courses-icon.svg'
    import  UniversityIcon from './svg/university-icon.svg'
    import WalkthroughIcon from './svg/walkthrough.svg'
    import {settingMenu} from './consts'
    import { mapActions,mapGetters } from 'vuex'
    import VDivider from "vuetify/src/components/VDivider/VDivider";
    export default {
        components: {VDivider,AboutUsIcon,ArrowButtonIcon,MyCoursesIcon,UniversityIcon,WalkthroughIcon},
        data(){
            return{
                menuData:settingMenu
            }
        },
        computed:
        {
            ...mapGetters(['getUniversityName'])
        }
    }
</script>