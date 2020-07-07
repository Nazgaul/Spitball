<template>
    <v-card class="uf-sEdit-item mb-3 px-3 py-2">
        <div class="text-right pb-4">
            <v-icon v-if="!isLastItem" class="uf-sEdit-close" color="#adadba" size="12" @click="deleteFile()">sbf-close</v-icon>
        </div>
        <v-row dense class="pa-0 ma-0">
            <v-col cols="12" sm="7" class="pa-0 pr-sm-4">
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
            <v-col cols="12" sm="9" class="uf_desc pa-0 pr-sm-4" order="4" order-sm="3">
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
            <v-col cols="12" sm="3" class="pa-0" order="3" order-sm="4">
                <v-select
                    v-model="item.priceType"
                    v-if="isTutorSubscribe"
                    :items="currentPriceItems"
                    :rules="[rules.required]"
                    :label="$t('upload_file_price_label')"
                    :append-icon="'sbf-menu-down'"
                    placeholder=" "
                    color="#4c59ff"
                    height="44"
                    autocomplete="off"
                    hide-no-data
                    outlined
                    dense
                >
                </v-select>
                <v-text-field class="uf_price pt-0"
                    v-model="item.price" 
                    v-else
                    type="number"
                    :label="$t('upload_file_price_label')"
                    :rules="[rules.integer,rules.minimum,rules.maximum]"
                    placeholder=" "
                    :suffix="item.price ? $t('upload_uf_price_pts') : ''"
                    color="#4c59ff"
                    autocomplete="off"
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
            currentPriceItems: [
                { text: this.$t('upload_free_all'), value: 'Free' },
                { text: this.$t('upload_subscriber_only'), value: 'Subscriber' }
            ],
            rules: {
                required: (value) => validationRules.required(value),
                integer: (value) => validationRules.integer(value),
                maximum: (value) => validationRules.maxVal(value, 2147483647),
                minimum: (value) => validationRules.minVal(value,0),
                minimumChars: value => validationRules.minimumChars(value, 4),
                maximumChars: value => validationRules.maximumChars(value, 150)
            }
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
        isTutorSubscribe() {
            return this.$store.getters.getIsTutorSubscription
        },
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
        ...mapActions(['changeFileByIndex', 'deleteFileByIndex', 'updatePriceToAll']),
        deleteFile() {
            this.deleteFileByIndex(this.singleFileIndex)
        },
        searchCourses: debounce(function(ev){
            let term = ev.target.value.trim()
            if(!term) {
                this.course = ''
                this.suggestsCourses = []
                return 
            }
            if(!!term){
                courseService.getCourse({term, page:0}).then(data=>{
                    this.suggestsCourses = data;
                    if(this.suggestsCourses.length) {
                        this.suggestsCourses.forEach(course=>{
                            if(course.text === this.course){
                                this.course = course
                            }}) 
                    } else {
                        this.course = term
                    }
                })
            }
        },300),
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
    .uf_price{
        input[type='number'] {
            -moz-appearance:textfield;
        }

        input[type=number]::-webkit-inner-spin-button, 
        input[type=number]::-webkit-outer-spin-button { 
            -webkit-appearance: none; 
        }
        .v-text-field__suffix {
            font-size: 14px;
            font-weight: 600;
            color: @global-purple; 
        }
        .v-input__slot {
            margin-bottom:8px;
        }
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