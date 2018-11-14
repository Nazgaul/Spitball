<template>
    <v-card :class="[  $vuetify.breakpoint.smAndDown ? '' : 'mb-5',  'sb-step-card']">
        <div class="upload-row-1">
            <h3 class="sb-title" v-language:inner>upload_files_step3_title</h3>
        </div>
        <div class="upload-row-2">
            <div :class="['sb-doc-type', singleType.id === selectedDoctype.id ? 'selected': '']"
                 v-for="singleType in documentTypes"
                 :key="singleType.id"
                 v-show="singleType.id !== 'none'"
                 @click="updateDocumentType(singleType)">
                <v-icon class="sb-doc-icon mt-2">{{singleType.icon}}</v-icon>
                <span class="sb-doc-title mb-2">
                                           {{singleType.title}}
                                       </span>
            </div>
        </div>
    </v-card>
</template>

<script>
    import { documentTypes } from "../consts";
    import { mapGetters, mapActions } from 'vuex';

    export default {
        name: "uploadStep_3",
        data() {
            return {
                documentTypes: documentTypes,
                // selectedDoctype: [],
                activeStep: 3,
            }
        },
        computed: {
            ...mapGetters({
                getFileData: 'getFileData'
            }),

            selectedDoctype() {
                return this.documentTypes.filter(item => {
                    return item.id === this.getFileData.type || item.id === "none";
                })[0];
            },

        },
        methods: {
            ...mapActions(['updateFile']),
            updateDocumentType(docType) {
                this.updateFile({'type': docType.id});

            },

        },
    }
</script>

<style>

</style>