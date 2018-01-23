<template>
    <div class="book-header">
        <dialog-toolbar height="48" v-if="$vuetify.breakpoint.xsOnly" toolbarTitle="Textbooks">
            <v-tabs class="mx-2" value="buy" grow slot="extension">
                <v-tabs-bar>
                    <v-tabs-item v-for="tab in sortOptions" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_changeTab(tab.id)"
                                 class="mr-2 vertical spitball-text-book">
                        {{tab.name}}
                    </v-tabs-item>
                    <v-tabs-slider color="color-book"></v-tabs-slider>
                </v-tabs-bar>
            </v-tabs>
        </dialog-toolbar>
        
        <regular-header v-else :$_calcTerm="$_calcTerm" :verticals="verticals" :callbackFunc="callbackFunc"
                        :currentSelection="currentSelection" :userText="userText"
                        currentPath="/book" :luisType="luisType"
                        :getLuisBox="getLuisBox" :name="name" :myClasses="myClasses"></regular-header>
    </div>
</template>

<script>
    import regularHeader from "../header/header.vue"
    import TabMixin from "./bookDetailsSort"
    import DialogToolbar from '../dialog-toolbar/DialogToolbar.vue'
    //import TopBar from "../header/detailsTopToolbar.vue";
    export default {
        mixins: [TabMixin],
        props: {
            $_calcTerm: { type: Function },
            verticals: { type: Array },
            callbackFunc: { type: Function },
            currentSelection: { type: String },
            userText: { type: String },
            currentPath: { type: String },
            luisType: { type: String },
            getLuisBox: { type: Function },
            name: { type: String },
            myClasses: {}
        },
        components: { regularHeader, DialogToolbar },
    }
</script>
<style src="./header.less" lang="less"></style>