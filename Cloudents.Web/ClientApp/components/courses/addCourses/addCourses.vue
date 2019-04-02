<template>
    <div class="add-courses-wrap">
        <v-layout row :class="[$vuetify.breakpoint.smAndUp ? 'py-4 px-4': 'grey-backgound py-2']" align-center justify-center>
            <v-flex grow xs10>
                <div class="d-inline-flex justify-center shrink">
                    <v-icon @click="lastStep()" class="course-back-btn mr-3" :class="{'rtl': isRtl}">sbf-arrow-back
                    </v-icon>
                    <span class="subheading font-weight-bold">Join Courses</span>
                    <span class="subheading font-weight-bold" v-if="quantatySelected">({{quantatySelected}})</span>
                </div>

            </v-flex>
            <v-flex xs2 shrink class="d-flex justify-end">
                <v-btn round  class="elevation-0 done-btn py-1 font-weight-bold my-0"  @click="submitAndGo()">
                    <span v-language:inner>uniSelect_done</span>
                </v-btn>
            </v-flex>
        </v-layout>
        <v-layout column :class="{'px-3' : $vuetify.breakpoint.smAndUp}">
            <v-flex>
                <v-text-field id="classes_input"
                              v-model="search"
                              class="class-input"
                              solo
                              prepend-inner-icon="sbf-search"
                              :placeholder="classNamePlaceholder"
                              autocomplete="off"
                              autofocus
                              spellcheck="true"
                              hide-details

                ></v-text-field>
            </v-flex>

            <v-flex v-show="quantatySelected"  transition="fade-transition">
                <div :class="['selected-classes-container', showBox ? 'mt-0': 'spaceTop' ]">
                    <div class="class-list selected-classes-list py-3 px-3">
                        <div class="selected-class-item d-inline-flex text-truncate font-weight-bold align-center justify-center pl-3 pr-1  py-1 mr-2"
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
            <v-flex v-if="showBox" class="search-classes-container">
                <div class="class-list search-classes-list">
                    <div class="list-item search-class-item cursor-pointer"
                         v-for="singleClass in classes">
                        <div v-html="$options.filters.boldText(singleClass.text, search)">
                            {{ singleClass.text }}</div>
                        <v-layout row align-center justify-end>
                            <div v-if="!singleClass.isFollowing">
                                <v-flex shrink v-if="singleClass.isSelected" class="d-flex align-center">
                                    <span class="light-purple caption font-weight-bold mr-2">Joined</span>
                                    <span>
                                     <v-icon class="checked-icon">sbf-check-circle</v-icon>
                                   </span>
                                </v-flex>
                                <v-flex shrink v-else class="d-flex align-center">
                                    <span class="light-purple caption font-weight-bold mr-2">Join</span>
                                    <span>
                                     <v-icon class="add-sbf-icon" @click="addClass(singleClass, classes)">sbf-plus-circle</v-icon>
                                   </span>
                                </v-flex>
                            </div>
                            <v-flex v-else shrink class="d-flex align-end">
                                <span class="light-purple caption font-weight-bold mr-2">Joinded</span>
                            </v-flex>
                        </v-layout>
                    </div>
                    <!--create new course-->
                    <v-flex  class="text-xs-center align-center justify-center cand-find caption cursor-pointer" @click="addManualUniversity()">
                        <span>Can't Find your course?</span>
                        <span class="add-item">Create new</span>
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

    export default {
        data() {
            return {
                search: "",
                classNamePlaceholder: LanguageService.getValueByKey(
                    "uniSelect_type_class_name_placeholder"
                ),
                isRtl: global.isRtl,
                global: global,
                localSelectedClasses: []
            };
        },
        // props: {
        //     callbackFunc: {
        //         required: true,
        //         type: Object
        //     },
        // },
        watch: {
            search: debounce(function (val) {
                let searchVal;
                if(!!val) {
                    searchVal = val.trim();
                    if(searchVal.length >= 3) {
                        this.updateClasses(searchVal);
                    }
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
            schoolName() {
                return this.getSchoolName();
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
            //edge hide placehloder fix
            placeholderVisible() {
                return this.getSelectedClasses.length < 1;
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
                              "addToCachedClasses"
                          ]),
            ...mapGetters(["getSchoolName", "getClasses"]),
            lastStep() {
                this.$router.go(-1);
            },
            nextStep(customClass) {
                this.callbackFunc.next();
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
                    let inputElm = document.getElementById('classes_input');
                    inputElm.value = "";
                    inputElm.focus();
                }, 200);
                this.checkAsSelected(className, this.classes);
            },
        },
        created(){
            this.updateClasses('');
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
        .search-classes-container {
            margin-top: 2px;
        }
        .class-list {
            background-color: #ffffff;
            max-height: 664px;
            overflow-y: scroll;
            padding-left: 0;
            &.selected-classes-list {
                overflow: auto;
                white-space: nowrap;
                background-color: #f0f0f7;
            }
        }
        //new
        .selected-class-item {
            max-width: 300px;
            height: 30px;
            border-radius: 16px;
            background-color: @purpleLight;
            color: lighten(@color-white, 87%);
            font-size: 12px;
            text-decoration: none;
        }
        .select-class-string {
            color: @color-blue-new;
            font-size: 18px;
        }
        //end new
        .list-item {
            align-items: center;
            justify-content: space-between;
            color: inherit;
            display: flex;
            font-size: 16px;
            font-weight: 400;
            height: 48px;
            margin: 0;
            padding: 0 16px;
            /*position: relative;*/
            text-decoration: none;
            transition: background .3s cubic-bezier(.25, .8, .5, 1);
        }
        .cand-find{
            height: 48px;
            margin: 0;
            padding: 0 16px;
        }
        .add-item {
            color: @colorBlue;
        }
        .search-class-item {
            &:hover {
                background: rgba(0, 0, 0, .04);
            }
        }
        .sbf-close {
            font-size: 8px !important;
            margin-bottom: 3px;
            margin-left: 8px;
        }

    }

</style>