<template>
    <div class="add-university-wrap">
        <v-layout :class="[$vuetify.breakpoint.smAndUp ? 'py-6 px-6': 'grey-backgound py-2 pl-4 pr-1']" align-center
                  justify-center>
            <v-flex grow xs10>
                <div class="d-inline-flex justify-center shrink">
                    <span class="subtitle-1 font-weight-bold" v-language:inner>university_choose_title</span>
                </div>

            </v-flex>
            <v-flex xs2 shrink class="d-flex justify-end">
                <v-btn rounded class="elevation-0 done-btn mx-2 py-1 font-weight-bold my-0" @click="getOut()">
                    <span class="text-capitalize" v-language:inner>university_not_student_btn</span>
                </v-btn>
            </v-flex>
        </v-layout>
        <v-layout column :class="{'px-3' : $vuetify.breakpoint.smAndUp}">
            <v-flex>
                <v-text-field id="university-input"
                              v-model="university"
                              @input="updateSearch($event)"
                              class="uni-input"
                              solo
                              prepend-inner-icon="sbf-search"
                              :placeholder="schoolNamePlaceholder"
                              :clear-icon="'sbf-close'"
                              @click:clear="clearData(search, university)"
                              autocomplete="off"
                              autofocus
                              spellcheck="true"
                              hide-details
                ></v-text-field>
            </v-flex>
        </v-layout>

        <v-layout align-center :class="[$vuetify.breakpoint.smAndUp ? 'px-2 mt-4': '']">
            <v-flex v-if="showBox">
                <div class="university-list" id="university-list">
                    <div class="list-item subtitle-1 cursor-pointer py-2 mx-2 justify-space-between align-center font-weight-regular"
                         v-for="singleUni in universities"
                         @click="selectUniversity(singleUni)">
                        <v-layout shrink>
                            <v-flex xs1  >
                                <span class="uni-logo">
                                    <img v-if="singleUni.image" :src="singleUni.image" alt="university logo" class="rounded uni-img">
                                    <span v-else>
                                    <empty-uni-logo></empty-uni-logo>
                                    </span>
                                </span>
                            </v-flex>
                        </v-layout>
                        <v-layout column class="ml-4 limit-width">
                            <v-flex shrink>
                                <div v-html="$options.filters.boldText(singleUni.text, search)">
                                    {{ singleUni.text }}
                                </div>
                            </v-flex>
                            <v-flex class="students-enrolled pt-1">
                                {{singleUni.students}}
                                <span class="students-enrolled" v-language:inner>courses_students</span>
                            </v-flex>
                        </v-layout>
                    </div>
                    <!--create new University-->
                    <v-flex class="text-center align-center justify-center cant-find py-2 px-2 caption cursor-pointer"
                            @click.prevent="openCreateUniDialog()">
                        <span v-language:inner>university_cant_find</span>
                        <span class="pl-1 add-item" v-language:inner>university_create_new</span>
                    </v-flex>
                </div>
            </v-flex>
        </v-layout>
    </div>
</template>


