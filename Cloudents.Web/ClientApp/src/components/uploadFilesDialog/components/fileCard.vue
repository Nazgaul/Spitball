<template>
    <v-card class="uf-sEdit-item mb-3 px-3 py-2" v-if="item">
        <div class="text-right pb-4">
            <v-icon v-if="!isLastItem" class="uf-sEdit-close" color="#adadba" size="12" @click="deleteFile()">sbf-close</v-icon>
        </div>
        <v-row dense class="pa-0 ma-0">
            <v-col cols="12" sm="7" class="pa-0 pe-sm-4">
                <v-text-field
                    v-model="item.name"
                    :rules="[rules.required, rules.minimumChars, rules.maximumChars]"
                    :label="$t('upload_file_title_label')"
                    placeholder=" "
                    color="#4c59ff"
                    height="44"
                    dir="ltr"
                    outlined
                    dense
                >
                </v-text-field>
            </v-col>
            <v-col cols="12" sm="5" class="pa-0">
                <v-combobox
                    v-model="course"
                    :items="suggestsCourses"
                    @keyup="searchCourses"
                    :rules="[rules.required,rules.minimumChars]"
                    :label="$t('upload_file_course_label')"
                    :append-icon="''"
                    placeholder=" "
                    color="#4c59ff" height="44"
                    hide-no-data outlined dense
                >
                </v-combobox>
            </v-col>
            <v-col cols="12" sm="9" class="uf_desc pa-0 pe-sm-4" order="4" order-sm="3">
                <v-text-field
                    v-model="item.description"
                    :placeholder="$t('upload_uf_desc')"
                    :label="$t('upload_file_description_label')"
                    color="#4c59ff"
                    height="44"
                    outlined
                    dense
                >
                </v-text-field>
            </v-col>
        </v-row>
    </v-card>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import debounce from "lodash/debounce";
import courseService from '../../../services/courseService.js'
import { validationRules } from '../../../services/utilities/formValidationRules';

export default {
    name: "fileCard",
    data() {
        return {
            suggestsCourses: [],
            currentPrice: null,
            rules: {
                required: (value) => validationRules.required(value),
                minimumChars: value => validationRules.minimumChars(value, 4),
                maximumChars: value => validationRules.maximumChars(value, 150)
            }
        }
    },
    props: {
        singleFileIndex: {
            type: Number,
            required: true
        }
    },
    watch: {
        item: {
            deep: true,
            handler(newVal) {
                let fileObj = {
                    index: this.singleFileIndex,
                    data: newVal
                };
                this.changeFileByIndex(fileObj);
            }
        }
    },
    computed: {
        ...mapGetters(['getFileData']),
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        isLastItem(){
            let nonErrorItems = this.getFileData.filter(item=>(!item.error))
            return (nonErrorItems.length < 2)
        },
        item() {
            return this.getFileData[this.singleFileIndex]
        },
        course:{
            get(){
                return this.item.course  
            },
            set(val){
                if(val) {
                    this.item.course = val.text || val
                }
            }
        }
    },
    methods: {
        ...mapActions(['changeFileByIndex', 'deleteFileByIndex']),
        deleteFile() {
            this.deleteFileByIndex(this.singleFileIndex)
        },
        searchDeboune: debounce(function(term){
            courseService.getCourse({term}).then(data=>{
                this.suggestsCourses = data;
                if(this.suggestsCourses.length) {
                    this.suggestsCourses.forEach(course=>{
                        if(course.text === this.course){
                            this.course = course
                        }}) 
                }
            })
        },300),
        searchCourses(ev){
            let term = ev.target.value.trim()
            if(!term) {
                this.course = ''
                this.suggestsCourses = []
                return 
            }
            if(!!term){
                this.course = term
                this.searchDeboune(term) 
            }
        },
    }
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
.uf-sEdit-item{
    box-shadow: 0 1px 1px 0 rgba(0, 0, 0, 0.14) !important;
    border-radius: 4px;
    position: relative;

    .uf-sEdit-close{
        z-index: 99;
    }
    .v-input__slot {
        border-radius: 6px;
        margin-bottom: 8px !important;
        ::placeholder{
            font-size: 14px;
            color: #a1a3b0;
        }
        input, .v-select__selections{
            font-size: 14px;
            font-weight: 600;
            color: @global-purple; 
        }
    }
}

</style>