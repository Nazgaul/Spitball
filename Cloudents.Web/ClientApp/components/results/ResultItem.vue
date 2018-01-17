<template>
    <a class="d-block" :target="($vuetify.breakpoint.xsOnly)?'_self':'_blank'" @click="(isSpitball?$_spitball($event):'')" :href="url" :class="'cell-'+$route.path.slice(1)">
        <v-container class="pa-2" @click="$ga.event('Search_Results', $route.path.slice(1),`#${index+1}_${item.source}`)">
            <v-layout row fluid>
                <v-flex class="img-wrap mr-2 pa-0" :class="['border-'+$route.path.slice(1),'spitball-bg-'+$route.path.slice(1)]">
                    <img :src="item.image" v-if="item.image" alt="" class="image-from-source">
                    <img :src="require(`./img/${sourcesImages[sourceName]}`)" v-else-if="sourcesImages[sourceName]" alt="">                    
                    <component :is="$route.path.slice(1)+'-default'" v-else :class="'spitball-bg-'+$route.path.slice(1) + 'spitball-border-'+$route.path.slice(1)" class="defaultImage"></component>
                </v-flex>
                <v-flex class="right-section">
                    <v-layout wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                        <v-flex class="pa-0 item-data" style="flex-grow:1">
                            <div class="cell-title" :class="'text-'+$route.path.slice(1)" v-html="item.title"></div>
                            <p v-html="item.snippet"></p>
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

    var sourcesImages = {
        quizlet: 'quizlet.png',
        cram: 'cram.png',
        koofers: 'koofers.png',
        coursehero: 'coursehero.png',
        studysoup: 'studysoup.png'
    }


    export default {
        components: { AskDefault, NoteDefault, FlashcardDefault},

        props: { item: { type: Object, required: true }, index: { Number } },
        computed: {
            isSpitball() { return this.item.source.includes('spitball') },
            url: function () {
                return this.isSpitball ? this.item.url.split('.co/')[1] : this.item.source.includes("studyblue") ? this.item.url = `${this.item.url}?utm_source=spitball&utm_medium=referral` : this.item.url
            },
            sourceName: function () {
                return this.item.source.replace("www.", "").split('.')[0];

            }
        },
        methods: {
            $_spitball(event) {
                event.preventDefault();
                this.$router.push(this.url)
            }
        },
        data() {
            return {
                sourcesImages: sourcesImages

            };
        },
    }
</script>
<style src="./ResultItem.less" lang="less" scoped></style>
