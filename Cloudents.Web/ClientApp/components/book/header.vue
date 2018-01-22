<template>
    <div class="book-header">
        <v-toolbar class="elevation-1" height="48" app fixed v-if="$vuetify.breakpoint.xsOnly" extended>
            <v-btn class="back" icon @click="$router.go(-1)">
                <v-icon>sbf-chevron-down</v-icon>
            </v-btn>
            <v-toolbar-title class="spitball-text-book">Textbooks</v-toolbar-title>
            <v-tabs class="mx-2" value="buy" grow slot="extension">
                <v-tabs-bar>
                    <v-tabs-item v-for="tab in sortOptions" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_changeTab(tab.id)"
                                 class="mr-2 vertical spitball-text-book">
                        {{tab.name}}
                    </v-tabs-item>
                    <v-tabs-slider color="color-book"></v-tabs-slider>
                </v-tabs-bar>
            </v-tabs>
        </v-toolbar>
        <regular-header v-else
                        currentSelection="book"
                        submitRoute="/book"
                        ></regular-header>
    </div>
</template>

<script>
    import regularHeader from "../header/header.vue"
    import TabMixin from "./bookDetailsSort"
    export default {
        mixins: [TabMixin],
        components: { regularHeader },
    }
</script>
<style src="./header.less" lang="less"></style>