<template>
    <v-list subheader>
        <template v-for="(list,index) in verticals">
            <v-subheader class="sub mt-2" v-text="list.label"></v-subheader>
            <v-list-tile append v-for="vertical in list.data" :key="vertical.name" :to="{name:vertical.id,,query:{q:userText}}">
                <v-list-tile-action-text :class="'bg-'+vertical.id" class="vertical-cycle">
                    <component class="item" v-bind:is="vertical.image"></component>
                </v-list-tile-action-text>
                <v-list-tile-content class="ml-2">
                    <v-list-tile-title>{{vertical.name}}</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
        </template>
    </v-list>
</template>


<script>
    import ask from './images/ask.svg';
    import book from './images/book.svg';
    import document from './images/document.svg';
    import flashcard from './images/flashcard.svg';
    import job from './images/job.svg';
    import food from './images/food.svg';
    import tutor from './images/tutor.svg';
    import upload from './images/upload.svg';
    import post from './images/post.svg';
    //import flashcard from './images/flashcard.svg';
    import chat from './images/chat.svg';
    import courses from './images/courses.svg';
    import likes from './images/likes.svg';
    import { verticalsNavbar as verticals } from '../data.js';
    

    export default {
        components: {
            ask, book, document, flashcard, job, food, tutor, upload, post, chat, courses, likes
        },
        data() {
            return {
                verticals: verticals,      
            }
        },

        computed: {
            userText: function () { return this.$route.query.q }
        }
    };
</script>
<style src="./link-verticals.less" lang="less" scoped></style>
