<template>
    <div>
        <first-line :name="name"></first-line>
        <v-tabs value="buy" grow>
            <v-tabs-bar class="cyan" dark>
                <v-tabs-item v-for="tab in sortOptions" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_changeTab(tab.id)"
                             class="mr-2 vertical">
                    {{tab.name}}
                </v-tabs-item>
                <v-tabs-slider color="white"></v-tabs-slider>
            </v-tabs-bar>
        </v-tabs>
    </div>
</template>

<script>
    import TabMixin from "../details/bookDetailsSort"
    import FirstLine from './headerFirstLineMobile.vue'
    export default {
        name: "header-book-details-mobile",
        mixins:[TabMixin],
        components:{FirstLine},
        props:{name:{type:String}}
    }
</script>

<style scoped>

</style>