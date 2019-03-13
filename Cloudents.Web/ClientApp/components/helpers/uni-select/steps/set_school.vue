<template>
    <!--<div class="select-university-container set-school" :class="{'selected': (!!search && search.length > 10)}">-->
    <div class="" :class="{'selected': (!!search && search.length > 10)}">
        <div class="title-container">
            <span v-language:inner>uniSelect_select_school</span>
            <!-- <a v-show="showNext" class="next-container" :class="{'loading': nextPressed}" @click="checkBeforeNextStep()"
               v-language:inner>uniSelect_next</a> -->

        </div>
        <div class="select-school-container">
            <v-combobox
                    class="hello-gaby"
                    v-model="university"
                    :items="universities"
                    :label="schoolNamePlaceholder"
                    :placeholder="schoolNamePlaceholder"
                    clearable
                    solo
                    :menu-props="{maxHeight: $vuetify.breakpoint.xsOnly ? dropDownAlphaHeight : 300}"
                    :search-input.sync="search"
                    :append-icon="''"
                    :clear-icon="'sbf-close'"
                    @click:clear="clearData(search, university)"
                    autofocus
                    no-filter
                    @keyup.delete="preventFocusLoose($event)"
                    :background-color="'rgba( 255, 255, 255, 1)'"
            >
                <template slot="no-data">
                    <!-- <v-list-tile v-show="showBox">
                        <div class="subheading" v-language:inner>uniSelect_keep_typing</div>
                    </v-list-tile>
                    <v-list-tile>
                        <div style="cursor:pointer;" @click="getAllUniversities()" class="subheading dark"
                             v-language:inner>uniSelect_show_all_schools
                        </div>
                    </v-list-tile> -->
                    <v-list-tile>
                        <div style="cursor:pointer;" @click="addManualUniversity()" class="subheading dark"
                             v-language:inner>uniSelect_didnt_find_university
                        </div>
                    </v-list-tile>
                </template>
                <template slot="item" slot-scope="{ index, item, parent }">
                    <v-list-tile-content style="max-width:385px;">
                        <span v-html="$options.filters.boldText(item.text, search)">{{ item.text }}</span>
                    </v-list-tile-content>
                </template>
            </v-combobox>
        </div>

        <div class="skip-container" v-if="$vuetify.breakpoint.xsOnly">
            <a @click="skipUniSelect()" v-language:inner>uniSelect_skip_for_now</a>
        </div>

    </div>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import debounce from "lodash/debounce";
    import { LanguageService } from "../../../../services/language/languageService";


    export default {
        props: {
            fnMethods: {
                required: true,
                type: Object
            },
            enumSteps: {
                required: true,
                type: Object
            }
        },
        data() {
            return {
                universityModel: '',
                search: '',
                schoolNamePlaceholder: LanguageService.getValueByKey('uniSelect_type_school_name_placeholder'),
                globalHeight: global.innerHeight,
                nextPressed: false
            }
        },
        watch: {
            search: debounce(function () {
                if (!!this.search) {
                    let searchVal = this.search.trim();
                    if(searchVal.length >= 2)
                    this.updateUniversities(searchVal);
                }
                if (this.search === "") {
                    this.clearData();
                }
            }, 500)
        },
        filters: {
            boldText(value, search) {
                let match;
                //mark the text bold according to the search value
                if (!value) return '';
                if (!!search) {
                    match = value.toLowerCase().indexOf(search.toLowerCase()) > -1;
                }
                if (match) {
                    let startIndex = value.toLowerCase().indexOf(search.toLowerCase())
                    let endIndex = search.length;
                    let word = value.substr(startIndex, endIndex)
                    return value.replace(word, '<b>' + word + '</b>')
                } else {
                    return value;
                }

            }
        },
        methods: {
            ...mapActions(["updateUniversities", "clearUniversityList", "updateSchoolName", "changeSelectUniState", "setUniversityPopStorage_session"]),
            ...mapGetters(["getUniversities", "getSchoolName", "accountUser"]),
            clearData(search, university) {
                search = '';
                university = undefined;
                this.clearUniversityList();
            },
            preventFocusLoose(event){
                let val = event.target.value;
                if(val.length===0){
                    event.preventDefault();
                }
            },
            getAllUniversities() {
                //leave space
                this.updateUniversities(' ');
            },
            skipUniSelect() {
                this.fnMethods.openNoWorriesPopup();
            },
            nextStep(dontChangeUniversity, universityName) {
                if (dontChangeUniversity) {
                    this.fnMethods.changeStep(this.enumSteps.set_class)
                } else {
                    let schoolName = universityName ? universityName : this.getCurrentSchoolName();
                    if (!schoolName) {
                        console.log("No university name found")
                        return;
                    };
                    this.nextPressed = true;
                    this.updateSchoolName(schoolName).then(() => {
                        this.fnMethods.changeStep(this.enumSteps.set_class);
                    }).finally(()=>{
                        this.nextPressed = false;
                    });
                }
            },
            checkBeforeNextStep() {
                let user = this.accountUser();
                if (!!user && user.universityExists) {
                    //compare previous and current school name, if different show popup
                    let previousSchoolName = this.getSchoolName();
                    let currentSchoolName = this.getCurrentSchoolName();
                    if (previousSchoolName.toLowerCase() !== currentSchoolName.toLowerCase()) {
                        // this.fnMethods.openAreYouSurePopup(this.nextStep);
                        //TODO  v15 are you sure commented, take to next step without confirmation
                        this.nextStep(false);
                    } else {
                        //if the same university is presented then skip the set on the server
                        this.nextStep(true);
                    }
                } else {
                    this.nextStep();
                }
            },
            addManualUniversity(){
                this.fnMethods.openAddSchoolOrClass(true, this.nextStep);
            },
            getCurrentSchoolName() {
                //!!this.universityModel.text ? this.universityModel.text : !!this.universityModel ? this.universityModel : !!this.search ? this.search : this.university;
                if (!!this.universityModel) {
                    if (!!this.universityModel.text) {
                        return this.universityModel.text;
                    } else {
                        return this.universityModel;
                    }
                } else if (!!this.search) {
                    return this.search;
                } else {
                    return this.university;
                }
            }
        },
        computed: {
            dropDownAlphaHeight(){
                return this.globalHeight - 470
            },
            showBox() {
                if (this.search && this.search > 0) {
                    return true
                }
            },
            showNext() {
                if (this.search && this.search.length > 10) {
                    return true
                }
            },
            universities() {
                return this.getUniversities();
            },
            university: {
                get: function () {
                    let schoolNameFromStore = this.getSchoolName();
                    return schoolNameFromStore || this.universityModel
                },
                set: function (newValue) {
                    this.universityModel = newValue
                    if(!!newValue && newValue.id){
                        this.checkBeforeNextStep();
                    }
                }
            }
        },

    }
</script>

<style lang="less" scoped>
    .subheading {
        font-size: 16px;
        font-weight: normal;
        font-style: normal;
        font-stretch: normal;
        line-height: normal;
        letter-spacing: normal;
        color: rgba(0, 0, 0, 0.38);
        &.dark {
            font-size: 13px !important;
            color: rgb(74, 135, 251);
        }
    }
</style>
