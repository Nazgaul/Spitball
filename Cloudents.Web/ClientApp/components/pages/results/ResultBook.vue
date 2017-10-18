<template v-once>
    <div class="elevation-1 pa-2 book-cell">
       
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
        <div class="text-xs-right">
            <v-btn flat small  class="sell mb-0 mr-1" :to="{name:'bookDetails',params:{id:item.isbn13,type:'sell'}}">Sell</v-btn>
            <v-btn flat small  class="buy mb-0 ml-1 mr-0"  append :to="{name:'bookDetails',params:{id:item.isbn13,type:'buy'}}">Buy or Rent</v-btn>
        </div>
    </div>
    <!--<div class="wrapper">
        <div class="container-box">
            <div class="box-main box-books">
                <div class="img">
                    <span class="up">
                        <img :src="item.image" alt="">
                    </span>
                </div>
                <div class="box-wrapper">
                    <div class="description">{{item.title}}</div>
                    <p>
                        <span v-if="item.author">Author: {{item.author}}<br /></span>
                        <span v-if="item.isbn13">ISBN 13: {{item.isbn13}}<br /></span>
                        <span v-if="item.isbn10">ISBN 10: {{item.isbn10}}<br /></span>
                        <span v-if="item.edition">Edition: {{item.edition}}<br /></span>
                        <span v-if="item.binding">Blinding: {{item.binding}}<br /></span>
                    </p>
                    <v-layout row v-show="!isDetails">
                        <v-flex xs12 class="text-xs-right">
                            <router-link :to="{name:'bookDetails',params:{id:item.isbn13,type:'buy'}}" append class="btn btn-blue">Buy or Rent</router-link>
                            <router-link :to="{name:'bookDetails',params:{id:item.isbn13,type:'sell'}}" class="btn btn-yellow">Sell</router-link>
                        </v-flex>
                    </v-layout>
                </div>
            </div>
        </div>
    </div>-->
</template>
<script>
    export default {
        props: { item: { type: Object, required: true }, isDetails: { type: Boolean, default: false } }
    }
</script>
<style src="./ResultBook.less" lang="less"></style>