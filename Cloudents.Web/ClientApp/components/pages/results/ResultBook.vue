<template>

            <!--<v-layout row wrap>
                    <v-card>
                        <v-card-title primary-title>{{item.title}}</v-card-title>
                        <v-card-text>
                            <span v-if="item.author">author:{{item.author}}</span>
                            <span v-if="item.isbn13">isbn13:{{item.isbn13}}</span>
                            <span v-if="item.isbn10">isbn10:{{item.isbn10}}</span>
                            <span v-if="item.edition">edition:{{item.edition}}</span>
                            <span v-if="item.binding">binding:{{item.binding}}</span>
                        </v-card-text>
                        <v-card-media :src="item.image"></v-card-media>
                    </v-card>
            </v-layout>-->
    <div class="wrapper">

        <div class="container-box">
            <div class="box-main box-books">
                <div class="img">
                    <span class="up">
                        <img :src="item.image" alt="">
                    </span>
                </div>
                <div class="box-wrapper">
                    <div class="description">Economics in One Lesson: The Shortest and Surest Way to Understand Basic Economisc</div>
                    <p>
                        <span v-if="item.author">Author: {{item.author}}<br /></span>
                        <span v-if="item.isbn13">ISBN 13: {{item.isbn13}}<br /></span>
                        <span v-if="item.isbn10">ISBN 10: {{item.isbn10}}<br /></span>
                        <span v-if="item.edition">Edition: {{item.edition}}<br /></span>
                        <span v-if="item.binding">Blinding: {{item.binding}}<br /></span>
                    </p>
                    <v-layout row>
                        <v-flex xs12 class="text-xs-right">
                            <a href="#" class="btn btn-blue">Buy or Rent</a>
                            <a href="#" class="btn btn-yellow">Sell</a>
                        </v-flex>
                    </v-layout>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: { item: {type:Object,required:true}}
    }
</script>
<style src="./ResultBook.less" lang="less" scoped></style>