<script>
    import { mapActions, mapGetters, mapMutations } from 'vuex';
    import debounce from "lodash/debounce";
    import { LanguageService } from "../../../services/language/languageService";
    import emptyUniLogo from '../images/empty-uni-logo.svg';

    export default {
        components: {emptyUniLogo},
        data() {
            return {
                isLoading: false,
                isComplete: false,
                page: 0,
                term: '',
                universityModel: '',
                search: '',
                schoolNamePlaceholder: LanguageService.getValueByKey('university_create_uni_placeholder'),
                globalHeight: global.innerHeight,
                isRtl: global.isRtl
            };
        },
        watch: {
            search: debounce(function () {
                
                let searchVal = '';
                if(!!this.search) {
                    searchVal = this.search.trim();
                }
                this.term = searchVal;
                let paramObj = {term : searchVal, page: 0};
                this.loadUniversities(paramObj);
            }, 500)
        },
        computed: {
            ...mapGetters(["getUniversities", "getSchoolName", "accountUser", "getSelectedClasses"]),
            showBox() {
                if(this.search && this.search.length > 0) {
                    return true;
                }
                return true;
            },
            universities() {
                return this.getUniversities;
            },
            university: {
                get: function () {
                    let schoolNameFromStore = this.getSchoolName;
                    return schoolNameFromStore || this.universityModel;
                },
                set: function (newValue) {
                    this.universityModel = newValue;
                    this.setSchoolName(newValue)
                }
            }
        },
        methods: {
            ...mapActions([
                              "updateUniversities",
                              "addUniversities",
                              "clearUniversityList",
                              "updateSchoolName",
                              "changeUniCreateDialogState"
                          ]),
            ...mapMutations(['UPDATE_SEARCH_LOADING','setSchoolName']),
            clearData(search, university) {
                search = '';
                university = undefined;
                this.clearUniversityList();
            },
            openCreateUniDialog() {
                this.changeUniCreateDialogState(true);
            },
            getOut() {
                let classesSet = this.getSelectedClasses && this.getSelectedClasses.length > 0;
                classesSet ? this.$router.push({name: 'feed'}) : this.$router.push({name: 'editCourse'});
            },
            loadUniversities(paramObj){
                let self = this;
                self.isComplete = false;
                self.isLoading = true;
                self.updateUniversities(paramObj).then((hasData) => {
                    if (!hasData) {
                        self.isComplete = true;
                    }
                    self.isLoading = false;
                    self.page = 1;
                }, (err) => {
                    self.isComplete = true;
                })
            },
            concatUniversities(paramObj){
                let self = this;
                self.isLoading = true;
                self.addUniversities(paramObj).then((hasData) => {
                    if (!hasData) {
                        self.isComplete = true;
                        return;
                    }
                    if(hasData.length < 30){
                        self.isComplete = true;
                    }
                    self.isLoading = false;
                    self.page++;
                }, (err) => {
                    self.isComplete = true;
                })
            },
            keepLoad(clientHeight, scrollTop){
                let totalHeight = clientHeight;
                let currentScroll = scrollTop;
                let scrollOffset = (currentScroll > (0.75 * totalHeight));
                let retVal = (!this.isLoading && !this.isComplete && currentScroll > 0 && scrollOffset);
                return retVal
            },
            scrollUniversities(e){
                let clientHeight = e.target.scrollHeight - e.target.offsetHeight;
                let scrollTop = e.target.scrollTop;
                    if(this.keepLoad(clientHeight, scrollTop)){
                        let paramObj = {term: this.term, page: this.page};
                        this.concatUniversities(paramObj)
                    }
            },
            updateSearch(val) {
                this.search = val;
            },
            checkBeforeNextStep(universityName) {
                let user = this.accountUser;
                if(!!user && user.universityExists) {
                    //compare previous and current school name, if different show popup
                    let previousSchoolName = this.getSchoolName;
                    let currentSchoolName = universityName;
                    return previousSchoolName.toLowerCase() !== currentSchoolName.toLowerCase();
                }else{
                    return true
                }
            },
            selectUniversity(university) {
                let schoolName = university.text ? university.text : '';
                let uniId = university.id;
                if(!schoolName) {
                    return;
                }
                let objToSend = {
                    name: schoolName,
                    id: uniId
                };
                // check if changed
                if(this.checkBeforeNextStep(schoolName)) {
                    //new if changed
                    this.updateSchoolName(objToSend)
                        .then((success) => {
                                this.UPDATE_SEARCH_LOADING(true);
                                this.getOut();
                              },
                              (error) => {
                                  console.log('error', error);
                              }
                        );
                } else {
                    //skip if not
                    this.getOut();
                }

            },
        },
        created(){
            let paramObj = {term: this.term, page: this.page};
            this.loadUniversities(paramObj);
            this.$nextTick(function(){
                let scrollableElm = document.getElementById('university-list');
                if(scrollableElm){
                    scrollableElm.addEventListener('scroll', (e)=>{
                        this.scrollUniversities(e)
                    })
                }
            })
        },
        filters: {
            boldText(value, search) {
                let match;
                //mark the text bold according to the search value
                if(!value) return '';
                if(!!search) {
                    match = value.toLowerCase().indexOf(search.toLowerCase()) > -1;
                }
                if(match) {
                    let startIndex = value.toLowerCase().indexOf(search.toLowerCase());
                    let endIndex = search.length;
                    let word = value.substr(startIndex, endIndex);
                    return value.replace(word, '<b>' + word + '</b>');
                } else {
                    return value;
                }
            }
        },


    };
</script>


<style lang="less">
    @import '../../../styles/mixin.less';
    .add-university-wrap {
        .scrollBarStyle(6px, #a2a2a9, inset 0 0 0px,  inset 0 0 0px);
        .rounded {
            border-radius: 50%;
            width: 42px;
            height: 42px;
        }
        .minimize-width{
            min-width: 90px;
            @media(max-width: @screen-xs){
                min-width: 90px;
            }
        }
        .uni-logo {
            border: 1px solid rgb(221, 221, 221);
            background-color: rgb(240, 240, 247);
            height: 42px;
            width: 42px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;

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
            min-width: 88px;
            color: @global-blue;
            border-radius: 36px;
            border: solid 1px @global-blue;
            background-color: transparent !important;
            @media(max-width: @screen-xs){
                min-width: unset;
            }
        }
        .university-list {
            background-color: #ffffff;
            max-height: 664px;
            padding-left: 0;
            overflow-y: auto;
        }
        .students-enrolled {
            color: rgba(128, 128, 128, 0.87);
            font-size: 10px;
        }
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
    }

</style>