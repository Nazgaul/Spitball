<template>
    <v-list subheader>
        <template v-for="(list,index) in verticals">
            <v-subheader class="ml-2">{{list.label}}</v-subheader>
            <v-list-tile append="vertical.name" v-for="vertical in list.data"  :key="vertical.name" replace="true" :to="{name:vertical.name}">
                <v-list-tile-action-text :class="'bg-'+vertical.name" class="vertical-cycle">
                    <component class="item"  v-bind:is="vertical.image"></component>
                </v-list-tile-action-text>
                <v-list-tile-content class="ml-2">
                    <v-list-tile-title>{{vertical.name }}</v-list-tile-title>
                </v-list-tile-content>
            </v-list-tile>
            <!--<div v-for="(list,index) in verticals" :key="index">
                <v-subheader class="uppper">{{list.label}}</v-subheader>
                <router-link :to="{name:vertical.name}" :key="vertical.name" v-for="vertical in list.data" tag="v-list-tile" avatar>
                    <v-list-tile-avatar>
                        <component class="item" :class="'bg-'+vertical.name" v-bind:is="vertical.image"></component>
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                        <v-list-tile-title>{{vertical.name}}</v-list-tile-title>
                    </v-list-tile-content>
                </router-link>
            </div>-->
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
    import { verticalsNavbar as verticals } from '../data.js'
    export default {
        components: {
            ask, book, document, flashcard, job, food, tutor
        },
        data() {
            return {
                verticals: verticals,
                userText: this.$store.getters.userText
            }
        }
    };
</script>
<style src="./link-verticals.less" lang="less" scoped></style>
