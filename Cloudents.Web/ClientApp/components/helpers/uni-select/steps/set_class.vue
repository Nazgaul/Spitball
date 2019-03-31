<template>
    <div>
        <div class="title-container">
            <div class="first-container">
                <div>
                    <v-icon @click="lastStep()" :class="{'rtl': isRtl}">sbf-arrow-back</v-icon>
                </div>
                <div>
                    <a class="next-container" @click="nextStep()" v-language:inner>uniSelect_done</a>
                </div>
            </div>
            <div class="select-class-string">
                <span v-language:inner>uniSelect_select_class</span>
            </div>
        </div>
        <div class="explain-container">
            <span v-language:inner>uniSelect_from</span>
            {{schoolName}}
        </div>
        <div class="select-school-container">
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
            <div v-if="showBox" class="search-classes-container">
                <div class="select-heading">
                    <span>Select from the list</span>
                </div>
                <ul class="class-list search-classes-list">
                    <li class="list-item search-class-item cursor-pointer" v-if="!hideIfChoosen"
                        v-for="singleClass in classes"
                        @click="addClass(singleClass, classes)">
                        <div v-html="$options.filters.boldText(singleClass.text, search)">{{ singleClass.text }}</div>
                    </li>
                    <li class="list-item add-item cursor-pointer" @click="addManualUniversity()">
                        <span v-language:inner>uniSelect_didnt_find_class</span>
                    </li>
                </ul>
            </div>
          <div :class="['selected-classes-container', showBox ? 'mt-0': 'spaceTop' ]">
              <div class="select-heading">
                  <span v-language:inner>uniSelect_yours_selected</span>
                  <span> ({{quantatySelected}})</span>
              </div>
              <ul  class="class-list selected-classes-list">
                  <li class="list-item selected-class-item" v-for="selectedClass in selectedClasses">
                      {{selectedClass.text}}
                      <span class="delete-class cursor-pointer" @click="deleteClass(selectedClass, selectedClasses)">
                        <v-icon>sbf-close</v-icon>
                    </span>
                  </li>
              </ul>
          </div>

        </div>
    </div>
</template>

