<template>
    <v-card class="mb-5 sb-step-card">
        <div class="upload-row-1">
            <h3 class="upload-cloud-text sb-title" v-language:inner>upload_files_step5_title</h3>
            <h4 class="sb-subtitle mt-2"  v-language:inner>upload_files_step5_subtitle</h4>
        </div>
        <div class="upload-row-2 paddingTopSm">
            <div class="btn-holder">
                <label :for="'school'" class="steps-form-label school mb-2">
                    <v-icon class="mr-1">sbf-tag-icon</v-icon>
                    <span v-language:inner>upload_files_label_tags</span></label>
                <v-combobox class="sb-combo"
                            v-language:placeholder
                            v-model="selectedTags"
                            height="'48px'"
                            append-icon=""
                            prepend-icon=""
                            :placeholder="placeholderTags"
                            color="'#979797'"
                            multiple
                            chips
                            solo
                            :allow-overflow="false">
                    <template slot="selection" slot-scope="data" class="sb-selection">
                        <v-chip class="sb-chip-tag">
                                                   <span class="chip-button px-2">
                                                       {{!!data.item ? data.item : ''}}
                                                   </span>
                            <v-icon class="chip-close ml-3" @click="removeTag(data.item)">
                                sbf-close
                            </v-icon>
                        </v-chip>
                    </template>
                </v-combobox>
            </div>
            <div class="btn-holder">
            </div>
        </div>
    </v-card>
</template>
<script>
    import { mapGetters, mapActions } from 'vuex';
    import { LanguageService } from "../../../../../services/language/languageService";
     export default {
        name: "uploadStep_5",
        data() {
            return {
                placeholderTags: LanguageService.getValueByKey('upload_files_placeholder_tags'),
            }
        },
        computed: {
            ...mapGetters({
                getFileData: 'getFileData',
            }),
            selectedTags:{
                get () {
                    return this.getFileData.tags;
                },
                set (value) {

                    this.updateFile({'tags': value});
                }
            }
        },

        methods: {
         ...mapActions(['updateFile']),

            removeTag(item) {
                this.selectedTags.splice(this.selectedTags.indexOf(item), 1);
                this.selectedTags = [...this.selectedTags];
                this.updateFile({'tags' : this.selectedTags})
            },

        },
         created(){
            console.log('step 5 created')
         }
    }
</script>

<style >

</style>