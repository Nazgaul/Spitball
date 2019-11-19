<template>
    <div class="add-courses-wrap">
        <v-layout row :class="[$vuetify.breakpoint.smAndUp ? 'py-4 px-4': 'grey-backgound py-2']" align-center
                  justify-center>
            <v-flex grow xs10>
                <div class="d-inline-flex justify-center shrink">
                    <v-icon @click="goToEditCourses()" class="course-back-btn mr-3" :class="{'rtl': isRtl}">sbf-arrow-back
                    </v-icon>
                    <span class="subtitle-1 font-weight-bold" v-language:inner>courses_join</span>
                    <span class="subtitle-1 font-weight-bold" v-if="quantatySelected">&nbsp;({{quantatySelected}})</span>
                </div>

            </v-flex>
            <v-flex xs2 shrink class="d-flex justify-end">
                <v-btn rounded :disabled="localSelectedClasses.length === 0" :loading="doneButtonLoading" class="elevation-0 done-btn py-1 font-weight-bold my-0 text-capitalize" @click="submitAndGo()">
                    <span v-language:inner>courses_btn_done</span>
                </v-btn>
            </v-flex>
        </v-layout>
        <v-layout column :class="{'px-3' : $vuetify.breakpoint.smAndUp}">
            <v-flex>
                <v-text-field id="classes_input"
                              v-model="search"
                              class="class-input"
                              ref="classInput"
                              solo
                              prepend-inner-icon="sbf-search"
                              :placeholder="classNamePlaceholder"
                              autocomplete="off"
                              autofocus
                              spellcheck="true"
                              hide-details

                ></v-text-field>
            </v-flex>
            <v-flex v-show="quantatySelected" transition="fade-transition" style="position: relative">
                <div :class="['selected-classes-container', showBox ? 'mt-0': 'spaceTop' ]">
                    <div class="class-list selected-classes-list py-3 px-3"
                         ref="listCourse">
                        <div class="selected-class-item caption d-inline-flex text-truncate font-weight-bold align-center justify-center pl-3 pr-1  py-1 mr-2"
                             v-for="selectedClass in localSelectedClasses">
                            <span class="text-truncate">{{selectedClass.text}}</span>
                            <span class="delete-class cursor-pointer pr-3"
                                  @click="deleteClass(selectedClass, selectedClasses)">
                        <v-icon color="white">sbf-close</v-icon>
                    </span>
                        </div>
                    </div>

                </div>
            </v-flex>
        </v-layout>
        <v-layout align-center class="mt-3 px-2" row wrap>
            <v-flex v-if="!classes && !classes.length" xs12 class="text-center">
                <div>
                    <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
                </div>
            </v-flex>
            <v-flex v-if="showBox">
                <div class="class-list search-classes-list" id="search-classes-list">
                    <div class="list-item subtitle-1 search-class-item cursor-pointer mx-2 justify-space-between align-center font-weight-regular"
                         v-for="singleClass in classes" @click="singleClass.isSelected ? deleteClass(singleClass, selectedClasses) : addClass(singleClass, classes)">
                        <v-layout column class="pl-3 limit-width">
                            <v-flex shrink class="course-name-wrap">
                                <div v-html="$options.filters.boldText(singleClass.text, search)">
                                    {{ singleClass.text }}
                                </div>
                            </v-flex>
                            <v-flex class="students-enrolled pt-1">
                                {{singleClass.students}}
                                <span class="students-enrolled" v-language:inner>courses_students</span>
                            </v-flex>
                        </v-layout>
                        <v-layout row align-center justify-end class="minimize-width">
                            <div v-if="!singleClass.isFollowing">
                                <v-flex shrink v-if="singleClass.isSelected" class="d-flex align-center">
                                    <span class="light-purple caption font-weight-medium mr-2"
                                          v-html="$Ph('courses_joined')"></span>
                                    <span>

                                     <v-icon class="checked-icon">sbf-check-circle</v-icon>
                                   </span>
                                </v-flex>
                                <v-flex shrink v-else class="d-flex align-center">
                                    <span class="light-purple caption font-weight-medium mr-2"
                                          v-html="$Ph('courses_join')"></span>
                                    <span>

                                     <v-icon class="cursor-pointer add-sbf-icon">sbf-plus-circle</v-icon>
                                   </span>
                                </v-flex>
                            </div>
                            <v-flex v-else shrink class="d-flex align-end">
                                <span class="light-purple caption font-weight-medium mr-2"
                                      v-html="$Ph('courses_joined')"></span>
                            </v-flex>
                        </v-layout>
                    </div>
                    <!--create new course-->
                    <v-flex class="text-center align-center justify-center cant-find py-2 px-2 caption cursor-pointer"
                            @click="openCreateDialog(true)">
                        <span v-language:inner>courses_cant_find</span>
                        <span class="pl-1 add-item" v-language:inner>courses_create_new</span>
                    </v-flex>
                </div>
            </v-flex>
        </v-layout>

    </div>
</template>

