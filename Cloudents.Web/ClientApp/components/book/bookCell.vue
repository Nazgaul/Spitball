<template functional>
    <v-container class="pa-0 book-cell">
        <v-layout row>
            <div class="img-wrap">
                <img :src="props.item.image" alt="">
            </div>
            <v-flex class="details ml-2">
                <div class="cell-title">{{props.item.title}}</div>
                <div class="book-info author" v-if="props.item.author">Author: {{props.item.author}}</div>
                <div class="book-info isbn13" v-if="props.item.isbn13">ISBN 13: {{props.item.isbn13}}</div>
                <div class="book-info isbn10" v-if="props.item.isbn10">ISBN 10: {{props.item.isbn10}}</div>
                <div class="book-info edition" v-if="props.item.edition">Edition: {{props.item.edition}}</div>
                <div class="book-info binding" v-if="props.item.binding">Binding: {{props.item.binding}}</div>
            </v-flex>
        </v-layout>
    </v-container>
</template>
<style src="./bookCell.less" lang="less"></style>