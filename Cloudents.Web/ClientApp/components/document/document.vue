<template>
    <v-layout class="document-container">
        <main-document :document="getDocument"></main-document>
        <v-divider vertical class="mx-3"></v-divider>
        <aside-document :document="getDocument"></aside-document>
    </v-layout>
</template>

<script>
import { mapActions, mapGetters, mapMutations } from 'vuex';
import mainDocument from './mainDocument/mainDocument.vue';
import asideDocument from './asideDocument/asideDocument.vue';

//store
import storeService from '../../services/store/storeService';
import document from '../../store/document';
import studyDocumentsStore from '../../store/studyDocuments_store';

export default {
    components: {
        mainDocument,
        asideDocument,
    },
    props: {
        id: {
            type: String
        }
    },
    methods: {
        ...mapActions(['documentRequest', 'clearDocument']),
    },
    computed: {
        ...mapGetters(['getDocumentDetails']),

        getDocument() {
            return this.getDocumentDetails
        },
    },
    beforeDestroy(){
        this.clearDocument();
        storeService.unregisterModule(this.$store,'document');
    },
    created() {    
         
        storeService.lazyRegisterModule(this.$store,'studyDocumentsStore',studyDocumentsStore); 
        storeService.registerModule(this.$store,'document', document);
        this.documentRequest(this.id)
    }
}
</script>

<style lang="less">
    @import "../../styles/mixin.less";

    .document-container {
        padding: 40px 10px;
        @media (max-width: @screen-sm) {
            padding: 20px 10px;
            flex-direction: column;
        }
    }
</style>