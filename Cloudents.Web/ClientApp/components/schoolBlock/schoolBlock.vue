<template>
    <transition name="fade">
        <div style="" class="school-block">
            <v-layout row>
                <v-flex xs12>
                    <div class="content-wrap">
                        <div class="university-holder d-flex" row>
                            <div v-show="schoolName && !mobileFilterState">
                                <v-icon class="university-icon">sbf-university-columns</v-icon>
                                <span class="university-name">{{schoolName}}</span>
                            </div>
                            <div v-show="mobileFilterState">
                                <span class="filter-text" v-language:inner>schoolBlock_filter_classes</span>
                            </div>
                            <div class="empty-school" v-show="!schoolName" @click="openPersonalizeUniversity()">
                                <v-icon class="empty-university-icon">sbf-university-columns</v-icon>
                                <span class="empty-university-name"
                                      v-language:inner>schoolBlock_school_empty_text</span>
                            </div>
                        </div>
                        <div class="classes-holder" >
                            <div v-show="!showAllClassesBlock"
                                 :class="[$vuetify.breakpoint.xsOnly ? 'd-flex  wrap-it align-start' : '']">
                                <transition-group name="list">
                                    <v-chip name="sbf-class-chip list-item" key="chip_one"
                                            v-for="(singleClass, index) in classesList"
                                            v-if="isClassesSet"
                                            class="sbf-class-chip"
                                            :class="[$vuetify.breakpoint.xsOnly ? 'mb-2' : '',
                                            mobileFilterState ?  'full-width-chip' : '']"
                                            @click="updateClass(singleClass)"
                                            :selected="singleClass.isSelected"
                                            v-show="index < classesToShow"
                                            :key="index">{{singleClass.text}}
                                    </v-chip>
                                </transition-group>
                                <transition name="fade-total">
                                    <v-chip name="sbf-class-chip" key="chip_two"
                                            class="sbf-class-chip total"
                                            :class="[$vuetify.breakpoint.smAndUp ? 'border-none' : '' ]"
                                            v-show="classesList.length > classesToShow"
                                            @click.prevent.stop="openAllClasses()">
                                        <span>
                                           {{classesPlus}}
                                        </span>
                                    </v-chip>
                                </transition>

                                <v-chip v-if="!isClassesSet" name="add class" class="sbf-class-chip empty-state-class"

                                        @click="openPersonalizeCourse()">
                                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                                    <span v-language:inner>schoolBlock_add_class</span>
                                </v-chip>
                            </div>
                            <div class="all-classes" v-show="showAllClassesBlock" transition="fade-transition"
                                 id="school_block_classesList">
                                <div class="classes-list-wrap">
                                    <v-chip class="class-chip-item"
                                            v-for="(singleClass, index) in classesList"
                                            @click="updateClass(singleClass)"
                                            :selected="singleClass.isSelected"
                                            :key="index">{{singleClass.text}}
                                    </v-chip>
                                </div>
                                <edit-action-block  @click.prevent.stop="openPersonalizeCourse()"></edit-action-block>
                            </div>
                        </div>
                    </div>
                </v-flex>
            </v-layout>
            <edit-action-block id="edit-mobile-target" v-show="mobileFilterState"  @click.prevent="openPersonalizeCourse()"></edit-action-block>
        </div>
    </transition>
</template>

<script>
    import { mapGetters, mapActions, mapMutations } from 'vuex';
    import schoolBlockService from '../../services/schoolBlockService'
    import editActionBlock from './helpers/editActionBlock.vue'
    export default {
        components:{editActionBlock},
        name: "schoolBlock",
        data() {
            return {
                showAllClassesBlock: false,
                selectedChips: {},
                classesToShow: this.$vuetify.breakpoint.smAndUp ? 5 : 3,
                mobileFilterState: false
            }
        },
        computed: {
            ...mapGetters([
                "getSelectedClasses",
                "getSchoolName",
                "getAllSteps",
                "accountUser"
            ]),
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
                            result.push(chipItem)
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
                return result;
            },
        },
        methods: {
            ...mapActions(["updateLoginDialogState", "updateCurrentStep", "changeSelectUniState"]),
            ...mapMutations(['UPDATE_SEARCH_LOADING']),
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
                this.updateFilter();
                this.$forceUpdate();
            },
            updateFilter() {
                this.UPDATE_SEARCH_LOADING(true);
                let newQueryArr = Object.keys(this.selectedChips);
                let newQueryObject = {
                    Course: newQueryArr
                };
                this.$router.push({query: newQueryObject});
            },
            openAllClasses() {
                if (this.$vuetify.breakpoint.smAndUp) {
                    this.showAllClassesBlock = true
                } else {
                    this.classesToShow = this.classesList.length;
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
                let specifiedElement = document.getElementById('school_block_classesList');
                let isClickInside = specifiedElement.contains(event.target);
                if (this.$vuetify.breakpoint.smAndUp && this.showAllClassesBlock && !isClickInside) {
                    this.closeAllClasses();
                } else {
                    if(event.target.id && event.target.id ==="edit-mobile-target"){
                        this.openPersonalizeCourse()
                    }else{
                        this.classesToShow = 3;
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
                })
            }
        }

    }

</script>

<style lang="less" src="./schoolBlock.less">

</style>