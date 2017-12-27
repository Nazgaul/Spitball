<template>
    <div>
        <v-toolbar app fixed v-if="$vuetify.breakpoint.xsOnly" extended>
            <v-btn icon @click="$router.go(-1)">
                <v-icon>sbf-arrow-button</v-icon>
            </v-btn>
            <v-toolbar-title>Textbooks</v-toolbar-title>
            <v-tabs value="buy" grow slot="extension">
                <v-tabs-bar>
                    <v-tabs-item v-for="tab in sortOptions" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_changeTab(tab.id)"
                                 class="mr-2 vertical">
                        {{tab.name}}
                    </v-tabs-item>
                    <v-tabs-slider color="color-book"></v-tabs-slider>
                </v-tabs-bar>
            </v-tabs>
        </v-toolbar>
        <regular-header v-else :$_calcTerm="$_calcTerm" :verticals="verticals" :callbackFunc="callbackFunc"
                        :currentSelection="currentSelection" :userText="userText"
                        :currentPath="currentPath" :luisType="luisType"
                        :getLuisBox="getLuisBox" :name="name" :myClasses="myClasses"></regular-header>
    </div>
</template>

<script>
    import regularHeader from "../header/header.vue"
    import TabMixin from "./bookDetailsSort"
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
        components: { regularHeader},
    }
</script>

<style>
    .tabs__item {
        color:red;
    }
</style>