<template>
    <v-card class="mb-5 sb-step-card">
        <div class="upload-row-1">
            <h3 class="sb-title">Choose document type, it would be easier to
                find it</h3>
        </div>
        <div class="upload-row-2">
            <div :class="['sb-doc-type', singleType.title === selectedDoctype.title ? 'selected': '']"
                 v-for="singleType in documentTypes"
                 :key="singleType.id"
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
                selectedDoctype: [],            }
        },

        methods: {
            ...mapActions(['updateFile']),
            updateDocumentType(docType) {
                this.selectedDoctype = docType;
                this.updateFile({'type': docType.id});

            },

        },
    }
</script>

<style >

</style>