<template>
    <v-card class="mb-5 sb-step-card">
        <div class="upload-row-1">
            <h3 class="upload-cloud-text sb-title" v-language:inner>upload_files_step4_title</h3>
        </div>
        <div class="upload-row-2 paddingTopSm">
            <div class="btn-holder">
                <label :for="'school'" class="steps-form-label school mb-2">
                    <!--<v-icon class="mr-1">sbf-university</v-icon>-->
                    <span v-language:inner>upload_files_label_document_title</span></label>
                <sb-input :bottomError="true"
                          v-model="documentTitle" placeholder="upload_files_label_document_title" name="document title"
                          type="text"
                          :autofocus="true">
                </sb-input>
            </div>
            <div class="btn-holder ml-0 ">

            </div>
        </div>
        <div class="upload-row-3">
            <label :for="'school'" class="steps-form-label school mb-2">
                <span v-language:inner>upload_files_label_proff_name</span></label>
            <sb-input :bottomError="true"
                      v-model="proffesorName" placeholder="upload_files_label_proff_name" name="proffesor"
                      type="text"
                      :autofocus="true">
            </sb-input>
        </div>
    </v-card>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import sbInput from "../../../../question/helpers/sbInput/sbInput";

    export default {
        name: "uploadStep_4",
        components:{
            sbInput
        },
        data() {
            return {
            }
        },
        computed: {
            ...mapGetters({
                getFileData: 'getFileData',
            }),
            documentTitle: {
                get () {
                    return this.getFileData.name;
                },
                set (value) {
                    this.updateFile({'name': value});
                }
            },
            proffesorName:{
                get () {
                    return this.getFileData.proffesorName;
                },
                set (profName) {
                    this.updateFile({'proffesorName': profName});
                }
            }
        },
        methods: {
            ...mapActions(['updateFile']),

        },
    }
</script>

<style >

</style>