﻿
<template>
    <v-dialog v-model="dialog" fullscreen content-class="dialog-choose" v-if="currentItem" class="settings" :overlay=false>
        <page-layout :title="title" :search="!currentAction" :titleImage="(currentType==='course'&&!currentAction)?getUniversityImage:''" :isLoading="isLoading" :emptyText="emptyText" :items="items" :selectedCourse="selectedCourse" :closeFunction="$_closeButton">
            <v-flex class="white--text" xs2 slot="extraClose" @click="$_closeButton" v-if="currentType!=='university'">DONE</v-flex>
            <close-button slot="closeAction"></close-button>
            <v-btn slot="actionButton" v-if="currentItem.action" @click="currentAction=currentItem.action">X</v-btn>
       
            <v-text-field light solo slot="inputField" @input="$_search" class="search-b" ref="searchText" :placeholder="currentItem.placeholder" prepend-icon="sbf-search"></v-text-field>

            <v-chip slot="selectedItems" slot-scope="props" v-if="selectedCourse" label>
                <strong>{{ props.course.name }}</strong>
                <span @click="$_removeCourse(props.course.id)"><v-icon name="close"></v-icon></span>
            </v-chip>
            <v-flex v-if="!currentAction" @click="$_clickItemCallback(keep)" slot-scope="props" slot="results">
                <component :is="'search-item-'+currentType" :item="props.item"></component>
            </v-flex>
            <component slot="actionContent" v-if="currentAction" :is="currentType+'-'+currentAction" @done="$_actionDone"></component>
        </page-layout>
    </v-dialog>
</template>
<script>
    import debounce from 'lodash/debounce'
    const RadioList = () => import('./../helpers/radioList.vue');
    const plusButton = () => import('./svg/plus-button.svg');
    const closeButton = () => import('./svg/close-icon.svg');
    import { searchObjects } from './consts'
    const searchItemUniversity = () => import('./searchItemUniversity.vue');
    const searchItemCourse = () => import('./searchItemCourse.vue');
    import { mapGetters, mapMutations } from 'vuex'
    import CourseAdd from './courseAdd.vue';
    import PageLayout from './layout.vue';
    import VDialog from "vuetify/src/components/VDialog/VDialog";
    import 'vue-awesome/icons/close';
    import VIcon from 'vue-awesome/components/Icon.vue'

    export default {
        model: {
            prop: 'value',
            event: 'change'
        },
        watch: {
            type(val) {
                this.currentType = val;
            },
            currentType(val) {
                this.isLoading = true;
                this.items = [];
                this.$refs.searchText ? this.$refs.searchText.inputValue = '' : this.$_search('');
            }
        },
        computed: {
            dialog: {
                get() {
                    return this.value;
                }, set(val) {
                    if (!val) this.$refs.searchText.inputValue = '';
                    this.$emit('change', val)
                }
            },
            title() {
                if (this.currentAction && !this.dialog) this.dialog = true;
                if (this.currentAction) return "Add Class";
                if(this.currentType==="course")return this.getUniversityName;
                return "Personalize Results"
            },
            ...mapGetters(['myCourses',"getUniversityImage","getUniversityName"]),
            currentItem: function () {
                return searchObjects[this.currentType]
            },
            emptyText: function () { return this.currentItem.emptyState },
            selectedCourse() { if (this.currentType === 'course') return this.myCourses; }
        },
        data() {
            return {
                items: [],
                isLoading: true,
                isChanged: false,
                currentType: "",
                currentAction: ""
            }
        },

        components: {
            CourseAdd, VDialog, PageLayout, VIcon, searchItemUniversity, searchItemCourse, closeButton
        },
        props: { type: { type: String, required: true }, value: { type: Boolean }, keep: { type: Boolean } },
        methods: {
            ...mapMutations({ updateUser: 'UPDATE_USER' }),
            $_removeCourse(val) {
                this.updateUser({ myCourses: this.myCourses.filter(i => i.id !== val) })
            },
            $_closeButton() {
                this.currentAction ? this.currentAction = "" : this.dialog = false;
            },
            $_clickItemCallback(keep) {
                this.currentItem.click ? this.currentItem.click.call(this, keep) : ''
            },
            $_actionDone(val) {
                this.items = [... this.items, val];
                this.currentAction = "";
            },
            $_actionsCallback(action) {
                this.currentAction = action;
            },
            $_search: debounce(function (val) {
                this.isLoading = true;
                this.$store.dispatch(this.currentItem.searchApi, { term: val }).then(({ body }) => {
                    this.items = body;
                    this.isLoading = false
                })
            }, 500)
        }
    }
</script>
<style src="./searchItem.less" lang="less"></style>







<!--<v-card>
                <v-card-title>
                    <v-flex xs12 class="headline">{{currentItem.title}}</v-flex>
                    <v-flex xs12>
                        <v-text-field label="Search" @input="$_search" v-debounce="500"
                                      class="input-group--focused"
                                      single-line></v-text-field>
                    </v-flex>
                    <v-container class="pa-0 mb-3" v-if="currentItem.filters">
                        <v-layout row>
                            <radio-list class="search" :values="currentItem.filters" model="filter" v-model="filter" :value="currentItem.defaultFilter"></radio-list>
                            <template v-for="act in currentItem.actions">
                                <v-flex @click="$_actionsCallback(act.id)" v-if="act.component"><component :is="act.component"></component></v-flex>
                            </template>
                        </v-layout>
                    </v-container>
                </v-card-title>
                <v-divider></v-divider>
                <v-card-text style="height: 300px;">
                    <div class="loader" v-if="isLoading">
                        <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
                    </div>
                    <div v-else>
                        <v-list v-if="items.length">
                            <template v-for="(item,index) in filterItems">
                                <div @click="$_selected({id:item.id,name:item.name})"> <component :is="'search-item-'+type" :item="item"></component></div>
                                <v-divider v-if="index < filterItems.length-1"></v-divider>
                            </template>
                        </v-list><div v-else>
                            <div>No Results Found</div>
                            <div v-html="emptyText"></div>
                        </div>
                    </div>
                </v-card-text>
                <v-divider></v-divider>
                <v-card-actions>
                    <v-btn color="blue darken-1" flat @click.native="dialog = false">Close</v-btn>
                    <v-btn color="blue darken-1" flat @click.native="dialog = false">Save</v-btn>
                </v-card-actions>
            </v-card>-->
