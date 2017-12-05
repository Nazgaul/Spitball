
<template>
    <v-dialog v-model="dialog" fullscreen content-class="dialog-choose" v-if="currentItem" class="settings" :overlay=false>
        <page-layout :type="currentType" :title="title" :search="!currentAction" :titleImage="(currentType==='course'&&!currentAction)?getUniversityImage:''" :isLoading="isLoading" :emptyText="emptyText" :items="items" :selectedCourse="selectedCourse" :closeFunction="$_closeButton">
            <button class="white--text" slot="extraClose" @click="$_closeButton" v-if="currentType!=='university'">DONE</button>
            <template slot="closeAction">
                <close-button v-if="currentType==='university'"></close-button>
                <i v-else class="sbf icon sbf-arrow-button"></i>
            </template>
            <template v-if="currentType==='course'" slot="courseExtraItem">
                <button class="add-course-btn ma-2" @click="showAddCourse=true" v-show="!showAddCourse">
                    <plus-button></plus-button>
                    <div>add course</div>
                </button>
                <div class="add-course-form ma-2 pa-2" v-show="showAddCourse">
                    <form @submit.prevent="$_submitAddCourse">
                        <v-text-field dark v-model="newCourseName" placeholder="Course Name"></v-text-field>
                        <v-btn @click="$_submitAddCourse">ADD</v-btn>
                    </form>
                </div>
            </template>

            <template slot="courseFirstTime" v-if="courseFirst&&showCourseFirst">
                <div class="first-time-message ma-3">
                    <div class="text">
                        <div>We are working hard on getting all the courses from your school into our system,</div>
                        <div><b>but we might have missed a few. If you can't find your course, use this icon to add it</b></div>
                    </div>
                    <div class="image">missing...</div>

                    <button>
                        <close-button @click="showCourseFirst=false"></close-button>
                    </button>
                </div>
            </template>

            <v-text-field light solo slot="inputField" @input="$_search" class="search-b" ref="searchText" :placeholder="currentItem.placeholder" prepend-icon="sbf-search"></v-text-field>

            <v-chip class="ma-2" slot="selectedItems" slot-scope="props" v-if="selectedCourse" label>
                <span class="name">{{ props.course.name }}</span>
                <button class="close pa-2" @click="$_removeCourse(props.course.id)">
                    <close-button></close-button>
                </button>
            </v-chip>
            <v-flex class="result" v-if="!currentAction" @click="$_clickItemCallback(keep)" slot-scope="props" slot="results">
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
    import { mapGetters, mapMutations, mapActions } from 'vuex'
    import CourseAdd from './courseAdd.vue';
    import PageLayout from './layout.vue';
    import VDialog from "vuetify/src/components/VDialog/VDialog";
    //import 'vue-awesome/icons/close';
    //import VIcon from 'vue-awesome/components/Icon.vue'

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
            ...mapGetters(['getUniversityName', 'courseFirstTime']),
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
                if (this.currentType === "course") return this.getUniversityName;
                return "Personalize Results"
            },
            ...mapGetters(['myCourses', "getUniversityImage", "getUniversityName"]),
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
                currentAction: "",
                newCourseName: "",
                showAddCourse: false,
                showCourseFirst: true,
                courseFirst: false
            }
        },

        components: {
            CourseAdd, VDialog, PageLayout, searchItemUniversity, searchItemCourse, closeButton, plusButton
        },
        props: { type: { type: String, required: true }, value: { type: Boolean }, keep: { type: Boolean }, isFirst: { type: Boolean } },
        methods: {
            ...mapMutations({ updateUser: 'UPDATE_USER' }),
            ...mapActions(["createCourse"]),
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
            }, 500),
            $_submitAddCourse() {
                this.createCourse({ name: this.newCourseName });
                this.newCourseName = '';
                this.showAddCourse = false;
            }
        },
        created() {
            this.courseFirst = this.courseFirstTime;
            this.$nextTick(() => {
                this.courseFirstTime ? this.$store.dispatch("updateFirstTime", "courseFirstTime") : "";
            })
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
