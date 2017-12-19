<template>
    <div class="box-search">
        <form id="labnol" method="get" @submit.prevent="props.submitFunction">
            <v-container>
                <v-layout row>
                    <v-flex class="tx-input">
                        <v-menu offset-y full-width content-class="h-p-menu">
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
                            <v-icon class="hidden-sm-and-up">sbf-search</v-icon>
                            <span class="hidden-xs-only">Search</span>
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

            selectos(item) {
                this.msg = item;
                this.search();
            }
        }
    }
</script>
<style lang="less" src="./search.less"></style>