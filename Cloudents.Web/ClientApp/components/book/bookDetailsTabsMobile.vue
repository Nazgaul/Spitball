<template>
    <v-tabs value="buy" grow>
        <!--<v-tabs-bar class="cyan" dark>-->
            <v-tabs-item v-for="tab in sortOptions" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_changeTab(tab.id)"
                         class="mr-2 vertical">
                {{tab.name}}
            </v-tabs-item>
            <v-tabs-slider color="white"></v-tabs-slider>
        <!--</v-tabs-bar>-->
    </v-tabs>
</template>

<script>
    import TabMixin from "./bookDetailsSort"
    export default {
        name: "book-details-tabs-mobile",
        mixins:[TabMixin]
    }
</script>
