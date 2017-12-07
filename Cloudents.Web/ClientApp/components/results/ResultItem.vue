<template>
    <!--@click="(isSpitball?$_spitball($event):'')"-->
    <a class="d-block" :target="$vuetify.breakpoint.xsOnly?'_self':'_blank'" :href="url">
        <v-container class="pa-2">
            <v-layout row fluid>
                <v-flex class="img-wrap mr-2 pa-0" :class="['border-'+$route.path.slice(1),'spitball-bg-'+$route.path.slice(1)]">
                    <img  :src="item.image" alt="" v-if="item.image">
                    <component :is="$route.path.slice(1)+'-default'" v-else :class="'spitball-bg-'+$route.path.slice(1)" class="defaultImage"></component>
                </v-flex>
                <v-flex class="right-section">
                        <v-layout  wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                            <v-flex class="pa-0" style="flex-grow:1">
                                <div class="cell-title" :class="'text-'+$route.path.slice(1)">{{item.title}}</div>
                                <p>{{item.snippet}}</p>
                            </v-flex>
                            <v-flex class="pa-0 bottom">
                               {{item.source}}
                            </v-flex>
                        </v-layout>
                </v-flex>
            </v-layout>
        </v-container>
    </a>
</template>
<script>
    import FlashcardDefault from './../navbar/images/flashcard.svg'
    import AskDefault from './../navbar/images/ask.svg'
    import NoteDefault from './../navbar/images/document.svg'
    export default {
        components:{AskDefault,FlashcardDefault,NoteDefault},

        props: { item: { type: Object, required: true } },
        computed: {
            //isSpitball() { return this.item.source.includes('spitball')},
            url: function () {
                return this.item.url;
                //return this.isSpitball ? this.item.url.split('.co/')[1]: this.item.url
            }
        },

        methods: {
            $_spitball(event) {
                event.preventDefault();
                this.$router.push(this.url)
            }
        }
    }
</script>
<style src="./ResultItem.less" lang="less" scoped></style>
