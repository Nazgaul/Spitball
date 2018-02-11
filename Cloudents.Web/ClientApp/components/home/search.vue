﻿<template>
    <div class="box-search" ref="search">
        <form action="." method="get" @submit.prevent="search" v-scroll="onScroll">
            <v-container>
                <v-layout row>
                    <v-flex class="tx-input">
                        <v-menu :allow-overflow="true" offset-y full-width content-class="search-menu" v-model="menuOpen">
                            <span slot="activator">
                                <v-text-field slot="inputField" type="search" solo
                                              @keyup.enter="search" autocomplete="off"
                                              required name="q"
                                              :class="{'record':isRecording}"
                                              id="transcript"
                                              v-model.trim="msg" :placeholder="placeholder"
                                              prepend-icon="sbf-search" :append-icon="voiceAppend"
                                              :append-icon-cb="$_voiceDetection"></v-text-field>
                            </span>
                            <v-list>
                                <v-subheader>Some things you can ask me:</v-subheader>
                                <template v-for="(item, index) in items">
                                    <v-list-tile @click="selectos({item:item,index})" :key="index">
                                        <v-list-tile-action hidden-xs-only>
                                            <v-icon>sbf-search</v-icon>
                                        </v-list-tile-action>
                                        <v-list-tile-content>
                                           <v-list-tile-title v-text="item"></v-list-tile-title>
                                        </v-list-tile-content>
                                    </v-list-tile>
                                </template>
                            </v-list>
                        </v-menu>
                    </v-flex>
                    <v-flex class="f-submit">
                        <button type="submit">
                            <v-icon class="hidden-md-and-up">sbf-search</v-icon>
                            <span class="hidden-sm-and-down">Search</span>
                        </button>
                    </v-flex>
                </v-layout>
            </v-container>
        </form>
    </div>
</template>
<script>
    import { micMixin } from '../helpers/mic';
    const homeSuggest = [
        "Flashcards for financial accounting",
        "Class notes for my Calculus class",
        "When did World War 2 end?",
        "Difference between Meiosis and Mitosis",
        "Tutor for Linear Algebra",
        "Job in marketing in NYC",
        "The textbook - Accounting: Tools for Decision Making",
        "Where can I get a burger near campus?"
    ];

    export default {
        mixins: [micMixin],
        props: ["headerMenu"],
        computed: {
            placeholder: function () {
                if (this.$vuetify.breakpoint.smAndUp) {
                    return "Find study documents, textbooks, tutors, jobs, deals and more...";
                }
                return "Study documents, textbooks, tutors …";
            }
        },
        data() {
            return {
                items: homeSuggest,
                menuOpen:false
            }
        },
        methods: {
            search() {
                if (this.msg) {
                    this.$router.push({ name: "result", query: { q: this.msg } });
                }
            },
            selectos({item,index}) {
                this.msg = item;
                this.$ga.event('Search','Suggest', `#${index+1}_${item}`);
                this.search();
            },
            onScroll(e) {
                this.menuOpen = false;

            },
            //callback for mobile submit mic
            submitMic(val){
                // this.search();
                this.$router.push({ name: "result", query: { q: val } });
            }
        }
    }
</script>
<style lang="less" src="./search.less"></style>