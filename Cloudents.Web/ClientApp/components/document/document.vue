<template>
    <v-layout class="document-container">
        <main-document :document="getDocument"></main-document>
        <v-divider vertical class="mx-3"></v-divider>
        <aside-document :document="getDocument"></aside-document>
        <aside-document-tutors v-if="$vuetify.breakpoint.smAndDown"></aside-document-tutors>
    </v-layout>
</template>

<script>
import { mapActions, mapGetters, mapMutations } from 'vuex';
import mainDocument from './mainDocument/mainDocument.vue';
import asideDocument from './asideDocument/asideDocument.vue';
import asideDocumentTutors from './asideDocument/asideDocumentTutors.vue';

export default {
    components: {
        mainDocument,
        asideDocument,
        asideDocumentTutors
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
    @import "../../styles/mixin.less";

    .document-container {
        padding: 40px 6%;
        @media (max-width: @screen-sm) {
            padding: 20px 10px;
            flex-direction: column;
        }
    }
</style>