<template>
    <v-card class="uf-sEdit-item mb-4 pa-3 pt-0">
        <div class="text-right pb-1">
            <v-icon v-if="!isLastItem" class="uf-sEdit-close" color="#adadba" size="12" @click="deleteFile()">sbf-close</v-icon>
        </div>
        <v-row dense class="pa-0 ma-0 pr-4">
            <v-row dense class="pa-0 ma-0 justify-space-between">   
                <v-col cols="12" md="8" class="pa-0 pr-4">
                    <v-text-field
                        v-model="item.name" 
                        :rules="[rules.required]"
                        dense
                        height="44"
                        dir="ltr"
                        style="direction: ltr;" 
                        outlined
                    />
                </v-col>
                <v-col cols="7" md="4" class="pa-0">
                    <v-combobox
                        v-model="course"
                        :placeholder="itemCoursePlaceholder"
                        :items="getSelectedClasses"
                        :rules="[rules.required,rules.matchCourse]"
                        :append-icon="''"
                        dense
                        height="44"
                        autocomplete="abcd"
                        hide-no-data
                        outlined
                    />
                </v-col>
                <v-col hidden-md-and-up xs4 class="pa-0 d-md-none d-flex">
                    <v-text-field
                        v-model="item.price"
                        type="number"
                        class="uf_price"
                        :rules="[rules.integer,rules.maximum,rules.minimum]"
                        :placeholder="emptyPricePlaceholder"
                        :suffix="item.price ? pricePts : ''"
                        dense
                        height="44"
                        outlined
                    >
                    </v-text-field>
                </v-col>
            </v-row>
            <v-row class="pa-0 ma-0 justify-space-between">
                <v-col cols="12" md="10" class="uf_desc pa-0 pr-sm-4">
                    <v-text-field 
                        class="pt-1"
                        v-model="item.description" 
                        :placeholder="itemDescPlaceholder"
                        dense
                        height="44"
                        outlined
                    >
                    </v-text-field>
                </v-col>
                <v-col md="2" class="d-none d-md-flex pa-0"> 
                    <v-text-field class="uf_price pt-1"
                        :rules="[rules.integer,rules.maximum,rules.minimum]" 
                        type="number" 
                        v-model="item.price" 
                        :placeholder="emptyPricePlaceholder"
                        :suffix="item.price? pricePts :''"
                        dense
                        height="44"
                        outlined
                        >
                    </v-text-field>
                </v-col>
            </v-row>
        </v-row>
    </v-card>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

import { LanguageService } from "../../../services/language/languageService";
import { validationRules } from '../../../services/utilities/formValidationRules';

export default {
    name: "fileCard",
    data() {
        return {
            fileNamePlaceholder: LanguageService.getValueByKey("upload_multiple_fileName_placeholder"),
            emptyPricePlaceholder: LanguageService.getValueByKey("upload_uf_price"),
            itemDescPlaceholder: LanguageService.getValueByKey("upload_uf_desc"),
            itemCoursePlaceholder: LanguageService.getValueByKey("upload_uf_course_name"),
            pricePts: LanguageService.getValueByKey("upload_uf_price_pts"),
            selectedCourse:'',
            rules: {
                required: (value) => validationRules.required(value),
                integer: (value) => validationRules.integer(value),
                matchCourse:() => ((this.getSelectedClasses.length && this.getSelectedClasses.some(course=>course.text === this.selectedCourse)
                        ) || this.isFromQuery) || LanguageService.getValueByKey("tutorRequest_invalid"),
                maximum: (value) => validationRules.maxVal(value, 1000),
                minimum: (value) => validationRules.minVal(value,0),
            },
            isFromQuery: false,
        }
    },
    props: {
        fileItem: {
            type: Object,
            default(){
                return {}
            }
        },
        singleFileIndex: {
            type: Number,
            required: true
        },
        quantity: {
            type: Number,
            required: false
        },
    },
    watch: {
        item: {
            deep: true,
            handler(newVal) {
                if(this.selectedCourse !== newVal.course){
                    this.isFromQuery = true
                }
                let fileObj = {
                    index: this.singleFileIndex,
                    data: newVal
                };
                this.changeFileByIndex(fileObj);
            }
        }
    },
    computed: {
        ...mapGetters(['getFileData', 'getSelectedClasses']),
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
                if(!!val && val.text){
                    this.item.course = val.text
                    this.selectedCourse = val.text
                }else{
                    this.item.course = val.text || ''
                }
            }
        }
    },
    methods: {
        ...mapActions(['changeFileByIndex', 'deleteFileByIndex']),
        deleteFile() {
            this.deleteFileByIndex(this.singleFileIndex)
        },
    },
    mounted() {
        if((this.$route.query && this.$route.query.course)){
            let courseName = this.$route.query.course;
            this.course = this.getSelectedClasses.filter((course)=>{
                return course.text === courseName;
            })[0];
        }
    }
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
.uf-sEdit-item{
    box-shadow: 0 1px 1px 0 rgba(0, 0, 0, 0.14) !important;
    border-radius: 4px;
    position: relative;
    .uf_price{
        input[type='number'] {
            -moz-appearance:textfield;
        }

        input[type=number]::-webkit-inner-spin-button, 
        input[type=number]::-webkit-outer-spin-button { 
            -webkit-appearance: none; 
        }
        .v-text-field__suffix{
            font-size: 14px;
            font-weight: 600;
            color: @global-purple; 
        }
        .v-messages__message {
            line-height: 1.2;
        }
        .v-input__slot{
            margin-bottom:6px;
        }
    }
    .uf-sEdit-close{
        z-index: 99;
    }
    .v-input__slot {
        ::placeholder{
            font-size: 14px;
            color: #a1a3b0;
        }
        input{
            font-size: 14px;
            font-weight: 600;
            color: @global-purple; 
        }
        .v-input__icon {
            justify-content: start;
            width: 18px;
            min-width: 18px;
        }
        .v-icon{
            color: @global-purple;
            font-size: 20px;
            transform: rotate(90deg);
        }
    }
}

</style>