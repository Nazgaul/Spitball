<template>
    <general-page title="Settings">
        <div slot="data">
            <div class="p-setting">
                <v-list class="pa-0">
                    <template v-for="(item,index) in menuData">
                        <v-list-tile @click="$_currentClick(item)">
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
                        <v-divider v-if="index < menuData.length -1"></v-divider>
                    </template>
                </v-list>
            </div>
            <div class="version">version:{{version}}</div>
        </div>
        <search-item v-model="showDialog" :type="type"></search-item>

    </general-page>
</template>
<script>
    import AboutUsIcon from './svg/about-us-icon.svg'
    import ArrowButtonIcon from './svg/arrow-button.svg'
    import MyCoursesIcon from './svg/my-courses-icon.svg'
    import UniversityIcon from './svg/university-icon.svg'
    import WalkthroughIcon from './svg/walkthrough.svg'
    import GeneralPage from './../helpers/generalPage.vue'
    import searchItem from './searchItem.vue'
    import { settingMenu } from './consts'
    import { mapActions, mapGetters } from 'vuex'
    import VDivider from "vuetify/src/components/VDivider/VDivider";
    export default {
        components: { VDivider, searchItem, AboutUsIcon, ArrowButtonIcon, MyCoursesIcon, UniversityIcon, WalkthroughIcon, GeneralPage, searchItem },
        data() {
            return {
                menuData: settingMenu, showDialog: false, type: "university"
            }
        },
        computed:
        {
            ...mapGetters(['getUniversityName']),
            version() { return window.version }
        },

        methods: {
            $_currentClick(item) {
                item.click.call(this);
            }
        }
    }
</script>
<style lang="less">
    .p-setting {
        background: #fff;
        border-radius: 8px;
    }
    .version {
        text-align:center;
    }
</style>