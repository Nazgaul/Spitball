<template>
        <div style="" :class="['school-block', isClassesSet ? 'pb-0' : '', minMode ? '' : 'expand' ]">
            <v-layout row>
                <v-flex xs12>
                    <div class="school-content-wrap">
                        <div class="university-holder" row>
                            <span class="tour-inject"></span>
                            <div class="uni-holder" v-show="schoolName && !mobileFilterState"
                                 @click="openPersonalizeUniversity()">
                                <v-icon class="university-icon ">sbf-university-columns</v-icon>
                                <span class="university-name">{{schoolName}}</span>
                            </div>
                            <div v-show="mobileFilterState">
                                <span class="filter-text" v-language:inner>schoolBlock_filter_classes</span>
                            </div>
                            <div class="empty-school" v-show="!schoolName" @click="openPersonalizeUniversity()">
                                <v-icon class="empty-university-icon">sbf-university-columns</v-icon>
                                <span class="empty-university-name "
                                      v-language:inner>schoolBlock_school_empty_text</span>
                            </div>
                        </div>
                        <div :class="['classes-holder', isClassesSet ? '' : 'emptyState' ]" >
                            <div v-show="!showAllClassesBlock"
                                 :class="[$vuetify.breakpoint.xsOnly ? 'd-flex  wrap-it align-start' : '']">
                                <transition-group name="list">
                                    <v-chip name="sbf-class-chip list-item" :key="index"
                                            v-for="(singleClass, index) in classesList"
                                            v-if="isClassesSet"
                                            class="sbf-class-chip"
                                            :class="[$vuetify.breakpoint.xsOnly ? 'mb-2' : '',
                                            isDisabled ? 'cursor-default' : '',
                                            mobileFilterState ?  'full-width-chip' : '']"
                                            @click="isDisabled ? '' : updateClass(singleClass)"
                                            :disabled="isDisabled"
                                            :selected="singleClass.isSelected"
                                            v-show="minMode ? index < classesToShow : true"
                                            >{{singleClass.text}}
                                    </v-chip>
                                </transition-group>
                                <transition-group name="dissapear-total-chip">
                                <!--<transition name="dissapear-total-chip">-->
                                    <span name=" sbf-class-chip" key="chip-total" v-show="isLoggedIn"
                                            class="sbf-class-chip total classes-total-chip"
                                         >
                                        <span
                                              @click.prevent.stop="openAllClasses()"
                                              v-show="minMode ? classesList.length > classesToShow : false">
                                           {{classesPlus}}
                                        </span>
                                        <span class="d-flex"
                                              @click.prevent.stop="openPersonalizeCourse()"
                                              v-if="minMode ? classesList.length <= classesToShow && isClassesSet : false">
                                            <v-icon class="small-font" color="white" >sbf-edit-icon</v-icon>
                                        </span>
                                    </span>
                                <!--</transition>-->
                                </transition-group>
                                <v-chip v-if="!isClassesSet && schoolName" name="add class" class="sbf-class-chip empty-state-class"
                                        @click="openPersonalizeCourse()">
                                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                                    <span v-language:inner>schoolBlock_add_class</span>
                                </v-chip>
                            </div>
                            <transition name="slide-y-transition">
                            <div class="all-classes" v-show="showAllClassesBlock && !$vuetify.breakpoint.xsOnly"
                                 id="school_block_classesList">
                                <div class="classes-list-wrap">
                                    <v-chip class="class-chip-item"
                                            v-for="(singleClass, index) in classesList"
                                             @click="isDisabled ? '' : updateClass(singleClass)"
                                            :disabled="isDisabled"
                                            :selected="singleClass.isSelected"
                                            :key="index">{{singleClass.text}}
                                    </v-chip>
                                </div>
                                <edit-action-block @click.native="openPersonalizeCourse()"></edit-action-block>
                            </div>
                            </transition>
                        </div>
                    </div>
                </v-flex>
            </v-layout>
            <edit-action-block id="edit-mobile-target" v-show="mobileFilterState && $vuetify.breakpoint.xsOnly"
                               @click.native="openPersonalizeCourse()"></edit-action-block>
        </div>

</template>

