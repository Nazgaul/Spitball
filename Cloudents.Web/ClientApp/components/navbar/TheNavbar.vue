<template>
    <v-navigation-drawer light app clipped permanent class="aside" width="216">
        <v-list subheader>
            <template v-for="(list,index) in verticals">
                <v-subheader class="sub mt-2" v-text="list.label"></v-subheader>
                <v-list-tile v-for="vertical in list.data" :key="vertical.id"  @click="$_updateType(vertical.id)" :class="{'list__tile--active':vertical.id==currentPage}">
                    <v-list-tile-action-text :class="'bg-'+vertical.id" class="vertical-cycle">
                        <component class="item" v-bind:is="vertical.image"></component>
                    </v-list-tile-action-text>
                    <v-list-tile-content class="ml-2">
                        <v-list-tile-title>{{vertical.name}}</v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>
            </template>
        </v-list>
        </v-navigation-drawer>
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
    import chat from './images/chat.svg';
    import courses from './images/courses.svg';
    import likes from './images/likes.svg';
    import { verticalsNavbar as verticals } from '../data.js';
    import {mapMutations} from 'vuex'

    export default {
        components: {
            ask, book, document, flashcard, job, food, tutor, upload, post, chat, courses, likes
        },
        data() {
            return {
                verticals: verticals,      
            }
        },

        methods:{
            ...mapMutations({'changeFlow':'ADD'}),
         $_updateType(result){
              this.changeFlow({result})
             this.$router.push({path:'/'+result,query:{q:this.userText}})
         }
        },
        props: { term: {type:String}},
        computed: {
            userText() { return this.term?this.term:this.$route.query.q},
            currentPage(){return this.$route.path.slice(1)}
        }
    };
</script>
<style src="./TheNavbar.less" lang="less" scoped></style>
<style scoped>
    /*need to override theme*/
    .navigation-drawer.aside {
        background: #3293f6;
    }

    .navigation-drawer {
        padding: 0;
    }
</style>
