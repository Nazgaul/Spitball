<template>
    <v-card elevation="0" class="mb-5 sb-step-card">
        <div class="upload-row-1 final-row" row>
            <v-icon class="five">sbf-five</v-icon>
            <h3 class="sb-title" v-language:inner>upload_files_step6_title</h3>
            <!--<h4 class="sb-subtitle mt-2" v-language:inner>upload_files_step6_subtitle</h4>-->
        </div>
        <div class="upload-row-2 final-row " style="padding-top: 32px;">
            <div class="final-item school">
                <div class="final-item-wrap">
                    <v-icon class="final-icon">sbf-university</v-icon>
                    <span class="item-name" v-language:inner>upload_files_label_school</span>
                </div>
                <div>
                    <span class="school-name"
                          v-line-clamp="1">{{getSchoolName}}</span>
                </div>
            </div>
            <div class="final-item class-selected" @click="changeStep(2)">
                <div class="edit">
                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                    <span v-language:inner>upload_files_edit</span>
                </div>
                <div class="final-item-wrap">
                    <v-icon class="final-icon">sbf-classes-new</v-icon>
                    <span class="item-name" v-language:inner>upload_files_class</span>
                </div>
                <span class="class-name"
                      v-line-clamp="1">{{getFileData.course}}</span>
            </div>
            <div class="final-item" @click="changeStep(4)">
                <div class="edit">
                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                    <span v-language:inner>upload_files_edit</span>
                </div>
                <div class="final-item-wrap">
                    <v-icon class="final-icon">sbf-classes-new</v-icon>
                    <span class="item-name" v-language:inner>upload_files_final_title</span>
                </div>
                <span class="class-name"
                      v-line-clamp="1">{{fileName}}</span>
            </div>
            <div class="final-item" @click="changeStep(4)">
                <div class="edit">
                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                    <span v-language:inner>upload_files_edit</span>
                </div>
                <div class="final-item-wrap">
                    <v-icon class="final-icon">sbf-professor</v-icon>
                    <span class="item-name" v-language:inner>upload_files_final_prof_label</span>
                </div>
                <span class="class-name"
                      v-line-clamp="1">{{getFileData.professor}}</span>
            </div>

            <div class="final-item doc-type-selected" @click="changeStep(3)">
                <div class="edit">
                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                    <span v-language:inner>upload_files_edit</span>
                </div>
                <div class="final-item-wrap doc-type-wrap">
                    <v-icon class="final-icon doc-type">sbf-{{selectedType ? selectedType.id : ''}}-note</v-icon>
                    <span class="item-name doc-type-name">{{selectedType ? selectedType.title : ''}}</span>
                </div>

            </div>
            <div class="final-item tags-selected" @click="changeStep(5)">
                <div class="edit">
                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                    <span v-language:inner>upload_files_edit</span>
                </div>
                <div class="final-item-wrap">
                    <v-icon class="final-icon tags">sbf-tag-icon</v-icon>
                    <span class="item-name" v-language:inner>upload_files_label_tags</span>
                    <span class="keywords-length mr-3">{{getFileData.tags.length}}</span>
                </div>
                <div class="sb-combo final-tags">
                    <v-chip class="sb-chip-tag" v-for="tag in getFileData.tags" :key="tag" v-if="getFileData.tags">
                                                   <span class="chip-button px-1">
                                                       {{tag}}
                                                   </span>
                    </v-chip>
                </div>
            </div>
            <div class="final-item price" @click="changeStep(6)">
                <div class="edit">
                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                    <span v-language:inner>upload_files_edit</span>
                </div>
                <div class="final-item-wrap">
                    <v-icon class="final-icon">sbf-credit-card</v-icon>
                    <span class="item-name" v-language:inner>upload_files_price_label</span>
                </div>
                <span class="price"
                      v-line-clamp="1"><bdi>{{getFileData.price || '00.00'}} <span class="price-sbl" v-language:inner>app_currency_dynamic</span></bdi></span>
            </div>
        </div>
        <div class="upload-row-4 final-row">
            <div class="legal-wrap">
                <span>{{CheckboxLabel}}</span>
            </div>
        </div>
    </v-card>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import { documentTypes } from "../consts"
    import { LanguageService } from "../../../../../services/language/languageService";

    export default {
        name: "uploadStep_6",
        data() {
            return {
                selected: {},
                checker: false,
                CheckboxLabel: LanguageService.getValueByKey("upload_files_label_legal")
            }
        },
        props: {
            callBackmethods: {
                type: Object,
                default: {},
                required: false
            }
        },
        computed: {
            ...mapGetters({
                getFileData: 'getFileData',
                getSchoolName: 'getSchoolName',
                getCustomFileName: 'getCustomFileName',


            }),
            fileName(){
                let name = this.getFileData.name;
                let customName = this.getCustomFileName;
                //will be 0 if even
                if(name.localeCompare(customName) === 0){
                    console.log('even');
                   return name;

                }else{
                    console.log('different');
                    return customName;

                }
            },
               selectedType() {
                if (this.getFileData.type) {
                    return this.selected = documentTypes.find((item) => {
                        return item.id === this.getFileData.type;
                    })
                }
            }
        },
        methods: {
            changeStep(step) {
                this.callBackmethods.changeStep(step)
            },
          },

    }
</script>

<style>

</style>