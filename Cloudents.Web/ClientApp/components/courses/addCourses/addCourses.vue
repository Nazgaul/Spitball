<template>
    <div class="add-courses-wrap">
        <v-layout row :class="[$vuetify.breakpoint.smAndUp ? 'py-4 px-4': 'grey-backgound py-2']" align-center justify-center>
            <v-flex grow xs10>
                <div class="d-inline-flex justify-center shrink">
                    <v-icon @click="lastStep()" class="course-back-btn mr-3" :class="{'rtl': isRtl}">sbf-arrow-back
                    </v-icon>
                    <span class="subheading font-weight-bold" v-language:inner>courses_join</span>
                    <span class="subheading font-weight-bold" v-if="quantatySelected">({{quantatySelected}})</span>
                </div>

            </v-flex>
            <v-flex xs2 shrink class="d-flex justify-end">
                <v-btn round  class="elevation-0 done-btn py-1 font-weight-bold my-0"  @click="submitAndGo()">
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

            <v-flex v-show="quantatySelected"  transition="fade-transition" style="position: relative">
                <div :class="['selected-classes-container', showBox ? 'mt-0': 'spaceTop' ]">
                    <div class="class-list selected-classes-list py-3 px-3" :class="{'higher': isFirefox}" ref="listCourse" >
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
        <v-layout align-center>
            <v-flex v-if="showBox">
                <div class="class-list search-classes-list">
                    <div class="list-item subheading search-class-item py-2 mx-2 justify-space-between align-center font-weight-regular"
                         v-for="singleClass in classes">
                        <v-layout column class="pl-3 limit-width">
                            <v-flex shrink class="text-truncate course-name-wrap">
                                <div v-html="$options.filters.boldText(singleClass.text, search)">
                                    {{ singleClass.text }}</div>
                            </v-flex>
                            <v-flex class="students-enrolled pt-2">
                                {{singleClass.students}}
                                <span class="students-enrolled" v-language:inner>courses_students</span>
                            </v-flex>
                        </v-layout>
                        <v-layout row align-center justify-end>
                            <div v-if="!singleClass.isFollowing">
                                <v-flex shrink v-if="singleClass.isSelected" class="d-flex align-center">
                                    <span class="light-purple caption font-weight-bold mr-2" v-html="$Ph('courses_joined')"></span>
                                    <span>
                                     <v-icon class="checked-icon">sbf-check-circle</v-icon>
                                   </span>
                                </v-flex>
                                <v-flex shrink v-else class="d-flex align-center">
                                    <span class="light-purple caption font-weight-bold mr-2" v-html="$Ph('courses_join')"></span>
                                    <span>
                                     <v-icon class="cursor-pointer add-sbf-icon" @click="addClass(singleClass, classes)">sbf-plus-circle</v-icon>
                                   </span>
                                </v-flex>
                            </div>
                            <v-flex v-else shrink class="d-flex align-end">
                                <span class="light-purple caption font-weight-bold mr-2" v-html="$Ph('courses_joined')"></span>
                            </v-flex>
                        </v-layout>
                    </div>
                    <!--create new course-->
                    <v-flex  class="text-xs-center align-center justify-center cant-find py-2 px-2 caption cursor-pointer" @click="changeCreateDialogState(true)">
                        <span v-language:inner>courses_cant_find</span>
                        <span class="pl-1 add-item" v-language:inner>courses_create_new</span>
                    </v-flex>
                </div>
            </v-flex>
        </v-layout>

    </div>
</template>

<script>
    import { mapActions, mapGetters } from "vuex";
    import { LanguageService } from "../../../services/language/languageService";
    import debounce from "lodash/debounce";
    import document from "../../../store/document";

    export default {
        data() {
            return {
                search: "",
                isFirefox: global.isFirefox,
                classNamePlaceholder: LanguageService.getValueByKey(
                    "courses_placeholder_find"
                ),
                isRtl: global.isRtl,
                global: global,
                localSelectedClasses: []
            };
        },

        watch: {
            search: debounce(function (val) {
                let searchVal;
                if(!!val) {
                    searchVal = val.trim();
                    if(searchVal.length >= 3) {
                        this.updateClasses(searchVal);
                    }
                }else if(val === ''){
                    this.updateClasses('');
                }
            }, 500)
        },
        computed: {
            ...mapGetters(["getSelectedClasses"]),
            dropDownAlphaHeight() {
                return {
                    maxHeight: this.$vuetify.breakpoint.xsOnly
                        ? this.global.innerHeight - 470
                        : 300
                };
            },
            quantatySelected() {
                return this.selectedClasses.length;
            },

            showBox() {
                return true
                // return !!this.search && this.search.length > 0;
            },
            classes() {
                return this.getClasses();
            },
            hideIfChoosen() {
                this.classes.some(r => this.selectedClasses.indexOf(r) >= 0);
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
                    this.localSelectedClasses.push(arrValidData)
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
                              "changeCreateDialogState"
                          ]),
            ...mapGetters(["getClasses"]),

            lastStep() {
                this.$router.go(-1);
            },

            submitAndGo(){
                //assign all saved in cached list to classes list
                this.changeClassesToCachedClasses();
                this.assignClasses().then(() => {
                    this.$router.push({name: 'editCourse'});
                });
            },
            deleteClass(classToDelete, from) {
                let index = from.indexOf(classToDelete);
                from.splice(index, 1);
            },
            checkAsSelected(classToCheck, from){
                let index = from.indexOf(classToCheck);
                from[index].isSelected = true;
            },
            addClass(className) {
                this.localSelectedClasses.push(className);
                //add to cached list
                this.addToCachedClasses(className);
                // this.pushClassToSelectedClasses(className);
                setTimeout(() => {
                    let inputElm = this.$refs.classInput;
                    inputElm.value = "";
                    inputElm.focus();
                }, 200);
                this.checkAsSelected(className, this.classes);
            },
        },
        created(){
            this.updateClasses('');


        },
        mounted(){
            let self = this;
            //mouse wheel fix horizontal scroll
            let el = self.$refs.listCourse;
            let mouseWheelEvt = function (event) {
                if (el.doScroll){
                    el.doScroll(event.wheelDelta>0?"left":"right");
                }
                else if ((event.wheelDelta || event.detail) > 0)
                {
                    el.scrollLeft -= 10;
                }
                else{
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

        .left-paddle {
            left: 0;
            top: 0;
            position: absolute;
        }
        .right-paddle {
            right: 0;
            top: 0;
            position: absolute;
        }
        .hidden {
            display: none;
        }
        .scrollBarStyle(0px, #0085D1);
        .v-input__slot{
            box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.17)!important;
        }
        .grey-backgound{
            background-color: #f0f0f7;
        }
        .light-purple {
            color: @purpleLight;
        }
        .cursor-pointer{
            cursor: pointer;
        }
        .checked-icon {
            font-size: 28px;
            color: @purpleLight;
        }
        .add-sbf-icon{
            color: @purpleLight;
            font-size: 28px;
        }
        .done-btn{
            color: @colorBlue;
            border-radius: 16px;
            border: solid 1px @colorBlue;
            background-color: transparent!important;
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
                overflow-y:hidden;
                height: 54px;
                &.higher{
                    height: 75px;
                }
            }
        }
        .students-enrolled{
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
        .cant-find{
            display: flex;
            margin: 0;
            min-height: 48px;
        }
        .add-item {
            color: @colorBlue;
        }
        .sbf-close {
            font-size: 8px !important;
            margin-bottom: 3px;
            margin-left: 8px;
        }

    }

</style>