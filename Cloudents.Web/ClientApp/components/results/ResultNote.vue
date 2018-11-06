<template>
    <a :target="($vuetify.breakpoint.xsOnly)?'_self':'_blank'"
       @click="(isOurs ? $_spitball($event):'')" :href="url" :class="['d-block', 'note-block',  'type-'+typeID]">

        <v-container class="pa-0"
                     @click="$ga.event('Search_Results', $route.path.slice(1),`#${index+1}_${item.source}`)">
            <v-flex class="wrapper">
                <v-flex  grow class="top-row">
                    <div class="type-wrap">
                        <v-icon :class="['type-'+typeID]">{{type ? type.icon : ''}}</v-icon>
                        <span :class="[ 'doc-type-text', 'ml-2', 'type-'+typeID]">{{typeTitle}}</span>
                    </div>
                </v-flex>
                <v-flex  grow class="details-row">
                    <div class="details-wrap">
                        <span class="aligned">{{item.university}}New Jersey Institute…
                        <v-icon class="sb-icon-arrow">sbf-nav-arrow-right</v-icon>
                        </span>
                        <span class="aligned">{{item.course}}Social Psych
                        <v-icon class="sb-icon-arrow">sbf-nav-arrow-right</v-icon>
                        </span>
                        <span class="aligned">{{item.proffesor}}Prof. Apelbaum</span>
                    </div>
                </v-flex>
            </v-flex>
        </v-container>
    </a>
</template>
<script>

    import FlashcardDefault from '../helpers/img/flashcard.svg';
    import AskDefault from '../helpers/img/ask.svg';
    import NoteDefault from '../helpers/img/document.svg';
    import { documentTypes } from "./helpers/uploadFiles/consts.js"

    export default {
        components: {AskDefault, NoteDefault, FlashcardDefault},

        props: {item: {type: Object, required: true}, index: {Number}},
        computed: {

            //change when server will return id of document type and not title of type
            type() {
                let self = this;
                if (!!self.item.documentType) {
                    return documentTypes.find((single) => {
                        if (single.title.toLowerCase() === self.item.documentType.toLowerCase()) {
                            return single
                        }
                    })
                } else {
                    return {
                        id: 'document',
                        title: self.item.source,
                        icon: 'sbf-document-note'
                    }
                }
            },
            typeID() {
                if (!!this.type) {
                    return this.type.id || ''
                }
            },
            typeTitle(){
                if (!!this.type) {
                    return this.type.title || ''
                }
            },

            isOurs() {
                return this.item.source.includes('Cloudents') || this.item.source.includes('Spitball')
            },
            isCloudents() {
                return this.item.source.includes('Cloudents')
            },
            isSpitball() {
                return this.item.source.includes('Spitball')
            },
            url: function () {
                if (this.isCloudents) {
                    return this.item.url.split('.co/')[1]
                } else {
                    return this.item.url
                }
            },

        },
        methods: {
            $_spitball(event) {
                event.preventDefault();
                this.$router.push(this.url)
            }
        },
    }
</script>
<style src="./ResultNote.less" lang="less"></style>
