<template v-once>
    <router-link class="pa-2 d-block" :to="{name:'bookDetails',params:{id:item.isbn13},query:{q:$route.query.q}}">
        <book-cell :item="item"></book-cell>
        <!--<v-container class="pa-0">
            <v-layout row>
                <div class="img-wrap">
                    <img :src="item.image" alt="">
                </div>
                <v-flex class="details ml-2">
                    <div class="cell-title">{{item.title}}</div>
                    <div class="book-info author" v-if="item.author">Author: {{item.author}}</div>
                    <div class="book-info isbn13" v-if="item.isbn13">ISBN 13: {{item.isbn13}}</div>
                    <div class="book-info isbn10" v-if="item.isbn10">ISBN 10: {{item.isbn10}}</div>
                    <div class="book-info edition" v-if="item.edition">Edition: {{item.edition}}</div>
                    <div class="book-info binding" v-if="item.binding">Binding: {{item.binding}}</div>
                </v-flex>
            </v-layout>
        </v-container>-->
    </router-link>
</template>
<script>
    import bookCell from "../book/bookCell.vue"
    export default {
        components: { bookCell},
        props: { item: { type: Object, required: true }, isDetails: { type: Boolean, default: false } }
    }
</script>
