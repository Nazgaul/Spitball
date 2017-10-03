<template>
    <scroll-list>
        <v-container grid-list-md>
            <v-layout row wrap>
                    <v-card v-for="(item,index) in items" :key="index">
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
            </v-layout>
        </v-container>
    </scroll-list>
</template>
<script>
    import { itemsList} from './mixins';
    export default {
        mixins: [itemsList]
    }
</script>