<script>
    import { mapGetters, mapActions, mapMutations } from 'vuex';
    import schoolBlockService from '../../services/schoolBlockService'
    import editActionBlock from './helpers/editActionBlock.vue'

    export default {
        components: {editActionBlock},
        name: "schoolBlock",
        data() {
            return {
                showAllClassesBlock: false,
                selectedChips: {},
                mobileFilterState: false,
                minMode: true
            }
        },
        props: {
            isDisabled: {
                type: Boolean,
                default: false
            },
        },
        computed: {
            ...mapGetters([
                "getSelectedClasses",
                "getSchoolName",
                "getAllSteps",
                "accountUser"
            ]),
            classesToShow(){
                return this.$vuetify.breakpoint.smAndUp ? 5 : 3;
            },
            classesPlus() {
                if (!!this.classesList)
                    return `+${this.classesList.length - this.classesToShow}`
            },
            isLoggedIn() {
                return !!this.accountUser
            },
            isClassesSet() {
                return this.getSelectedClasses.length > 0
            },
            schoolName() {
                return this.getSchoolName
            },
            classesList() {
                let result = [];
                let chipItem;
                if (this.isClassesSet) {
                    this.getSelectedClasses.forEach(chip => {
                        if (chip.text) {
                            chip.isSelected = !!this.selectedChips[chip.text];
                            chipItem = schoolBlockService.createChipItem(chip);
                            result.push(chipItem);
                        } else {
                            let newChip = {
                                text: chip,
                                isSelected: !!this.selectedChips[chip.text]
                            };
                            chipItem = schoolBlockService.createChipItem(newChip);
                            result.push(chipItem)
                        }
                    })
                }
                this.sortClassesByIsSelected(result, 'isSelected');
                return result;
            },
        },
        methods: {
            ...mapActions(["updateLoginDialogState", "updateCurrentStep", "changeSelectUniState"]),
            ...mapMutations(['UPDATE_SEARCH_LOADING', 'UPDATE_LOADING']),
            updateClass(val) {
                if (!!this.selectedChips[val.text]) {
                    //remove from selected chips dictionary
                    val.isSelected = false;
                    delete this.selectedChips[val.text];
                } else {
                    //add
                    val.isSelected = true;
                    this.selectedChips[val.text] = true;
                }

                this.sortClassesByIsSelected(this.classesList, 'isSelected');
                this.updateFilter();
                this.$forceUpdate();
            },
            updateFilter() {
                this.UPDATE_SEARCH_LOADING(true);
                this.UPDATE_LOADING(true);
                let newQueryArr = Object.keys(this.selectedChips);
                let newQueryObject = {
                    Course: newQueryArr
                };
                let filter = this.$route.query.Filter;
                if(filter){
                    newQueryObject.Filter = filter;
                }else{
                    delete newQueryObject.Filter;
                }
                this.$router.push({query: newQueryObject});
            },
            sortClassesByIsSelected(arr, sortBy) {
                arr.sort(function (obj1, obj2,) {
                    // Ascending: first age less than the previous
                    return obj2[sortBy] - obj1[sortBy];
                });
            },
            openAllClasses() {
                if (this.$vuetify.breakpoint.smAndUp) {
                    this.showAllClassesBlock = true
                } else {
                    //this.classesToShow = this.classesList.length;
                    this.minMode = false;
                    this.mobileFilterState = true;
                }
            },
            closeAllClasses() {
                this.showAllClassesBlock = false;
            },

            openPersonalizeCourse() {
                if (!this.isLoggedIn) {
                    this.updateLoginDialogState(true);
                } else {
                    this.closeAllClasses();
                    let steps = this.getAllSteps;
                    this.updateCurrentStep(steps.set_class);
                    this.changeSelectUniState(true);
                }
            },
            openPersonalizeUniversity() {
                if (!this.isLoggedIn) {
                    this.updateLoginDialogState(true);
                } else {
                    let steps = this.getAllSteps;
                    this.updateCurrentStep(steps.set_school);
                    this.changeSelectUniState(true);
                }
            },
            detectOutsideClick(event) {
                let isClickInside = false;
                let ignoredElements = null;
                if (this.$vuetify.breakpoint.smAndUp) {
                    ignoredElements = document.querySelector('#school_block_classesList');
                    isClickInside = ignoredElements.contains(event.target);
                    if (this.showAllClassesBlock && !isClickInside) {
                        this.closeAllClasses();
                    }
                } else {
                    ignoredElements = document.querySelector('.school-block');
                    isClickInside = ignoredElements.contains(event.target);
                    if (!isClickInside) {
                        //this.classesToShow = 3;
                        this.minMode = true;
                        this.mobileFilterState = false;
                    }
                }

            }
        },
        beforeMount: function () {
            if (document) {
                document.addEventListener('click', this.detectOutsideClick)
            }
        },
        beforeDestroy: function () {
            if (document) {
                document.removeEventListener('click', this.detectOutsideClick)
            }
        },
        created() {
            if (!!this.$route.query.Course) {
                let courses = [].concat(this.$route.query.Course);
                courses.forEach(courseName => {
                    this.selectedChips[courseName] = true;
                });
            }
        }

    }

</script>

<style lang="less" src="./schoolBlock.less">

</style>