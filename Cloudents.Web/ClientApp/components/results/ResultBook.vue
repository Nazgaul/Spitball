<template v-once>
    <router-link class="pa-2 book-cell" :to="{name:'bookDetails',params:{id:item.isbn13}}" tag="div">
        <v-container class="pa-0">
            <v-layout row>
                <div class="img-wrap">
                    <img :src="item.image" alt="">
                </div>
                <v-flex class="details ml-2">
                    <div class="mb-3 cell-title">{{item.title}}</div>
                    <div v-if="item.author">Author: {{item.author}}</div>
                    <div v-if="item.isbn13">ISBN 13: {{item.isbn13}}</div>
                    <div v-if="item.isbn10">ISBN 10: {{item.isbn10}}</div>
                    <div v-if="item.edition">Edition: {{item.edition}}</div>
                    <div v-if="item.binding">Blinding: {{item.binding}}</div>
                </v-flex>
            </v-layout>
        </v-container>
        <!--<div class="text-xs-right" v-if="!isDetails">-->
            <!--<v-btn flat small  class="sell mb-0 mr-1" :to="{name:'bookDetails',params:{id:item.isbn13,type:'sell'}}">Sell</v-btn>-->
            <!--<v-btn flat small  class="buy mb-0 ml-1 mr-0"  append :to="{name:'bookDetails',params:{id:item.isbn13,type:'buy'}}">Buy or Rent</v-btn>-->
        <!--</div>-->
    </router-link>
</template>
<script>
    export default {
        props: { item: { type: Object, required: true }, isDetails: { type: Boolean, default: false } }
    }
</script>
<style src="./ResultBook.less" lang="less"></style>