<script>
    import { mapActions, mapGetters, mapMutations } from "vuex";
    import { LanguageService } from "../../../services/language/languageService";
    import debounce from "lodash/debounce";
    import universityService from '../../../services/universityService';

    export default {
        data() {
            return {
                search: "",
                isLoading: false,
                isComplete: false,
                page: 0,
                term: '',
                isEdge: global.isEdge,
                classNamePlaceholder: LanguageService.getValueByKey(
                    "courses_placeholder_find"
                ),
                isRtl: global.isRtl,
                global: global,
                localSelectedClasses: [],
                doneButtonLoading: false
            };
        },

        watch: {
            search: debounce(function (val) {
                let searchVal = '';
                if(!!val) {
                    searchVal = val.trim();
                }
                this.term = searchVal;
                let paramObj = {term: searchVal, page: 0};
                this.loadCourses(paramObj);                
            }, 500)
        },
        computed: {
            ...mapGetters(["getSelectedClasses", "accountUser"]),
            isTutor(){
                return this.accountUser.isTutor;
            },
            quantatySelected() {
                return this.selectedClasses.length;
            },

            showBox() {
                return true
                    // return !!this.search && this.search.length > 0;
            },
            classes() {
                let classesList = this.getClasses();
                if(this.localSelectedClasses.length > 0) {
                    this.localSelectedClasses.forEach((item) => {
                        for (let i = 0; i < classesList.length; i++) {
                            if(classesList[i].text === item.text) {
                                classesList[i]['isSelected'] = true;
                            }
                        }
                    });
                }
                return classesList;

            },
            selectedClasses: {
                get() {
                    return this.localSelectedClasses;
                },
                set(val) {
                    let arrValidData = [];
                    if(val.length > 0) {
                        arrValidData = val.filter(singleClass => {
                            if(singleClass.text) {
                                return singleClass.text.length > 3;
                            } else {
                                return singleClass.length > 3;
                            }
                        });
                    }
                    this.localSelectedClasses.push(arrValidData);
                    this.updateSelectedClasses(arrValidData);
                }
            }
        },
        methods: {
            ...mapActions([
                              "updateClasses",
                              "updateSelectedClasses",
                              "assignClasses",
                              "pushClassToSelectedClasses",
                              "changeClassesToCachedClasses",
                              "addToCachedClasses",
                              "changeCreateDialogState",
                              "removeFromCached",
                              "addClasses",
                              "clearClassesCahce"
                          ]),
            ...mapGetters(["getClasses"]),
            ...mapMutations(['UPDATE_SEARCH_LOADING','setSearchedCourse']),
            openCreateDialog(val){
                this.setSearchedCourse(this.search)
                this.changeCreateDialogState(val)
            },
            goToEditCourses() {
                if(this.getSelectedClasses.length === 0 && this.quantatySelected === 0){
                    this.UPDATE_SEARCH_LOADING(true);
                    this.$router.push({name: 'feed'})
                }else{
                    this.clearClassesCahce();
                    this.$router.push({name: 'editCourse'})
                }
                
            },
            concatCourses(paramObj) {
                let self = this;
                self.isLoading = true;
                self.addClasses(paramObj)
                    .then((hasData) => {
                        if(!hasData) {
                            self.isComplete = true;
                            return;
                        }
                        if(hasData.length < 50) {
                            self.isComplete = true;
                        }
                        self.isLoading = false;
                        self.page++;
                    }, (err) => {
                        self.isComplete = true;
                    });
            },
            keepLoad(clientHeight, scrollTop) {
                let totalHeight = clientHeight;
                let currentScroll = scrollTop;
                let scrollOffset = (currentScroll > (0.75 * totalHeight));
                let retVal = (!this.isLoading && !this.isComplete && currentScroll > 0 && scrollOffset);
                return retVal;
            },
            scrollCourses(e) {
                let clientHeight = e.target.scrollHeight - e.target.offsetHeight;
                let scrollTop = e.target.scrollTop;
                if(this.keepLoad(clientHeight, scrollTop)) {
                    let paramObj = {term: this.term, page: this.page};
                    this.concatCourses(paramObj);
                }

            },
            loadCourses(paramObj) {
                let self = this;
                self.isComplete = false;
                self.isLoading = true;
                this.updateClasses(paramObj).then((hasData) => {
                    if(!hasData) {
                        self.isComplete = true;
                    }
                    self.isLoading = false;
                    self.page = 1;
                }, (err) => {
                    self.isComplete = true;
                });
            },
            setTeachActiveOnSelectedClass(){
                universityService.teachCourse(course.text).then((resp) => {
                    }, (error) => {
                });
            },
            submitAndGo() {
                //assign all saved in cached list to classes list
                this.changeClassesToCachedClasses();
                this.doneButtonLoading = true;
                this.assignClasses(this.localSelectedClasses).then(() => {
                    if(this.isTutor){
                            this.localSelectedClasses.forEach(course=>{
                                universityService.teachCourse(course.text).then(resp=>{
                                    course.isTeaching = true;
                                    this.doneButtonLoading = false;
                                    this.$router.push({name: 'editCourse'});
                                })
                            });
                    }else{
                        this.doneButtonLoading = false;
                        this.$router.push({name: 'editCourse'});
                    }
                    
                });
            },
            deleteClass(classToDelete, from) {
                let index = from.indexOf(classToDelete);
                from.splice(index, 1);
                //clean from cached list and request new list, and refresh data
                this.removeFromCached(classToDelete);
                let paramObj = {term: this.search, page: 0};
                this.loadCourses(paramObj);

            },
            checkAsSelected(classToCheck, from) {
                let index = from.indexOf(classToCheck);
                from[index].isSelected = true;
            },
            addClass(className) {
                if(className.isFollowing)return;
                this.localSelectedClasses.push(className);
                //add to cached list
                this.addToCachedClasses(className);
                setTimeout(() => {
                    let inputElm = this.$refs.classInput;
                    // inputElm.value = "";
                    inputElm.focus();
                }, 200);
                this.checkAsSelected(className, this.classes);
            },
        },
        created() {
            let paramObj = {term: this.term, page: 0};
            this.loadCourses(paramObj);
            this.$nextTick(function () {
                let scrollableElm = document.getElementById('search-classes-list');
                if(scrollableElm){
                    scrollableElm.addEventListener('scroll', (e) => {
                        this.scrollCourses(e);
                    });
                }
            });
        },
        mounted() {
            let self = this;
            //mouse wheel fix horizontal scroll
            let el = self.$refs.listCourse;
            let mouseWheelEvt = function (event) {
                if(el.doScroll) {
                    el.doScroll(event.wheelDelta > 0 ? "left" : "right");
                }
                else if((event.wheelDelta || event.detail) > 0) {
                    el.scrollLeft -= 10;
                }
                else {
                    el.scrollLeft += 10;
                }
                return false;
            };
            self.$refs.listCourse.addEventListener("mousewheel", mouseWheelEvt);
        },
        filters: {
            boldText(value, search) {
                if(!value) return "";
                if(!search) return value;
                let match = value.toLowerCase().indexOf(search.toLowerCase()) > -1;
                if(match) {
                    let startIndex = value.toLowerCase().indexOf(search.toLowerCase());
                    let endIndex = search.length;
                    let word = value.substr(startIndex, endIndex);
                    return value.replace(word, "<b>" + word + "</b>");
                } else {
                    return value;
                }
            }
        },
    };
