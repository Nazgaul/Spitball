<template functional>
    <v-container class="pa-0 book-cell">
        <v-layout row class="result-cell-content">
            <div class="img-wrap">
                <img :src="props.item.image" alt="">
            </div>
            <v-flex class="details ml-2">
                <div class="cell-title">
                    <span>{{props.item.title}}</span>
                </div>
                <div class="book-info author" v-if="props.item.author">
                    <span>{{props.item.author}}</span>
                </div>
                <div class="book-info isbn13" v-if="props.item.isbn13">
                    <span>ISBN 13: {{props.item.isbn13}}</span>
                </div>
                <div class="book-info isbn10" v-if="props.item.isbn10">
                    <span>ISBN 10: {{props.item.isbn10}}</span>
                </div>
                <div class="book-info edition" v-if="props.item.edition">
                    <span>Edition: {{props.item.edition}}</span>
                </div>
                <div class="book-info binding" v-if="props.item.binding">
                    <span>Binding: {{props.item.binding}}</span>
                </div>
            </v-flex>
        </v-layout>
    </v-container>
</template>
<style src="./bookCell.less" lang="less"></style>