<script>
    import { mapActions, mapGetters } from "vuex";
    import { LanguageService } from "../../../../services/language/languageService";
    import debounce from "lodash/debounce";

    export default {
        data() {
            return {
                search: "",
                classNamePlaceholder: LanguageService.getValueByKey(
                    "uniSelect_type_class_name_placeholder"
                ),
                isRtl: global.isRtl,
                global: global
            };
        },
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
        watch: {
            search: debounce(function (val) {
                let searchVal;
                if (!!val) {
                    searchVal = val.trim();
                    if (searchVal.length >= 3) {
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
            quantatySelected(){
                return this.selectedClasses.length
            },
            schoolName() {
                return this.getSchoolName();
            },
            showBox() {
                return !!this.search && this.search.length > 0;
            },
            classes() {
                return this.getClasses();
            },
            hideIfChoosen(){
                this.classes.some(r=> this.selectedClasses.indexOf(r) >= 0)
            },
            //edge hide placehloder fix
            placeholderVisible() {
                return this.getSelectedClasses.length < 1;
            },
            selectedClasses: {
                get() {
                    return this.getSelectedClasses;
                },
                set(val) {
                    let arrValidData = [];
                    if (val.length > 0) {
                        arrValidData = val.filter(singleClass => {
                            if (singleClass.text) {
                                return singleClass.text.length > 3;
                            } else {
                                return singleClass.length > 3;
                            }
                        });
                    }
                    this.updateSelectedClasses(arrValidData);
                }
            }
        },
        methods: {
            ...mapActions(["updateClasses", "updateSelectedClasses", "assignClasses", "pushClassToSelectedClasses"]),
            ...mapGetters(["getSchoolName", "getClasses"]),

            lastStep() {
                this.fnMethods.changeStep(this.enumSteps.set_school);
            },
            nextStep(customClass) {
                //TODO add action update the server instead of 'updateSelectedClasses'
                if(customClass){
                    this.addClass(customClass);
                    this.assignClasses().then(() => {
                        this.fnMethods.changeStep(this.enumSteps.set_class);
                    });
                }else{
                    this.assignClasses().then(() => {
                        this.fnMethods.changeStep(this.enumSteps.done);
                    });
                }
            },
            deleteClass(classToDelete, from){
             let index = from.indexOf(classToDelete);
             from.splice(index, 1);
            },

            addManualUniversity(){
                this.fnMethods.openAddSchoolOrClass(false, this.nextStep);
            },
            addClass(className) {
                this.pushClassToSelectedClasses(className);
                setTimeout(() => {
                    // let container = document.querySelector('.v-select__selections');
                    let inputElm = document.getElementById('classes_input');
                    inputElm.value = "";
                    inputElm.focus();
                }, 200);
                this.deleteClass(className, this.classes);
            },
            itemInList(item) {
                if (typeof item !== "object") {
                    return false;
                } else {
                    return true;
                }
            }
        },
        filters: {
            boldText(value, search) {
                if (!value) return "";
                if (!search) return value;
                let match = value.toLowerCase().indexOf(search.toLowerCase()) > -1;
                if (match) {
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

<style lang="less" scoped>
    @import "../../../../styles/mixin.less";
    .scrollBarStyle(0px, #0085D1);

    .cursor-pointer{
        cursor: pointer;
    }

    .search-classes-container{
        margin-top: 2px;
    }
    .selected-classes-container{
        &.spaceTop{
            margin-top: 2px;
        }
    }
    .select-heading{
        display: flex;
        align-items: center;
        height: 28px;
        padding: 0 16px;
        background-color: #f0f0f7;
        span{
            height: 18px;
            font-family: @fontFiraSans;
            font-size: 13px;
            font-weight: 600;
            color: @colorBlackNew;
        }

    }
    .class-list{
        background-color: #ffffff;
        max-height: 250px;
        overflow-y: scroll;
        padding-left: 0;
        &.selected-classes-list{
            box-shadow: 0 2px 4px 0 rgba(0, 0, 0, 0.16);
            background-color: #f7f7f7;
        }
    }

    .list-item{
        align-items: center;
        justify-content: space-between;
        color: inherit;
        display: flex;
        font-size: 16px;
        font-weight: 400;
        height: 48px;
        margin: 0;
        padding: 0 16px;
        position: relative;
        text-decoration: none;
        transition: background .3s cubic-bezier(.25,.8,.5,1);
    }
    .add-item{
        font-size: 14px;
        color: @color-blue-new;
        border-top: solid 2px rgba(81, 88, 175, 0.24);
    }
    .search-class-item{
        &:hover{
            background: rgba(0,0,0,.04);
        }
        span{
            height: 48px;
            font-size: 16px;
            font-weight: 400;
        }
    }
    .chip-style {
        background-color: rgba(68, 82, 252, 0.09);
        &.dark-chip {
            background-color: rgba(68, 82, 252, 0.27);
        }
        &.selected {
            -webkit-box-shadow: 0px 2px 4px 0 rgba(0, 0, 0, 0.2);
            -moz-box-shadow: 0px 2px 4px 0 rgba(0, 0, 0, 0.2);
            box-shadow: 0px 2px 4px 0 rgba(0, 0, 0, 0.2);
        }
    }

    .chip-button {
        cursor: pointer;
        max-width: 150px;
        text-overflow: ellipsis;
        white-space: nowrap;
        overflow: hidden;
    }

    .sbf-close {
        font-size: 8px !important;
        margin-bottom: 3px;
        margin-left: 8px;
    }

    .subheading {
        font-size: 16px;
        color: rgba(0, 0, 0, 0.38);
        &.dark {
            font-size: 13px !important;
            color: rgba(0, 0, 0, 0.54);
        }
    }
</style>