</script>
<style lang="less">
    @import '../../../styles/mixin.less';
    .add-courses-wrap {
        .scrollBarStyle(6px, #a2a2a9, inset 0 0 0px,  inset 0 0 0px);
        .sbf-search{
            height: 22px;
            min-width: 34px;
            font-size: 18px;
            opacity: 0.5;
        }
        .hidden {
            display: none;
        }
        //keep to align course name
        .minimize-width{
            white-space: nowrap;
            min-width: 90px;
            @media(max-width: @screen-xs){
                min-width: 90px;
            }
        }
        .v-input__slot {
            box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.17) !important;
        }
        .grey-backgound {
            background-color: #f0f0f7;
        }
        .light-purple {
            color: @purpleLight;
        }
        .cursor-pointer {
            cursor: pointer;
        }
        .checked-icon {
            font-size: 28px;
            color: @purpleLight;
        }
        .add-sbf-icon {
            color: @purpleLight;
            font-size: 28px;
        }
        .done-btn {
            color: @global-blue;
            border-radius: 36px;
            border: solid 1px @global-blue;
            background-color: transparent !important;
            &.v-btn--disabled{
                border:none;
            }
        }
        .class-list {
            background-color: #ffffff;
            max-height: 664px;
            padding-left: 0;
            overflow-y: scroll;

            &.selected-classes-list {
                position: relative;
                white-space: nowrap;
                overflow-x: scroll;
                background-color: #f0f0f7;
                overflow-y: hidden;
                // height: 54px;
                // &.higher {
                //     height: 75px;
                // }
            }
        }
        .selected-classes-container{
            .scrollBarStyle(6px, #a3a0fb, @color-blue-new);
        }
        .search-class-item{
            padding-top: 10px;
            padding-bottom: 10px;
        }
        .students-enrolled {
            color: rgba(128, 128, 128, 0.87);
            font-size: 10px;
        }
        //new
        .selected-class-item {
            max-width: 300px;
            height: 30px;
            border-radius: 16px;
            background-color: @purpleLight;
            color: lighten(@color-white, 87%);
            text-decoration: none;
        }
        .select-class-string {
            color: @color-blue-new;
            font-size: 18px;
        }
        //end new
        .list-item {
            color: inherit;
            display: flex;
            margin: 0;
            border-bottom: solid 1px #f0f0f7;
            text-decoration: none;
            transition: background .3s cubic-bezier(.25, .8, .5, 1);
        }
        .cant-find {
            display: flex;
            margin: 0;
            min-height: 48px;
        }
        .add-item {
            color: @global-blue;
        }
        .sbf-close {
            font-size: 8px !important;
            margin-bottom: 3px;
            margin-left: 8px;
        }
        .course-back-btn {
            &.rtl {
                /*rtl:ignore*/
                transform: rotate(-180deg);
            }
        }

    }

</style>