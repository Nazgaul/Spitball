<template>
    <div class="box-search" ref="search">
        <form method="get" @submit.prevent="props.submitFunction">
            <v-container>
                <v-layout row>
                    <v-flex class="tx-input">
                        <v-menu offset-y full-width content-class="search-menu">
                            <span slot="activator">
                                <v-text-field slot="inputField" type="search" solo
                                              @keyup.enter="search" autocomplete="off"
                                              required name="q"
                                              :class="{'record':isRecording}"
                                              id="transcript"
                                              v-on:focus="focus"
                                              v-model.trim="msg" :placeholder="placeholder"
                                              prepend-icon="sbf-search" :append-icon="voiceAppend"
                                              :append-icon-cb="$_voiceDetection"></v-text-field>
                            </span>
                            <v-list>
                                <v-subheader>Some things you can ask me:</v-subheader>
                                <template v-for="(item, index) in items">
                                    <v-list-tile @click="props.selectosFunction(item)" :key="index">
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
    import { homeSuggest } from "./consts";
    export default {
        mixins: [micMixin],
        props: ["headerMenu"],
        computed: {
            placeholder: function () {
                if (this.$vuetify.breakpoint.smAndUp) {
                    return "Find study documents, textbooks, deals, tutors and more…";
                }
                return "Study documents, textbooks, tutors …";
            }
        },
        data() {
            return {
                items: homeSuggest
            }
        },
        methods: {
            search() {
                this.$router.push({ name: "result", query: { q: this.msg } });
            },
            focus() {
                if (this.$vuetify.breakpoint.smAndDown && !this.headerMenu) {
                    const element = this.$refs.search;
                    this.$scrollTo(element, 500, {
                        offset : -64
                    })
                }
            },
            selectos(item) {
                this.msg = item;
                this.search();
            }
        }
    }
</script>
<style lang="less" src="./search.less"></style>