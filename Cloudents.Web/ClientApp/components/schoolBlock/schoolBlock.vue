<template>
    <div style="display: flex;" class="school-block">
        <v-layout row>
            <v-flex xs12>
                <div class="content-wrap">
                    <div class="university-holder d-flex" row>
                        <div v-show="schoolName">
                            <v-icon class="university-icon">sbf-university-columns</v-icon>
                            <span class="university-name">{{schoolName}}</span>
                        </div>
                        <div class="empty-school" v-show="!schoolName" @click="openPersonalizeUniversity()">
                            <v-icon class="empty-university-icon">sbf-university-columns</v-icon>
                            <span class="empty-university-name" v-language:inner>schoolBlock_school_empty_text</span>
                        </div>
                    </div>
                    <div class="classes-holder">
                        <div v-show="!showAllClassesBlock">
                            <v-chip name="sbf-class-chip" v-if="isClassesSet" class="sbf-class-chip mb-2"
                                    v-for="(singleClass, index) in classesList"
                                    @click="updateClass(singleClass)"
                                    :selected="singleClass.isSelected"
                                    v-show="index < desClassesToShow"
                                    :key="index">{{singleClass.text}}
                            </v-chip>
                            <v-chip name="sbf-class-chip" class="sbf-class-chip total border-none mb-2"
                                    v-show="classesList.length > desClassesToShow"
                                    @click="openAllClasses()">
                            <span>
                               {{classesPlus}}
                            </span>
                            </v-chip>
                            <v-chip v-if="!isClassesSet" name="add class" class="sbf-class-chip empty-state-class mb-2"
                                    outline
                                    @click="openPersonalizeCourse()">
                                <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                                <span v-language:inner>schoolBlock_add_class</span>
                            </v-chip>
                        </div>


                        <div class="all-classes" v-if="showAllClassesBlock" transition="fade-transition">
                            <div class="classes-list-wrap">
                                <v-chip name="sbf-class-chip" class="class-chip-item mb-2"
                                        v-for="(singleClass, index) in classesList"
                                        @click="updateClass(singleClass)"
                                        :selected="singleClass.isSelected"
                                        :key="index">{{singleClass.text}}
                                </v-chip>
                            </div>
                            <div class="edit-action-block">
                                <div>
                                    <v-icon>sbf-edit-icon</v-icon>
                                    <span v-language:inner>schoolBlock_edit_classes</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </v-flex>
        </v-layout>

    </div>
</template>

<script>
    import { mapGetters, mapActions, mapMutations } from 'vuex';
    import schoolBlockService from '../../services/schoolBlockService'

    export default {
        name: "schoolBlock",
        data() {
            return {
                showAllClassesBlock: false,
                selectedChips: {},
                desClassesToShow: 5,
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
                    return `+${this.classesList.length - this.desClassesToShow}`
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
                this.showAllClassesBlock = true;
            },
            openPersonalizeCourse() {
                if (!this.isLoggedIn) {
                    this.updateLoginDialogState(true);
                } else {
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


        },
        created() {
            if (!!this.$route.query.Course) {
                let courses = [].concat(this.$route.query.Course)
                courses.forEach(courseName => {
                    this.selectedChips[courseName] = true;
                })
            }
        }
    }

</script>

<style lang="less" src="./schoolBlock.less">

</style>