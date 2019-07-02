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

export default {
    components: {
        mainDocument,
        asideDocument
    },
    props: {
        id: {
            type: String
        }
    },
    methods: {
        ...mapActions(['documentRequest']),
    },
    computed: {
        ...mapGetters(['getDocumentDetails']),

        getDocument() {
            return this.getDocumentDetails
        },
    },
    created() {      
        this.documentRequest(this.id)
    }
}
</script>

<style lang="less">
    .document-container {
        padding: 40px 200px;
    }
</style>