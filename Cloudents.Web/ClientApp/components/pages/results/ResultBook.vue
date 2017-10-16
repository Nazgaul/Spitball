<template>

    <div class="wrapper">

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
    </div>
</template>
<script>
    export default {
        props: { item: { type: Object, required: true }, isDetails: {type:Boolean,default:false}}
    }
</script>
<style src="./ResultBook.less" lang="less" scoped></style>